using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;
using TheOtherRoles.Patches;
using UnityEngine.SceneManagement;
using TheOtherRoles.Utilities;
using AmongUs.Data;
using Assets.InnerNet;
using System.Linq;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using TMPro;
using System.Text;
using TheOtherRoles.Helpers;

namespace TheOtherRoles.Modules
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public class MainMenuPatch {
        private static bool horseButtonState = TORMapOptions.enableHorseMode;
        //private static Sprite horseModeOffSprite = null;
        //private static Sprite horseModeOnSprite = null;
        private static AnnouncementPopUp popUp;

        private static void Prefix(MainMenuManager __instance) {
            var template = GameObject.Find("ExitGameButton");
            var template2 = GameObject.Find("CreditsButton");
            if (template == null || template2 == null) return;
            template.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
            template.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.625f, 0.5f);
            template.transform.FindChild("FontPlacer").transform.localScale = new Vector3(1.8f, 0.9f, 0.9f);
            template.transform.FindChild("FontPlacer").transform.localPosition = new Vector3(-1.1f, 0f, 0f);

            template2.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
            template2.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.378f, 0.5f);
            template2.transform.FindChild("FontPlacer").transform.localScale = new Vector3(1.8f, 0.9f, 0.9f);
            template2.transform.FindChild("FontPlacer").transform.localPosition = new Vector3(-1.1f, 0f, 0f);



            var buttonDiscord = UnityEngine.Object.Instantiate(template, template.transform.parent);
            buttonDiscord.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
            buttonDiscord.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.542f, 0.5f);

            var textDiscord = buttonDiscord.transform.GetComponentInChildren<TMPro.TMP_Text>();
            __instance.StartCoroutine(Effects.Lerp(0.5f, new System.Action<float>((p) => {
                textDiscord.SetText(ModTranslation.getString("modDiscord"));
            })));
            PassiveButton passiveButtonDiscord = buttonDiscord.GetComponent<PassiveButton>();
            
            passiveButtonDiscord.OnClick = new Button.ButtonClickedEvent();
            passiveButtonDiscord.OnClick.AddListener((System.Action)(() => Application.OpenURL(AmongUs.Data.DataManager.Settings.Language.CurrentLanguage == SupportedLangs.SChinese
                ? "http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=zye92aL_kkuduaPjnHj5MppLWoKrAEse&authKey=%2FUCYHv33zSXF%2FqRcqGFrAvgJO92yp%2B%2FF5BliBtsh9HmCqA56pW1dWgMiLYASnxhJ&noverify=0&group_code=787132035" : "https://discord.gg/w7msq53dq7")));

            CheckAndUnpatch();


            // TOR credits button
            if (template == null) return;
            var creditsButton = Object.Instantiate(template, template.transform.parent);

            creditsButton.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
            creditsButton.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.462f, 0.5f);

            var textCreditsButton = creditsButton.transform.GetComponentInChildren<TMPro.TMP_Text>();
            __instance.StartCoroutine(Effects.Lerp(0.5f, new System.Action<float>((p) => {
                textCreditsButton.SetText(ModTranslation.getString("modCredits"));
            })));
            PassiveButton passiveCreditsButton = creditsButton.GetComponent<PassiveButton>();

            passiveCreditsButton.OnClick = new Button.ButtonClickedEvent();

            passiveCreditsButton.OnClick.AddListener((System.Action)delegate {
                // do stuff
                if (popUp != null) Object.Destroy(popUp);
                var popUpTemplate = Object.FindObjectOfType<AnnouncementPopUp>(true);
                if (popUpTemplate == null) {
                    TheOtherRolesPlugin.Logger.LogError("couldnt show credits, popUp is null");
                    return;
                }
                popUp = Object.Instantiate(popUpTemplate);

                popUp.gameObject.SetActive(true);
                string creditsString = @$"<align=""center""><b>Github Contributors:</b>
Alex2911    amsyarasyiq    MaximeGillot
Psynomit    probablyadnf    JustASysAdmin

[https://discord.gg/w7msq53dq7]Discord[] Moderators:
Ryuk    K    XiezibanWrite
Thanks to all our discord helpers!

Thanks to miniduikboot & GD for hosting modded servers

";
                creditsString += $@"<size=60%> Other Credits & Resources:
OxygenFilter - For the versions v2.3.0 to v2.6.1, we were using the OxygenFilter for automatic deobfuscation
Reactor - The framework used for all versions before v2.0.0, and again since 4.2.0
BepInEx - Used to hook game functions
Essentials - Custom game options by DorCoMaNdO:
Before v1.6: We used the default Essentials release
v1.6-v1.8: We slightly changed the default Essentials.
Four Han sinicization group - Some of the pictures are made by them
Jackal and Sidekick - Original idea for the Jackal and Sidekick came from Dhalucard
Among-Us-Love-Couple-Mod - Idea for the Lovers modifier comes from Woodi-dev
Jester - Idea for the Jester role came from Maartii
ExtraRolesAmongUs - Idea for the Engineer and Medic role came from NotHunter101. Also some code snippets from their implementation were used.
Among-Us-Sheriff-Mod - Idea for the Sheriff role came from Woodi-dev
TooManyRolesMods - Idea for the Detective and Time Master roles comes from Hardel-DW. Also some code snippets from their implementation were used.
TownOfUs - Idea for the Swapper, Shifter, Arsonist and a similar Mayor role came from Slushiegoose
Ottomated - Idea for the Morphling, Snitch and Camouflager role came from Ottomated
Crowded-Mod - Our implementation for 10+ player lobbies was inspired by the one from the Crowded Mod Team
Goose-Goose-Duck - Idea for the Vulture role came from Slushiegoose
TheEpicRoles - Idea for the first kill shield (partly) and the tabbed option menu (fully + some code), by LaicosVK DasMonschta Nova</size>";
                creditsString += "</align>";

                Assets.InnerNet.Announcement creditsAnnouncement = new() {
                    Id = "torCredits",
                    Language = 0,
                    Number = 500,
                    Title = "The Other Roles GM IA\nCredits & Resources",
                    ShortTitle = "TORGMIA Credits",
                    SubTitle = "",
                    PinState = false,
                    Date = "01.07.2022",
                    Text = creditsString,
                };
                __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) => {
                    if (p == 1) {
                        var backup = DataManager.Player.Announcements.allAnnouncements;
                        DataManager.Player.Announcements.allAnnouncements = new();
                        popUp.Init(false);
                        DataManager.Player.Announcements.SetAnnouncements(new Announcement[] { creditsAnnouncement });
                        popUp.CreateAnnouncementList();
                        popUp.UpdateAnnouncementText(creditsAnnouncement.Number);
                        popUp.visibleAnnouncements._items[0].PassiveButton.OnClick.RemoveAllListeners();
                        DataManager.Player.Announcements.allAnnouncements = backup;
                    }
                })));
            });
            
        }

        private static void CheckAndUnpatch()
        {
            // 获取所有已加载的插件
            var loadedPlugins = IL2CPPChainloader.Instance.Plugins.Values;
            // 检查是否有目标插件
            var targetPlugin = loadedPlugins.FirstOrDefault(plugin => plugin.Metadata.Name == "MalumMenu");

            if (targetPlugin != null)
            {
                TheOtherRolesPlugin.Logger.LogMessage("Hacker Plugin Found.\n GMIA Does Not Allow You Using Such Plugins.\nWill Unpatch Soon.");
                Harmony.UnpatchAll();//当进入MainMenu时检测加载如果有MM 就自动关闭
            }
        }

        public static void addSceneChangeCallbacks() {
            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, _) => {
                if (!scene.name.Equals("MatchMaking", StringComparison.Ordinal)) return;
                TORMapOptions.gameMode = CustomGamemodes.Classic;
                // Add buttons For Guesser Mode, Hide N Seek in this scene.
                // find "HostLocalGameButton"
                var template = GameObject.FindObjectOfType<HostLocalGameButton>();
                var gameButton = template.transform.FindChild("CreateGameButton");
                var gameButtonPassiveButton = gameButton.GetComponentInChildren<PassiveButton>();

                var guesserButton = GameObject.Instantiate<Transform>(gameButton, gameButton.parent);
                guesserButton.transform.localPosition += new Vector3(0f, -0.5f);
                var guesserButtonText = guesserButton.GetComponentInChildren<TMPro.TextMeshPro>();
                var guesserButtonPassiveButton = guesserButton.GetComponentInChildren<PassiveButton>();
                
                guesserButtonPassiveButton.OnClick = new Button.ButtonClickedEvent();
                guesserButtonPassiveButton.OnClick.AddListener((System.Action)(() => {
                    TORMapOptions.gameMode = CustomGamemodes.Guesser;
                    template.OnClick();
                }));

                var HideNSeekButton = GameObject.Instantiate<Transform>(gameButton, gameButton.parent);
                HideNSeekButton.transform.localPosition += new Vector3(1.7f, -0.5f);
                var HideNSeekButtonText = HideNSeekButton.GetComponentInChildren<TMPro.TextMeshPro>();
                var HideNSeekButtonPassiveButton = HideNSeekButton.GetComponentInChildren<PassiveButton>();
                
                HideNSeekButtonPassiveButton.OnClick = new Button.ButtonClickedEvent();
                HideNSeekButtonPassiveButton.OnClick.AddListener((System.Action)(() => {
                    TORMapOptions.gameMode = CustomGamemodes.HideNSeek;
                    template.OnClick();
                }));

                template.StartCoroutine(Effects.Lerp(0.1f, new System.Action<float>((p) => {
                    guesserButtonText.SetText(ModTranslation.getString("torGuesser"));
                    HideNSeekButtonText.SetText(ModTranslation.getString("torHideNSeek"));
                 })));
            }));
        }
    }

    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    public static class VersionShower_Start
    {
        public static void Postfix(VersionShower __instance)
        {
            __instance.text.text = $"Among Us v{Application.version} - {Helpers.GradientColorText("FFD700", "FF0000", $"The Other Roles GM IA")} <color=#FCCE03FF>v{TheOtherRolesPlugin.VersionString + (TheOtherRolesPlugin.betaDays > 0 ? "-BETA" : "")}</color>";
        }
    }

    // Original credits to Nebula On the Ship
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Awake))]
    public static class MainMenuSetUpPatch
    {
        public static Sprite sprite;
        public static GameObject modScreen = null;
        public static GameObject aboutScreen = null;

        public static void Postfix(MainMenuManager __instance)
        {
            var leftPanel = __instance.mainMenuUI.transform.FindChild("AspectScaler").FindChild("LeftPanel");
            leftPanel.GetComponent<SpriteRenderer>().size += new Vector2(0f, 0.5f);
            var auLogo = leftPanel.FindChild("Sizer").GetComponent<AspectSize>();
            auLogo.PercentWidth = 0.14f;
            auLogo.DoSetUp();
            auLogo.transform.localPosition += new Vector3(-0.8f, 0.25f, 0f);

            float height = __instance.newsButton.transform.localPosition.y - __instance.myAccountButton.transform.localPosition.y;

            // disable old panels and create a new button
            var reworkedPanel = Helpers.CreateObject<SpriteRenderer>("ReworkedLeftPanel", leftPanel, new Vector3(0f, height * 0.5f, 0f));
            var oldPanel = leftPanel.GetComponent<SpriteRenderer>();
            reworkedPanel.sprite = oldPanel.sprite;
            reworkedPanel.tileMode = oldPanel.tileMode;
            reworkedPanel.drawMode = oldPanel.drawMode;
            reworkedPanel.size = oldPanel.size;
            oldPanel.enabled = false;

            loadSprite();

            // Move the buttons upwards
            foreach (var button in __instance.mainButtons.GetFastEnumerator())
                if (Math.Abs(button.transform.localPosition.x) < 0.1f) button.transform.localPosition += new Vector3(0f, height, 0f);
            leftPanel.FindChild("Main Buttons").FindChild("Divider").transform.localPosition += new Vector3(0f, height, 0f);

            var modButton = Object.Instantiate(__instance.settingsButton, __instance.settingsButton.transform.parent);
            modButton.transform.localPosition += new Vector3(0f, -height, 0f);
            modButton.gameObject.name = "TORButton";
            modButton.gameObject.ForEachChild((Il2CppSystem.Action<GameObject>)((obj) => {
                var icon = obj.transform.FindChild("Icon");
                if (icon != null)
                {
                    icon.localScale = new Vector3(1f, 1f, 1f);
                    icon.GetComponent<SpriteRenderer>().sprite = sprite;
                }
            }));

            var modPassiveButton = modButton.GetComponent<PassiveButton>();
            modPassiveButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            modPassiveButton.OnClick.AddListener((System.Action)(() =>
            {
                __instance.ResetScreen();
                modScreen.SetActive(true);
                __instance.screenTint.enabled = true;
            }));
            modButton.transform.FindChild("FontPlacer").GetChild(0).GetComponent<TextTranslatorTMP>().SetModText("GMIA");

            modScreen = Object.Instantiate(__instance.accountButtons, __instance.accountButtons.transform.parent);
            modScreen.name = "modScreen";
            modScreen.transform.GetChild(0).GetChild(0).GetComponent<TextTranslatorTMP>().SetModText("The Other Roles GMIA");
            __instance.mainButtons.Add(modButton);

            Object.Destroy(modScreen.transform.GetChild(4).gameObject);

            var temp = modScreen.transform.GetChild(3);
            int index = 0;

            void SetUpButton(string button, Action clickAction)
            {
                GameObject obj = temp.gameObject;
                if (index > 0) obj = Object.Instantiate(obj, obj.transform.parent);

                obj.transform.GetChild(0).GetChild(0).GetComponent<TextTranslatorTMP>().SetModText(button);
                var passiveButton = obj.GetComponent<PassiveButton>();
                passiveButton.OnClick = new ButtonClickedEvent();
                passiveButton.OnClick.AddListener((System.Action)(() => {
                    clickAction.Invoke();
                }));
                obj.transform.localPosition = new Vector3(0f, 0.98f - index * 0.68f, 0f);

                index++;
            }
            SetUpButton(ModTranslation.getString("modAbout"), () => {
                __instance.ResetScreen();
                if (!aboutScreen) createAboutScreen();
                aboutScreen?.SetActive(true);
                __instance.screenTint.enabled = true;
            });
            SetUpButton(ModTranslation.getString("modHowToPlay"), () =>
            {
                PlayManager.Open(__instance);
            });

            void createAboutScreen()
            {
                aboutScreen = Helpers.CreateObject("About", __instance.accountButtons.transform.parent, new Vector3(0, 0, -1f));
                aboutScreen.transform.localScale = modScreen!.transform.localScale;
                var screen = Helpers.CreateObject("Screen", aboutScreen.transform, new Vector3(-0.1f, 0, 0f), LayerMask.NameToLayer("UI"));
                screen.ForEachChild((Il2CppSystem.Action<GameObject>)(obj => GameObject.Destroy(obj)));
                var text = screen.AddComponent<TextMeshPro>();
                text.SetText("<line-height=50%><size=50%>" + Helpers.GradientColorText("E78C0B", "37A796", ModTranslation.getString("mainMenuAboutSection")) + "</size></line-height>");
                text.alignment = TextAlignmentOptions.Center;
                text.fontSize *= 0.05f;
                text.transform.SetParent(screen.transform);
                text.transform.localPosition = 2 * Vector3.up - new Vector3(0f, 0.3f);
            }
        }

        public static void loadSprite()
        {
            if (sprite == null) sprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.LogoButton.png", 100f);
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.ResetScreen))]
    public static class MainMenuClearScreenPatch
    {
        public static void Postfix(MainMenuManager __instance)
        {
            if (MainMenuSetUpPatch.modScreen) MainMenuSetUpPatch.modScreen?.SetActive(false);
            if (MainMenuSetUpPatch.aboutScreen) MainMenuSetUpPatch.aboutScreen?.SetActive(false);
        }
    }
}

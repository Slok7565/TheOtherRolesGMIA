using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Linq;
using static TheOtherRoles.Roles.TheOtherRoles;
using TheOtherRoles.Modules;
using HarmonyLib;
using Hazel;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using System.Threading.Tasks;
using System.Net;
using TheOtherRoles.CustomGameModes;
using Reactor.Utilities.Extensions;
using AmongUs.GameOptions;
using Innersloth.Assets;
using MonoMod.Cil;
using static HarmonyLib.InlineSignature;
using System.Globalization;
using TheOtherRoles.Patches;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using TheOtherRoles.Roles;
using static TheOtherRoles.Helpers.ResourcesHelper;

namespace TheOtherRoles.Helpers
{

    public enum MurderAttemptResult
    {
        PerformKill,
        SuppressKill,
        BlankKill,
        ReverseKill,
        DelayVampireKill
    }

    public enum CustomGamemodes
    {
        Classic,
        Guesser,
        HideNSeek
    }

    public static class Direction
    {
        public static Vector2 up = Vector2.up;
        public static Vector2 down = Vector2.down;
        public static Vector2 left = Vector2.left;
        public static Vector2 right = Vector2.right;
        public static Vector2 upleft = new Vector2(-0.70710677f, 0.70710677f);
        public static Vector2 upright = new Vector2(0.70710677f, 0.70710677f);
        public static Vector2 downleft = new Vector2(-0.70710677f, -0.70710677f);
        public static Vector2 downright = new Vector2(0.70710677f, -0.70710677f);
    }

    public static class OtherHelper
    {


        // Intersteing Color Gradient Feature :)


        public static string cs(Color c, string s)
        {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
        }
        public static bool isLighterColor(int colorId)
        {
            return CustomColors.lighterColors.Contains(colorId);
        }

        public static bool isCustomServer()
        {
            if (FastDestroyableSingleton<ServerManager>.Instance == null) return false;
            StringNames n = FastDestroyableSingleton<ServerManager>.Instance.CurrentRegion.TranslateName;
            return n != StringNames.ServerNA && n != StringNames.ServerEU && n != StringNames.ServerAS;
        }



        public static int lineCount(string text)
        {
            return text.Count(c => c == '\n');
        }

        public static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
        {
            tie = true;
            KeyValuePair<byte, int> result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);
            foreach (KeyValuePair<byte, int> keyValuePair in self)
                if (keyValuePair.Value > result.Value)
                {
                    result = keyValuePair;
                    tie = false;
                }
                else if (keyValuePair.Value == result.Value)
                    tie = true;
            return result;
        }

        public static TMPro.TextMeshPro getFirst(this TMPro.TextMeshPro[] text)
        {
            if (text == null) return null;
            foreach (var self in text)
                if (self.text == "") return self;
            return text[0];
        }

        public static int totalCounts(this TMPro.TextMeshPro[] text)
        {
            if (text == null) return 0;
            int count = 0;
            foreach (var self in text)
                if (self.text != "") count++;
            return count;
        }



        public static string getGithubUrl(this string url)
        {
            if (!isChinese()) return url;
            return url.Replace("https://", "https://github.moeyy.xyz/");
        }

        private static Sprite roleSummaryBackground;
        public static Sprite getRoleSummaryBackground()
        {
            if (roleSummaryBackground != null) return roleSummaryBackground;
            roleSummaryBackground = loadSpriteFromResources("TheOtherRoles.Resources.LobbyRoleInfo.TeamScreen.png", 110f);
            return roleSummaryBackground;
        }

        private static Sprite menuBackground;
        public static Sprite getMenuBackground()
        {
            if (menuBackground != null) return menuBackground;
            menuBackground = loadSpriteFromResources("TheOtherRoles.Resources.LobbyRoleInfo.RoleListScreen.png", 110f);
            return menuBackground;
        }

        public static GameObject CreateObject(string objName, Transform parent, Vector3 localPosition, int? layer = null)
        {
            var obj = new GameObject(objName);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPosition;
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            if (layer.HasValue) obj.layer = layer.Value;
            else if (parent != null) obj.layer = parent.gameObject.layer;
            return obj;
        }

        public static T CreateObject<T>(string objName, Transform parent, Vector3 localPosition, int? layer = null) where T : Component
        {
            return CreateObject(objName, parent, localPosition, layer).AddComponent<T>();
        }

        public static void SetModText(this TextTranslatorTMP text, string translationKey)
        {
            text.TargetText = (StringNames)short.MaxValue;
            text.defaultStr = translationKey;
        }



        public static PlainShipRoom getPlainShipRoom(PlayerControl p)
        {
            PlainShipRoom[] array = null;
            Il2CppReferenceArray<Collider2D> buffer = new Collider2D[10];
            ContactFilter2D filter = default;
            filter.layerMask = Constants.PlayersOnlyMask;
            filter.useLayerMask = true;
            filter.useTriggers = false;
            array = MapUtilities.CachedShipStatus?.AllRooms;
            if (array == null) return null;
            foreach (PlainShipRoom plainShipRoom in array)
                if (plainShipRoom.roomArea)
                {
                    int hitCount = plainShipRoom.roomArea.OverlapCollider(filter, buffer);
                    if (hitCount == 0) continue;
                    for (int i = 0; i < hitCount; i++)
                        if (buffer[i]?.gameObject == p.gameObject)
                            return plainShipRoom;
                }
            return null;
        }

        public static void shareGameVersion()
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.VersionHandshake, SendOption.Reliable, -1);
            writer.Write((byte)TheOtherRolesPlugin.Version.Major);
            writer.Write((byte)TheOtherRolesPlugin.Version.Minor);
            writer.Write((byte)TheOtherRolesPlugin.Version.Build);
            writer.Write(AmongUsClient.Instance.AmHost ? GameStartManagerPatch.timer : -1f);
            writer.WritePacked(AmongUsClient.Instance.ClientId);
            writer.Write((byte)(TheOtherRolesPlugin.Version.Revision < 0 ? 0xFF : TheOtherRolesPlugin.Version.Revision));
            writer.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.versionHandshake(TheOtherRolesPlugin.Version.Major, TheOtherRolesPlugin.Version.Minor, TheOtherRolesPlugin.Version.Build, TheOtherRolesPlugin.Version.Revision, Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
        }

        public static List<PlayerControl> getKillerTeamMembers(PlayerControl player)
        {
            List<PlayerControl> team = new List<PlayerControl>();
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
                if (player.Data.Role.IsImpostor && p.Data.Role.IsImpostor && player.PlayerId != p.PlayerId && team.All(x => x.PlayerId != p.PlayerId)) team.Add(p);
                else if (player == Jackal.jackal && p == Sidekick.sidekick) team.Add(p);
                else if (player == Sidekick.sidekick && p == Jackal.jackal) team.Add(p);

            return team;
        }


        public static bool zoomOutStatus = false;
        public static void toggleZoom(bool reset = false)
        {
            float orthographicSize = reset || zoomOutStatus ? 3f : 12f;

            zoomOutStatus = !zoomOutStatus && !reset;
            Camera.main.orthographicSize = orthographicSize;
            foreach (var cam in Camera.allCameras)
                if (cam != null && cam.gameObject.name == "UI Camera") cam.orthographicSize = orthographicSize;  // The UI is scaled too, else we cant click the buttons. Downside: map is super small.

            if (HudManagerStartPatch.zoomOutButton != null)
            {
                HudManagerStartPatch.zoomOutButton.Sprite = zoomOutStatus ? loadSpriteFromResources("TheOtherRoles.Resources.PlusButton.png", 75f) : loadSpriteFromResources("TheOtherRoles.Resources.MinusButton.png", 150f);
                HudManagerStartPatch.zoomOutButton.PositionOffset = zoomOutStatus ? new Vector3(0f, 3f, 0) : new Vector3(0.4f, 2.8f, 0);
            }
            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen); // This will move button positions to the correct position.
        }

        private static long GetBuiltInTicks()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var builtin = assembly.GetType("Builtin");
            if (builtin == null) return 0;
            var field = builtin.GetField("CompileTime");
            if (field == null) return 0;
            var value = field.GetValue(null);
            if (value == null) return 0;
            return (long)value;
        }

        public static async Task checkBeta()
        {
            if (TheOtherRolesPlugin.betaDays > 0)
            {
                TheOtherRolesPlugin.Logger.LogMessage($"Beta check");
                var ticks = GetBuiltInTicks();
                var compileTime = new DateTime(ticks, DateTimeKind.Utc);  // This may show as an error, but it is not, compilation will work!
                TheOtherRolesPlugin.Logger.LogMessage($"Compiled at {compileTime.ToString(CultureInfo.InvariantCulture)}");
                DateTime? now;
                // Get time from the internet, so no-one can cheat it (so easily).
                try
                {
                    var client = new System.Net.Http.HttpClient();
                    using var response = await client.GetAsync("http://www.google.com/");
                    if (response.IsSuccessStatusCode)
                        now = response.Headers.Date?.UtcDateTime;
                    else
                    {
                        TheOtherRolesPlugin.Logger.LogMessage($"Could not get time from server: {response.StatusCode}");
                        now = DateTime.UtcNow; //In case something goes wrong. 
                    }
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    now = DateTime.UtcNow;
                }
                if ((now - compileTime)?.TotalDays > TheOtherRolesPlugin.betaDays)
                {
                    TheOtherRolesPlugin.Logger.LogMessage($"Beta expired!");
                    BepInExUpdater.MessageBoxTimeout(BepInExUpdater.GetForegroundWindow(), "BETA is expired. You cannot play this version anymore.", "The Other Roles Beta", 0, 0, 10000);
                    Application.Quit();

                }
                else TheOtherRolesPlugin.Logger.LogMessage($"Beta will remain runnable for {TheOtherRolesPlugin.betaDays - (now - compileTime)?.TotalDays} days!");
            }
        }

        public static object TryCast(this Il2CppObjectBase self, Type type)
        {
            return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, Array.Empty<object>());
        }

    }
}

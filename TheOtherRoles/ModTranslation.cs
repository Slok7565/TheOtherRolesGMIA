using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using TheOtherRoles.Patches;
using UnityEngine;
using System;
using static TheOtherRoles.Helpers.OtherHelper;

namespace TheOtherRoles;

public static class ModTranslation
{
    public static int defaultLanguage = (int)SupportedLangs.English;
    public static Dictionary<string, Dictionary<int, string>> stringData;
    private const string blankText = "[BLANK]";

    public static void Load()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Stream stream = assembly.GetManifestResourceStream("TheOtherRoles.Resources.stringData.json");
        var byteArray = new byte[stream.Length];
        var read = stream.Read(byteArray, 0, (int)stream.Length);
        string json = System.Text.Encoding.UTF8.GetString(byteArray);

        stringData = new Dictionary<string, Dictionary<int, string>>();
        JObject parsed = JObject.Parse(json);

        for (int i = 0; i < parsed.Count; i++)
        {
            JProperty token = parsed.ChildrenTokens[i].TryCast<JProperty>();
            if (token == null) continue;

            string stringName = token.Name;
            var val = token.Value.TryCast<JObject>();

            if (token.HasValues)
            {
                var strings = new Dictionary<int, string>();

                for (int j = 0; j < (int)SupportedLangs.Irish + 1; j++)
                {
                    string key = j.ToString();
                    var text = val[key]?.TryCast<JValue>().Value.ToString();

                    if (text != null && text.Length > 0)
                    {
                        if (text == blankText) strings[j] = "";
                        else strings[j] = text;
                    }
                }

                stringData[stringName] = strings;
            }
        }
    }

    public static string getString(string key, string def = null)
    {
        // Strip out color tags.
        string keyClean = Regex.Replace(key, "<.*?>", "");
        keyClean = Regex.Replace(keyClean, "^-\\s*", "");
        keyClean = keyClean.Trim();

        def ??= key;
        if (!stringData.ContainsKey(keyClean))
        {
            return def;
        }

        var data = stringData[keyClean];
        int lang = (int)AmongUs.Data.DataManager.Settings.Language.CurrentLanguage;

        if (data.ContainsKey(lang))
        {
            return key.Replace(keyClean, data[lang]);
        }
        else if (data.ContainsKey(defaultLanguage))
        {
            return key.Replace(keyClean, data[defaultLanguage]);
        }

        return key;
    }

    public static string GetString(this TranslationController t, StringNames key, params Il2CppSystem.Object[] parts)
    {
        return t.GetString(key, parts);
    }

    public static string cs(Color c, string s)
    {
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
    }


    public static bool isChinese()
    {
        try
        {
            var name = CultureInfo.CurrentUICulture.Name;
            if (name.StartsWith("zh")) return true;
            return false;
        }
        catch
        {
            return false;
        }
    }
    public static void DoTransitionFade(this TransitionFade transitionFade, GameObject transitionFrom, GameObject transitionTo, Action onTransition, Action callback)
    {
        if (transitionTo) transitionTo!.SetActive(false);

        System.Collections.IEnumerator Coroutine()
        {
            yield return Effects.ColorFade(transitionFade.overlay, Color.clear, Color.black, 0.1f);
            if (transitionFrom && transitionFrom!.gameObject) transitionFrom.gameObject.SetActive(false);
            if (transitionTo && transitionTo!.gameObject) if (transitionTo != null) transitionTo.gameObject.SetActive(true);
            onTransition.Invoke();
            yield return null;
            yield return Effects.ColorFade(transitionFade.overlay, Color.black, Color.clear, 0.1f);
            callback.Invoke();
            yield break;
        }

        transitionFade.StartCoroutine(Coroutine().WrapToIl2Cpp());
    }

}

[HarmonyPatch(typeof(LanguageSetter), nameof(LanguageSetter.SetLanguage))]
class SetLanguagePatch
{
    static void Postfix()
    {
        ClientOptionsPatch.updateTranslations();
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.Helpers.ResourcesHelper;

namespace TheOtherRoles.Helpers;

public static class PluginHelper
{
    public static void enableCursor(bool initalSetCursor)
    {
        if (initalSetCursor)
        {
            Sprite sprite = loadSpriteFromResources("TheOtherRoles.Resources.Cursor.png", 115f);
            Cursor.SetCursor(sprite.texture, Vector2.zero, CursorMode.Auto);
            return;
        }
        if (TheOtherRolesPlugin.ToggleCursor.Value)
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        else
        {
            Sprite sprite = loadSpriteFromResources("TheOtherRoles.Resources.Cursor.png", 115f);
            Cursor.SetCursor(sprite.texture, Vector2.zero, CursorMode.Auto);
        }
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

}

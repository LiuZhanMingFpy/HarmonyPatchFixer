using System;
using System.Reflection;
using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Modding;

namespace HarmonyPatchFixer;

[ModInitializer("Init")]
public class Entry
{
    public static void Init()
    {
        GD.Print("[HarmonyPatchFixer] Init() called");
        var tree = (SceneTree)Engine.GetMainLoop();
        tree.CreateTimer(2.0).Timeout += ScanAndFix;
    }

    private static void ScanAndFix()
    {
        GD.Print("[HarmonyPatchFixer] --- Starting scan ---");

        int fixedAssemblies = 0;

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            var name = asm.GetName().Name;
            if (name == null) continue;
            if (name == "0Harmony" || name == "sts2" || name == "HarmonyPatchFixer") continue;
            if (name.StartsWith("System.") || name.StartsWith("Microsoft.") || name.StartsWith("Mono.") || name.StartsWith("Godot") || name.StartsWith("netstandard")) continue;
            if (name == "MegaCrit.Sts2.Core" || name == "MegaCrit.Sts2.Core.Modding") continue;

            var loc = asm.Location ?? "";
            if (!loc.Contains("mods")) continue;

            GD.Print($"[HarmonyPatchFixer] Repatching mod assembly: {name}");

            try
            {
                var fixHarmony = new Harmony("com.patchfixer." + name);
                fixHarmony.PatchAll(asm);
                fixedAssemblies++;
                GD.Print($"[HarmonyPatchFixer] PatchAll({name}) succeeded");
            }
            catch (Exception e)
            {
                GD.PrintErr($"[HarmonyPatchFixer] Error patching {name}: {e.GetType().Name}: {e.Message}");
            }
        }

        GD.Print($"[HarmonyPatchFixer] --- Done: repatched {fixedAssemblies} mod assemblies ---");
    }
}

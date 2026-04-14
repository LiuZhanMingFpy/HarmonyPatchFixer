using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        // Build a set of assembly names that already have active Harmony patches
        var alreadyPatched = new HashSet<string>();
        try
        {
            foreach (var method in Harmony.GetAllPatchedMethods())
            {
                try
                {
                    var info = Harmony.GetPatchInfo(method);
                    if (info == null) continue;
                    CollectPatchAssemblies(info.Prefixes, alreadyPatched);
                    CollectPatchAssemblies(info.Postfixes, alreadyPatched);
                    CollectPatchAssemblies(info.Transpilers, alreadyPatched);
                    CollectPatchAssemblies(info.Finalizers, alreadyPatched);
                }
                catch { }
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"[HarmonyPatchFixer] Error scanning existing patches: {e.Message}");
        }

        GD.Print($"[HarmonyPatchFixer] Assemblies with active patches: {string.Join(", ", alreadyPatched)}");

        int skipped = 0;
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

            if (alreadyPatched.Contains(name))
            {
                skipped++;
                GD.Print($"[HarmonyPatchFixer] Skipping {name}: already has active patches");
                continue;
            }

            try
            {
                GD.Print($"[HarmonyPatchFixer] Repatching mod assembly: {name}");
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

        GD.Print($"[HarmonyPatchFixer] --- Done: repatched {fixedAssemblies}, skipped {skipped} ---");
    }

    private static void CollectPatchAssemblies(ReadOnlyCollection<Patch> patches, HashSet<string> result)
    {
        if (patches == null) return;
        foreach (var p in patches)
        {
            try
            {
                var patchAsm = p.PatchMethod?.DeclaringType?.Assembly;
                if (patchAsm != null)
                {
                    var asmName = patchAsm.GetName().Name;
                    if (asmName != null)
                        result.Add(asmName);
                }
            }
            catch { }
        }
    }
}

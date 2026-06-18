using BepInEx.Logging;
using CU_ModSettings.HarmonyPatching.Patches;
using HarmonyLib;
using UnityEngine;
using static HarmonyLib.AccessTools;

namespace CU_ModSettings.HarmonyPatching;

public static class HarmonyPatcher
{
    public static void ApplyPatches(ManualLogSource logger)
    {
        logger.LogInfo("Applying Harmony Patches.");

        Harmony harmony = new(ModSettingsPlugin.MOD_NAME);
        harmony.Patch(Method(typeof(SettingsMenu), nameof(SettingsMenu.SelectTab), [typeof(int)]), postfix: new(typeof(SettingsMenuPatches), nameof(SettingsMenuPatches.SelectTabPatch)));
        harmony.Patch(Method(typeof(SettingsMenu), nameof(SettingsMenu.OpenMenu), [typeof(Transform)]), postfix: new(typeof(SettingsMenuPatches), nameof(SettingsMenuPatches.OpenMenuPatch)));

        harmony.Patch(Method(typeof(Settings), nameof(Settings.DefaultSettings)), postfix: new(typeof(SettingsPatches), nameof(SettingsPatches.DefaultSettingsPatch)));

        harmony.Patch(Method(typeof(Locale), nameof(Locale.LoadLanguage)), postfix: new(typeof(LocalePatches), nameof(LocalePatches.LoadLanguagePatch)));
    }
}

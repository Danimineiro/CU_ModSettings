namespace CU_ModSettings.HarmonyPatching.Patches;

internal class SettingsPatches
{
    internal static void DefaultSettingsPatch(ref List<Setting> __result) => __result.AddRange(ModSettingsPlugin.ModSettingDefaults);
}

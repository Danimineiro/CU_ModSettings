namespace CU_ModSettings.HarmonyPatching.Patches;

internal class LocalePatches
{
    public static void LoadLanguagePatch()
    {
        if (Locale.currentLangName is not "EN") return;

        Dictionary<string, string> other = Locale.currentLang.other;

        other[ModSettingsPlugin.MOD_NAME_TRANSLATION_KEY] = "Mod Settings";
    }
}

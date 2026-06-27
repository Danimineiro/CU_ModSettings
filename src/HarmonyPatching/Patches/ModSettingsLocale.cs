namespace CU_ModSettings.HarmonyPatching.Patches;

public class ModSettingsLocale
{
    public const string TRANSLATION_PREFIX = ModSettingsPlugin.TRANSLATION_PREFIX;

    public const string MOD_VERSION_OUTDATED = $"{TRANSLATION_PREFIX}{nameof(MOD_VERSION_OUTDATED)}";
    public const string MOD_VERSION_NEWEST = $"{TRANSLATION_PREFIX}{nameof(MOD_VERSION_NEWEST)}";
    public const string CHECKOUT_GITHUB = $"{TRANSLATION_PREFIX}{nameof(CHECKOUT_GITHUB)}";
    public const string GITHUB = $"{TRANSLATION_PREFIX}{nameof(GITHUB)}";

    public static void LoadLanguagePatch()
    {
        //if (Locale.currentLangName is not "EN") return;

        Dictionary<string, string> other = Locale.currentLang.other;

        other[ModSettingsPlugin.MOD_NAME_TRANSLATION_KEY] = "Mod Settings";
        other[MOD_VERSION_OUTDATED] = $"Your installed version of {{0}} [<color={Colors.ASE_RED}>{{1}}</color>] is <color={Colors.ASE_RED}>outdated</color>.<br>The newest version is [<color={Colors.ASE_GREEN}>{{2}}.</color>]. Check github for a new version.";
        other[MOD_VERSION_NEWEST] = @$"You're on the <color={Colors.ASE_GREEN}>newest version</color> of {{0}} [<color={Colors.ASE_GREEN}>{{1}}</color>].";
        other[CHECKOUT_GITHUB] = @$"Click the button to check out the latest release of <color={Colors.LIGHT_GRAY}>{{0}}</color> on {{1}}:";
        other[GITHUB] = @$"GitHub";
    }

    public static class Colors
    {
        public const string ASE_GREEN = "#6ABE30";
        public const string ASE_RED = "#AC3232";
        public const string VS_GRAY = "#1F1F1F";
        public const string LIGHT_GRAY = "#8F8F8F";
    }
}

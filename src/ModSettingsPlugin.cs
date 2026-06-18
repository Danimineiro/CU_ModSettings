using BepInEx;
using BepInEx.Logging;
using CU_ModSettings.HarmonyPatching;
using System.Diagnostics.CodeAnalysis;
using static Setting;

namespace CU_ModSettings;

[BepInPlugin("me.danimineiro.modsettings", MOD_NAME, "1.0.0")]
public class ModSettingsPlugin : BaseUnityPlugin
{
    public const string MOD_NAME = "Mod Settings";
    public const string TRANSLATION_PREFIX = "me.danimineiro.modsettings.";
    public const string MOD_NAME_TRANSLATION_KEY = TRANSLATION_PREFIX + "MOD_NAME";

    public static int MOD_SETTINGS_TAB_INDEX { get; } = MOD_NAME.GetHashCode(StringComparison.Ordinal);

    internal static List<Setting> ModSettingDefaults { get; } = [];
    internal static List<string> ModNameTranslationKeys { get; } = [MOD_NAME_TRANSLATION_KEY];

    [AllowNull] internal static ManualLogSource LogSource { get; private set; }

    public void Awake()
    {
        LogSource = Logger;

        Logger.LogInfo("Initializing Mod Settings Mod.");

        HarmonyPatcher.ApplyPatches(Logger);
    }

    /// <summary>
    ///     Adds a settings page to the settings menu.
    /// </summary>
    /// <param name="titleTranslationKey">A translation key. When displayed, it uses <see cref="Locale"/>.GetOther(string str) for the display name.</param>
    /// <param name="settings">
    ///     The settings to be displayed. <see cref="Setting.name"/> is a translation key used by the game.
    ///     Add translation entries to <see cref="Locale"/>.currentLang.other in order to see them.
    /// </param>
    public static void AddModSettingsPageDefaults(string titleTranslationKey, params IEnumerable<Setting> settings)
    {
        ModNameTranslationKeys.Add(titleTranslationKey);

        int modHash = titleTranslationKey.GetHashCode(StringComparison.Ordinal);
        ModSettingDefaults.AddRange(settings.Select(setting =>
        {
            setting.category = (SettingCategory)modHash;
            return setting;
        }));
    }

    internal static int GetModIndexByHash(int hash) => ModNameTranslationKeys.Select(name => name.GetHashCode()).ToArray().IndexOf(hash);
}

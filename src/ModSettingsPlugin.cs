using BepInEx;
using BepInEx.Logging;
using CU_ModSettings.HarmonyPatching;
using CUCoreLib.Data;
using CUCoreLib.Registries;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Setting;

namespace CU_ModSettings;

[BepInPlugin("me.danimineiro.modsettings", MOD_NAME, "1.0.1.0")]
public class ModSettingsPlugin : BaseUnityPlugin
{
    public const string AUTHOR = "danimineiro";
    public const string REPOSITORY = "CU_ModSettings";
    public const string MOD_NAME = "Mod Settings";
    public const string TRANSLATION_PREFIX = "me.danimineiro.modsettings.";
    public const string MOD_NAME_TRANSLATION_KEY = TRANSLATION_PREFIX + "MOD_NAME";

    public static int MOD_SETTINGS_TAB_INDEX { get; } = MOD_NAME_TRANSLATION_KEY.GetHashCode(StringComparison.Ordinal);

    internal static List<Setting> ModSettingDefaults { get; } = [];
    internal static List<string> ModNameTranslationKeys { get; } = [MOD_NAME_TRANSLATION_KEY];

    [AllowNull] internal static ManualLogSource LogSource { get; private set; }

    public static Version? NewVersion { get; set; } //= new(1, 1, 1, 1);
    public static bool NewVersionAvailable => Assembly.GetExecutingAssembly().GetName().Version < (NewVersion ?? new Version(0, 0, 0, 0));

    public void Awake()
    {
        LogSource = Logger;

        Logger.LogMessage($"Initializing Mod Settings Mod [{Assembly.GetExecutingAssembly().GetName().Version}].");

        HarmonyPatcher.ApplyPatches(Logger);

        Task.Run(async () =>
        {
            try
            {
                Logger.LogInfo("Checking for new version information.");
                if ((await GitHubVersionChecker.TryGetNewestVersionInformation(AUTHOR, REPOSITORY)) is not Version version || Assembly.GetExecutingAssembly().GetName().Version >= version)
                {
                    Logger.LogMessage("You have the latest version installed.");
                    return; 
                }

                Logger.LogMessage($"Found new version: [{version}].");
                Logger.LogMessage($"You can download the new version from {GitHubVersionChecker.GetGithubLinkFor(AUTHOR, REPOSITORY)}");

                NewVersion = version;
            } 
            catch (Exception ex)
            {
                Logger.LogError($"Failed when attempting to gather new version information. {ex}");
            }
        });
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

    internal static int GetModIndexByHash(int hash) => ModNameTranslationKeys.Select(name => name.GetHashCode(StringComparison.Ordinal)).ToArray().IndexOf(hash);

    public static bool TryGetNewVersion([NotNullWhen(true)]out Version? version)
    {
        if (!NewVersionAvailable)
        {
            version = null;
            return false;
        }

        if (NewVersion is Version newVersion)
        {
            version = newVersion;
            return true;
        }

        version = null;
        return false;
    }

    #region Logging Shortcuts
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Log(LogLevel logLevel, object data) => LogSource.Log(logLevel, data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void LogFatal(object data) => LogSource.Log(LogLevel.Fatal, data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void LogError(object data) => LogSource.Log(LogLevel.Error, data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void LogWarning(object data) => LogSource.Log(LogLevel.Warning, data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void LogMessage(object data) => LogSource.Log(LogLevel.Message, data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void LogInfo(object data) => LogSource.Log(LogLevel.Info, data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void LogDebug(object data) => LogSource.Log(LogLevel.Debug, data);
    #endregion Logging Shortcuts
}

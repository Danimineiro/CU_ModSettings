# ModSettingsLib <br>
**Requirements:** BepInEx <br>
**Nexus Mods:** TBD <br>
**Nexus Mods:** TBD 

--- 
## Description
ModSettingsLib is a small mod to help create and unify settings menus for other mods in **Casualties Unknown**.

> **Note for players:** This is a core dependency library. It does not add any
> content on its own, but is required by other mods to function.

---
## Installation
1. Ensure you have **BepInEx 5** installed for Casualties Unknown.
2. Download [the latest release here.](https://github.com/Danimineiro/CU_ModSettings/releases/latest).
3. Extract the folder within the the zip file into the game directory under `BepInEx/plugins/`.
4. Launch the game. Within the games settings menu, you should be able to find a new tab called "Mod Settings".

---
## For Developers
Implement a settings page for your mod as follows. <br>
Within in your plugins `Awake()` function, define the default entries for your mods setting pages:
```cs
using CU_ModSettings;

public void Awake()
{
    ModSettingsPlugin.AddModSettingsPageDefaults("Dani.ExampleModName", new SettingBool
    {
        name = "Dani.ExampleConsume",
        value = true
    },
    new SettingBool
    {
        name = "Dani.ExampleSure",
        value = true
    },
    new SettingInt
    {
        name = "Dani.ExampleHowMuch",
        value = 1,
        min = 0,
        max = 255
    });
}
```

### Translation keys
This mod uses the games translation system in order to determine the display strings for settings pages. <br>
The `Setting.name` field is therefor a translation key. You will have to register translation keys in the `Locale` class.
```cs
// Example for adding translation keys for the mod settings page entry above
internal class LocalePatches
{
    public static void LoadLanguagePatch()
    {
        // Include this check to avoid overwriting modded translations for your mod
        if (Locale.currentLangName is not "EN") return;

        Dictionary<string, string> other = Locale.currentLang.other;

        other["Dani.ExampleModName"] = "Example Mod";
          
        // The game prefixes settings translationkeys with "gameset" and
        // postfixes the description keys with "dsc"
        other["gamesetDani.ExampleConsume"] = "Consume example";
        other["gamesetDani.ExampleConsumedsc"] = "Determines if you want to be able to consume.";

        other["gamesetDani.ExampleSure"] = "Are you sure?";
        other["gamesetDani.ExampleSuredsc"] = "Checks to see if you are really sure to consume it.";

        other["gamesetDani.ExampleHowMuch"] = "How much do you want to consume?";
        other["gamesetDani.ExampleHowMuchdsc"] = "Asks how much you want to consume.";
    }
}

public static class HarmonyPatcher
{
    // Call this function in your plugins Awake() function.
    public static void ApplyPatches()
    {
        Harmony harmony = new("Example Mod");
        harmony.Patch(Method(typeof(Locale), nameof(Locale.LoadLanguage)), 
            postfix: new(typeof(LocalePatches), nameof(LocalePatches.LoadLanguagePatch)));
    }
}
```

### The games `Setting` classes
- `name` is a translation key
- `value` holds the saved or the default value
- `apply` can be given a lambda, that runs immediately when the setting was changed in the settings menu

> **Note:** Some `Setting` classes like `SettingInt` have fields like `min` and `max` which default to 0.
> If you don't assign them, players can never set the value to anything other than 0.
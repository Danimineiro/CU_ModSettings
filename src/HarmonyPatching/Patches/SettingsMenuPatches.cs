using CU_ModSettings.UserInterface;
using HarmonyLib;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Setting;

namespace CU_ModSettings.HarmonyPatching.Patches;

internal class SettingsMenuPatches
{
    private static List<GameObject> SpawnedObjects { get; } = [];

    #region Patches
    internal static void SelectTabPatch(int index)
    {
        // Kill dropdown seperately since we never add it to spawnedSettings
        SpawnedObjects.ForEach(UnityEngine.Object.Destroy);
        SpawnedObjects.Clear();

        if (Enum.GetValues(typeof(SettingCategory)).OfType<SettingCategory>().Contains((SettingCategory)index)) return;

        // Set Mod Settings sprite to open tab sprite
        SettingsMenu.instance.buttons[^1].GetComponent<Image>().sprite = SettingsMenu.instance.buttonOpen;

        DrawDropDown(index, out float dropDownHeight);
        DrawModSettingsInfo(index, dropDownHeight);
    }

    internal static void OpenMenuPatch()
    {
        TMP_DefaultControls.Resources resources = new()
        {
            standard = SettingsMenu.instance.buttonOpen
        };

        List<Button> buttons = SettingsMenu.instance.buttons;
        RectTransform firstTransform = (RectTransform)buttons[0].transform;

        GameObject newButton = TMP_DefaultControls.CreateButton(resources);
        TextMeshProUGUI newButtonTextMesh = newButton.GetComponentInChildren<TextMeshProUGUI>();
        newButtonTextMesh.text = ModSettingsPlugin.MOD_NAME;
        TextMeshProUGUI textMeshProUGUI = buttons[0].GetComponentInChildren<TextMeshProUGUI>();

        newButtonTextMesh.ApplyFontSettings();

        newButton.transform.SetParent(firstTransform.parent, false);
        newButton.GetComponent<Image>().sprite = SettingsMenu.instance.buttonOpen;

        float buttonMargin = Math.Abs(firstTransform.localPosition.x + firstTransform.rect.width - buttons[1].transform.localPosition.x);

        float totalWidth = buttons.Sum(button => ((RectTransform)button.transform).rect.width) - buttonMargin;
        float newButtonWidth = totalWidth / (buttons.Count + 1);

        for (int buttonIndex = 0; buttonIndex < buttons.Count; buttonIndex++)
        {
            Button button = buttons[buttonIndex];
            RectTransform transform = (RectTransform)button.transform;

            float widthDiff = transform.rect.width - newButtonWidth;
            transform.sizeDelta = new Vector2(newButtonWidth, transform.sizeDelta.y);

            // the buttons are weirdly positioned based on their center so this math moves them left by how much thinner they got
            // the '+ widthDiff / 2' at the end aligns them at the original edge
            float newLocalXPos = transform.localPosition.x - widthDiff * (buttonIndex + 1f) + widthDiff / 2f;
            transform.localPosition = new Vector3(newLocalXPos, transform.localPosition.y, transform.localPosition.z);
        }

        RectTransform newButtonTransform = (RectTransform)newButton.transform;
        newButtonTransform.sizeDelta = firstTransform.sizeDelta;

        Vector3 lastLocalPosition = buttons[^1].transform.localPosition;
        newButtonTransform.localPosition = new Vector3(lastLocalPosition.x + newButtonTransform.rect.width + buttonMargin, lastLocalPosition.y, lastLocalPosition.z);

        Button buttonComponent = newButton.GetComponent<Button>();
        buttons.Add(buttonComponent);

        buttonComponent.onClick.AddListener(() => SettingsMenu.instance.SelectTab(ModSettingsPlugin.MOD_SETTINGS_TAB_INDEX));
    }
    #endregion Patches

    private static void DrawModSettingsInfo(int index, float dropDownHeight)
    {
        if (ModSettingsPlugin.GetModIndexByHash(index) is not 0) return;

        string content = GetVersionText();
        GameObject label = UIHelper.CreateLabel(SettingsMenu.instance.content, content, dropDownHeight, 0f);

        SpawnedObjects.Add(label);
    }

    private static string GetVersionText()
    {
        Version current = Assembly.GetExecutingAssembly().GetName().Version;
        string modName = Locale.GetOther(ModSettingsPlugin.MOD_NAME_TRANSLATION_KEY);

        if (ModSettingsPlugin.TryGetNewVersion(out Version? version))
        {
            return string.Format(Locale.GetOther(ModSettingsLocale.MOD_VERSION_OUTDATED), modName, current, version);
        }

        return string.Format(Locale.GetOther(ModSettingsLocale.MOD_VERSION_NEWEST), modName, current);
    }

    private static void DrawDropDown(int index, out float dropDownHeight)
    {
        // Create Mod selection drop down
        CreateModSelectionDropDown(index, out dropDownHeight);

        // Push down created setting items by the height of the drop down
        List<GameObject> spawnedSettings = Traverse.Create(SettingsMenu.instance).Field("spawnedSettings").GetValue<List<GameObject>>();
        foreach (GameObject spawnedSetting in spawnedSettings)
        {
            RectTransform rectTransform = spawnedSetting.RectTransform;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y - dropDownHeight);
        }

        // Increase height of settings menu to fix scrolling issue
        SettingsMenu.instance.content.sizeDelta += new Vector2(0f, dropDownHeight);
    }

    private static void CreateModSelectionDropDown(int index, out float dropDownHeight)
    {
        GameObject dropDown = Utils.Create("Special/GameSettingDropdown", SettingsMenu.instance.content);
        SpawnedObjects.Add(dropDown);

        List<TMP_Dropdown.OptionData> options = [.. ModSettingsPlugin.ModNameTranslationKeys
            .Select(translationKey => Locale.GetOther(translationKey))
            .Select(modName => new TMP_Dropdown.OptionData(modName))];

        TMP_Dropdown component = dropDown.transform.GetChild(1).GetComponent<TMP_Dropdown>();
        component.AddOptions(options);

        component.SetValueWithoutNotify(ModSettingsPlugin.GetModIndexByHash(index));
        component.onValueChanged.AddListener((index) =>
        {
            SettingsMenu.instance?.SelectTab(ModSettingsPlugin.ModNameTranslationKeys[index].GetHashCode(StringComparison.Ordinal));
        });

        dropDown.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Select Mod";
        UITooltip uITooltip = dropDown.transform.GetChild(0).gameObject.AddComponent<UITooltip>();
        uITooltip.skipLocale = true;
        uITooltip.tipName = "Select which Mods settings you want to change.";

        dropDown.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f - dropDown.GetComponent<RectTransform>().sizeDelta.y * 0.5f);
        dropDownHeight = dropDown.GetComponent<RectTransform>().sizeDelta.y;
    }
}

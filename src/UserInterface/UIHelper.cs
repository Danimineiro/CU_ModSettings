using System.ComponentModel;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CU_ModSettings.UserInterface;

public static class UIHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ZeroRectTransform(this RectTransform transform)
    {
        transform.anchorMin = Vector2.zero;
        transform.anchorMax = Vector2.one;
        transform.sizeDelta = Vector2.zero;
    }

    public static GameObject CreateUIObject(string name, GameObject parent) => CreateUIObject(name, parent.transform);

    public static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject gameObject = new(name);
        gameObject.AddComponent<RectTransform>();
        gameObject.transform.SetParent(parent, false);
        gameObject.RectTransform.ZeroRectTransform();

        return gameObject;
    }

    private static TVar GetFromSettingsMenu<TVar>(Func<SettingsMenu, TVar> variableGetter)
    {

        bool instanceExists = SettingsMenu.instance != null;
        SettingsMenu menu = instanceExists ? SettingsMenu.instance! : Resources.Load<SettingsMenu>("Special/SettingsMenu");

        try
        {
            return variableGetter(menu);
        }
        finally
        {
            if (!instanceExists) UnityEngine.Object.Destroy(menu);
        }
    }

    public static GameObject CreateLabel(RectTransform parent, string content, float labelHeight = 32f, float verticalMargin = 3f)
    {
        GameObject spriteObject = CreateUIObject(nameof(spriteObject), parent);

        Image image = spriteObject.AddComponent<Image>();
        image.sprite = GetFromSettingsMenu(menu => menu.buttonClosed);
        image.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        image.type = Image.Type.Sliced;

        GameObject textObject = CreateUIObject(nameof(spriteObject), parent);
        ContentSizeFitter contentSizeFitter = textObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        TextMeshProUGUI textMesh = textObject.AddComponent<TextMeshProUGUI>();
        textMesh.text = content;
        textMesh.ApplyFontSettings();

        RectTransform textRectTransform = textObject.RectTransform;
        textRectTransform.SetParent(spriteObject.transform, false);
        textRectTransform.ZeroRectTransform();
        textRectTransform.sizeDelta += new Vector2(-24, 0);

        spriteObject.RectTransform.anchoredPosition = new Vector2(textRectTransform.anchoredPosition.x, - (labelHeight + verticalMargin));

        return textObject;
    }
}

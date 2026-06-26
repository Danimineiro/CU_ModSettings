using System.ComponentModel;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
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

    public static GameObject CreateRectangle(RectTransform parent, float heightOffset, float verticalMargin = 0f, Color? rectangleColor = null)
    {
        GameObject spriteObject = CreateUIObject(nameof(spriteObject), parent);

        Image image = spriteObject.AddComponent<Image>();
        image.sprite = GetFromSettingsMenu(menu => menu.buttonClosed);
        image.color = rectangleColor ?? new Color(0.3f, 0.3f, 0.3f, 1f);
        image.type = Image.Type.Sliced;

        spriteObject.RectTransform.anchoredPosition = new Vector2(parent.anchoredPosition.x, -(heightOffset + verticalMargin));

        return spriteObject;
    }

    public static GameObject CreateLabel(GameObject gameObj, string content, float heightOffset = 0f, float verticalMargin = 0f) =>
        CreateLabel(gameObj.RectTransform, content, heightOffset, verticalMargin);

    public static GameObject CreateLabel(RectTransform parent, string content, float heightOffset = 32f, float verticalMargin = 0f)
    {
        GameObject textObject = CreateUIObject(nameof(textObject), parent);
        textObject.RectTransform.sizeDelta += new Vector2(-24, 0);

        ContentSizeFitter contentSizeFitter = textObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        TextMeshProUGUI textMesh = textObject.AddComponent<TextMeshProUGUI>();
        textMesh.text = content;
        textMesh.ApplyFontSettings();

        textObject.RectTransform.anchoredPosition = new Vector2(parent.anchoredPosition.x, -(heightOffset + verticalMargin));

        return textObject;
    }

    public static GameObject CreateLabelWithBorder(RectTransform parent, string content, float heightOffset = 32f, float verticalMargin = 0f, Color? borderColor = null)
    {
        GameObject spriteObject = CreateRectangle(parent, heightOffset, verticalMargin, borderColor);
        GameObject textObject = CreateLabel(spriteObject, content);

        return textObject;
    }
}

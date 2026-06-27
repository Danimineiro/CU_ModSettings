using System.Reflection;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace CU_ModSettings.UserInterface;

public static class UIHelper
{
    private static Color RectDefaultColor => new(0.3f, 0.3f, 0.3f, 1f);

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ZeroRectTransform(this RectTransform transform)
    {
        transform.anchorMin = Vector2.zero;
        transform.anchorMax = Vector2.one;
        transform.sizeDelta = Vector2.zero;
    }

    private static void SetDefaultColorTransitionValues(Selectable selectable)
    {
        ColorBlock colors = selectable.colors;
        colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
        colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
        colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
    }

    public static GameObject CreateUIObject(string name, GameObject parent) => CreateUIObject(name, parent.RectTransform);

    public static GameObject CreateUIObject(string name, RectTransform parent, float heightOffset = 0f, float verticalMargin = 0f)
    {
        GameObject gameObject = new(name);
        gameObject.AddComponent<RectTransform>();
        gameObject.transform.SetParent(parent, false);

        RectTransform rectTransform = gameObject.RectTransform;
        rectTransform.ZeroRectTransform();

        rectTransform.anchoredPosition = new(parent.anchoredPosition.x, -(heightOffset + verticalMargin));

        return gameObject;
    }

    public static GameObject CreateRectangle(GameObject parent, float heightOffset = 0f, float verticalMargin = 0f, Color? rectangleColor = null) =>
        CreateRectangle(parent.RectTransform, heightOffset, verticalMargin, rectangleColor);

    public static GameObject CreateRectangle(RectTransform parent, float heightOffset = 0f, float verticalMargin = 0f, Color? rectangleColor = null)
    {
        GameObject spriteObject = CreateUIObject(nameof(spriteObject), parent);

        AttachRectangleSprite(spriteObject, rectangleColor);

        spriteObject.RectTransform.anchoredPosition = new Vector2(parent.anchoredPosition.x, -(heightOffset + verticalMargin));

        return spriteObject;
    }

    public static GameObject CreateLabel(GameObject gameObj, string content, TextAlignmentOptions? alignment = null, float heightOffset = 0f, float verticalMargin = 0f) =>
        CreateLabel(gameObj.RectTransform, content, alignment, heightOffset, verticalMargin);

    public static GameObject CreateLabel(RectTransform parent, string content, TextAlignmentOptions? alignment = null, float heightOffset = 0f, float verticalMargin = 0f)
    {
        GameObject textObject = CreateUIObject(nameof(textObject), parent);
        textObject.RectTransform.sizeDelta += new Vector2(-24, 0);

        ContentSizeFitter contentSizeFitter = textObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        TextMeshProUGUI textMesh = textObject.AddComponent<TextMeshProUGUI>();
        textMesh.text = content;        
        if (alignment is not null) textMesh.alignment = alignment.Value;
        textMesh.ApplyFontSettings();

        textObject.RectTransform.anchoredPosition = new Vector2(parent.anchoredPosition.x, -(heightOffset + verticalMargin));

        return textObject;
    }

    public static GameObject CreateButton(GameObject gameObj, string content, UnityAction? buttonAction = null, TextAlignmentOptions? alignment = null, bool addClickSound = true, float heightOffset = 0f, float verticalMargin = 0f, Color? buttonColor = null) =>
        CreateButton(gameObj.RectTransform, content, buttonAction, alignment, addClickSound, heightOffset, verticalMargin, buttonColor);

    public static GameObject CreateButton(RectTransform parent, string content, UnityAction? buttonAction = null, TextAlignmentOptions? alignment = null, bool addClickSound = true, float heightOffset = 0f, float verticalMargin = 0f, Color? buttonColor = null)
    {
        GameObject rootObject = CreateUIObject(nameof(rootObject), parent, heightOffset, verticalMargin);
        GameObject buttonObject = CreateUIObject(nameof(buttonObject), rootObject);

        GameObject labelObject = CreateUIObject(nameof(labelObject), buttonObject);
        AttachRectangleSprite(buttonObject, buttonColor);

        CreateLabel(labelObject, content, alignment);
        Button button = buttonObject.AddComponent<Button>();
        SetDefaultColorTransitionValues(button);

        if (buttonAction is not null) button.onClick.AddListener(buttonAction);
        if (addClickSound) button.onClick.AddListener(() => PlayerCamera.PlayUISound("miniClick"));

        return rootObject;
    }

    public static GameObject CreateButtonWithImage(GameObject gameObj, string content, Sprite sprite, float imageSize, UnityAction? buttonAction = null, TextAlignmentOptions? alignment = null, bool addClickSound = true, float heightOffset = 0f, float verticalMargin = 0f, Color? buttonColor = null) =>
        CreateButtonWithImage(gameObj.RectTransform, content, sprite, imageSize, buttonAction, alignment, addClickSound, heightOffset, verticalMargin, buttonColor);

    public static GameObject CreateButtonWithImage(RectTransform parent, string content, Sprite sprite, float imageSize, UnityAction? buttonAction = null, TextAlignmentOptions? alignment = null, bool addClickSound = true, float heightOffset = 0f, float verticalMargin = 0f, Color? buttonColor = null)
    {
        GameObject rootObject = CreateUIObject(nameof(rootObject), parent, heightOffset, verticalMargin);
        GameObject buttonObject = CreateUIObject(nameof(buttonObject), rootObject);

        AttachRectangleSprite(buttonObject, buttonColor);

        const float margin = 12f;
        float percentageForImage = (imageSize + margin) / buttonObject.RectTransform.rect.width;

        SplitRectTransformHorizontally(buttonObject, percentageForImage, out GameObject imageObject, out GameObject labelObject);

        imageObject.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imageSize);
        imageObject.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, imageSize);
        imageObject.AddComponent<Image>().sprite = sprite;

        CreateLabel(labelObject, content, alignment);
        Button button = buttonObject.AddComponent<Button>();
        SetDefaultColorTransitionValues(button);

        if (buttonAction is not null) button.onClick.AddListener(buttonAction);
        if (addClickSound) button.onClick.AddListener(() => PlayerCamera.PlayUISound("miniClick"));

        return rootObject;
    }

    public static GameObject CreateLabelWithBorder(GameObject parent, string content, TextAlignmentOptions? alignment = null, float heightOffset = 0f, float verticalMargin = 0f, Color? borderColor = null) =>
        CreateLabelWithBorder(parent.RectTransform, content, alignment, heightOffset, verticalMargin, borderColor);

    public static GameObject CreateLabelWithBorder(RectTransform parent, string content, TextAlignmentOptions? alignment = null, float heightOffset = 0f, float verticalMargin = 0f, Color? borderColor = null)
    {
        GameObject rootObject = CreateUIObject(nameof(rootObject), parent, heightOffset, verticalMargin);

        CreateRectangle(rootObject, rectangleColor: borderColor);
        CreateLabel(rootObject, content, alignment);

        return rootObject;
    }

    private static void ResetAnchorPositon(GameObject gameObject, RectTransform parent, float heightOffset = 0f, float verticalMargin = 0f)
        => ResetAnchorPositon(gameObject.RectTransform, parent, heightOffset, verticalMargin);

    private static void ResetAnchorPositon(RectTransform rectTransform, RectTransform parent, float heightOffset = 0f, float verticalMargin = 0f)
    {
        rectTransform.anchoredPosition = new Vector2(parent.anchoredPosition.x, -(heightOffset + verticalMargin));
    }

    private static void AttachRectangleSprite(GameObject buttonObject, Color? buttonColor = null)
    {
        Image image = buttonObject.AddComponent<Image>();
        image.sprite = GetFromSettingsMenu(menu => menu.buttonClosed);
        image.color = buttonColor ?? RectDefaultColor;
        image.type = Image.Type.Sliced;
    }

    public static void SplitRectTransformHorizontally(GameObject parent, float leftSplitPercentage, out GameObject leftObject, out GameObject rightObject) =>
        SpitRectTransformHorizontally(parent.RectTransform, leftSplitPercentage, out leftObject, out rightObject);

    public static void SpitRectTransformHorizontally(RectTransform parent, float leftSplitPercentage, out GameObject leftObject, out GameObject rightObject)
    {
        leftObject = CreateUIObject(nameof(leftObject), parent);
        rightObject = CreateUIObject(nameof(leftObject), parent);

        RectTransform leftRectTransform = leftObject.RectTransform;
        RectTransform rightRectTransform = rightObject.RectTransform;

        leftRectTransform.anchorMax = new(leftSplitPercentage, 1f);
        rightRectTransform.anchorMin = new(leftSplitPercentage, 0f);
    }

    public static Sprite ConvertToSprite(this Texture2D texture) => Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new(.5f, .5f));

    internal static Texture2D GetTexture(string path) => GetTexture2DFromAssembly(Assembly.GetExecutingAssembly(), path);

    public static Texture2D GetTexture2DFromAssembly(Assembly assembly, string path)
    {
        using Stream stream = assembly.GetManifestResourceStream(path);
        Texture2D texture = new(0, 0, TextureFormat.RGBA32, false);

        byte[] data = new byte[stream.Length];
        _ = stream.Read(data, 0, (int)stream.Length);
        texture.LoadImage(data);
        return texture;
    }

    //public static void SplitRectTransformVertically(GameObject parent, float topSplitPercentage, out GameObject topObject, out GameObject bottomObject) =>
    //    SplitRectTransformVertically(parent.RectTransform, topSplitPercentage, out topObject, out bottomObject);

    //public static void SplitRectTransformVertically(RectTransform parent, float topSplitPercentage, out GameObject topObject, out GameObject bottomObject)
    //{
    //    topObject = CreateUIObject(nameof(topObject), parent);
    //    bottomObject = CreateUIObject(nameof(topObject), parent);

    //    RectTransform topRectTransform = topObject.RectTransform;
    //    RectTransform bottomRectTransform = bottomObject.RectTransform;

    //    float topSideHeight = -parent.rect.height + (parent.rect.height * topSplitPercentage);
    //    float bottomSideHeight = -parent.rect.height + (parent.rect.height * (1 - topSplitPercentage));

    //    topRectTransform.sizeDelta = new(0f, topSideHeight);
    //    bottomRectTransform.sizeDelta = new(0f, bottomSideHeight);

    //    topRectTransform.anchoredPosition = new(0f, topSideHeight / 4f);
    //    bottomRectTransform.anchoredPosition = new(0f, -bottomSideHeight / 4f);
    //}
}

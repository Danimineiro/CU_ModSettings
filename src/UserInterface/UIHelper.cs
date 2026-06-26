using TMPro;
using UnityEngine;

namespace CU_ModSettings.UserInterface;

public static class UIHelper
{
    public static GameObject CreateLabel(RectTransform parent, string content, float height = 32f, float verticalMargin = 3f, float horizontalMargin = 4f)
    {
        TMP_DefaultControls.Resources resources = new()
        {
            standard = SettingsMenu.instance.buttonClosed
        };

        GameObject textObject = TMP_DefaultControls.CreateText(resources);

        textObject.Text = content;
        textObject.TextMesh.ApplyFontSettings();

        RectTransform rectTransform = textObject.RectTransform;
        rectTransform.SetParent(parent, false);
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + horizontalMargin, rectTransform.anchoredPosition.y - height - verticalMargin);
        rectTransform.sizeDelta = new Vector2(parent.rect.width - horizontalMargin * 2, height - verticalMargin);

        return textObject;
    }
}

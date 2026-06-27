using TMPro;
using UnityEngine;

namespace CU_ModSettings.UserInterface;

public static class Extensions
{
    extension(Transform transform)
    {
        public RectTransform RectTransform => transform.GetComponent<RectTransform>();
    }

    extension(GameObject gameObject)
    {
        public string Text
        {
            get
            {
                TextMeshProUGUI textMesh = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                return textMesh.text;
            }
            set
            {
                TextMeshProUGUI textMesh = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                textMesh.text = value;
            }
        }

        public TextMeshProUGUI TextMesh => gameObject.GetComponentInChildren<TextMeshProUGUI>();

        public RectTransform RectTransform => gameObject.GetComponent<RectTransform>();
    }

    extension(TextMeshProUGUI textMesh)
    {
        public void ApplyFontSettings(TextMeshProUGUI sourceMesh)
        {
            textMesh.fontMaterial = sourceMesh.fontMaterial;
            textMesh.color = sourceMesh.color;
            textMesh.characterSpacing = sourceMesh.characterSpacing;
            textMesh.enableAutoSizing = sourceMesh.enableAutoSizing;
            textMesh.font = sourceMesh.font;
            textMesh.fontSize = sourceMesh.fontSize;
            textMesh.fontSizeMax = sourceMesh.fontSizeMax;
            textMesh.fontSizeMin = sourceMesh.fontSizeMin;
        }

        public void ApplyFontSettings()
        {
            GameObject sourceObj = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Special/GameSettingFloat"));
            ApplyFontSettings(textMesh, sourceObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>());
            UnityEngine.Object.Destroy(sourceObj);
        }
    }
}

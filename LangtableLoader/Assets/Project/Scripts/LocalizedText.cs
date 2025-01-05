using UnityEngine;
using UnityEngine.UI;
using TMPro;

[AddComponentMenu("UI/Localized Text")]
public class LocalizedText : MonoBehaviour
{
    [Tooltip("텍스트 데이터 설정\n- TextObject: 텍스트를 표시할 UI 오브젝트\n- fileName: CSV 파일 이름 (비워두면 기본 언어 파일 사용)\n- lineNumber: CSV 파일의 줄 번호")]
    public LocalizedTextData[] data;

    void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        foreach (LocalizedTextData textData in data)
        {
            string localizedText = string.IsNullOrEmpty(textData.fileName) ?
                UIManager.Instance.GetText(textData.lineNumber) :
                UIManager.Instance.GetText(textData.fileName, textData.lineNumber);

            var uiText = textData.TextObject.GetComponent<Text>();
            var tmpText = textData.TextObject.GetComponent<TextMeshProUGUI>();

            if (uiText != null)
                uiText.text = localizedText;
            if (tmpText != null)
                tmpText.text = localizedText;
        }
    }
}

[System.Serializable]
public struct LocalizedTextData
{
    [Tooltip("텍스트를 표시할 UI 오브젝트 (Text 또는 TextMeshProUGUI 컴포넌트가 있어야 함)")]
    public GameObject TextObject;

    [Tooltip("사용할 CSV 파일 이름 (비워두면 기본 언어 파일 사용)")]
    public string fileName;

    [Tooltip("CSV 파일에서 가져올 텍스트의 줄 번호 (1부터 시작)")]
    public int lineNumber;
}

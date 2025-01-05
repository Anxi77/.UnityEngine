using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    [SerializeField] private DataManager.DataFile[] LanguageFile;
    private Dictionary<string, List<string>> processedData = new Dictionary<string, List<string>>();

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("UIManager");
                instance = go.AddComponent<UIManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        InitializeLanguageSystem();
    }

    private void InitializeLanguageSystem()
    {
        foreach (var langFile in LanguageFile)
        {
            CSVManager.Instance.LoadAdditionalCSV(langFile.path);
            LoadLanguageData(langFile.path);
        }
    }

    private void LoadLanguageData(string path)
    {
        List<string[]> csvData = CSVManager.Instance.GetDataSet(path);
        if (csvData == null)
        {
            Debug.LogError($"언어 데이터를 불러오는데 실패했습니다: {path}");
            return;
        }

        List<string> langData = new List<string>();
        foreach (string[] row in csvData)
            langData.Add(row.Length >= 3 ? row[2] : "");

        processedData[path] = langData;
    }

    public string GetText(string fileName, int lineNumber)
    {
        var langFile = LanguageFile.FirstOrDefault(file => file.path.Contains(fileName));
        if (langFile.path == null)
        {
            Debug.LogError($"언어 파일을 찾을 수 없습니다: {fileName}");
            return "Missing Text";
        }

        int index = lineNumber - 1;
        if (processedData.ContainsKey(langFile.path) &&
            index >= 0 &&
            index < processedData[langFile.path].Count)
        {
            return processedData[langFile.path][index];
        }
        return "Missing Text";
    }

    public string GetText(int lineNumber)
    {
        if (LanguageFile == null || LanguageFile.Length == 0)
        {
            Debug.LogError("설정된 언어 파일이 없습니다.");
            return "Missing Text";
        }

        return GetText(LanguageFile[0].path, lineNumber);
    }

    public void UpdateAllLocalizedTexts()
    {
        foreach (LocalizedText text in FindObjectsOfType<LocalizedText>())
            text.UpdateText();
    }

    public void ReloadLanguageData()
    {
        foreach (var langFile in LanguageFile)
            LoadLanguageData(langFile.path);
        UpdateAllLocalizedTexts();
    }
}
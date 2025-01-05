using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class CSVManager : DataManager
{
    private static CSVManager instance;
    public static CSVManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("CSVManager");
                instance = go.AddComponent<CSVManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    public override void LoadData()
    {
        if (dataFiles.Count == 0)
        {
            Debug.LogWarning("로드할 CSV 파일이 설정되지 않았습니다.");
            return;
        }

        foreach (DataFile file in dataFiles)
            LoadCSVFromResources(file.path);
    }

    protected override string ProcessPath(string fullPath)
    {
        string resourcePath = fullPath;
        int resourcesIndex = fullPath.IndexOf("Resources/");

        if (resourcesIndex != -1)
            resourcePath = fullPath.Substring(resourcesIndex + 10);

        if (resourcePath.EndsWith(".csv", System.StringComparison.OrdinalIgnoreCase))
            resourcePath = resourcePath.Substring(0, resourcePath.Length - 4);

        return resourcePath;
    }

    public List<string[]> LoadCSVFromResources(string fullPath)
    {
        string resourcePath = ProcessPath(fullPath);
        List<string[]> csvData = new List<string[]>();

        TextAsset csvFile = Resources.Load<TextAsset>(resourcePath);
        if (csvFile == null)
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다. 경로: {fullPath}");
            return csvData;
        }

        string[] lines = Regex.Split(csvFile.text, "\r\n|\n|\r");
        foreach (string line in lines)
        {
            if (!string.IsNullOrEmpty(line))
                csvData.Add(line.Split(','));
        }

        csvDataSets[resourcePath] = csvData;
        return csvData;
    }

    public bool LoadAdditionalCSV(string fullPath)
    {
        string resourcePath = ProcessPath(fullPath);
        dataFiles.Add(new DataFile { path = resourcePath });
        List<string[]> data = LoadCSVFromResources(fullPath);
        return data != null && data.Count > 0;
    }
}
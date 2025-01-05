using UnityEngine;
using System.Collections.Generic;

public abstract class DataManager : MonoBehaviour
{
    [System.Serializable]
    public struct DataFile
    {
        public string path;
    }

    protected List<DataFile> dataFiles = new List<DataFile>();
    protected Dictionary<string, List<string[]>> csvDataSets = new Dictionary<string, List<string[]>>();

    public abstract void LoadData();
    protected abstract string ProcessPath(string fullPath);

    public List<string[]> GetDataSet(string path)
    {
        string processedPath = ProcessPath(path);
        if (csvDataSets.ContainsKey(processedPath))
            return csvDataSets[processedPath];
        return null;
    }

    public void ClearDataSet(string path)
    {
        string processedPath = ProcessPath(path);
        if (csvDataSets.ContainsKey(processedPath))
            csvDataSets.Remove(processedPath);
    }

    public void ClearAllData()
    {
        csvDataSets.Clear();
        dataFiles.Clear();
    }
}
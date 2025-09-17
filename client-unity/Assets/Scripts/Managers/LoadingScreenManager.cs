using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenManager : UnitySingleton<LoadingScreenManager>
{
    [SerializeField]
    private GameObject loadingScreenCanvas;

    private GameObject instancedLoadingScreenCanvas;

    private List<string> loadingOperations = new();

    public void AddLoadingOperation(string jobName)
    {
        if (instancedLoadingScreenCanvas == null)
        {
            instancedLoadingScreenCanvas = Instantiate(loadingScreenCanvas);
        }

        loadingOperations.Add(jobName);
        instancedLoadingScreenCanvas.SetActive(true);
    }

    public void CompleteLoadingOperation(string jobName)
    {
        if (loadingOperations.Contains(jobName))
        {
            loadingOperations.Remove(jobName);
        }

        if (loadingOperations.Count == 0)
        {
            instancedLoadingScreenCanvas.SetActive(false);
        }
    }
}

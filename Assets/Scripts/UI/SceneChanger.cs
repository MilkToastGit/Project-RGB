using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    //public string[] Levels;
    private int sceneToLoad;

    public static bool firstLoad = true;

    public void ChangeScene(string sceneName)
    {
        LoadScene (SceneManager.GetSceneByName (sceneName).buildIndex);
    }

    public void LoadLevel (int levelNumber)
    {
        LoadScene (levelNumber + 1);
    }

    public void NextLevel ()
    {
        int nextIndex = SceneManager.GetActiveScene ().buildIndex + 1;
        if (nextIndex > SceneManager.sceneCountInBuildSettings - 1)
            LoadScene (0);
        else 
            LoadScene (nextIndex);
    }

    private void LoadScene (int buildIndex)
    {
        firstLoad = false;
        //print ($"Loading scene {buildIndex}");
        sceneToLoad = buildIndex;
        AsyncOperation async = SceneManager.UnloadSceneAsync (SceneManager.GetActiveScene ());
        async.completed += OnUnloadComplete;

        //SceneManager.UnloadSceneAsync (SceneManager.GetActiveScene ());
        //SceneManager.LoadScene (buildIndex, LoadSceneMode.Additive);
        //SceneManager.SetActiveScene (SceneManager.GetSceneByBuildIndex (buildIndex));
    }

    private void OnUnloadComplete (AsyncOperation async)
    {
        async.completed -= OnUnloadComplete;
        SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Additive).completed += SetActive;
    }

    private void SetActive (AsyncOperation async)
    {
        print ("Scene Loaded");
        async.completed -= SetActive;
        SceneManager.SetActiveScene (SceneManager.GetSceneByBuildIndex (sceneToLoad));
        EventManager.Instance.SceneLoaded ();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string[] Levels;
    private int currentLevelIndex;

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        for (int i = 0; i < Levels.Length; i++)
            if (sceneName == Levels[i])
                currentLevelIndex = i;
    }

    public void NextLevel ()
    {
        if (currentLevelIndex >= Levels.Length - 1)
            SceneManager.LoadScene (0);
        else SceneManager.LoadScene (Levels[++currentLevelIndex]);
    }
}

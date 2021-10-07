using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLevelCompleteScreen : MonoBehaviour
{
    public GameObject enableObject;

    private void Awake ()
    {
        if (ManagerLoader.loaded) OnManagersLoaded ();
        else ManagerLoader.OnManagersLoaded += OnManagersLoaded;
    }

    private void OnManagersLoaded ()
    {
        ManagerLoader.OnManagersLoaded -= OnManagersLoaded;
        EventManager.Instance.OnAllOutputsCorrect += ShowScreen;
    }

    private void OnDisable ()
    {
        EventManager.Instance.OnAllOutputsCorrect -= ShowScreen;
    }

    private void ShowScreen ()
    {
        enableObject.SetActive (true);
    }
}

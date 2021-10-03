using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerLoader : MonoBehaviour
{
    public static readonly string PersistentSceneName = "Managers";
    public static Action OnManagersLoaded;
    private void ManagersLoaded (AsyncOperation suckMyDick) { OnManagersLoaded?.Invoke (); loaded = true; }

    public static bool loaded = false;

    private void Awake ()
    {
        AsyncOperation managerLoad = SceneManager.LoadSceneAsync (PersistentSceneName, LoadSceneMode.Additive);
        managerLoad.completed += ManagersLoaded;
    }

    private void OnApplicationQuit ()
    {
        OnManagersLoaded = null;
        loaded = false;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class InstantiatePersistentScene : MonoBehaviour
{
    public static readonly string PersistentSceneName = "Managers";

    private void Awake ()
    {
        SceneManager.LoadSceneAsync (PersistentSceneName, LoadSceneMode.Additive);
    }
}

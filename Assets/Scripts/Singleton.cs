using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                T foundInstance = FindObjectOfType<T> ();
                if (foundInstance != null)
                    instance = foundInstance;
                else
                {
                    GameObject obj = new GameObject ();
                    obj.name = typeof (T).Name;
                    instance = obj.AddComponent<T> ();
                }
            }
            return instance;
        }
    }

    public static T CurrentInstance => instance;

    protected void SetInstance (T instance) => Singleton<T>.instance = instance;
    protected void TrySetInstance (T instance) { if (Singleton<T>.instance != null) Singleton<T>.instance = instance;}

    private void OnEnable ()
    {
        if (instance == null)
            instance = this as T;
    }

    private void OnDestroy ()
    {
        if (instance == this)
            instance = null;
    }
}

public class SingletonPersistent<T> : MonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //if (!ManagerLoader.loaded) return null;

                Scene activeScene = SceneManager.GetActiveScene ();
                SceneManager.SetActiveScene (SceneManager.GetSceneByName (ManagerLoader.PersistentSceneName));

                T foundInstance = FindObjectOfType<T> ();
                if (foundInstance != null)
                    instance = foundInstance;
                else
                {
                    GameObject obj = new GameObject ();
                    obj.name = typeof (T).Name;
                    instance = obj.AddComponent<T> ();
                }

                SceneManager.SetActiveScene (activeScene);
            }
            return instance;
        }
    }

    public static T CurrentInstance => instance;

    private void OnDestroy ()
    {
        if (instance == this)
            instance = null;
    }
}

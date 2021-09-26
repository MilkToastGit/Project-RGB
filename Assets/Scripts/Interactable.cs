using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public delegate void InteractEvent (Vector2 touchPos);
    public event InteractEvent OnInteract;
    public void Interact (Vector2 touchPos) => OnInteract?.Invoke (touchPos);
}

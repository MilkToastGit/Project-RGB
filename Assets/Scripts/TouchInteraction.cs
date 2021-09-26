using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInteraction : MonoBehaviour
{
    [SerializeField]
    private float touchRadius;

    private void Start ()
    {
        InputManager.Instance.OnStartTouch += CheckForInteractables;
    }

    private void CheckForInteractables (Vector2 touchPos)
    {
        Debug.DrawRay (touchPos, Vector2.one * touchRadius, Color.red, 2);
        Collider2D[] hits = Physics2D.OverlapCircleAll (touchPos, touchRadius);
        if (hits.Length > 0)
        {
            foreach (Collider2D hit in hits)
            {
                if (hit.TryGetComponent (out Interactable interact))
                    interact.Interact (touchPos);
            }
        }
    }
}

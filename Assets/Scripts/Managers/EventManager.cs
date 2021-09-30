using System;
using UnityEngine;

public class EventManager : SingletonPersistent<EventManager>
{
    public event Action OnGridObjectMoved;
    public void GridObjectMoved () => OnGridObjectMoved?.Invoke ();

    public event Action OnAllBeamsTerminated;
    public void AllBeamsTerminated () => OnAllBeamsTerminated?.Invoke ();
    public bool AllBeamsTerminatedHasListeners => OnAllBeamsTerminated != null;
}

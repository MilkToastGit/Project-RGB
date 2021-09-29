using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EventManager : SingletonPersistent<EventManager>
{
    public event Action OnGridChanged;
    public void GridChanged () => OnGridChanged?.Invoke ();
}

using UnityEngine;
using UnityEngine.Events;

namespace NamPhuThuy.ComponentSM
{
    /// <summary>
    /// GameObjectEvent lets you create events that pass a GameObject as a parameter to listeners. BoolEvent lets you create events that pass a bool value to listeners. Marking them as [System.Serializable] allows you to use them in the Unity Inspector, so you can assign listeners in the editor. Both inherit from UnityEvent<T>, which is Unity's event system for sending data to subscribed methods.
    /// </summary>
    [System.Serializable]
    public class GameObjectEvent : UnityEvent<GameObject> { }

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
}
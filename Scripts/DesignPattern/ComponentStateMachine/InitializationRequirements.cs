using UnityEngine;

using System;

#if UNITY_EDITOR
using UnityEditor;


namespace NamPhuThuy.ComponentSM
{
    [InitializeOnLoad]
    public class InitializationRequirements
    {
        static InitializationRequirements ()
        {
            //state machines:
            ComponentStateMachine[] stateMachines = Resources.FindObjectsOfTypeAll<ComponentStateMachine> ();
            foreach (ComponentStateMachine item in stateMachines) 
            {
                if (item.GetComponent<Initialization> () == null) item.gameObject.AddComponent<Initialization> ();	
            }

            //display object:
            /*DisplayObject[] displayObjects = Resources.FindObjectsOfTypeAll<DisplayObject> ();
            foreach (DisplayObject item in displayObjects) 
            {
                if (item.GetComponent<Initialization> () == null) item.gameObject.AddComponent<Initialization> ();	
            }*/

            //singleton (generics require some hackery to find what we need):
            foreach (GameObject item in Resources.FindObjectsOfTypeAll<GameObject> ()) 
            {
                foreach (Component subItem in item.GetComponents<Component> ())
                {
                    //bypass this component if its currently unavailable due to a broken or missing script:
                    if (subItem == null) continue;

                    string baseType;

                    #if NETFX_CORE
                    baseType = subItem.GetType ().GetTypeInfo ().BaseType.ToString ();
                    #else
                    baseType = subItem.GetType ().BaseType.ToString ();
                    #endif

                    if (baseType.Contains ("Singleton") && baseType.Contains ("Pixelplacement")) 
                    {
                        if (item.GetComponent<Initialization> () == null) item.gameObject.AddComponent<Initialization> ();
                        continue;
                    }
                }
            }
        }
    }
}
#endif
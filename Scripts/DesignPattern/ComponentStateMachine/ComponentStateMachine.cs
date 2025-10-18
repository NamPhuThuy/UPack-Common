// Used to disable the lack of usage of the exception in a try/catch:
#pragma warning disable 168


using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.ComponentSM
{
    [RequireComponent (typeof (Initialization))]
    public class ComponentStateMachine : MonoBehaviour 
    {
        //Public Variables:
        public GameObject defaultState;
        public GameObject currentState;
        public bool _unityEventsFolded;

        /// <summary>
        /// Should log messages be thrown during usage?
        /// </summary>
        [Tooltip("Should log messages be thrown during usage?")]
        public bool verbose = true;

        /// <summary>
        /// Can States within this StateMachine be reentered?
        /// </summary>
        [Tooltip("Can States within this StateMachine be reentered?")]
        public bool allowReentry = false;

        /// <summary>
        /// Return to default state on disable?
        /// </summary>
        [Tooltip("Return to default state on disable?")]
        public bool returnToDefaultOnDisable = true;

        //Public Events:
        public GameObjectEvent OnStateExited;
        public GameObjectEvent OnStateEntered;
        public UnityEvent OnFirstStateEntered;
        public UnityEvent OnFirstStateExited;
        public UnityEvent OnLastStateEntered;
        public UnityEvent OnLastStateExited;

        //Public Properties:
        /// <summary>
        /// Internal flag used to determine if the StateMachine is set up properly.
        /// </summary>
        public bool CleanSetup 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Are we at the first state in this state machine.
        /// </summary>
        public bool AtFirst
        {
            get
            {
                return _atFirst;
            }

            private set
            {
                if (_atFirst)
                {
                    _atFirst = false;
                    if (OnFirstStateExited != null) OnFirstStateExited.Invoke ();
                } else {
                    _atFirst = true;
                    if (OnFirstStateEntered != null) OnFirstStateEntered.Invoke ();
                }
            }
        }

        /// <summary>
        /// Are we at the last state in this state machine.
        /// </summary>
        public bool AtLast
        {
            get
            {
                return _atLast;
            }

            private set
            {
                if (_atLast)
                {
                    _atLast = false;
                    if (OnLastStateExited != null) OnLastStateExited.Invoke ();
                } else {
                    _atLast = true;
                    if (OnLastStateEntered != null) OnLastStateEntered.Invoke ();
                }
            }
        }

        //Private Variables:
        bool _initialized;
        bool _atFirst;
        bool _atLast;

        //Public Methods:
        /// <summary>
        /// Change to the next state if possible.
        /// </summary>
        public GameObject Next ()
        {
            if (currentState == null) return ChangeState (0);
            int currentIndex = currentState.transform.GetSiblingIndex();
            if (currentIndex == transform.childCount - 1)
            {
                return currentState;	
            }else{
                return ChangeState (++currentIndex);
            }
        }

        /// <summary>
        /// Change to the previous state if possible.
        /// </summary>
        public GameObject Previous ()
        {
            if (currentState == null) return ChangeState(0);
            int currentIndex = currentState.transform.GetSiblingIndex();
            if (currentIndex == 0)
            {
                return currentState;	
            }else{
                return ChangeState(--currentIndex);
            }
        }

        /// <summary>
        /// Exit the current state.
        /// </summary>
        public void Exit ()
        {
            if (currentState == null) return;
            Log ("(-) " + name + " EXITED: " + currentState.name);
            int currentIndex = currentState.transform.GetSiblingIndex ();

            //no longer at first:
            if (currentIndex == 0) AtFirst = false;

            //no longer at last:
            if (currentIndex == transform.childCount - 1) AtLast = false;	

            if (OnStateExited != null) OnStateExited.Invoke (currentState);
            currentState.SetActive (false);
            currentState = null;
        }

        /// <summary>
        /// Changes the state.
        /// </summary>
        public GameObject ChangeState (int childIndex)
        {
            if (childIndex > transform.childCount-1)
            {
                Log("Index is greater than the amount of states in the StateMachine \"" + gameObject.name + "\" please verify the index you are trying to change to.");
                return null;
            }
            return ChangeState(transform.GetChild(childIndex).gameObject);
        }

        /// <summary>
        /// Changes the state.
        /// </summary>
        public GameObject ChangeState (GameObject state)
        {
            if (currentState != null)
            {
                if (!allowReentry && state == currentState)
                {
                    Log("State change ignored. State machine \"" + name + "\" already in \"" + state.name + "\" state.");
                    return null;
                }
            }

            if (state.transform.parent != transform)
            {
                Log("State \"" + state.name + "\" is not a child of \"" + name + "\" StateMachine state change canceled.");
                return null;
            }

            Exit();
            Enter(state);

            return currentState;
        }

        /// <summary>
        /// Changes the state.
        /// </summary>
        public GameObject ChangeState (string state)
        {
            Transform found = transform.Find(state);
            if (!found)
            {
                Log("\"" + name + "\" does not contain a state by the name of \"" + state + "\" please verify the name of the state you are trying to reach.");
                return null;
            }

            return ChangeState(found.gameObject);
        }

        /// <summary>
        /// Internally used within the framework to auto start the state machine.
        /// </summary>
        public void Initialize()
        {
            //turn off all states:
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Internally used within the framework to auto start the state machine.
        /// </summary>
        public void StartMachine ()
        {
            //start the machine:
            if (Application.isPlaying && defaultState != null) ChangeState (defaultState.name);
        }

        //Private Methods:
        void Enter (GameObject state)
        {
            currentState = state;
            int index = currentState.transform.GetSiblingIndex ();

            //entering first:
            if (index == 0)
            {
                AtFirst = true;
            }

            //entering last:
            if (index == transform.childCount - 1)
            {
                AtLast = true;	
            }

            Log( "(+) " + name + " ENTERED: " + state.name);
            if (OnStateEntered != null) OnStateEntered.Invoke (currentState);
            currentState.SetActive (true);
        }

        void Log (string message)
        {
            if (!verbose) return;
            Debug.Log (message, gameObject);
        }
    }
    
    
    
    #if UNITY_EDITOR
    [CustomEditor (typeof (ComponentStateMachine), true)]
    public class ComponentStateMachineEditor : Editor 
    {
        //Private Variables:
        ComponentStateMachine _target;

        //Init:
        void OnEnable()
        {
            _target = target as ComponentStateMachine;
        }

        //Inspector GUI:
        public override void OnInspectorGUI()
        {
            //if no states are found:
            if (_target.transform.childCount == 0)
            {
                DrawNotification("Add child Gameobjects for this State Machine to control.", Color.yellow);
                return;
            }

            //change buttons:
            if (EditorApplication.isPlaying)
            {
                DrawStateChangeButtons();
            }

            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, new string[] {
                "currentState",
                "_unityEventsFolded",
                "defaultState",
                "verbose",
                "allowReentry",
                "returnToDefaultOnDisable",
                "Unity Events",
                "OnStateExited",
                "OnStateEntered",
                "OnFirstStateEntered",
                "OnFirstStateExited",
                "OnLastStateEntered",
                "OnLastStateExited"
            });

            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultState"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("verbose"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("allowReentry"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("returnToDefaultOnDisable"));

            //fold events:
            _target._unityEventsFolded = EditorGUILayout.Foldout(_target._unityEventsFolded, "Unity Events", true);
            if (_target._unityEventsFolded)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnStateExited"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnStateEntered"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnFirstStateEntered"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnFirstStateExited"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnLastStateEntered"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnLastStateExited"));
            }

            serializedObject.ApplyModifiedProperties();

            if (!EditorApplication.isPlaying)
            {
                DrawHideAllButton();
            }
        }

        //GUI Draw Methods:
        void DrawStateChangeButtons()
        {
            if (_target.transform.childCount == 0) return;
            Color currentColor = GUI.color;
            for (int i = 0; i < _target.transform.childCount; i++)
            {
                GameObject current = _target.transform.GetChild(i).gameObject;

                if (_target.currentState != null && current == _target.currentState)
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }

                if (GUILayout.Button(current.name)) _target.ChangeState(current);
            }
            GUI.color = currentColor;
            if (GUILayout.Button("Exit")) _target.Exit();
        }

        void DrawHideAllButton()
        {
            GUI.color = Color.red;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Hide All"))
            {
                Undo.RegisterCompleteObjectUndo(_target.transform, "Hide All");
                foreach (Transform item in _target.transform)
                {
                    item.gameObject.SetActive(false);
                }
            }
            GUILayout.EndHorizontal();
            GUI.color = Color.white;
        }

        void DrawNotification(string message, Color color)
        {
            Color currentColor = GUI.color;
            GUI.color = color;
            EditorGUILayout.HelpBox(message, MessageType.Warning);
            GUI.color = currentColor;
        }
    }
    #endif
}
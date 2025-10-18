using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace NamPhuThuy.ComponentSM
{
    public class ComponentState : MonoBehaviour 
    {
        //Public Properties:
        /// <summary>
        /// Gets a value indicating whether this instance is the first state in this state machine.
        /// </summary>
        public bool IsFirst
        {
            get
            {
                return transform.GetSiblingIndex () == 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is the last state in this state machine.
        /// </summary>
        public bool IsLast
        {
            get
            {
                return transform.GetSiblingIndex () == transform.parent.childCount - 1;
            }
        }

        /// <summary>
        /// Gets or sets the state machine.
        /// </summary>
        public ComponentStateMachine ComponentStateMachine
        {
            get
            {
                if (_componentStateMachine == null)
                {
                    _componentStateMachine = transform.parent.GetComponent<ComponentStateMachine>();
                    if (_componentStateMachine == null)
                    {
                        Debug.LogError("States must be the child of a StateMachine to operate.");
                        return null;
                    }
                }

                return _componentStateMachine;
            }
        }

        //Private Variables:
        ComponentStateMachine _componentStateMachine;

        //Public Methods
        /// <summary>
        /// Changes the state.
        /// </summary>
        public void ChangeState(int childIndex)
        {
            ComponentStateMachine.ChangeState(childIndex);
        }

        /// <summary>
        /// Changes the state.
        /// </summary>
        public void ChangeState (GameObject state)
        {
            ComponentStateMachine.ChangeState (state.name);
        }

        /// <summary>
        /// Changes the state.
        /// </summary>
        public void ChangeState (string state)
        {
            if (ComponentStateMachine == null) return;
            ComponentStateMachine.ChangeState (state);
        }

        /// <summary>
        /// Change to the next state if possible.
        /// </summary>
        public GameObject Next ()
        {
            return ComponentStateMachine.Next ();
        }

        /// <summary>
        /// Change to the previous state if possible.
        /// </summary>
        public GameObject Previous ()
        {
            return ComponentStateMachine.Previous ();
        }

        /// <summary>
        /// Exit the current state.
        /// </summary>
        public void Exit ()
        {
            ComponentStateMachine.Exit ();
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor (typeof (ComponentState), true)]
    public class ComponentStateEditor : Editor 
    {
        //Private Variables:
        ComponentState _target;
        
        //Init:
        void OnEnable()
        {
            _target = target as ComponentState;
        }

        //Inspector GUI:
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector ();
            if (!Application.isPlaying)
            {
                GUILayout.BeginHorizontal();
                DrawSoloButton();
                DrawHideAllButton();
                GUILayout.EndHorizontal();
            }
            else
            {
                DrawChangeStateButton();
            }
        }

        //GUI Draw Methods:
        void DrawChangeStateButton ()
        {
            GUI.color = Color.green;
            if (GUILayout.Button("Change State"))
            {
                _target.ChangeState(_target.gameObject);
            }
        }

        void DrawHideAllButton ()
        {
            GUI.color = Color.red;
            if (GUILayout.Button ("Hide All"))
            {
                Undo.RegisterCompleteObjectUndo (_target.transform.parent.transform, "Hide All");
                foreach (Transform item in _target.transform.parent.transform) 
                {
                    item.gameObject.SetActive (false);
                }
            }
        }

        void DrawSoloButton ()
        {
            GUI.color = Color.green;
            if (GUILayout.Button ("Solo"))
            {
                foreach (Transform item in _target.transform.parent.transform) 
                {
                    if (item != _target.transform) item.gameObject.SetActive (false);
                    Undo.RegisterCompleteObjectUndo (_target, "Solo");
                    _target.gameObject.SetActive (true);
                }
            }
        }
    }
#endif
}
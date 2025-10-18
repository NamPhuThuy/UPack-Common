using UnityEngine;
using System.Reflection;



namespace NamPhuThuy.ComponentSM
{
    public class Initialization : MonoBehaviour
    {
        //Private Variables:
        ComponentStateMachine _stateMachine;

        //Init:
        void Awake()
        {
            //singleton initialization:
            InitializeSingleton();

            //values:
            _stateMachine = GetComponent<ComponentStateMachine>();

            //state machine initialization:
            if (_stateMachine != null) _stateMachine.Initialize();
        }

        void Start()
        {
            //state machine start:
            if (_stateMachine != null) _stateMachine.StartMachine();
        }

        //Deinit:
        void OnDisable()
        {
            if (_stateMachine != null)
            {
                if (!_stateMachine.returnToDefaultOnDisable || _stateMachine.defaultState == null) return;

                if (_stateMachine.currentState == null)
                {
                    _stateMachine.ChangeState(_stateMachine.defaultState);
                    return;
                }

                if (_stateMachine.currentState != _stateMachine.defaultState)
                {
                    _stateMachine.ChangeState(_stateMachine.defaultState);
                }
            }
        }

        //Private Methods:
        void InitializeSingleton()
        {
            foreach (Component item in GetComponents<Component>())
            {
                
                string baseType;

#if NETFX_CORE
                baseType = item.GetType ().GetTypeInfo ().BaseType.ToString ();
#else
                //Retrieve information of baseType of currentComponent
                //example: type: NamPhuThuy.GameObjectSingleton`1[GameManager_2]
                baseType = item.GetType().BaseType.ToString();
#endif
                // Debug.Log($"type: {baseType}");
                
                if (baseType.Contains("GameObjectSingleton") && baseType.Contains("NamPhuThuy"))
                {
                    MethodInfo m;

#if NETFX_CORE
                    m = item.GetType ().GetTypeInfo ().BaseType.GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
#else
                    m = item.GetType().BaseType.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
#endif
                    // Call the method Initialize() (if public) of class GameObjectSingleton
                    m.Invoke(item, new Component[] { item });
                }
            }
        }
    }
}
/*
Github: https://github.com/NamPhuThuy
*/

using UnityEngine;

namespace NamPhuThuy.Common
{
    public class DontDestroyOnLoadTag : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
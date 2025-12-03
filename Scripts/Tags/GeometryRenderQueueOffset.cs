using UnityEngine;

namespace NamPhuThuy.Common
{
    
    public class GeometryRenderQueueOffset : MonoBehaviour
    {
        [Tooltip("Default Geometry render queue is 2000. This offset will be added to it.")]
        [SerializeField] private int renderQueueOffset = 1;
        
    
        private void Start()
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                // Geometry queue is 2000, add offset to render later
                renderer.material.renderQueue = 2000 + renderQueueOffset;
            }
        }
    }
}
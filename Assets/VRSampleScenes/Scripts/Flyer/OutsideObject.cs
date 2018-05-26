using System;
using VRStandardAssets.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VRStandardAssets.Flyer
{
    class OutsideObject : MonoBehaviour
    {
        public event Action<OutsideObject> OnOutsideObjectRemovalDistance;    // This event is triggered when it is far enough behind the camera to be removed.

        [SerializeField] private float m_OutsideObjectMinSize = 2f;     // The minimum amount an OutsideObject can be scaled up.
        [SerializeField] private float m_OutsideObjectMaxSize = 3f;     // The maximum amount an OutsideObject can be scaled up.

        private Transform m_Cam;                                    // Reference to the camera so this can be destroyed when it's behind the camera.

        private const float k_RemovalDistance = 50f;                // How far behind the camera the OutsideObject must be before it is removed.


        private void Awake()
        {
            m_Cam = Camera.main.transform;
        }


        private void Start()
        {
            // Set a random scale for the OutsideObject.
            float scaleMultiplier1 = Random.Range(m_OutsideObjectMinSize, m_OutsideObjectMaxSize);
            float scaleMultiplier2 = 2 * Random.Range(m_OutsideObjectMinSize, m_OutsideObjectMaxSize);
            float scaleMultiplier3 = Random.Range(m_OutsideObjectMinSize, m_OutsideObjectMaxSize);
            transform.localScale = new Vector3(scaleMultiplier1, scaleMultiplier2, scaleMultiplier3);
        }


        private void Update()
        {
            // If the OutsideObject is far enough behind the camera and something has subscribed to OnOutsideObjectRemovalDistance call it.
            if (transform.position.z < m_Cam.position.z - k_RemovalDistance)
                if (OnOutsideObjectRemovalDistance != null)
                    OnOutsideObjectRemovalDistance(this);
        }


        private void OnDestroy()
        {
            // Ensure the events are completely unsubscribed when the OutsideObject is destroyed.
            OnOutsideObjectRemovalDistance = null;
        }
    }
}

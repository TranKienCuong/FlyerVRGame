using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

namespace VRStandardAssets.Flyer
{
    public class OpponentLaser : MonoBehaviour
    {
        [SerializeField] private float m_Speed = 500f;              // The speed each laser moves forward at.
        [SerializeField] private float m_LaserLifeDuration = 3f;    // How long the laser lasts before it's returned to it's object pool.


        private Rigidbody m_RigidBody;                              // Reference to the rigidbody of the laser.
        private bool m_Hit;                                         // Whether the laser has hit something.
        private FlyerHealthController flyerHealthController;
        private GameObject m_Flyer;

        public ObjectPool ObjectPool { private get; set; }          // The object pool the laser belongs to.


        private void Awake()
        {
            flyerHealthController = FindObjectOfType<FlyerHealthController>();
            m_Flyer = flyerHealthController.gameObject;
            m_RigidBody = GetComponent<Rigidbody>();
        }


        private void Update()
        {
            m_RigidBody.MovePosition(m_RigidBody.position - transform.forward * m_Speed * Time.deltaTime);
        }


        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Opponent hit...");

            // Only continute if the opponent laze hit the flyer
            if (other.gameObject != m_Flyer)
                return;

            // Damage the flyer

            m_Hit = true;
            
            Debug.Log("Flyer take damage");
            flyerHealthController.TakeDamage(10);
            

            // Return the laser to the object pool.
            ObjectPool.ReturnGameObjectToPool(gameObject);
            Debug.Log("opponent laser hit...");
            
        }


        private IEnumerator Timeout()
        {
            // Wait for the life time of the laser.
            yield return new WaitForSeconds(m_LaserLifeDuration);

            // If the laser hasn't hit something return it to the object pool.
            if (!m_Hit)
                ObjectPool.ReturnGameObjectToPool(gameObject);
        }


        public void Restart()
        {
            // At restart the laser hasn't hit anything.
            m_Hit = false;

            // Start the lifetime timeout.
            StartCoroutine(Timeout());
        }
        
    }
}

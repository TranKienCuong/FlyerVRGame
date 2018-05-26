using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Common;
using VRStandardAssets.Utils;

namespace VRStandardAssets.Flyer
{
    // This script handles the spawning and some of the
    // interactions of Rings and Asteroids with the flyer.
    public class EnvironmentController : MonoBehaviour
    {
        [SerializeField] private float m_AsteroidSpawnFrequency = 3f;       // The time between asteroids spawning in seconds.
        [SerializeField] private float m_RingSpawnFrequency = 10f;          // The time between rings spawning in seconds.
        [SerializeField] private int m_InitialAsteroidCount = 3;           // The number of asteroids present at the start.
        [SerializeField] private float m_AsteroidSpawnZoneRadius = 90f;    // The radius of the sphere in which the asteroids spawn.
        [SerializeField] private float m_RingSpawnZoneRadius = 50f;         // The radius of the sphere in which the rings spawn.
        [SerializeField] private float m_SpawnZoneDistance = 600f;          // The distance from the camera of the spawn spheres.
        [SerializeField] private ObjectPool m_AsteroidObjectPool;           // The object pool that stores the asteroids.
        [SerializeField] private ObjectPool m_AsteroidExplosionObjectPool;  // The object pool that stores the expolosions made when asteroids are hit.
        [SerializeField] private ObjectPool m_RingObjectPool;               // The object pool that stores the rings.
        [SerializeField] private Transform m_Cam;                           // Reference to the camera's position.

        private List<Ring> m_Rings;                                         // Collection of all the currently unpooled rings.
        private List<Asteroid> m_Asteroids;                                 // Collection of all the currently unpooled asteroids.
        private bool m_Spawning;                                            // Whether the environment should keep spawning rings and asteroids.

        // opponent code
        [SerializeField] private float m_OpponentSpawnFrequency = 3f;
        [SerializeField] private int m_InitialOpponentCount = 3;
        [SerializeField] private float m_OpponentSpawnZoneRadius = 90f;
        [SerializeField] private ObjectPool m_OpponentObjectPool;
        [SerializeField] private ObjectPool m_OpponentExplosionObjectPool;
        [SerializeField] private ObjectPool m_LaserOpponentObjectPool;
        private List<Opponent> m_Opponents;

        // outside object code
        [SerializeField] private float m_OutsideObjectSpawnFrequency = 2f;
        [SerializeField] private int m_InitialOutsideObjectCount = 3;
        [SerializeField] private float m_OutsideObjectSpawnMinDistance = 75f;
        [SerializeField] private float m_OutsideObjectSpawnMaxDistance = 250f;
        [SerializeField] private float m_OutsideObjectHeight = 100f;
        [SerializeField] private ObjectPool m_OutsideObjectPool;
        private List<OutsideObject> m_OutsideObjects;

        public ObjectPool GetLaserOpponentPool()
        {
            return m_LaserOpponentObjectPool;
        }


        public void StartEnvironment()
        {
            // Create new empty lists for the rings, asteroids and opponents.
            m_Rings = new List<Ring>();
            m_Asteroids = new List<Asteroid>();
            m_Opponents = new List<Opponent>();
            m_OutsideObjects = new List<OutsideObject>();

            // Spawn all the starting asteroids.
            for (int i = 0; i < m_InitialAsteroidCount; i++)
            {
                SpawnAsteroid();
            }

            // Spawn all the starting opponents.
            for (int i = 0; i < m_InitialOpponentCount; i++)
            {
                SpawnOpponent();
            }

            // Spawn all the starting outside objects.
            for (int i = 0; i < m_InitialOpponentCount; i++)
            {
                SpawnOutsideObject();
            }

            // Restart the score and set the score's type to be FLYER
            SessionData.Restart();
            SessionData.SetGameType(SessionData.GameType.FLYER);
            
            // The environment has started so spawning can start.
            m_Spawning = true;

            // Start spawning asteroids and rings.
            StartCoroutine (SpawnAsteroidRoutine ());
            StartCoroutine (SpawnRingRoutine ());
            StartCoroutine (SpawnOpponentRoutine());
            StartCoroutine(SpawnOutsideObjectRoutine());
        }


        public void StopEnvironment()
        {
            // The environment has stopped so spawning should no longer happen.
            m_Spawning = false;
            
            // While there are asteroids in the collection, remove the first asteroid.
            while (m_Asteroids.Count > 0)
            {
                HandleAsteroidRemoval(m_Asteroids[0]);
            }

            // While there are rings in the collection, remove the first ring.
            while (m_Rings.Count > 0)
            {
                HandleRingRemove (m_Rings[0]);
            }

            // While there are opponents in the collection, remove the first opponent.
            while (m_Opponents.Count > 0)
            {
                HandleOpponentRemoval(m_Opponents[0]);
            }

            // While there are outside objects in the collection, remove the first outside object.
            while (m_Opponents.Count > 0)
            {
                HandleOpponentRemoval(m_Opponents[0]);
            }
        }


        private IEnumerator SpawnAsteroidRoutine()
        {
            // While the environment is spawning, spawn an asteroid and wait for another one.
            do
            {
                SpawnAsteroid ();
                yield return new WaitForSeconds (m_AsteroidSpawnFrequency);
            }
            while (m_Spawning);
        }


        private void SpawnAsteroid ()
        {
            // Get an asteroid from the object pool.
            GameObject asteroidGameObject = m_AsteroidObjectPool.GetGameObjectFromPool ();

            // Generate a position at a distance forward from the camera within a random sphere and put the asteroid at that position.
            Vector3 asteroidPosition = m_Cam.position + Vector3.forward * m_SpawnZoneDistance + Random.insideUnitSphere * m_AsteroidSpawnZoneRadius;
            asteroidGameObject.transform.position = asteroidPosition;

            // Get the asteroid component and add it to the collection.
            Asteroid asteroid = asteroidGameObject.GetComponent<Asteroid>();
            m_Asteroids.Add(asteroid);

            // Subscribe to the asteroids events.
            asteroid.OnAsteroidRemovalDistance += HandleAsteroidRemoval;
            asteroid.OnAsteroidHit += HandleAsteroidHit;
        }


        private IEnumerator SpawnRingRoutine ()
        {
            // With an initial delay, spawn a ring and delay whilst the environment is spawning.
            yield return new WaitForSeconds(m_RingSpawnFrequency);
            do
            {
                SpawnRing ();
                yield return new WaitForSeconds(m_RingSpawnFrequency);
            }
            while (m_Spawning);
        }


        private void SpawnRing()
        {
            // Get a ring from the object pool.
            GameObject ringGameObject = m_RingObjectPool.GetGameObjectFromPool ();

            // Generate a position at a distance forward from the camera within a random sphere and put the ring at that position.
            Vector3 ringPosition = m_Cam.position + Vector3.forward * m_SpawnZoneDistance + Random.insideUnitSphere * m_RingSpawnZoneRadius;
            ringGameObject.transform.position = ringPosition;

            // Get the ring component, restart it and add it to the collection.
            Ring ring = ringGameObject.GetComponent<Ring> ();
            ring.Restart ();
            m_Rings.Add (ring);

            // Subscribe to the remove event.
            ring.OnRingRemove += HandleRingRemove;
        }


        private void HandleAsteroidRemoval(Asteroid asteroid)
        {
            // Only one of HandleAsteroidRemoval and HandleAsteroidHit should be called so unsubscribe both.
            asteroid.OnAsteroidRemovalDistance -= HandleAsteroidRemoval;
            asteroid.OnAsteroidHit -= HandleAsteroidHit;

            // Remove the asteroid from the collection.
            m_Asteroids.Remove(asteroid);

            // Return the asteroid to its object pool.
            m_AsteroidObjectPool.ReturnGameObjectToPool (asteroid.gameObject);
        }


        private void HandleAsteroidHit(Asteroid asteroid)
        {
            // Remove the asteroid when it's hit.
            HandleAsteroidRemoval (asteroid);

            // Get an explosion from the object pool and put it at the asteroids position.
            GameObject explosion = m_AsteroidExplosionObjectPool.GetGameObjectFromPool ();
            explosion.transform.position = asteroid.transform.position;

            // Get the asteroid explosion component and restart it.
            ObstacleExplosion asteroidExplosion = explosion.GetComponent<ObstacleExplosion>();
            asteroidExplosion.Restart();

            // Subscribe to the asteroid explosion's event.
            asteroidExplosion.OnExplosionEnded += HandleAsteroidExplosionEnded;
        }


        private void HandleAsteroidExplosionEnded(ObstacleExplosion explosion)
        {
            // Now the explosion has finished unsubscribe from the event.
            explosion.OnExplosionEnded -= HandleAsteroidExplosionEnded;

            // Return the explosion to its object pool.
            m_AsteroidExplosionObjectPool.ReturnGameObjectToPool (explosion.gameObject);
        }


        private void HandleOpponentExplosionEnded(ObstacleExplosion explosion)
        {
            // Now the explosion has finished unsubscribe from the event.
            explosion.OnExplosionEnded -= HandleOpponentExplosionEnded;

            // Return the explosion to its object pool.
            m_OpponentExplosionObjectPool.ReturnGameObjectToPool(explosion.gameObject);
        }


        private void HandleRingRemove(Ring ring)
        {
            // Now the ring has been removed, unsubscribe from the event.
            ring.OnRingRemove -= HandleRingRemove;

            // Remove the ring from it's collection.
            m_Rings.Remove(ring);

            // Return the ring to its object pool.
            m_RingObjectPool.ReturnGameObjectToPool(ring.gameObject);
        }

        // OPPONENT CODE

        private IEnumerator SpawnOpponentRoutine()
        {
            // While the environment is spawning, spawn an opponent and wait for another one.
            do
            {
                SpawnOpponent();
                yield return new WaitForSeconds(m_OpponentSpawnFrequency);
            }
            while (m_Spawning);
        }

        private void SpawnOpponent()
        {
            // Get an opponent from the object pool.
            GameObject opponentGameObject = m_OpponentObjectPool.GetGameObjectFromPool();

            // Generate a position at a distance forward from the camera within a random sphere and put the opponent at that position.
            Vector3 opponentPosition = m_Cam.position + Vector3.forward * m_SpawnZoneDistance + Random.insideUnitSphere * m_OpponentSpawnZoneRadius;
            opponentGameObject.transform.position = opponentPosition;

            // Get the opponent component and add it to the collection.
            Opponent opponent = opponentGameObject.GetComponent<Opponent>();
            m_Opponents.Add(opponent);

            // Subscribe to the opponents events.
            opponent.OnOpponentRemovalDistance += HandleOpponentRemoval;
            opponent.OnOpponentHit += HandleOpponentHit;
        }

        private void HandleOpponentRemoval(Opponent opponent)
        {
            // Only one of HandleOpponentRemoval and HandleOpponentHit should be called so unsubscribe both.
            opponent.OnOpponentRemovalDistance -= HandleOpponentRemoval;
            opponent.OnOpponentHit -= HandleOpponentHit;

            // Remove the opponent from the collection.
            m_Opponents.Remove(opponent);

            // Return the opponent to its object pool.
            m_OpponentObjectPool.ReturnGameObjectToPool(opponent.gameObject);
        }

        private void HandleOpponentHit(Opponent opponent)
        {
            // Remove the opponent when it's hit.
            HandleOpponentRemoval(opponent);

            // Get an explosion from the object pool and put it at the asteroids position.
            GameObject explosion = m_OpponentExplosionObjectPool.GetGameObjectFromPool();
            explosion.transform.position = opponent.transform.position;

            // Get the asteroid explosion component and restart it.
            ObstacleExplosion obstacleExplosion = explosion.GetComponent<ObstacleExplosion>();
            obstacleExplosion.Restart();

            // Subscribe to the asteroid explosion's event.
            obstacleExplosion.OnExplosionEnded += HandleOpponentExplosionEnded;
        }

        // OUTSIDE OBJECT CODE

        private IEnumerator SpawnOutsideObjectRoutine()
        {
            // While the environment is spawning, spawn an outside object and wait for another one.
            do
            {
                SpawnOutsideObject();
                yield return new WaitForSeconds(m_OutsideObjectSpawnFrequency);
            }
            while (m_Spawning);
        }

        private void SpawnOutsideObject()
        {
            // Get an outside object from the object pool.
            GameObject outsideGameObject = m_OutsideObjectPool.GetGameObjectFromPool();

            // Generate a position at a distance forward from the camera within a random position between min and max distance and put the outside object at that position.
            int dir = Random.Range(0f, 1f) > 0.5 ? 1 : -1;
            var x = dir * Random.Range(m_OutsideObjectSpawnMinDistance, m_OutsideObjectSpawnMaxDistance);
            Vector3 outsideObjectPosition = m_Cam.position + Vector3.forward * m_SpawnZoneDistance + new Vector3(x, -m_OutsideObjectHeight, 0);
            outsideGameObject.transform.position = outsideObjectPosition;

            // Get the outside object component and add it to the collection.
            OutsideObject outsideObject = outsideGameObject.GetComponent<OutsideObject>();
            m_OutsideObjects.Add(outsideObject);

            // Subscribe to the asteroids events.
            outsideObject.OnOutsideObjectRemovalDistance += HandleOutsideObjectRemoval;
        }

        private void HandleOutsideObjectRemoval(OutsideObject outsideObject)
        {
            // Only one of HandleOutsideRemoval should be called so unsubscribe it
            outsideObject.OnOutsideObjectRemovalDistance -= HandleOutsideObjectRemoval;

            // Remove the object from the collection.
            m_OutsideObjects.Remove(outsideObject);

            // Return the object to its object pool.
            m_OutsideObjectPool.ReturnGameObjectToPool(outsideObject.gameObject);
        }
    }
}
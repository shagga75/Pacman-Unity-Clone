using UnityEngine;

namespace BitManSatChase
{
    public class Teleporter : MonoBehaviour
    {
        private GameObject leftTeleport;
        private GameObject rightTeleport;
        // Keep track of objects that have been teleported to prevent teleporting them again
        private System.Collections.Generic.HashSet<GameObject> teleportedObjects = new System.Collections.Generic.HashSet<GameObject>();
        private float checkInterval = 0.3f;
        private float nextCheckTime = 0f;

        void Start()
        {
            leftTeleport = transform.Find("Left Teleport").gameObject;
            rightTeleport = transform.Find("Right Teleport").gameObject;
        }

        void Update()
        {
            // Check for collisions at regular intervals
            if (Time.time >= nextCheckTime)
            {
                CheckForCollisions();
                nextCheckTime = Time.time + checkInterval;
            }
        }

        /// <summary>
        /// Check for collisions around both teleporters
        /// </summary>
        private void CheckForCollisions()
        {
            CheckForCollisionsAround(leftTeleport);
            CheckForCollisionsAround(rightTeleport);
        }

        /// <summary>
        /// Check for collisions around a specific teleporter
        /// </summary>
        /// <param name="teleport"></param>
        private void CheckForCollisionsAround(GameObject teleport)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(teleport.transform.position, teleport.transform.localScale / 2, 0, LayerMask.GetMask("Default"));
            // If there are no colliders, clear the list of teleported objects
            if (colliders.Length == 0)
            {
                teleportedObjects.Clear();
            }
            foreach (Collider2D collider in colliders)
            {
                if (!teleportedObjects.Contains(collider.gameObject))
                {
                    TeleportObject(collider.gameObject);
                }
            }
        }

        /// <summary>
        /// Teleport the object to the opposite teleporter
        /// </summary>
        /// <param name="obj"></param>
        private void TeleportObject(GameObject obj)
        {
            GameObject parentObject = obj.transform.parent != null ? obj.transform.parent.gameObject : obj;

            if (Vector2.Distance(parentObject.transform.position, leftTeleport.transform.position) < Vector2.Distance(parentObject.transform.position, rightTeleport.transform.position))
            {
                parentObject.transform.position = rightTeleport.transform.position;
            }
            else
            {
                parentObject.transform.position = leftTeleport.transform.position;
            }
            // Add the object to the list of teleported objects
            teleportedObjects.Add(obj);
        }
    }
}

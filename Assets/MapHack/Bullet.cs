using UnityEngine;

namespace MapHack
{
    public class Bullet: MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            UnityEngine.Debug.Log(other.gameObject.name);
            if (other.gameObject.name.Contains("Building"))
            {
                var building = other.gameObject.GetComponentInChildren<MeshCollider>();
                building.convex = true;
                building.gameObject.AddComponent<Rigidbody>();
            }
            Destroy(gameObject);        
        }
    }
}
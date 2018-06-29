using UnityEngine;

namespace MapHack
{
    public class Car : MonoBehaviour
    {
        public static Car CreateComponent(Vector3 vector3, Camera main)
        {
            var prefab = (GameObject) Resources.Load("Prefabs/Car");
            var carObj = Instantiate(prefab, vector3, Quaternion.identity);
            return carObj.AddComponent<Car>()._CreateComponent(main);
        }

        private void Update()
        {
            if(Input.GetKeyDown (KeyCode.Z))
            {
                GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                bullet.AddComponent<Bullet>();
                var rigidbody = bullet.AddComponent<Rigidbody>();
//                rigidbody.useGravity = false;
 
                Vector3 force;
                force = gameObject.transform.forward * 10000 + gameObject.transform.up * 1000;
                rigidbody.AddForce(force);
                bullet.transform.position = transform.position + transform.forward * 2 + transform.up * 2;
            }
        }

        private Car _CreateComponent(Camera main)
        {
            main.transform.parent = transform;
            return this;
        }

        private void OnCollisionEnter(Collision other)
        {
            UnityEngine.Debug.Log(other.gameObject.name);
            if (other.gameObject.name.Contains("Building"))
            {
                var building = other.gameObject.GetComponentInChildren<MeshCollider>();
                building.convex = true;
                building.gameObject.AddComponent<Rigidbody>();
            }
        }
    }
}
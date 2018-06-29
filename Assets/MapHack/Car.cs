using UnityEngine;

namespace MapHack
{
    public class Car: MonoBehaviour
    {
        public static Car CreateComponent(Vector3 vector3, Camera main)
        {
            var prefab = (GameObject)Resources.Load ("Prefabs/Car");
            var carObj = Instantiate(prefab, vector3, Quaternion.identity);
            return carObj.AddComponent<Car>()._CreateComponent(main);
        }

        private Car _CreateComponent(Camera main)
        {
            main.transform.parent = transform;
            return this;
        }
    }
}
using UnityEngine;

namespace MapHack
{
    public class Car: MonoBehaviour
    {
        public static Car CreateComponent()
        {
            var prefab = (GameObject)Resources.Load ("Prefabs/Car");
            var carObj = Instantiate(prefab);
            return carObj.AddComponent<Car>();
        }
        
    }
}
using RLCreature.Sample.SimpleHunting;
using UnityEngine;
using Wrld.Space;

namespace MapHack
{
    public class EntryPoint : MonoBehaviour
    {
        private Map _map;
        private double m_latitudeDegrees = 37.771092;
        private double m_longitudeDegrees = -122.47;

        private void Start()
        {
            _map = gameObject.GetComponent<Map>();
            _map.InitCamera(new LatLongAltitude(m_latitudeDegrees, m_longitudeDegrees, 100));
        }


        private void Feed()
        {
            var cameraPos = _map.CurrentWorldPosition();
            var targetPos = new Vector3(cameraPos.x, 0, cameraPos.z)
                            + new Vector3(
                                (float) (((Random.value - 0.5) * 2) * 100), 0,
                                (float) (((Random.value - 0.5) * 2) * 100));
            RaycastHit hit;
            if (Physics.Raycast(targetPos + Vector3.up * 100, Vector3.down, out hit, 100))
            {
                if (hit.transform.gameObject.name.Contains("_terrain_"))
                {
                    UnityEngine.Debug.Log(targetPos);
                    targetPos = hit.point;
                    UnityEngine.Debug.Log(targetPos);
                }
                else
                {
                    return;
                }
            }


            var foodObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            foodObject.transform.localScale = Vector3.one * 5;
            var food = foodObject.AddComponent<Food>();
            food.GetComponent<Renderer>().material.color = Color.green;
            food.GetComponent<Collider>().isTrigger = true;
//            food.transform.position = new Vector3(
//                x: _size.xMin + Random.value * _size.width,
//                y: 0,
//                z: _size.yMin + Random.value * _size.height
//            );

            food.transform.position = targetPos;
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                Feed();
            }

            if (Input.GetKeyUp(KeyCode.C))
            {
                var cameraPos = _map.CurrentWorldPosition();
                var targetPos = cameraPos + Camera.main.transform.forward * 20 + Camera.main.transform.up * -10;
                Car.CreateComponent(targetPos, Camera.main);
            }
        }
    }
}
using RLCreature.Sample.Common.UI;
using RLCreature.Sample.Common.UI.Actions;
using RLCreature.Sample.SimpleHunting;
using UnityEngine;
using Wrld.Space;

namespace MapHack
{
    public class EntryPoint : MonoBehaviour
    {
        private Map _map;

        // 37.329
        // -121.9
        public double m_latitudeDegrees = 37.775;
        public double m_longitudeDegrees = -122.5;
        private CastUIPresenter GameUI;
        private GameObject _terrainRoot;

        private void Start()
        {
            _terrainRoot = GameObject.Find("Terrain");

            _map = gameObject.GetComponent<Map>();
            _map.InitCamera(new LatLongAltitude(m_latitudeDegrees, m_longitudeDegrees, 100));
//            var cameraPos = _map.CurrentWorldPosition();
//
//            GameUI = CastUIPresenter.CreateComponent(Camera.main, gameObject);
//            CastCameraController.CreateComponent(Camera.main, GameUI.SelectedCreature,
//                GameUI.FallbackedEventsObservable);
//            GameUI.LeftToolBar.Add(new SystemActions());
//            Camera.main.transform.position = cameraPos;
        }


        private void UpdateTerrain()
        {
            foreach (Transform terrainchild in _terrainRoot.transform)
            {
                foreach (Transform terrainchild2 in terrainchild)
                {
                    terrainchild2.GetComponent<MeshCollider>().material = (Resources.Load<PhysicMaterial>("Ground"));
                }
            }
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
            UpdateTerrain();
            if (Input.GetKeyUp(KeyCode.F))
            {
                Feed();
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                var cameraPos = _map.CurrentWorldPosition();
                var targetPos = cameraPos + Camera.main.transform.forward * 20 + Camera.main.transform.up * 100;
                var agent = CrasherCreature.CreateComponent(targetPos, Camera.main);
//                var info = GameUI.AddAgent(agent);
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
using UnityEngine;
using Wrld.Space;

namespace MapHack
{
    public class EntryPoint : MonoBehaviour
    {
        private Map _map;
        private double m_latitudeDegrees = 37.771092;
        private double m_longitudeDegrees = -122.468385;

        private void Start()
        {
            _map = gameObject.GetComponent<Map>();
            _map.InitCamera(new LatLongAltitude(m_latitudeDegrees, m_longitudeDegrees, 100));
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.C))
            {
                var cameraPos = _map.CurrentWorldPosition();
                var targetPos = cameraPos + Camera.main.transform.forward * 20 + Camera.main.transform.up * -10;
                Car.CreateComponent(targetPos, Camera.main);
            }            
        }
    }
}
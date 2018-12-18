using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using MotionGenerator;
using RLCreature.BodyGenerator.Manipulatables;
using UnityEngine;

namespace MapHack
{
    public class PointSensor : MonoBehaviour
    {
        private GameObject testObject;
        private Vector3 _relativePosition;

        public static PointSensor CreateComponent(GameObject obj, Vector3 relativePosition)
        {
            return obj.AddComponent<PointSensor>()._CreateComponent(relativePosition);
        }

        private PointSensor _CreateComponent(Vector3 relativePosition)
        {
            _relativePosition = relativePosition;
            testObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            testObject.transform.localScale = new Vector3(1, 10, 1);
            foreach (var collider in testObject.GetComponentsInChildren<Collider>())
            {
                Destroy(collider);
            }

            return this;
        }

        public double GetState()
        {
            RaycastHit hit;
            var targetpos = gameObject.transform.position + transform.rotation * _relativePosition;
            testObject.transform.position = targetpos;
            if (Physics.Raycast(targetpos + Vector3.up * 100, Vector3.down, out hit, 100))
            {
                Debug.DrawRay(targetpos + Vector3.up * 100, Vector3.down * hit.distance, Color.yellow);
                if (!hit.transform.gameObject.name.Contains("_terrain_"))
                {
//                    UnityEngine.Debug.Log(hit.transform.gameObject.name);
                    testObject.GetComponent<Renderer>().material.color = Color.red;
                    return 1;                    
                }
            }
            testObject.GetComponent<Renderer>().material.color = Color.black;

            return 0;
        }
    }

    public class SurroundSensor : ManipulatableBase
    {
        private GameObject testObject;
        private readonly State _state = new State();
        private List<PointSensor> _pointSensors;
        public const string Key = "SurroundSensor";

        public static SurroundSensor CreateComponent(GameObject obj)
        {
            return obj.AddComponent<SurroundSensor>()._CreateComponent();
        }

        private SurroundSensor _CreateComponent()
        {
            _pointSensors = new List<PointSensor>();
            foreach (var x in new int[] {-5, 0, 5})
            {
                foreach (var z in new int[] {-5, 0, 5})
                {
                    if (x == 0 && z == 0) continue;
                    _pointSensors.Add(PointSensor.CreateComponent(gameObject, new Vector3(x, 0, z)));
                }
            }

            foreach (var x in new int[] {-10, 0, 10})
            {
                foreach (var z in new int[] {-10, 0, 10})
                {
                    if (x == 0 && z == 0) continue;
                    _pointSensors.Add(PointSensor.CreateComponent(gameObject, new Vector3(x, 0, z)));
                }
            }

            return this;
        }

        public override State GetState()
        {
            var tmp = _pointSensors.Select(x => x.GetState()).ToArray();
            _state[Key] = (Vector) Vector.Build.DenseOfArray(tmp);
            return _state;
        }
    }
}
using MathNet.Numerics.LinearAlgebra.Double;
using MotionGenerator;
using RLCreature.BodyGenerator.Manipulatables;
using UnityEngine;

namespace MapHack
{
    public class CrashSensor : ManipulatableBase
    {
        private readonly State _state = new State();
        private bool _crashed;
        public const string Key = "crashed";

        public static CrashSensor CreateComponent(GameObject obj)
        {
            return obj.AddComponent<CrashSensor>()._CreateComponent();
        }

        private CrashSensor _CreateComponent()
        {
            _state[Key] = new DenseVector(1);
            _crashed = false;
            return this;
        }


        public override State GetState()
        {
            if (_crashed)
            {
                _crashed = false;
                _state.Set(Key, 1);
            }
            else
            {
                _state.Set(Key, 0);
            }

            return _state;
        }

        private void OnCollisionEnter(Collision other)
        {
            UnityEngine.Debug.Log(other.gameObject.name);
            _crashed = true;
//            if (other.gameObject.name.Contains("Building"))
//            {
//                var building = other.gameObject.GetComponentInChildren<MeshCollider>();
//                building.convex = true;
//                building.gameObject.AddComponent<Rigidbody>();
//            }
        }
    }
}
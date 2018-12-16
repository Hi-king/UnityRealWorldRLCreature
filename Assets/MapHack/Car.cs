using System.Collections.Generic;
using MotionGenerator;
using MotionGenerator.Entity.Soul;
using RLCreature.BodyGenerator;
using RLCreature.BodyGenerator.Manipulatables;
using RLCreature.Sample.Driving;
using RLCreature.Sample.SimpleHunting;
using UnityEngine;

namespace MapHack
{
    public class Car : MonoBehaviour
    {
        public static Car CreateComponent(Vector3 vector3, Camera main)
        {
            var prefab = (GameObject) Resources.Load("Prefabs/Car");
            var car = Instantiate(prefab, vector3, Quaternion.identity);

            CarControlManipulatable.CreateComponent(car);
            SurroundSensor.CreateComponent(car);
            Sensor.CreateComponent(car, typeof(Food), State.BasicKeys.RelativeFoodPosition, range: 300f);
            Mouth.CreateComponent(car, typeof(Food));

            var actions = LocomotionAction.EightDirections();
            var sequenceMaker = new EvolutionarySequenceMaker(epsilon: 0.3f, minimumCandidates: 30);
            var brain = new Brain(
                new ReinforcementDecisionMaker(keyOrder: new []
                {
                    State.BasicKeys.Forward, 
                    State.BasicKeys.RelativeFoodPosition,
                    SurroundSensor.Key
                }, soulWeights: new []
                {
                    1f, 1f
                }),
                sequenceMaker
            );
            Agent.CreateComponent(car, brain, new Body(car), actions, souls: new List<ISoul>()
            {
                new SnufflingDifferencialSoul(),
                new AvoidCrashSoul()
            });

            return car.AddComponent<Car>()._CreateComponent(main);
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
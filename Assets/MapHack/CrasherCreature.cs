using System.Collections.Generic;
using MotionGenerator;
using MotionGenerator.Entity.Soul;
using RLCreature.Assets.RLCreature.Sample.Common;
using RLCreature.BodyGenerator;
using RLCreature.Sample.Driving;
using UnityEngine;

namespace MapHack
{
    public class CrashedBuilding : MonoBehaviour
    {
        
    }
    
    public class CrasherBehaviour : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            UnityEngine.Debug.Log($"Collision {other.gameObject.name}");
            if (!other.gameObject.name.Contains("_terrain_"))
            {
                var building = other.gameObject.GetComponentInChildren<MeshCollider>();
                var rigid = building.gameObject.AddComponent<Rigidbody>();
            }
        }
    }

    public class CrasherCreature : MonoBehaviour
    {
        private Vector3 originalPos;

        public static Agent CreateComponent(Vector3 vector3, Camera main)
        {
            var prefab = (GameObject) Resources.Load("Prefabs/Car");
            prefab.transform.localScale = prefab.transform.localScale * 5;
            var creature = Instantiate(prefab, vector3, Quaternion.identity);
            CarControlManipulatable.CreateComponent(creature);

//            var prefab = (GameObject) Resources.Load("CreaturePrefabs/GymAnt");
//            prefab.transform.localScale = prefab.transform.localScale * 30;
//            var creature = Instantiate(prefab, vector3, Quaternion.identity);

            UnityEngine.Debug.Log(creature.name);
            foreach (Transform part in creature.transform)
            {
                var obj = part.gameObject;
                if(obj == null) continue;
                UnityEngine.Debug.Log(obj.name);
                obj.AddComponent<CrasherBehaviour>();
            }
            
            
//            SurroundSensor.CreateComponent(car);
//            Sensor.CreateComponent(car, typeof(Food), State.BasicKeys.RelativeFoodPosition, range: 300f);
//            Mouth.CreateComponent(car, typeof(Food));

//            CrashSensor.CreateComponent(car);
            BuildingSensor.CreateComponent(creature, 500);

            var actions = LocomotionAction.EightDirections();
            var sequenceMaker = new EvolutionarySequenceMaker(epsilon: 0.3f, minimumCandidates: 30);
            IDecisionMaker decisionMaker = new ReinforcementDecisionMaker(
                keyOrder: new[]
                {
                    State.BasicKeys.Forward,
                    State.BasicKeys.RelativeFoodPosition
                }, soulWeights: new[]
                {
                    1f
                });
            decisionMaker = new LoggingDecisionMaker(decisionMaker);
            var brain = new Brain(
                decisionMaker,
                sequenceMaker
            );
            var agent = Agent.CreateComponent(creature, brain, new Body(creature.transform.GetChild(0).gameObject), actions,
                souls: new List<ISoul>()
                {
                    new SnufflingDifferencialSoul()
                });

            creature.AddComponent<CrasherCreature>()._CreateComponent(main, vector3);
            return agent;
        }

        private void Update()
        {
            if (transform.rotation.eulerAngles.x < -90)
            {
                transform.rotation = Quaternion.identity;
                ;
            }

            if (Input.GetKeyDown(KeyCode.Z))
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

        private CrasherCreature _CreateComponent(Camera main, Vector3 vector3)
        {
//            main.transform.parent = transform.GetChild(0);
//            main.transform.parent = transform;
            originalPos = vector3;
            return this;
        }

        private void OnCollisionEnter(Collision other)
        {
            UnityEngine.Debug.Log($"Collision {other.gameObject.name}");
            if (!other.gameObject.name.Contains("_terrain_"))
            {
                var building = other.gameObject.GetComponentInChildren<MeshCollider>();
                building.gameObject.AddComponent<Rigidbody>();
                building.gameObject.transform.parent.gameObject.AddComponent<CrashedBuilding>();
            }
        }
    }
}
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
                building.gameObject.transform.parent.gameObject.AddComponent<CrashedBuilding>();
            }
        }
    }

    public class CrasherCreature : MonoBehaviour
    {
        private Vector3 cameraPos;
        private Camera _camera;

        public static Agent CreateComponent(Vector3 vector3, Camera main)
        {
//            var prefab = (GameObject) Resources.Load("Prefabs/Car");
//            prefab.transform.localScale = prefab.transform.localScale * 5;
//            var creature = Instantiate(prefab, vector3, Quaternion.identity);
//            CarControlManipulatable.CreateComponent(creature);

            var prefab = (GameObject) Resources.Load("CreaturePrefabs/GymAnt");
            prefab.transform.localScale = prefab.transform.localScale * 1;
            var creature = Instantiate(prefab, vector3, Quaternion.identity);
            foreach (var rigid in creature.GetComponentsInChildren<Rigidbody>())
            {
                rigid.mass *= 300;
                rigid.solverIterations = 1000;
                rigid.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }

            foreach (var collider in creature.GetComponentsInChildren<Collider>())
            {
                collider.material = (Resources.Load<PhysicMaterial>("Ground"));
            }

            creature.transform.GetChild(0).GetComponent<Rigidbody>().mass *= 5;
            creature.transform.GetChild(0).GetComponent<Rigidbody>().angularDrag *= 15;

            UnityEngine.Debug.Log(creature.name);
            foreach (Transform part in creature.transform)
            {
                var obj = part.gameObject;
                if (obj == null) continue;
                obj.AddComponent<CrasherBehaviour>();
            }

            BuildingSensor.CreateComponent(creature.transform.GetChild(0).gameObject, 500);

            var actions = LocomotionAction.EightDirections();
            var sequenceMaker = new EvolutionarySequenceMaker(epsilon: 0.3f, minimumCandidates: 30);
//            IDecisionMaker decisionMaker = new ReinforcementDecisionMaker(
//                keyOrder: new[]
//                {
//                    State.BasicKeys.Forward,
//                    State.BasicKeys.RelativeFoodPosition
//                }, soulWeights: new[]
//                {
//                    1f
//                });
//            decisionMaker = new LoggingDecisionMaker(decisionMaker);
            IDecisionMaker decisionMaker = new FollowPointDecisionMaker(State.BasicKeys.RelativeFoodPosition);
            var brain = new Brain(
                decisionMaker,
                sequenceMaker
            );
            var agent = Agent.CreateComponent(creature, brain, new Body(creature.transform.GetChild(0).gameObject),
                actions,
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
            }
            var centralBodyPos = transform.GetChild(0).position;
            var targetCameraPos = centralBodyPos + transform.GetChild(0).rotation * (new Vector3(-200, 200, 200));
            cameraPos = Vector3.Slerp(cameraPos, targetCameraPos, 0.05f);
            _camera.transform.position = cameraPos;
            _camera.transform.rotation = Quaternion.LookRotation(centralBodyPos - cameraPos);


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
            _camera = main;
            var centralBodyPos = transform.GetChild(0).position;
            var targetCameraPos = centralBodyPos + transform.GetChild(0).rotation * (new Vector3(-200, 200, 200));
//            cameraPos = Vector3.Slerp(cameraPos, targetCameraPos, 0.2f);
            cameraPos = targetCameraPos;
            _camera.transform.position = cameraPos;
            _camera.transform.rotation = Quaternion.LookRotation(centralBodyPos - cameraPos);
//            main.transform.parent = transform.GetChild(0);
//            main.transform.parent = transform;

//            originalPos = vector3;
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
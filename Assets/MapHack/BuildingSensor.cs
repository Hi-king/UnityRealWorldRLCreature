using MathNet.Numerics.LinearAlgebra.Double;
using MotionGenerator;
using RLCreature.BodyGenerator.Manipulatables;
using UnityEngine;

namespace MapHack
{
    public class BuildingSensor : ManipulatableBase
    {
        private readonly State _state = new State();
        private GameObject _root;
        private string _key;
        private float _range;
        private LineRenderer _renderer;

        public static BuildingSensor CreateComponent(GameObject obj, float range)
        {
            return obj.AddComponent<BuildingSensor>()._CreateComponent(range: range);
        }

        private BuildingSensor _CreateComponent(float range)
        {
            _root = GameObject.Find("Buildings");
            _key = State.BasicKeys.RelativeFoodPosition;
            _range = range;
            _state[_key] = new DenseVector(3);
            SetupRenderer();
            return this;
        }
        
        public override State GetState()
        {
            UnityEngine.Debug.Log(_root.name);
            var minDistance = float.MaxValue;
            _state.Set(_key, -Vector3.one); // when no candidate found
            GameObject targetObject = null;
            foreach (Transform candidateobj in _root.transform)
            {
                
                var candidate = candidateobj.gameObject;
                if(candidate == null) continue;
                
                if(candidate.GetComponent<CrashedBuilding>() != null) continue; // already crashed
                
                var distance = Vector3.Distance(transform.position, candidate.transform.position);
                if(distance > _range) continue;
                
                if (distance < minDistance)
                {
                    targetObject = candidate;
//                    UnityEngine.Debug.Log($"distance {distance}");
                    minDistance = distance;
                    var inversedMyRotation = Quaternion.Inverse(transform.rotation);
                    var relativeEachTargetPosition =
                        inversedMyRotation * (candidate.transform.position - transform.position) / _range;
                    _state.Set(_key, relativeEachTargetPosition);
                }
            }

            if (targetObject != null)
            {
                _renderer.SetPosition(0, gameObject.transform.position);
                _renderer.SetPosition(1, targetObject.transform.position);            
                UnityEngine.Debug.Log($"target: {targetObject.name}");
                
            }

            return _state;
        }
        
        private void SetupRenderer()
        {
            _renderer = gameObject.AddComponent<LineRenderer>();
            _renderer.startWidth = 10;
            _renderer.endWidth = 10;
            _renderer.positionCount = 2;
            _renderer.startColor = Color;
            _renderer.endColor = Color;

            // TODO: 仮でとりあえず適当なプリセットテクスチャ
            var shader = Shader.Find("Particles/Additive");
            if (shader == null)
            {
                // 2018.3では上記みつからないので、半透明やってくれる以下のが良さそう
                shader = Shader.Find("GUI/Text Shader");
                var startColor = Color;
                startColor.a = 0.7f;
                _renderer.startColor = startColor;
                var endColor = Color;
                endColor.a = 0.2f;
                _renderer.endColor = endColor;
            }
        }

        public Color Color = Color.red;
    }
}
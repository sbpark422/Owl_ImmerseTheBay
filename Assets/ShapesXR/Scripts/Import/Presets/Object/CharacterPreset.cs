using UnityEngine;

namespace ShapesXr
{
    public class CharacterPreset : ProceduralObjectPreset
    {
        [SerializeField] private GameObject _pose;

        public GameObject Pose => _pose;

#if UNITY_EDITOR
        public void SetPose(GameObject target)
        {
            _pose = target;
        }
#endif
    }
}
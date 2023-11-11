using ShapesXr.Import.Core;
using UnityEngine;

namespace ShapesXr
{
    public abstract class BaseBrushPreset : BasePreset
    {
        [Header("Brush Preset")]
        [SerializeField] private float _scale = 0.005f;
        [SerializeField] private Vector3[] _shape = {Vector3.left, Vector3.right};
        
        public float Scale => _scale;
        public Vector3[] Shape => _shape;

        public abstract IStrokeParameters GetParameters();
    }
}

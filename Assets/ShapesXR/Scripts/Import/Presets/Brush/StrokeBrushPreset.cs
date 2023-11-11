using ShapesXr.Import.Core;
using UnityEngine;

namespace ShapesXr
{
    public sealed class StrokeBrushPreset : BaseBrushPreset
    {
        [SerializeField] private Vector3[] _lodShape;
        [SerializeField, Range(0, 32)] int _smoothedPoints = 1;

        [SerializeField] private bool _doubleSided;
        [SerializeField] private bool _closedShape;
        [SerializeField] private bool _closedCaps;

        [SerializeField] private Gradient _color;

        [SerializeField] private AnimationCurve _profile;

        public Vector3[] LODShape => _lodShape;

        public int SmoothedPoints => _smoothedPoints;

        public bool DoubleSided => _doubleSided;
        public bool ClosedShape => _closedShape;
        public bool ClosedCaps => _closedCaps;

        public Gradient Color => _color;

        public AnimationCurve Profile => _profile;

        public override IStrokeParameters GetParameters()
        {
            return new BrushStrokeParameters
            {
                Shape = Shape,
                LODShape = _lodShape,

                Scale = Scale,

                SmoothedPoints = _smoothedPoints,

                DoubleSided = _doubleSided,
                ClosedShape = _closedShape,
                ClosedCaps = _closedCaps,

                Color = _color,
                Profile = _profile
            };
        }
    }
}
using UnityEngine;

namespace ShapesXr
{
    public enum FitterType
    {
        BoundsFillFitted,
        UI3DStretcherFitted
    }
    
    public class ProceduralObjectPreset : BasePreset
    {
        [SerializeField] private FitterType _fitterType;
    }
}

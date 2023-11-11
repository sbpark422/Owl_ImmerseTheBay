using ShapesXr.Import.Core;
using UnityEngine;

namespace ShapesXr
{
    public class ResourcePreset : BasePreset
    {
        [SerializeField] private ResourceType _resourceType;
        public ResourceType ResourceType => _resourceType;
    }
}
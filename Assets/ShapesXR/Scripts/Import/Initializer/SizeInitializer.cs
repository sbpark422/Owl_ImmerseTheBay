using System;
using ShapesXr.Import.Core;
using UnityEngine;

namespace ShapesXr
{
    public sealed class SizeInitializer: IInitializer
    {
        public void Initialize(ISpaceDataProvider spaceData, Guid objectId, GameObject reactorObject)
        {
            Vector3 size = spaceData.PropertyHub.GetValue<Vector3>(objectId, Properties.BOUNDS_SIZE_VEC3);
            
            var stretchers = reactorObject.GetComponentsInChildren<UI3DStretcher>();

            foreach (var stretcher in stretchers)
            {
                stretcher.Size = size;

                if (stretcher.ColliderType == ColliderType.None)
                {
                    stretcher.ColliderType = ColliderType.Mesh;
                }
            }
        }
    }
}
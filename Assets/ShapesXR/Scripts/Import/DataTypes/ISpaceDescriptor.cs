using System;
using System.Collections.Generic;
using ShapesXr.Import.Core;
using UnityEngine;

namespace ShapesXr
{
    public interface ISpaceDescriptor: ISpaceObjectsManipulator
    {
        Dictionary<Guid, BasePreset> ObjectPresets { get; }

        List<GameObject> StageObjects { get; set; }
        
        int ActiveStage { get; set; }
        
        
        void ReadObjectPresets();
    }
}
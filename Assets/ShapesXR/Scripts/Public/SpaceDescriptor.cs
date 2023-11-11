using UnityEngine;

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using ShapesXr.Import.Core;
#endif

namespace ShapesXr
{
    public class SpaceDescriptor : MonoBehaviour
#if UNITY_EDITOR
        , ISpaceDescriptor
    {

        [SerializeField, HideInInspector] private List<GameObject> _stageObjects = new List<GameObject>();
        [SerializeField, HideInInspector] private string _accessCode;
        [SerializeField, HideInInspector] private string _instanceId;
        [SerializeField] private int _activeStage = -1;
        
        public PropertyHub PropertyHub { get; } = new PropertyHub();
        public InstanceCache InstanceCache { get; } = new InstanceCache();
        
        public Dictionary<Guid, GameObject> Objects { get; } = new Dictionary<Guid, GameObject>();
        public Dictionary<Guid, BasePreset> ObjectPresets { get; } = new Dictionary<Guid, BasePreset>();
        public Dictionary<Guid, Resource> Resources { get; } = new Dictionary<Guid, Resource>();

        public List<GameObject> StageObjects
        {
            get => _stageObjects;
            set => _stageObjects = value;
        }
        
        public string AccessCode
        {
            get => _accessCode;
            set
            {
                _accessCode = value;
                _instanceId = Guid.NewGuid().ToString().Substring(0, 8);
            }
        }
        
        public string InstanceId
        {
            get => _instanceId;
        }
        
        public int ActiveStage
        {
            get => _activeStage;
            set
            {
                if (value == _activeStage)
                {
                    return;
                }

                _activeStage = value;
                
                for (int i = 0; i < StageObjects.Count; i++)
                {
                    if (StageObjects[i].name == Constants.BackgroundStageName)
                    {
                        continue;
                    }

                    if (StageObjects[i])
                    {
                        StageObjects[i].SetActive(_activeStage == i);
                    }
                }
            }
        }
        
        public void CreateObject(Guid objectId)
        {
            Objects.AddOrUpdate(objectId, null);
            ObjectPresets.AddOrUpdate(objectId, null);
        }

        public void RemoveObject(Guid objectId)
        {
            if (!Objects.ContainsKey(objectId))
            {
                return;
            }
            
            Objects.Remove(objectId);
            ObjectPresets.Remove(objectId);
        }
        
        public void ReadObjectPresets()
        {
            foreach (var objectId in Objects.Keys)
            {
                var presetId = PropertyHub.GetValue<Guid>(objectId, Properties.PRESET_GUID);
                ImportResources.PresetLibrary.TryGetPreset(presetId, out var preset);

                ObjectPresets[objectId] = preset;
            }
        }
    }
#else
        {}
#endif
}
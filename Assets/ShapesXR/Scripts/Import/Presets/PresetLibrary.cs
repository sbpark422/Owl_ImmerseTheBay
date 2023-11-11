using System;
using System.Collections.Generic;
using ShapesXr.Import.Core;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ShapesXr
{
    public class PresetLibrary : ScriptableObject
    {
        [SerializeField] private List<BasePreset> _presets = new List<BasePreset>();
        
        private readonly Dictionary<Guid, BasePreset> _presetLib = new Dictionary<Guid, BasePreset>();

        public List<BasePreset> Presets => _presets;

        private void OnEnable()
        {
            CreateDictionary();
        }

        public bool TryGetPreset(Guid guid, out BasePreset preset)
        {
            return _presetLib.TryGetValue(guid, out preset);
        }

        public void CreateDictionary()
        {
            _presetLib.Clear();
            foreach (var prefab in Presets)
            {
                _presetLib.AddOrUpdate(prefab.PresetID, prefab);
            }
        }
        
        public bool TryInstantiateAssetFromPreset(Guid presetId, Transform parent, out GameObject instance)
        {
            instance = null;

            if (!TryGetPreset(presetId, out var preset))
            {
                return false;
            }
            
            instance = Instantiate(preset.Asset, parent);
                
            instance.transform.ResetLocalTransform();
                
            return true;
        }

#if UNITY_EDITOR
        public void AddPresetsFromUnityAssetDatabase()
        {
            _presets.Clear();

            var guids = new List<string>(AssetDatabase.FindAssets("t:BasePreset"));
            var counter = 0;

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path == null)
                {
                    continue;
                }

                var preset = AssetDatabase.LoadAssetAtPath<BasePreset>(path);
                if (preset != null)
                {
                    if (preset.PresetID == default)
                    {
                        preset.GenerateID();
                    }

                    _presets.Add(preset);
                    counter++;
                }
            }

            if (counter > 0)
            {
                Debug.Log($"Found {counter} prefabs with {typeof(BasePreset).Name} component for the local library of assets");
            }
            else
            {
                Debug.Log($"Haven't found any prefabs with {typeof(BasePreset).Name} component for the local library of assets");
            }

            CreateDictionary();
        }
#endif
    }
}
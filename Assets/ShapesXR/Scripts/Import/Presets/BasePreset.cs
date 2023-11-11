using System;
using UnityEditor;
using UnityEngine;

namespace ShapesXr
{
    public class BasePreset : ScriptableObject, ISerializationCallbackReceiver
    {
        [Header("Base settings")]
        [SerializeField] protected GameObject _asset;
        [SerializeField] protected Guid _presetID;

        [SerializeField, HideInInspector] private byte[] _serializedGuid = new byte[16];

        public virtual GameObject Asset => _asset;
        
        public Guid PresetID => _presetID;

        public string Name => name;

#if UNITY_EDITOR
        public void Initialize(GameObject target)
        {
            _asset = target;
        }

        public void SetNewPresetID(Guid id)
        {
            _presetID = id;
        }
        private void OnValidate()
        {
            EditorUtility.SetDirty(this);
        }

        public void GenerateID()
        {
            _presetID = Guid.NewGuid();

            OnBeforeSerialize();
            Debug.Log($"Generated new GUID for {name} prefab. GUID: {_presetID}");
        }
#endif

        public void OnBeforeSerialize()
        {
            _serializedGuid = _presetID.ToByteArray();
        }

        public void OnAfterDeserialize()
        {
            if (_serializedGuid == null || _serializedGuid.Length != 16)
            {
                _serializedGuid = new byte[16];
            }

            _presetID = new Guid(_serializedGuid);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BasePreset), true)]
    public class PresetInspector : Editor
    {
        private static readonly string[] _dontIncludeFields = new string[] { "m_Script" };
        private bool _foldoutAssetList;
        private BasePreset _preset;
        private SerializedObject _serializedObject;

        private void OnEnable()
        {
            _preset = target as BasePreset;
            _serializedObject = new SerializedObject(_preset);
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            if (GUILayout.Button($"PresetID [{ _preset.PresetID}]", EditorStyles.foldoutHeader))
            {
                _foldoutAssetList = !_foldoutAssetList;
            }

            if (_foldoutAssetList)
            {
                EditorGUILayout.BeginHorizontal();

                // Do this only if the ID is not set yet. Otherwise it's very dangerous to allow to regenerate IDs because of a high
                // chance of doing it by accident on presets that are used in production.
                if (_preset.PresetID == default)
                {
                    if (GUILayout.Button("Generate", EditorStyles.miniButton))
                    {
                        _preset.GenerateID();
                        EditorUtility.SetDirty(_preset);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            //DrawPropertiesExcluding(serializedObject, _dontIncludeFields);
            base.DrawDefaultInspector();
            _serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
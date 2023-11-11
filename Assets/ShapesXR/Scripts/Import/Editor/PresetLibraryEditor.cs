using UnityEditor;
using UnityEngine;

namespace ShapesXr
{
    [CustomEditor(typeof(PresetLibrary))]
    public class PresetLibraryEditor : Editor
    {
        private static readonly string[] _dontIncludeFields = new string[] { "m_Script", "_assets" };
        private bool _foldoutAssetList;
        private PresetLibrary _presetLibrary;
        private SerializedObject _serializedObject;

        private void OnEnable()
        {
            _presetLibrary = target as PresetLibrary;
            _serializedObject = new SerializedObject(_presetLibrary);
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            if (GUILayout.Button("Update Presets"))
            {
                _presetLibrary.AddPresetsFromUnityAssetDatabase();
                EditorUtility.SetDirty(_presetLibrary);
            }

            //DrawPropertiesExcluding(serializedObject, _dontIncludeFields);
            //base.OnInspectorGUI();
            DrawDefaultInspector();

            //GUILayout.Label("Number of Presets: " + _assetLibrary.Presets.Count);
            GUILayout.Space(6);

            if (GUILayout.Button($"{_presetLibrary.Presets.Count} Presets", EditorStyles.foldoutHeader))
            {
                _foldoutAssetList = !_foldoutAssetList;
            }

            if (_foldoutAssetList)
            {
                EditorGUI.indentLevel++;
                foreach (var prefab in _presetLibrary.Presets)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(prefab, typeof(ScriptableObject), false);
                    GUILayout.Label("guid: " + prefab.PresetID);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(6);
                }
                EditorGUI.indentLevel--;
            }

            _serializedObject.ApplyModifiedProperties();
        }
    }
}

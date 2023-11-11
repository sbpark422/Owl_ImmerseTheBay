#if UNITY_EDITOR

using System.IO;
using ShapesXr.Import.Core;
using UnityEditor;
using UnityEngine;

namespace ShapesXr
{
    [CustomEditor(typeof(SpaceDescriptor))]
    public class SpaceDescriptorEditor : Editor
    {
        private SpaceDescriptor _spaceDescriptor;

        
        private void OnEnable()
        {
            _spaceDescriptor = serializedObject.targetObject as SpaceDescriptor;
        }

        public override void OnInspectorGUI()
        {
            if (!_spaceDescriptor)
            {
                return;
            }

            int stageCount = _spaceDescriptor.StageObjects.Count;

            if (stageCount == 0)
            {
                return;
            }

            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.SelectableLabel($"Space: {_spaceDescriptor.AccessCode.Insert(3, " ")}", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(stageCount == 0);

            if (_spaceDescriptor.StageObjects[0].name == Constants.BackgroundStageName)
            {
                var activeStage = _spaceDescriptor.ActiveStage;
                activeStage = EditorGUILayout.IntSlider($"Stage: {activeStage}/{stageCount - 1}", activeStage, 1, stageCount - 1);
                _spaceDescriptor.ActiveStage = activeStage;
            }
            else
            {
                _spaceDescriptor.ActiveStage = EditorGUILayout.IntSlider($"Stage: {_spaceDescriptor.ActiveStage + 1}/{stageCount}", _spaceDescriptor.ActiveStage + 1, 1, stageCount) - 1;
            }

            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Reimport"))
            {
                Reimport();
            }
            
            EditorGUILayout.EndVertical();
        }

        private void Reimport()
        {
            string spaceDataPath = PathUtils.GetSpaceDataPath(_spaceDescriptor);

            if (Directory.Exists(spaceDataPath))
            {
                Directory.Delete(spaceDataPath, true);
            }
            
            string metaPath = spaceDataPath + ".meta";

            if (File.Exists(metaPath))
            {
                File.Delete(metaPath);
            }
            
            SpaceImporter.ImportSpace(_spaceDescriptor.AccessCode.ToLower());
            DestroyImmediate(_spaceDescriptor.gameObject);
        }
    }
}

#endif
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ShapesXr
{
    [CustomEditor(typeof(UI3DStretcher))]
    public class UI3DStretcherEditor : Editor
    {
        private UI3DStretcher _stretcher;
        
        private SerializedProperty _sourceMeshProperty;
        
        private SerializedProperty _pivotPositionProperty;
        private SerializedProperty _sourceMeshRotationProperty;
        private SerializedProperty _finalPositionOffsetProperty;
        
        private SerializedProperty _sizeProperty;
        private SerializedProperty _customChamferProperty;
        private SerializedProperty _chamferScaleProperty;
        private SerializedProperty _outerMeshScaleProperty;
        
        private SerializedProperty _slicePlusProperty;
        private SerializedProperty _sliceMinusProperty;
        private SerializedProperty _showSliceLineProperty;

        private SerializedProperty _recalculateNormalsProperty;
        
        private SerializedProperty _setCustomColorProperty;
        private SerializedProperty _colorModificatorProperty;
        private SerializedProperty _colorProperty;
        private SerializedProperty _setAlphaProperty;
        
        private SerializedProperty _colliderTypeProperty;
        private SerializedProperty _colliderSizeProperty;

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedo;

            _stretcher = (UI3DStretcher) target;
            
            _sourceMeshProperty = serializedObject.FindProperty("_sourceMesh");
            _recalculateNormalsProperty = serializedObject.FindProperty("_recalculateNormals");
            
            _pivotPositionProperty = serializedObject.FindProperty("_pivotPosition");
            _sourceMeshRotationProperty = serializedObject.FindProperty("_sourceMeshRotation");
            _finalPositionOffsetProperty = serializedObject.FindProperty("_finalPositionOffset");

            _sizeProperty = serializedObject.FindProperty("_size");
            _customChamferProperty = serializedObject.FindProperty("_customChamfer");
            _chamferScaleProperty = serializedObject.FindProperty("_chamferScale");
            _outerMeshScaleProperty = serializedObject.FindProperty("_outerMeshScale");
            
            _slicePlusProperty = serializedObject.FindProperty("_slicePlus");
            _sliceMinusProperty = serializedObject.FindProperty("_sliceMinus");
            _showSliceLineProperty = serializedObject.FindProperty("_showSliceLine");

            _setCustomColorProperty = serializedObject.FindProperty("_setCustomColor");
            _colorModificatorProperty = serializedObject.FindProperty("_colorModificator");
            _colorProperty = serializedObject.FindProperty("_color");
            _setAlphaProperty = serializedObject.FindProperty("_setAlpha");

            _colliderTypeProperty = serializedObject.FindProperty("_colliderType");
            _colliderSizeProperty = serializedObject.FindProperty("_colliderSize");
        }

        public void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            if(_stretcher.transform.rotation != Quaternion.identity || _stretcher.transform.localScale != Vector3.one)
            {
                EditorGUILayout.HelpBox("Please setup slicing with zero rotation and unit scale", MessageType.Warning);
            }
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(_sourceMeshProperty);
            EditorGUILayout.PropertyField(_recalculateNormalsProperty);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Transform", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(_pivotPositionProperty);
            EditorGUILayout.PropertyField(_sourceMeshRotationProperty);
            EditorGUILayout.PropertyField(_finalPositionOffsetProperty);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(_sizeProperty);
            EditorGUILayout.PropertyField(_customChamferProperty);
            EditorGUILayout.PropertyField(_chamferScaleProperty);
            EditorGUILayout.PropertyField(_outerMeshScaleProperty);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Slicing", EditorStyles.boldLabel);
            
            Vector3 sliceMinus = _sliceMinusProperty.vector3Value;
            Vector3 slicePlus = _slicePlusProperty.vector3Value;

            EditorGUILayout.MinMaxSlider($"X [{sliceMinus.x:F2} - {slicePlus.x:F2}]", ref sliceMinus.x, ref slicePlus.x, 0f, 1f);
            EditorGUILayout.MinMaxSlider($"Y [{sliceMinus.y:F2} - {slicePlus.y:F2}]", ref sliceMinus.y, ref slicePlus.y, 0f, 1f);
            EditorGUILayout.MinMaxSlider($"Z [{sliceMinus.z:F2} - {slicePlus.z:F2}]", ref sliceMinus.z, ref slicePlus.z, 0f, 1f);

            _sliceMinusProperty.vector3Value = sliceMinus;
            _slicePlusProperty.vector3Value = slicePlus;
            
            EditorGUILayout.PropertyField(_showSliceLineProperty);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Color", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(_setCustomColorProperty, new GUIContent("Custom Color"));

            if (_setCustomColorProperty.boolValue)
            {
                EditorGUILayout.PropertyField(_colorModificatorProperty, new GUIContent("Modificator"));

                if (_colorModificatorProperty.intValue != (int) ColorModificator.Invert)
                {
                    EditorGUILayout.PropertyField(_colorProperty);
                }

                string alphaText = (ColorModificator) _colorModificatorProperty.intValue + " Alpha";
                
                EditorGUILayout.PropertyField(_setAlphaProperty, new GUIContent(alphaText));
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Collision", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(_colliderTypeProperty);
            
            if(_colliderTypeProperty.enumValueIndex != (int) ColliderType.None)
            {
                _colliderSizeProperty.floatValue = EditorGUILayout.Slider("Collider Size", _colliderSizeProperty.floatValue, 0f, 2f);
            }
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Reset Size"))
            {
                _stretcher.ResetSize();
            }
            
            if (GUILayout.Button("Reset Slicing"))
            {
                _stretcher.SliceMinus = 0.49f * Vector3.one;
                _stretcher.SlicePlus = 0.51f * Vector3.one;
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Updated UI 3D Stretcher properties");

                serializedObject.ApplyModifiedProperties();
                _stretcher.ResetAll();
            }
        }
        
        private void OnUndoRedo()
        {
            serializedObject.Update();

            _stretcher.ResetAll();
        }
    }
}
#endif
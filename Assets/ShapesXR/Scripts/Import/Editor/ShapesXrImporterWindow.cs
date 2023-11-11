using ShapesXr.Import.Core;
using UnityEditor;
using UnityEngine;

namespace ShapesXr
{
    public class ShapesXrImporterWindow : EditorWindow
    {
        private const string ShapesXRPluginSettingsOpened = "shapes_xr_plugin_settings_opened";
        
        private static Texture2D _logo;

        private static bool SettingsOpened
        {
            get => EditorPrefs.GetBool(ShapesXRPluginSettingsOpened);
            set => EditorPrefs.SetBool(ShapesXRPluginSettingsOpened, value);
        }


        private static SerializedObject _importSettingsObject;
        
        private static SerializedProperty _materialMapperProperty;
        private static SerializedProperty _importedDataDirectoryProperty;
        private static SerializedProperty _materialModeProperty;
        private static SerializedProperty _gltfMaterialTextureProperties;

        public static string ErrorMessage { get; set; } = "";

        [MenuItem("ShapesXR/Importer")]
        public static void ShowWindow()
        {
            GetWindow<ShapesXrImporterWindow>(false, "ShapesXR Importer", true);
        }

        private void OnEnable()
        {
            _importSettingsObject = new SerializedObject(ImportSettings.Instance);
            
            _importedDataDirectoryProperty = _importSettingsObject.FindProperty("_importedDataDirectory");
            _materialMapperProperty = _importSettingsObject.FindProperty("_materialMapper");
            _materialModeProperty = _importSettingsObject.FindProperty("_materialMode");
            _gltfMaterialTextureProperties = _importSettingsObject.FindProperty("_gltfMainTextureProperties");

            ImportSettingsProvider.ImportSettings = ImportSettings.Instance;
        }

        private void OnGUI()
        {
            _importSettingsObject.Update();

            EditorGUILayout.BeginVertical();

            DrawLogo();

            if (!ErrorMessage.IsNullOrEmpty())
            {
                EditorGUILayout.HelpBox(ErrorMessage, MessageType.Error);
            }

            EditorGUILayout.BeginHorizontal();

            string accessCode = GUILayout.TextField(ImportSettings.Instance.AccessCode);

            ImportSettings.Instance.AccessCode = accessCode;

            if (GUILayout.Button("Import Space"))
            {
                ErrorMessage = "";
                
                _importSettingsObject.ApplyModifiedProperties();
                
                var trimmedCode = accessCode.Trim().Replace(" ", "");

                if (string.IsNullOrEmpty(trimmedCode))
                {
                    Analytics.SendEvent(EventStatus.incorrect_code_signature);
                    ErrorMessage = "You haven't entered a code. Enter the code to import space";
                    
                    Debug.LogError(ErrorMessage);
                }
                else
                {
                    SpaceImporter.ImportSpace(accessCode.Replace(" ", "").ToLower()); 
                }
            }

            EditorGUILayout.EndHorizontal();

            SettingsOpened = EditorGUILayout.Foldout(SettingsOpened, "Settings", EditorStyles.foldoutHeader);

            if (!SettingsOpened)
            {
                EditorGUILayout.EndVertical();
                _importSettingsObject.ApplyModifiedProperties();
                
                return;
            }

            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(_importedDataDirectoryProperty);
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Material", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(_materialModeProperty,new GUIContent("Import Mode"));
            EditorGUILayout.PropertyField(_materialMapperProperty,new GUIContent("Mapper"));
            
            GUILayout.Space(10f);
            EditorGUILayout.HelpBox(
                "If you're using GLB/glTF models in your space and want to import them with textures, you can add material properties for main textures that your GLB/glTF plugin is using below.\n\n" +
                "Please note that more than one texture per renderer is not supported for import right now.",
                MessageType.Info
            );
            EditorGUILayout.PropertyField(
                _gltfMaterialTextureProperties,
                new GUIContent("GLB/glTF Material Texture Properties:")
            );

            var rawString = _importedDataDirectoryProperty.stringValue;
            _importedDataDirectoryProperty.stringValue = rawString.Trim(' ', '\\', '/');
            
            _importSettingsObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }

        private void DrawLogo()
        {
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.FlexibleSpace();
            GUILayout.Label(ImportResources.ShapesXrLogo);
            GUILayout.FlexibleSpace();
            
            EditorGUILayout.EndHorizontal();
        }
    }
}
using UnityEngine;

namespace ShapesXr
{
    [CreateAssetMenu(fileName = "ImportResources", menuName = "ShapesXR/Import Resources")]
    public class ImportResources : ScriptableObject
    {
        public const string MtlNameTag = "<name>";
        
        [Header("Editor")]
        [SerializeField] private Texture2D _shapesXrLogo;
        
        [Header("Space")]
        [SerializeField] private GameObject _spaceDescriptorPrefab;

        [Header("Presets")] 
        [SerializeField] private PresetLibrary _presetLibrary;

        [Header("Import")] 
        [SerializeField] private GameObject _emptyObjectPrefab;

        [SerializeField] private TextAsset _mtlTemplate;

        private static ImportResources _instance;

        public static ImportResources Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Resources.Load<ImportResources>("ImportResources");
                }

                return _instance;
            }
        }

        public static Texture2D ShapesXrLogo => Instance._shapesXrLogo;
        
        public static GameObject SpaceDescriptorPrefab => Instance._spaceDescriptorPrefab;
        
        public static PresetLibrary PresetLibrary => Instance._presetLibrary;
        
        public static GameObject EmptyObjectPrefab => Instance._emptyObjectPrefab;

        public static string MtlTemplateString => Instance._mtlTemplate.text;
    }
}
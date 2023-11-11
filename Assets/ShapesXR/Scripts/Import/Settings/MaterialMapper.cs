using ShapesXr.Import.Core;
using UnityEngine;

namespace ShapesXr
{
    [CreateAssetMenu(fileName = "MaterialMapper", menuName = "ShapesXR/Material Mapper")]
    public class MaterialMapper : ScriptableObject, IMaterialMapper
    {
        [Header("PBR")]
        [SerializeField] private BaseMaterialParameters _pbrOpaque;
        [SerializeField] private BaseMaterialParameters _pbrTransparent;
        [SerializeField] private CutoutMaterialParameters _pbrCutout;

        [Header("Unlit")]
        [SerializeField] private BaseMaterialParameters _unlitOpaque;
        [SerializeField] private BaseMaterialParameters _unlitTransparent;
        [SerializeField] private CutoutMaterialParameters _unlitCutout;

        [Header("Special")]
        [SerializeField] private BaseMaterialParameters _additive;
        [SerializeField] private BaseMaterialParameters _multiply;

        
        public IBaseMaterialParametersProvider PbrOpaque => _pbrOpaque;
        public IBaseMaterialParametersProvider PbrTransparent => _pbrTransparent;
        public ICutoffMaterialParameterProvider PbrCutout=> _pbrCutout;

        public IBaseMaterialParametersProvider UnlitOpaque => _unlitOpaque;
        public IBaseMaterialParametersProvider UnlitTransparent => _unlitTransparent;
        public ICutoffMaterialParameterProvider UnlitCutout => _unlitCutout;

        public IBaseMaterialParametersProvider Additive => _additive;
        public IBaseMaterialParametersProvider Multiply => _multiply;
    }
}
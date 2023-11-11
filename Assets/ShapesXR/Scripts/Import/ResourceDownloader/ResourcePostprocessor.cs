using ShapesXr.Import.Core;
using UnityEditor;
using UnityEngine;

namespace ShapesXr
{
    public static class ResourcePostprocessor
    {
        public static void PostProcessAllResources(ISpaceDescriptor spaceDescriptor)
        {
            if (spaceDescriptor.Resources.IsNullOrEmpty())
            {
                return;
            }

            AssetDatabase.Refresh();

            var postProcessorFactory = new ResourcePostProcessorFactory(ImportSettings.Instance.GltfMainTextureProperties);

            foreach (var kvp in spaceDescriptor.Resources)
            {
                var resource = kvp.Value;
                var postProcessor = postProcessorFactory.GetPostProcessor(
                    spaceDescriptor,
                    resource
                );

                if (postProcessor != null)
                {
                    postProcessor.PostProcess();
                }
                else
                {
                    Debug.LogError($"Failed to find post processor for resource {resource.Id}");
                }
            }
        }
    }
}
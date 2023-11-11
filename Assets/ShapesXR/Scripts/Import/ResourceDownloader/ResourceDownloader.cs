using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShapesXr.Import.Core;
using UnityEngine;

namespace ShapesXr
{
    public static class ResourceDownloader
    {
#if SHAPES_XR_DEV
        private const string ServerURL = "https://api-dev.shapes.app/";
#else
        private const string ServerURL = "https://api.shapes.app/";
#endif

        private static readonly HashSet<Guid> DownloadingResources = new HashSet<Guid>();

        public static void DownloadAllResources(ISpaceDescriptor spaceDescriptor)
        {
            foreach (var kvp in spaceDescriptor.ObjectPresets)
            {
                if (!(kvp.Value is ResourcePreset preset))
                {
                    continue;
                }

                var objectId = kvp.Key;
                var resourceId = spaceDescriptor.PropertyHub.GetValue<Guid>(objectId, Properties.RESOURCE_GUID);

                if (spaceDescriptor.Resources.ContainsKey(resourceId))
                {
                    continue;
                }

                var resourceResponse =
                    ResourceDownloaderHelper.DownloadResource(ServerURL, spaceDescriptor, resourceId,
                        preset.ResourceType);

                if (resourceResponse == null)
                {
                    Debug.LogWarning($"Resource with id {resourceId} not found on server. Skipping object import");
                    continue;
                }

                var resource = new Resource(preset.ResourceType, resourceId, resourceResponse);
                spaceDescriptor.Resources.Add(resourceId, resource);
            }
        }

        public static void DownloadAllResourcesInParallel(ISpaceDescriptor spaceDescriptor)
        {
            var presetsToDownload = spaceDescriptor.ObjectPresets.Where(
                p => p.Value is ResourcePreset
            ).ToList();
            
            DownloadingResources.Clear();
            
            var tasks = new List<Task>();
            
            foreach (var kvp in presetsToDownload)
            {
                tasks.Add(Task.Run(() => DownloadAndAddResource(spaceDescriptor, kvp)));
            }

            while (!tasks.TrueForAll(t => t.IsCompleted))
            {
                
            }
        }

        private static async Task DownloadAndAddResource(ISpaceDescriptor spaceDescriptor, KeyValuePair<Guid, BasePreset> kvp)
        {
            var resource = await DownloadResourcePresetAsync(spaceDescriptor, kvp.Key, kvp.Value);
            if (resource != null)
            {
                spaceDescriptor.Resources.Add(resource.Id, resource);
            }
        }
        
        private static async Task<Resource> DownloadResourcePresetAsync(ISpaceDescriptor spaceDescriptor, Guid objectId, BasePreset preset)
        {
            var resourceId = spaceDescriptor.PropertyHub.GetValue<Guid>(objectId, Properties.RESOURCE_GUID);
            if (spaceDescriptor.Resources.ContainsKey(resourceId) || DownloadingResources.Contains(resourceId))
            {
                return null;
            }

            DownloadingResources.Add(resourceId);

            var resourceType = ((ResourcePreset)preset).ResourceType;
            var resourceResponse = await ResourceDownloaderHelper.DownloadResourceAsync(
                ServerURL, spaceDescriptor, resourceId,
                resourceType
            );

            if (resourceResponse == null)
            {
                Debug.LogWarning($"Resource with id {resourceId} not found on server. Skipping object import");
                return null;
            }

            return new Resource(resourceType, resourceId, resourceResponse);
        }
    }
}
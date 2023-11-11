using System.Diagnostics;
using System.IO;
using System.Net;
using MessagePack;
using ShapesXr.Import.Core;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace ShapesXr
{
    public static class SpaceImporter
    {
        private static bool _messagePackInitialized;
        private const int RequestTimeout = 5 * 1000;

        public static void ImportSpace(string accessCode)
        {
            var importTimer = Stopwatch.StartNew();

            if (!CheckSettingsValidness())
            {
                Analytics.SendEvent(EventStatus.invalid_import_settings);
                return;
            }

            string spaceUrl = GetSpaceUrl(accessCode);

            if (string.IsNullOrEmpty(spaceUrl))
            {
                Analytics.SendEvent(EventStatus.cannot_get_space_url);

                ShapesXrImporterWindow.ErrorMessage = "The code is incorrect. If you're sure you're entering a valid code, write us at hey@shapesxr.com";
                Debug.LogError(ShapesXrImporterWindow.ErrorMessage);
                return;
            }

            var spaceInfoData = GetSpaceInfoDataStream(spaceUrl);

            if (spaceInfoData == null)
            {
                Analytics.SendEvent(EventStatus.cannot_get_space_data);
                ShapesXrImporterWindow.ErrorMessage = $"Error while downloading space. If you see continue to see this, write us at hey@shapesxr.com";
                Debug.LogError(ShapesXrImporterWindow.ErrorMessage);
            }

            if (!_messagePackInitialized)
            {
                Utils.InitializeMessagePack();
                _messagePackInitialized = true;
            }

            SpaceInfo spaceInfo;
            
            try
            {
                spaceInfo = MessagePackSerializer.Deserialize<SpaceInfo>(spaceInfoData);
            }
            catch (MessagePackSerializationException e)
            {
                Analytics.SendEvent(EventStatus.cannot_deserialize_space_data);

                ShapesXrImporterWindow.ErrorMessage = $"Cannot deserialize space data: {e.Message}";
                Debug.LogError(ShapesXrImporterWindow.ErrorMessage);
                
                return;
            }
            
            var spaceDescriptor = CreateSpaceDescriptorObject(accessCode, out var descriptorObject);

            EditsExecutor.ExecuteAll(spaceDescriptor, spaceInfo.Edits);

            spaceDescriptor.ReadObjectPresets();

            AssetDatabase.Refresh();

            MaterialCollection.Refresh(ImportSettings.Instance);
            
            // For some reason, only in unity 2020 parallel downloads do not work as intended.
            // They cause some race conditions where they should not and generally are very inconsistent.
#if ENABLE_PARALLEL_DOWNLOADS
            ResourceDownloader.DownloadAllResourcesInParallel(spaceDescriptor);
#else
            ResourceDownloader.DownloadAllResources(spaceDescriptor);
#endif
            ResourcePostprocessor.PostProcessAllResources(spaceDescriptor);
            
            var objectSpawner = new ObjectSpawner(spaceDescriptor);
            objectSpawner.SpawnAllObjects();
            Selection.activeObject = descriptorObject;
            Debug.Log($"Space import finished in: {importTimer.Elapsed.TotalSeconds}");
            Analytics.SendEvent(EventStatus.success, importTimer.Elapsed.TotalSeconds);
        }

        private static bool CheckSettingsValidness()
        {
            ImportSettingsProvider.ImportSettings = ImportSettings.Instance;
            ImportSettingsProvider.IsDev = Utils.IsDev;
            
            if (PathUtils.PathContainsInvalidCharacters(ImportSettingsProvider.ImportSettings.ImportedDataDirectory))
            {
                ShapesXrImporterWindow.ErrorMessage = $"Incorrect Imported Data Directory path: {ImportSettingsProvider.ImportSettings.ImportedDataDirectory}";
                Debug.LogError(ShapesXrImporterWindow.ErrorMessage);
                return false;
            }

            if (ImportSettingsProvider.ImportSettings.MaterialMapper == null)
            {
                ShapesXrImporterWindow.ErrorMessage = "Material Mapper not found. Please specify one in ShapesXR Importer settings";
                Debug.LogError(ShapesXrImporterWindow.ErrorMessage);
                return false;
            }
            
            return true;
        }
        
        private static string GetSpaceUrl(string spaceCode)
        {
#if SHAPES_XR_DEV
            string requestUrl = $"https://api-dev.shapes.app/accesscode/space-url/{spaceCode}";
#else
            string requestUrl = $"https://api.shapes.app/accesscode/space-url/{spaceCode}";
#endif

            var webRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            webRequest.Method = "GET";
            webRequest.Timeout = RequestTimeout;
            
            try
            {
                var response = (HttpWebResponse)webRequest.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());

                return reader.ReadToEnd().Trim('"');
            }
            catch (WebException e)
            {
                Debug.LogError($"Error getting space Url: {e.Message}");
                return null;
            }
        }

        private static Stream GetSpaceInfoDataStream(string spaceUrl)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(spaceUrl);
            webRequest.Method = "GET";
            webRequest.Timeout = RequestTimeout; 
            
            try
            {
                var response = (HttpWebResponse)webRequest.GetResponse();

                if (response.ContentLength == 0)
                {
                    return null;
                }
                
                return response.GetResponseStream();

            }
            catch (WebException e)
            {
                Debug.LogError($"Error getting space info: {e.Message}");
                return null;
            }
        }
        
        private static ISpaceDescriptor CreateSpaceDescriptorObject(string accessCode, out GameObject descriptorObject)
        {
            descriptorObject = Object.Instantiate(ImportResources.SpaceDescriptorPrefab);
            
            // TODO: add space name as well, eg Space: Bright moon (dima)
            descriptorObject.name = $"Space - {accessCode}";

            var spaceDescriptor = descriptorObject.GetComponent<ISpaceDescriptor>();
            spaceDescriptor.AccessCode = accessCode;
            
            Properties.InitProperties(spaceDescriptor.PropertyHub);

            return spaceDescriptor;
        }
    }
}
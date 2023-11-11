using System;
using System.Collections.Generic;
using System.Linq;
using ShapesXr.Import.Core;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace ShapesXr
{
    public static class Constants
    {
        public const string BackgroundStageId = "00000000-0000-0000-0000-000000000001";
        public const string BackgroundStageName = "Background";
    }

    public class ObjectSpawner
    {
        private readonly ISpaceDescriptor _spaceDescriptor;

        private readonly List<Guid> _specialObjectIds = new List<Guid>();
        private readonly Guid[] _allObjectIds;



        public ObjectSpawner(ISpaceDescriptor spaceDescriptor)
        {
            _spaceDescriptor = spaceDescriptor;
            _allObjectIds = _spaceDescriptor.Objects.Keys.ToArray();
        }

        public void SpawnAllObjects()
        {
            SpawnViewpoints();
            SpawnStageContainers();

            int objectCounter = 0;

            for (var i = 0; i < _allObjectIds.Length; i++)
            {
                var objectId = _allObjectIds[i];
                if (_specialObjectIds.Contains(objectId) || !NeedsSpawning(objectId))
                {
                    continue;
                }

                objectCounter++;

                var spawnedObject = SpawnObject(objectId);
                var preset = _spaceDescriptor.ObjectPresets[objectId];

                if (!preset)
                {
                    Debug.LogWarning($"Preset not found for object '{objectId}'. Skipping initialization.");
                    continue;
                }

                spawnedObject.name = $"{_spaceDescriptor.ObjectPresets[objectId].Name} {objectCounter}";

                if (NeedsInitialization(objectId))
                {
                    InitializeObject(objectId);
                }
            }

            for (var i = 0; i < _allObjectIds.Length; i++)
            {
                var objectId = _allObjectIds[i];
                if (_specialObjectIds.Contains(objectId) || !NeedsSpawning(objectId))
                {
                    continue;
                }

                if (NeedsInitialization(objectId))
                {
                    InitializeParentingObjects(objectId);
                }
            }
        }

        private void SpawnViewpoints()
        {
            var spaceRoot = ((Component)_spaceDescriptor).transform;

            var viewpointsRoot = new GameObject("Viewpoints");
            viewpointsRoot.transform.SetParent(spaceRoot);
            viewpointsRoot.transform.ResetLocalTransform();

            var viewpointControllerId = _spaceDescriptor.ObjectPresets.First(kvp => kvp.Value is ViewpointControllerPreset).Key;

            Guid[] viewpointIds = null;

            if (viewpointControllerId != default)
            {
                _specialObjectIds.Add(viewpointControllerId);
                viewpointIds = _spaceDescriptor.PropertyHub.GetValue<Guid[]>(viewpointControllerId, Properties.VIEWPOINT_ORDER_ARR);
            }

            if (viewpointIds.IsNullOrEmpty())
            {
                Debug.LogWarning("Viewpoint order property not found: using legacy space fallback");
                viewpointIds = _spaceDescriptor.ObjectPresets.Where(kvp => kvp.Value is ViewpointPreset).Select(kvp => kvp.Key).ToArray();
            }

            if (viewpointIds == null)
            {
                return;
            }

            for (int i = 0; i < viewpointIds.Length; i++)
            {
                var id = viewpointIds[i];
                var viewpointObject = SpawnObject(id, viewpointsRoot.transform);
                viewpointObject.name = $"Viewpoint {i + 1}";

                _specialObjectIds.Add(id);
            }
        }

        private void SpawnStageContainers()
        {
            var spaceRoot = ((Component)_spaceDescriptor).transform;

            var stagesRoot = new GameObject("Stages");
            stagesRoot.transform.SetParent(spaceRoot);
            stagesRoot.transform.ResetLocalTransform();

            var stageControllerId = _spaceDescriptor.ObjectPresets.First(kvp => kvp.Value is StageControllerPreset).Key;

            Guid[] stageIds = null;

            if (stageControllerId != default)
            {
                _specialObjectIds.Add(stageControllerId);
                stageIds = _spaceDescriptor.PropertyHub.GetValue<Guid[]>(stageControllerId, Properties.STAGE_ORDER_ARR);
            }

            if (stageIds.IsNullOrEmpty())
            {
                Debug.LogWarning("Stage order property not found: using legacy space fallback");
                stageIds = _spaceDescriptor.ObjectPresets.Where(kvp => kvp.Value is StagePreset).Select(kvp => kvp.Key).ToArray();
            }

            if (stageIds == null)
            {
                return;
            }

            for (int i = 0; i < stageIds.Length; i++)
            {
                var stageId = stageIds[i];


                var stageObject = SpawnObject(stageId, stagesRoot.transform);
                stageObject.name = (stageId == Guid.Parse(Constants.BackgroundStageId)) ? Constants.BackgroundStageName : $"Stage {i}";

                _specialObjectIds.Add(stageId);
                _spaceDescriptor.StageObjects.Add(stageObject);
            }
        }

        private GameObject SpawnObject(Guid objectId, Transform rootTransform = null)
        {
            if (!rootTransform)
            {
                var stageObjectId = _spaceDescriptor.PropertyHub.GetValue<Guid>(objectId, Properties.STAGE_GUID);

                if (stageObjectId != default)
                {
                    if (_spaceDescriptor.Objects.ContainsKey(stageObjectId) && _spaceDescriptor.Objects[stageObjectId] != null)
                    {
                        rootTransform = _spaceDescriptor.Objects[stageObjectId].transform;
                    }
                    else
                    {
                        Debug.LogWarning($"Stage with Id '{stageObjectId}' no longer exists but object '{objectId}' has reference to it. Putting object to space root");
                    }
                }
            }

            if (!rootTransform)
            {
                rootTransform = ((Component)_spaceDescriptor).transform;
            }

            var objectContainer = Object.Instantiate(ImportResources.EmptyObjectPrefab, rootTransform);

            var trInfo = _spaceDescriptor.PropertyHub.GetValue<TrInfo>(objectId, Properties.TRANSFORM);

            trInfo.SetTransformValues(objectContainer.transform);

            _spaceDescriptor.Objects[objectId] = objectContainer;

            return objectContainer;
        }

        private void InitializeObject(Guid objectId)
        {
            var objectContainer = _spaceDescriptor.Objects[objectId];

            var preset = _spaceDescriptor.ObjectPresets[objectId];

            if (!ImportResources.PresetLibrary.TryInstantiateAssetFromPreset(preset.PresetID, objectContainer.transform,
               out var createdObject))
            {
                return;
            }

            InitializeReactors(objectId, preset, createdObject);
        }

        private void InitializeParentingObjects(Guid objectId)
        {
            var objectContainer = _spaceDescriptor.Objects[objectId];

            var preset = _spaceDescriptor.ObjectPresets[objectId];

            InitializeParentReactor(objectId, preset, objectContainer);
        }


        private void InitializeParentReactor(Guid objectId, BasePreset preset, GameObject createdObject)
        {
            var reactor = createdObject.transform.GetComponentInChildren<GroupPropertyReactor>(true);

            if (reactor == null)
            {
                return;
            }

            IInitializer initializer = InitializerFactory.GetInitializer(reactor, preset);
            initializer.Initialize(_spaceDescriptor, objectId, reactor.gameObject);
            var trInfo = _spaceDescriptor.PropertyHub.GetValue<TrInfo>(objectId, Properties.TRANSFORM);

            trInfo.SetTransformValues(createdObject.transform);
        }


        private void InitializeReactors(Guid objectId, BasePreset preset, GameObject createdObject)
        {
            var reactors = createdObject.transform.parent.GetComponentsInChildren<PropertyReactorComponent>(true);

            IInitializer initializer;
            BaseMaterialReactor materialReactor = null;

            foreach (var reactor in reactors)
            {
                if (reactor is BaseMaterialReactor mr)
                {
                    materialReactor = mr;
                    continue;
                }
                else if (reactor is GroupPropertyReactor)
                {
                    continue;
                }

                initializer = InitializerFactory.GetInitializer(reactor, preset);
                initializer.Initialize(_spaceDescriptor, objectId, reactor.gameObject);
            }

            if (materialReactor == null)
            {
                return;
            }

            initializer = InitializerFactory.GetInitializer(materialReactor, preset);
            initializer.Initialize(_spaceDescriptor, objectId, materialReactor.gameObject);
        }

        private bool NeedsInitialization(Guid objectId)
        {
            var preset = _spaceDescriptor.ObjectPresets[objectId];

            return !(preset is StagePreset);
        }

        private bool NeedsSpawning(Guid objectId)
        {
            var preset = _spaceDescriptor.ObjectPresets[objectId];
            return !(preset is EnvironmentSettingsPreset);
        }
    }
}
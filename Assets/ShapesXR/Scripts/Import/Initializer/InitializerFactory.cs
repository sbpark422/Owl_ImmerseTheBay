using System.Collections.Generic;
using System.Linq;
using ShapesXr.Import.Core;
using UnityEngine;

namespace ShapesXr
{
    public static class InitializerFactory
    {
        // These should work in unity 2020
        // ReSharper disable ArrangeObjectCreationWhenTypeEvident
        private static readonly SizeInitializer SizeInitializer = new SizeInitializer();
        private static readonly TextInitializer TextInitializer = new TextInitializer();
        private static readonly ImageInitializer ImageInitializer = new ImageInitializer();
        private static readonly ModelInitializer ModelInitializer = new ModelInitializer();
        private static readonly GroupInitializer GroupInitializer = new GroupInitializer();
        // ReSharper restore ArrangeObjectCreationWhenTypeEvident
        
        public static IInitializer GetInitializer(PropertyReactorComponent reactor, BasePreset preset)
        {
            switch (reactor)
            {
                case CharacterReactorComponent _:
                    return GetCharacterInitializer(preset);
                case SizePropertyReactor _:
                    return SizeInitializer;
                case TextReactorComponent _:
                    return TextInitializer;
                case StrokePropertyReactor _:
                    return GetStrokeInitializer(preset);
                case ImageReactor _:
                    return ImageInitializer;
                case ModelReactor _:
                    return ModelInitializer;
                case BaseMaterialReactor r:
                    return GetMaterialInitializer(r);
                case GroupPropertyReactor _:
                    return GroupInitializer;
                default:
                    return new NullInitializer(reactor.ToString());
            }
        }

        private static IInitializer GetCharacterInitializer(BasePreset preset)
        {
            if (preset is CharacterPreset characterPreset)
            {
                return new CharacterInitializer(characterPreset.Pose);
            }

            return new NullInitializer(preset.name);
        }

        private static IInitializer GetStrokeInitializer(BasePreset preset)
        {
            if (preset is BaseBrushPreset p)
            {
                return new StrokeInitializer(p.GetParameters());
            }

            return new NullInitializer(preset.ToString());
        }

        private static IInitializer GetMaterialInitializer(BaseMaterialReactor reactor)
        {
            var renderers = new List<Renderer>();
            
            var materialAssigner = reactor.GetComponent<MaterialAssigner>();

            if (materialAssigner)
            {

                foreach (var group in materialAssigner.Groups)
                {
                    foreach (var renderer in group.Renderers)
                    {
                        if(renderer is MeshRenderer meshRenderer)
                        {
                            renderers.Add(meshRenderer);
                        }
                    }
                }
            }
            
            if (renderers.Count == 0)
                renderers = reactor.GetComponentsInChildren<Renderer>(true).ToList();

            switch (reactor)
            {
                case MaterialReactor _:
                    return new MaterialInitializer(renderers);
                case ImageMaterialReactor _:
                    return new ImageMaterialInitializer(renderers);
                case ModelMaterialReactor _:
                    return new ModelMaterialInitializer(renderers);
                default:
                    return new NullInitializer(reactor.ToString());
            }
        }
    }
}
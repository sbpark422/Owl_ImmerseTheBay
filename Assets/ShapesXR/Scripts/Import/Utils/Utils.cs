using MessagePack;
using MessagePack.Resolvers;
using MessagePack.Unity;
using MessagePack.Unity.Extension;

namespace ShapesXr
{
    public static class Utils
    {
        #if SHAPES_XR_DEV
        public const bool IsDev = true;
        #else
        public const bool IsDev = false;
        #endif
        
        public static void InitializeMessagePack()
        {
            StaticCompositeResolver.Instance.Register(
                ShapesXR.Resolvers.GeneratedResolver.Instance,
                UnityBlitResolver.Instance,
                UnityResolver.Instance,
                NativeGuidResolver.Instance,
                StandardResolver.Instance
            );

            MessagePackSerializer.DefaultOptions = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
        }
    }
}
using Brio.Capabilities.Actor;
using Brio.Capabilities.Camera;
using Brio.Capabilities.Core;
using Brio.Capabilities.Posing;
using Brio.Capabilities.World;
using Brio.Entities.Core;

namespace Brio.Xtension.helpers;

public static class XtensionHelpers {
    public static void DbgExploreCapabilities(Entity ent) {
        Brio.Log.Info($"## Entity capabilities for {ent.FriendlyName}");

        HasCapability<ActionTimelineCapability>(ent);
        HasCapability<ActorAppearanceCapability>(ent);
        HasCapability<ActorCapability>(ent);
        HasCapability<ActorContainerCapability>(ent);
        HasCapability<ActorDebugCapability>(ent);
        HasCapability<ActorDynamicPoseCapability>(ent);
        HasCapability<ActorLifetimeCapability>(ent);
        HasCapability<CompanionCapability>(ent);
        HasCapability<StatusEffectCapability>(ent);

        HasCapability<BrioCameraCapability>(ent);
        HasCapability<CameraCapability>(ent);
        HasCapability<CameraContainerCapability>(ent);
        HasCapability<CameraLifetimeCapability>(ent);

        HasCapability<ModelPosingCapability>(ent);
        HasCapability<PosingCapability>(ent);
        HasCapability<SkeletonPosingCapability>(ent);

        HasCapability<FestivalCapability>(ent);
        HasCapability<LightCapability>(ent);
        HasCapability<LightContainerCapability>(ent);
        HasCapability<LightDebugCapability>(ent);
        HasCapability<LightLifetimeCapability>(ent);
        HasCapability<LightRenderingCapability>(ent);
        HasCapability<LightTransformCapability>(ent);
        HasCapability<TimeWeatherCapability>(ent);
        HasCapability<WorldRenderingCapability>(ent);

        static void HasCapability<T>(Entity en) where T : Capability {
            if(en.HasCapability<T>()){
                Brio.Log.Info($"Has: {typeof(T)}");
            }
        }
    }
}

public static class XTCap {
    public static T? EntityLastValid<T>(Entity? ent, ref T? store) where T : Capability {
        if(ent == null) return store;
        if(ent.HasCapability<T>()) {
            store = ent.GetCapability<T>();
        }
        return store ;
    }
}

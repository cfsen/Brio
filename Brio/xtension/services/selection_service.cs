using Brio.Capabilities.Actor;
using Brio.Capabilities.Camera;
using Brio.Capabilities.Core;
using Brio.Capabilities.Posing;
using Brio.Entities.Core;
using Brio.Xtension.helpers;

namespace Brio.Xtension.services;

public class XTSelectionService {
    public PosingCapability? LastPosing;
    public CameraCapability? LastCamera;
    public ActorAppearanceCapability? LastAppearance;

    public PosingCapability? UpdateLastPosing(Entity? ent)
        => XTCap.EntityLastValid<PosingCapability>(ent, ref LastPosing);

    public ActorAppearanceCapability? UpdateLastAppearance(Entity? ent)
        => XTCap.EntityLastValid<ActorAppearanceCapability>(ent, ref LastAppearance);

    public CameraCapability? UpdateLastCamera(Entity? ent)
        => XTCap.EntityLastValid<CameraCapability>(ent, ref LastCamera);
}

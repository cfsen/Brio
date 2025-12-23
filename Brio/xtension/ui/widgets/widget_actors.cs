using System;
using Brio.Capabilities.Actor;
using Brio.Entities;
using Brio.Entities.Actor;
using Brio.Entities.Core;
using Brio.UI.Entitites;
using Brio.Xtension.helpers;
using Dalamud.Interface;

namespace Brio.Xtension.ui.widgets;

public class XTWidgetActors(
        Entity _actorContainer,
        EntityHierarchyView entView,
        EntityManager entMan
        ) : XTWidget {

    private readonly Entity ActorContainer = _actorContainer;

    public override XTWidgetCategory WidgetCategory => XTWidgetCategory.Posing;

    public override void Draw(){
        if(ActorContainer.IsLoading) return;

        DrawHeader();
        entView.Draw(ActorContainer);

    }

    private void DrawHeader(){
        XUI.WidgetHeader("Actors", FontAwesomeIcon.PersonRays);

        Action pop_enable = XUI.EnableIf(ActorContainer.HasCapability<ActorContainerCapability>());

        XUI.Button(SpawnActor, FontAwesomeIcon.Plus, "SpawnActor", XUI.Size.BtnMid, XUI.Inline.Yes);
        XUI.Button(CloneActor, FontAwesomeIcon.Copy, "CloneActor", XUI.Size.BtnMid, XUI.Inline.Yes);
        XUI.Button(SpawnProp, FontAwesomeIcon.PlusCircle, "SpawnProp", XUI.Size.BtnMid, XUI.Inline.Yes);
        XUI.Button(TargetActor, FontAwesomeIcon.Bullseye, "TargetActor", XUI.Size.BtnMid, XUI.Inline.Yes);
        XUI.Button(DestroyActor, FontAwesomeIcon.Minus, "DestroyActor", XUI.Size.BtnMid);

        pop_enable();
    }

    private void SpawnActor()
        => ActorContainer.GetCapability<ActorContainerCapability>()?.CreateCharacter(false, true, true);

    private void SpawnProp()
        => ActorContainer.GetCapability<ActorContainerCapability>()?.CreateProp(true);

    private void CloneActor(){
        if(entMan.SelectedEntity is not ActorEntity actor) return;
        ActorContainer.GetCapability<ActorContainerCapability>()?.CloneActor(actor, false);
    }

    private void DestroyActor(){
        if(entMan.SelectedEntity is not ActorEntity actor) return;
        ActorContainer.GetCapability<ActorContainerCapability>()?.DestroyCharacter(actor);
    }

    private void TargetActor(){
        if(entMan.SelectedEntity is not ActorEntity actor) return;
        ActorContainer.GetCapability<ActorContainerCapability>()?.Target(actor);
    }

}

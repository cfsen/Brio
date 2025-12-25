using Brio.Capabilities.Actor;
using Brio.Entities;
using Brio.Entities.Core;
using Brio.UI.Controls.Editors;
using Brio.Xtension.helpers;
using Brio.Xtension.services;
using Dalamud.Interface;

namespace Brio.Xtension.ui.widgets;

public class XTWidgetIPC(
        EntityManager entMan,
        XTSelectionService selectionService
        ): XTWidget {

    public override XTWidgetCategory WidgetCategory => XTWidgetCategory.Actors;

    public override void Draw(){
        DrawHeader();
        DrawAppearanceController();
    }

    private static readonly string MSG_NO_ACTOR = "No actor selected.";
    private void DrawHeader(){
        XUI.WidgetHeader("IPC", FontAwesomeIcon.NetworkWired);
        XUI.Text(selectionService.LastPosing?.Entity.FriendlyName ?? MSG_NO_ACTOR);
    }

    private void DrawAppearanceController(){
        ActorAppearanceCapability? appcap = AppearanceLastValid(entMan.SelectedEntity);
        if(appcap == null) return;

        AppearanceEditorCommon.DrawPenumbraCollectionSwitcher(appcap);
        AppearanceEditorCommon.DrawGlamourerDesignSwitcher(appcap);
    }

    private ActorAppearanceCapability? AppearanceLastValid(Entity? ent) 
        => selectionService.UpdateLastAppearance(ent);
}

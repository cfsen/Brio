using Brio.Entities.Core;
using Brio.UI.Entitites;

namespace Brio.Xtension.ui.widgets;

public class XTWidgetActors(
        Entity _actorContainer,
        EntityHierarchyView entView
        ) : XTWidget {

    private readonly Entity ActorContainer = _actorContainer;

    public override XTWidgetCategory WidgetCategory => XTWidgetCategory.Posing;

    public override void Draw(){
        ActorContainer.DrawContextButton();
        entView.Draw(ActorContainer);
    }
}

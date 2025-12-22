using Brio.Entities.Core;
using Brio.UI.Entitites;

namespace Brio.Xtension.ui.widgets;

public class XTWidgetEnv(
        Entity _envContainer,
        EntityHierarchyView entView
        ): XTWidget {

    private readonly Entity EnvContainer = _envContainer;

    public override XTWidgetCategory WidgetCategory => XTWidgetCategory.Lighting;

    public override void Draw(){
        EnvContainer.DrawContextButton();
        entView.Draw(EnvContainer);
    }
}

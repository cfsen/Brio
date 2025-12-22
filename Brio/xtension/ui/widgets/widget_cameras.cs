using Brio.Entities.Core;
using Brio.UI.Entitites;

namespace Brio.Xtension.ui.widgets;

public class XTWidgetCameras(
        Entity _cameraContainer,
        EntityHierarchyView entView
        ) : XTWidget {

    private readonly Entity CameraContainer = _cameraContainer;

    public override XTWidgetCategory WidgetCategory => XTWidgetCategory.Camera;

    public override void Draw(){
        CameraContainer.DrawContextButton();
        entView.Draw(CameraContainer);
    }
}

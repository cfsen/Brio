using System;
using System.Collections.Generic;
using System.Numerics;
using Brio.UI.Controls.Core;
using Brio.UI.Controls.Stateless;
using Brio.UI.Entitites;
using Brio.Xtension.ui.widgets;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;

namespace Brio.Xtension.ui;

public class XTWidgetSelector {
    private readonly RootEntityContainers rec;
    private readonly EntityHierarchyView entView;
    private readonly List<XTWidget> widgets;

    private XTWidgetCategory ActiveWidgets;

    public XTWidgetSelector(
            RootEntityContainers _rec,
            EntityHierarchyView _entView
            ) {
        rec = _rec;
        entView = _entView;

        widgets = InitializeWidgets();
        ActiveWidgets = XTWidgetCategory.Posing;
    }

    private List<XTWidget> InitializeWidgets(){
        List<XTWidget> _widgets = [];

        XTWidgetActors actors = new(rec.Actors!, entView);
        XTWidgetCameras cameras = new(rec.Cameras!, entView);
        XTWidgetEnv env = new(rec.Environment!, entView);

        _widgets.Add(actors);
        _widgets.Add(cameras);
        _widgets.Add(env);

        return _widgets;
    }

    public void DrawWidgets() {
        try {
            foreach(XTWidget widget in widgets){
                if (ActiveWidgets.HasFlag(widget.WidgetCategory)) {
                    widget.Draw();
                }
            }
        }
        catch (InvalidOperationException) {
            // Collection mutated during draw
            Brio.Log.Warning("XT: InvalidOp: Widget iteration, collection mutated.");
        }
    }

    private delegate void WidgetToggleDelegate(XTWidgetCategory category);
    private void WidgetToggle(XTWidgetCategory category){
        ActiveWidgets ^= category;
    }
    public void DrawWidgetSelector(){
        SelectorBtn(XTWidgetCategory.Selection, FontAwesomeIcon.Crosshairs, WidgetToggle);
        SelectorBtn(XTWidgetCategory.Expression, FontAwesomeIcon.Portrait, WidgetToggle);
        SelectorBtn(XTWidgetCategory.Posing, FontAwesomeIcon.PersonRays, WidgetToggle);
        SelectorBtn(XTWidgetCategory.Scene, FontAwesomeIcon.PeopleLine, WidgetToggle);
        SelectorBtn(XTWidgetCategory.Camera, FontAwesomeIcon.Camera, WidgetToggle);
        SelectorBtn(XTWidgetCategory.Lighting, FontAwesomeIcon.Sun, WidgetToggle);
        SelectorBtn(XTWidgetCategory.Config, FontAwesomeIcon.Cog, WidgetToggle, Inline.No);
    }

    private static readonly Vector2 _widgetBtnSize = new(40 * ImGuiHelpers.GlobalScale, 40 * ImGuiHelpers.GlobalScale);
    private static readonly uint _activeButton = 0xFF47028B;
    private enum Inline { Yes, No };
    private void SelectorBtn(
            XTWidgetCategory widget,
            FontAwesomeIcon icon,
            WidgetToggleDelegate closure,
            Inline inline = Inline.Yes
            ) {

        bool should_pop = false;
        if(ActiveWidgets.HasFlag(widget)) {
            ImGui.PushStyleColor(ImGuiCol.Button, _activeButton);
            should_pop = true;
        }

        if(ImBrio.FontIconButton(icon, _widgetBtnSize)){
            closure(widget);
        }

        if(should_pop){
            ImGui.PopStyleColor();
        }

        if(inline != Inline.No) {
            ImGui.SameLine();
        }
    }
}

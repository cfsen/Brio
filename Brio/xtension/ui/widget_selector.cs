using System;
using System.Collections.Generic;
using System.Numerics;
using Brio.Entities;
using Brio.Game.Posing;
using Brio.UI.Controls.Stateless;
using Brio.UI.Entitites;
using Brio.Xtension.ui.widgets;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;

namespace Brio.Xtension.ui;

public class XTWidgetSelector {
    private readonly RootEntityContainers rec;
    private readonly EntityManager entMan;
    private readonly PosingService posingService;
    private readonly EntityHierarchyView entView;

    private readonly List<XTWidget> widgets;

    private XTWidgetCategory ActiveWidgets;

    public XTWidgetSelector(
            RootEntityContainers _rec,
            EntityHierarchyView _entView,
            EntityManager _entMan,
            PosingService _posingService
            ) {
        rec = _rec;
        entView = _entView;
        entMan = _entMan;
        posingService = _posingService;

        widgets = InitializeWidgets();
        ActiveWidgets = XTWidgetCategory.Posing;
    }

    private List<XTWidget> InitializeWidgets() => [
        new XTWidgetActors(rec.Actors!, entView, entMan, posingService),
        new XTWidgetCameras(rec.Cameras!, entView, entMan),
        new XTWidgetEnv(rec.Environment!, entView),
    ];

    public void DrawWidgets() {
        try {
            foreach(XTWidget widget in widgets){
                if (ActiveWidgets.HasFlag(widget.WidgetCategory)) {
                    widget.Draw();
                }
            }
        }
        catch (InvalidOperationException e) {
            Brio.Log.Warning($"XT: DrawWidgets: InvalidOp: {e}"); // expected on entities being destroyed
        }
        catch (Exception e) {
            Brio.Log.Error($"XT: DrawWidgets: Unhandled: {e}");
        }
    }

    public void DrawWidgetSelector(){
        SelectorBtn(XTWidgetCategory.Selection, FontAwesomeIcon.Crosshairs);
        SelectorBtn(XTWidgetCategory.Expression, FontAwesomeIcon.Portrait);
        SelectorBtn(XTWidgetCategory.Posing, FontAwesomeIcon.PersonRays);
        SelectorBtn(XTWidgetCategory.Scene, FontAwesomeIcon.PeopleLine);
        SelectorBtn(XTWidgetCategory.Camera, FontAwesomeIcon.Camera);
        SelectorBtn(XTWidgetCategory.Lighting, FontAwesomeIcon.Sun);
        SelectorBtn(XTWidgetCategory.Config, FontAwesomeIcon.Cog, Inline.No);
    }

    private void WidgetToggle(XTWidgetCategory category) => ActiveWidgets ^= category;

    private static readonly Vector2 _widgetBtnSize = new(40 * ImGuiHelpers.GlobalScale, 40 * ImGuiHelpers.GlobalScale);
    private static readonly uint _btnColActive = 0xFF47028B;
    private enum Inline { Yes, No };

    private void SelectorBtn(
            XTWidgetCategory widget,
            FontAwesomeIcon icon,
            Inline inline = Inline.Yes,
            Action<XTWidgetCategory>? closure = null
            ){

        Action pop_style = PushToggleBtnStyle(ActiveWidgets.HasFlag(widget), _btnColActive, inline);

        using (ImRaii.PushFont(UiBuilder.IconFont)){
            if(ImGui.Button($"{icon.ToIconString()}###xtui_btn_main_widget_toggle{widget}", _widgetBtnSize)) {
                closure ??= WidgetToggle;
                closure(widget);
            }
        }

        pop_style();

        static Action PushToggleBtnStyle(bool condition, uint col, Inline inline){
            if(!condition)
                return inline == Inline.Yes ? static () => { ImGui.SameLine(); } : static () => { };

            ImGui.PushStyleColor(ImGuiCol.Button, col);

            return inline == Inline.Yes ? static () => {
                ImGui.PopStyleColor();
                ImGui.SameLine();
            } : static () => { ImGui.PopStyleColor(); };
        }
    }
}

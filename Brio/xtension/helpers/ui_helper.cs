using System;
using System.Numerics;
using Brio.UI.Controls.Stateless;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;

namespace Brio.Xtension.helpers;

public static class XUI {
    public static void Button(
            Action closure,
            FontAwesomeIcon icon,
            Vector2? size = null,
            Inline inline = Inline.No,
            bool enable = true
            ) {

        Action pop_enable = EnableIf(enable); 
        if(ImBrio.FontIconButton(icon, size ?? Size.BtnMid)){
            closure();
        }
        pop_enable();
        PushInline(inline);
    }

    public static void WidgetHeader(String header, FontAwesomeIcon icon) {
		ImGui.Dummy(new(0, Size.BtnSmall.Y/5));
		ImGui.Spacing();
		using(ImRaii.PushFont(UiBuilder.IconFont)) {
			ImGui.Text(icon.ToIconString());
		}
		ImGui.SameLine();
		ImGui.Text(header);
        ImGui.Separator();
    }

    public static void Text(String text, Inline inline = Inline.No) {
        ImGui.Text(text);
        PushInline(inline);
    }

    public static Action PushStyleColorIf(bool condition, ImGuiCol entity, uint color) {
        if(!condition) return static () => { };

        ImGui.PushStyleColor(entity, color);

        return static () => { ImGui.PopStyleColor(); };
    }

    /// <summary>
    /// IMPORTANT: You MUST call the returned Action to cleanup ImGui state
    /// </summary>
    public static Action EnableIf(bool condition) {
        if(condition) return static () => {};

        ImGui.BeginDisabled();

        return static () => { ImGui.EndDisabled(); };
    }

    public enum Inline { Yes, No, }
    private static void PushInline(Inline inline) { if(inline == Inline.Yes) ImGui.SameLine(); }

    public static class Size {
        public static readonly Vector2 BtnBig = new(40 * ImGuiHelpers.GlobalScale, 40 * ImGuiHelpers.GlobalScale);
        public static readonly Vector2 BtnMid = new(30 * ImGuiHelpers.GlobalScale, 30 * ImGuiHelpers.GlobalScale);
        public static readonly Vector2 BtnSmall = new(24 * ImGuiHelpers.GlobalScale, 24 * ImGuiHelpers.GlobalScale);
    }

    public static class Color {
        public static readonly uint ActiveLoud = 0xFF47028B;
    }
}

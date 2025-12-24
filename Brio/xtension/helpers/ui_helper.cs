using System;
using System.Numerics;
using Brio.UI.Controls.Stateless;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;

namespace Brio.Xtension.helpers;

public static class XUI {
    //
    // Composites
    //

    public static void WidgetHeader(String header, FontAwesomeIcon icon) {
        ImGui.Dummy(new(0, Size.BtnSmall.Y/5));
        ImGui.Spacing();
        IconText(icon);
        ImGui.Text(header);
        ImGui.Separator();
    }

    public static void ControllerVector3(ref Vector3 vec, String id, Inline inline = Inline.Yes) {
        DragFloatVector3AxisColor(ref vec.X, Axis.X, id);
        DragFloatVector3AxisColor(ref vec.Y, Axis.Y, id);
        DragFloatVector3AxisColor(ref vec.Z, Axis.Z, id, inline);
    }

    public static void ControllerCameraAngle(ref Vector3 vec, String id, Inline inline = Inline.Yes) {
        IconText(FontAwesomeIcon.ArrowsLeftRight);
        DragFloatAxis(ref vec.X, Axis.X, id);
        IconText(FontAwesomeIcon.ArrowsUpDown);
        DragFloatAxis(ref vec.Y, Axis.Y, id, inline);
    }

    //
    // Wrappers
    //

    public static void Button(
            Action closure,
            FontAwesomeIcon icon,
            String id,
            Vector2? size = null,
            Inline inline = Inline.No,
            bool enable = true
            ) {

        Action pop_enable = EnableIf(enable); 
        using (ImRaii.PushFont(UiBuilder.IconFont)){
            if(ImGui.Button($"{icon.ToIconString()}###xtui_btn_{id}", size ?? Size.BtnMid)) {
                closure();
            }
        }
        pop_enable();
        PushInline(inline);
    }

    public static void BtnToggle(
            Action closure,
            bool condition,
            FontAwesomeIcon icon,
            String id,
            Vector2? size = null,
            Inline inline = Inline.Yes
            ){

        Action pop_style = PushStyleColorIf(condition, ImGuiCol.Button, Color.ActiveLoud); 
        using (ImRaii.PushFont(UiBuilder.IconFont)){
            if(ImGui.Button($"{icon.ToIconString()}###xtui_btntgl_{id}", size ?? Size.BtnMid)) {
                closure();
            }
        }
        pop_style();
        PushInline(inline);

    }

    //
    // DragFloat
    //

    public static void DragFloat(ref float val, string id, Inline inline = Inline.Yes){
        ImRaii.ItemWidth(Size.DragFloatWidth);
        ImGui.DragFloat($"###xtui_dragfloat_{id}", ref val, Size.DragFloatSpeed);
        if(inline == Inline.Yes) ImGui.SameLine();
    }

    public static void DragFloatAxis(ref float val, Axis axis, string id, Inline inline = Inline.Yes){
        ImRaii.ItemWidth(Size.DragFloatWidth);
        ImGui.DragFloat($"###xtui_dragfloat_{id}_{AxisId(axis)}", ref val, Size.DragFloatSpeed);
        if(inline == Inline.Yes) ImGui.SameLine();
    }

    public static void DragFloatVector3AxisColor(ref float val, Axis axis, string id, Inline inline = Inline.Yes){
        ImRaii.ItemWidth(Size.DragFloatWidth);
        using(ImRaii.PushColor(ImGuiCol.FrameBg, AxisColor(axis))){
            ImGui.DragFloat($"###xtui_dragfloat_{id}_{AxisId(axis)}", ref val, Size.DragFloatSpeed);
        }
        if(inline == Inline.Yes) ImGui.SameLine();
    }

    public static void DragFloatAngle(ref float val, string id, Inline inline = Inline.Yes){
        RadToDeg(ref val);
        Invert(ref val);
        FloatSanitize(ref val);

        ImRaii.ItemWidth(Size.DragFloatWidth);
        ImGui.DragFloat($"###xtui_dragfloat_{id}", ref val, Size.DragFloatSpeedDegrees);
        if(inline == Inline.Yes) ImGui.SameLine();

        Invert(ref val);
        DegToRad(ref val);
        RadModulo(ref val);
    }

    //
    // text & icons
    //

    public static void Text(String text, Inline inline = Inline.No) {
        ImGui.Text(text);
        PushInline(inline);
    }

    public static void IconText(FontAwesomeIcon icon, Inline inline = Inline.Yes) {
        using(ImRaii.PushFont(UiBuilder.IconFont)) {
            ImGui.Text(icon.ToIconString());
        }
        if(inline == Inline.Yes) ImGui.SameLine();
    }



    /// <summary>
    /// IMPORTANT: You must call the returned Action to cleanup ImGui state
    /// </summary>
    public static Action PushStyleColorIf(bool condition, ImGuiCol entity, uint color) {
        if(!condition) return static () => { };

        ImGui.PushStyleColor(entity, color);

        return static () => { ImGui.PopStyleColor(); };
    }

    /// <summary>
    /// IMPORTANT: You must call the returned Action to cleanup ImGui state
    /// </summary>
    public static Action EnableIf(bool condition) {
        if(condition) return static () => {};

        ImGui.BeginDisabled();

        return static () => { ImGui.EndDisabled(); };
    }

    //
    // Helpers
    //

    private static void DegToRad(ref float deg) => deg *= MathF.PI/180.0f;
    private static void RadToDeg(ref float rad) => rad *= 180.0f/MathF.PI; 
    private static void Invert(ref float val) => val *= -1.0f; 
    private static void FloatSanitize(ref float val) => val = val == -0.0f ? 0.0f : val;
    private static void RadModulo(ref float rad) => rad %= MathF.Tau;

    private static void PushInline(Inline inline) { if(inline == Inline.Yes) ImGui.SameLine(); }

    private static uint AxisColor(Axis axis) => axis switch {
        Axis.X => Color.X,
        Axis.Y => Color.Y,
        Axis.Z => Color.Z,
        _ => Color.Error,
    };
    private static String AxisId(Axis axis) => axis switch {
        Axis.X => "AxisX",
        Axis.Y => "AxisY",
        Axis.Z => "AxisZ",
        _ => "Invariant",
    };

    //
    // Sizes, colors, enums
    //

    public enum Axis { X, Y, Z }
    public enum Inline { Yes, No, }

    public static class Size {
        public static readonly Vector2 BtnBig = new(40 * ImGuiHelpers.GlobalScale, 40 * ImGuiHelpers.GlobalScale);
        public static readonly Vector2 BtnMid = new(30 * ImGuiHelpers.GlobalScale, 30 * ImGuiHelpers.GlobalScale);
        public static readonly Vector2 BtnSmall = new(24 * ImGuiHelpers.GlobalScale, 24 * ImGuiHelpers.GlobalScale);

        public static readonly float DragFloatWidth = 75.0f;
        public static readonly float DragFloatSpeed = 0.001f;
        public static readonly float DragFloatSpeedDegrees = 0.1f;
    }

    public static class Color {
        public static readonly uint ActiveLoud = 0xFF47028B;

        public static readonly uint X = 0xFF000044;
        public static readonly uint Y = 0xFF004400;
        public static readonly uint Z = 0xFF440000;

        public static readonly uint Error = 0xFF0000FF;
    }
}

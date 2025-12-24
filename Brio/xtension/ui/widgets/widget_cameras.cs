using System;
using System.Numerics;
using Brio.Capabilities.Camera;
using Brio.Entities;
using Brio.Entities.Camera;
using Brio.Entities.Core;
using Brio.UI.Entitites;
using Brio.Xtension.helpers;
using Dalamud.Interface;

namespace Brio.Xtension.ui.widgets;

public class XTWidgetCameras(
        Entity _cameraContainer,
        EntityHierarchyView entView,
        EntityManager entMan
        ) : XTWidget {

    private readonly Entity CameraContainer = _cameraContainer;

    public override XTWidgetCategory WidgetCategory => XTWidgetCategory.Camera;

    public override void Draw(){
        if(CameraContainer.IsLoading) return;

        DrawHeader();
        DrawControls();
        entView.Draw(CameraContainer);
    }

    //
    // camera controls
    //

    private void DrawControls(){
        BrioCameraCapability? camcap = CameraLastValid(entMan.SelectedEntity);
        CameraControllerData camdat = GetCameraControllerData(camcap);

        Action pop_enable = XUI.EnableIf(!camdat.IsMock);

        XUI.ControllerVector3(ref camdat.PositionOffset, "CamPositionOffset");

        XUI.IconText(FontAwesomeIcon.Recycle);
        XUI.DragFloat(ref camdat.Pivot, "CamPivot", XUI.Inline.No);

        Action pop_disable_position = XUI.EnableIf(camdat.CameraType == CameraType.Free);
        XUI.ControllerVector3(ref camdat.Position, "CamPosition");
        pop_disable_position();

        XUI.ControllerCameraAngle(ref camdat.Rotation, "CamRotation", XUI.Inline.No);

        XUI.IconText(FontAwesomeIcon.Glasses);
        XUI.DragFloatAngle(ref camdat.FoV, "CamFoV");

        XUI.IconText(FontAwesomeIcon.MagnifyingGlassChart);
        XUI.DragFloatAngle(ref camdat.Zoom, "CamZoom");

        Action pop_disable_movement = XUI.EnableIf(camdat.CameraType == CameraType.Free);
        XUI.BtnToggle(_toggleCamMove, camdat.CanMove,
                FontAwesomeIcon.Car, "CamMove", XUI.Size.BtnSmall);
        XUI.BtnToggle(_toggleCamLatLock, camdat.LateralLock,
                FontAwesomeIcon.SolarPanel, "CamLatLock", XUI.Size.BtnSmall, XUI.Inline.No);
        pop_disable_movement();

        pop_enable();

        if(!camdat.IsMock) SetCameraControllerData(camcap, camdat);

        void _toggleCamMove(){
            camdat.CanMove = !camdat.CanMove;
        }
        void _toggleCamLatLock() {
            camdat.LateralLock = !camdat.LateralLock;
        }
    }

    private struct CameraControllerData {
        public bool IsMock;
        public CameraType CameraType;
        public Vector3 Position;
        public Vector3 PositionOffset;
        public Vector3 Rotation;
        public float Pivot;
        public Vector3 Angle;
        public float Zoom;
        public float FoV;
        public bool CanMove;
        public bool LateralLock;
    }

    private BrioCameraCapability? _lastValidCam = null;
    private BrioCameraCapability? CameraLastValid(Entity? ent) => 
        XTCap.EntityLastValid<BrioCameraCapability>(ent, ref _lastValidCam);

    private CameraControllerData GetCameraControllerData(BrioCameraCapability? ent){
        if(ent is not BrioCameraCapability cap) return mock();

        return new CameraControllerData {
            IsMock = false,
            CameraType = cap.CameraEntity.CameraType,

            Position = cap.VirtualCamera.Position,
            Rotation = cap.CameraEntity.CameraType == CameraType.Free 
                ? cap.VirtualCamera.Rotation
                : new(cap.VirtualCamera.Angle.X, cap.VirtualCamera.Angle.Y, 0.0f),
            PositionOffset = cap.VirtualCamera.PositionOffset,
            Pivot = cap.VirtualCamera.PivotRotation,
            Angle = new(cap.VirtualCamera.Angle.X, cap.VirtualCamera.Angle.Y, 0.0f),
            Zoom = cap.VirtualCamera.Zoom,
            FoV = cap.VirtualCamera.FoV,
            CanMove = cap.VirtualCamera.FreeCamValues.IsMovementEnabled,
            LateralLock = cap.VirtualCamera.FreeCamValues.Move2D,
        };

        static CameraControllerData mock(){
            return new CameraControllerData{
                IsMock = true,

                Position = Vector3.Zero,
                Rotation = Vector3.Zero,
                PositionOffset = Vector3.Zero,
                Pivot = 0.0f,
                Angle = Vector3.Zero,
                Zoom = 0.0f,
                FoV = 0.0f,
                CanMove = false,
                LateralLock = false,
            };
        }
    }

    private void SetCameraControllerData(BrioCameraCapability? ent, CameraControllerData camdat){
        if(ent is not BrioCameraCapability cap) return;

        cap.VirtualCamera.Position = camdat.Position;
        cap.VirtualCamera.Rotation = camdat.Rotation;
        cap.VirtualCamera.PositionOffset = camdat.PositionOffset;
        cap.VirtualCamera.PivotRotation = camdat.Pivot;
        cap.VirtualCamera.Angle = camdat.CameraType == CameraType.Free
            ? new(camdat.Angle.X, camdat.Angle.Y)
            : new(camdat.Rotation.X, camdat.Rotation.Y);
        cap.VirtualCamera.Zoom = camdat.Zoom;
        cap.VirtualCamera.FoV = camdat.FoV;
        cap.VirtualCamera.FreeCamValues.IsMovementEnabled = camdat.CanMove;
        cap.VirtualCamera.FreeCamValues.Move2D = camdat.LateralLock;
    }

    //
    // header and buttons
    //

    private void DrawHeader(){
        XUI.WidgetHeader("Cameras", FontAwesomeIcon.Camera);

        Action pop_enable = XUI.EnableIf(CameraContainer.HasCapability<CameraContainerCapability>());

        XUI.Button(SpawnCamera, FontAwesomeIcon.Plus, "SpawnCamera", XUI.Size.BtnMid, XUI.Inline.Yes);
        XUI.Button(SpawnFixedCamera, FontAwesomeIcon.PlusCircle, "SpawnedFixedCamera", XUI.Size.BtnMid, XUI.Inline.Yes);
        XUI.Button(DestroyCamera, FontAwesomeIcon.Minus, "DestroyCamera", XUI.Size.BtnMid);

        pop_enable();
    }

    private void SpawnCamera() => CameraContainer.GetCapability<CameraContainerCapability>()?
        .VirtualCameraManager.CreateCamera(CameraType.Game);

    private void SpawnFixedCamera() => CameraContainer.GetCapability<CameraContainerCapability>()?
        .VirtualCameraManager.CreateCamera(CameraType.Free);

    private void DestroyCamera() {
        if(entMan.SelectedEntity is not CameraEntity camera) return;
        CameraContainer.GetCapability<CameraContainerCapability>()?
        .VirtualCameraManager.DestroyCamera(camera.CameraID);
    }
}

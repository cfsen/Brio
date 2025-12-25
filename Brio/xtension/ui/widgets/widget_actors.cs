using System;
using Brio.Capabilities.Actor;
using Brio.Capabilities.Core;
using Brio.Capabilities.Posing;
using Brio.Core;
using Brio.Entities;
using Brio.Entities.Actor;
using Brio.Entities.Core;
using Brio.Files;
using Brio.Game.Posing;
using Brio.Library;
using Brio.Library.Filters;
using Brio.UI.Controls.Stateless;
using Brio.UI.Entitites;
using Brio.Xtension.helpers;
using Brio.Xtension.services;
using Dalamud.Interface;

namespace Brio.Xtension.ui.widgets;

public class XTWidgetActors(
        Entity _actorContainer,
        EntityHierarchyView entView,
        EntityManager entMan,
        PosingService posingService,
        XTSelectionService selectionService
        ) : XTWidget {

    private readonly Entity ActorContainer = _actorContainer;
    public override XTWidgetCategory WidgetCategory => XTWidgetCategory.Actors;

    // checkboxes
    private bool _chkLoadPosition = false;
    private bool _chkLoadRotation = true;
    private bool _chkLoadScale = false;

    private bool _chkLoadExpression = true;
    private bool _chkLoadBody = true;

    private bool _chkfreezeOnLoad = false;
    private bool _chkxfmModelOverride = false;
    private bool _chkxfmModel = false;
    //
    // draw
    //

    public override void Draw(){
        if(ActorContainer.IsLoading) return;

        DrawHeader();
        DrawActorController();
        entView.Draw(ActorContainer);

    }

    private void DrawHeader(){
        XUI.WidgetHeader("Actors", FontAwesomeIcon.PersonRays);

        Action pop_enable = XUI.EnableIf(ActorContainer.HasCapability<ActorContainerCapability>());

        XUI.Button(SpawnActor, FontAwesomeIcon.Plus, "SpawnActor");
        XUI.Button(CloneActor, FontAwesomeIcon.Copy, "CloneActor");
        XUI.Button(SpawnProp, FontAwesomeIcon.PlusCircle, "SpawnProp");
        XUI.Button(TargetActor, FontAwesomeIcon.Bullseye, "TargetActor");
        XUI.Button(DestroyActor, FontAwesomeIcon.Minus, "DestroyActor", XUI.Size.BtnMid, XUI.Inline.No);

        pop_enable();
    }

    private void DrawActorController(){
        PosingCapability? posecap = ActorLastValid(entMan.SelectedEntity);
        PoseControllerData posedat = GetPoseControllerData(posecap);

        Action pop_actor_selected = XUI.EnableIf(posecap != null);

        XUI.Text(LastActorName(selectionService.LastPosing));
        XUI.Checkbox(ref _chkLoadPosition, "Position", "LoadPosition", XUI.Inline.Yes, enable: !_chkLoadExpression);
        XUI.Checkbox(ref _chkLoadRotation, "Rotation", "LoadRotation", XUI.Inline.Yes, enable: !_chkLoadExpression);
        XUI.Checkbox(ref _chkLoadScale, "Scale", "LoadScale", XUI.Inline.No, enable: !_chkLoadExpression);
        XUI.Checkbox(ref _chkLoadExpression, "Expression", "LoadExpression");
        XUI.Checkbox(ref _chkLoadBody, "Body", "LoadBody", XUI.Inline.No);
        // XUI.Checkbox(ref _chkfreezeOnLoad, "Freeze", "FreezeOnLoad");
        // XUI.Checkbox(ref _chkxfmModel, "xfmModel", "xfmModel");
        // XUI.Checkbox(ref _chkxfmModelOverride, "xfmModOverride", "xfmModOverride", XUI.Inline.No);

        XUI.Button(ExportPose(posecap!), FontAwesomeIcon.Save, "SavePose");
        XUI.Button(ImportPose(posecap!), FontAwesomeIcon.Folder, "LoadPose", XUI.Size.BtnMid, XUI.Inline.No);

        pop_actor_selected();

        if(!posedat.IsMock) SetPoseControllerData(posedat);
    }

    //
    // widget logic
    //

    private string _lastFriendlyName = "No actor selected";
    private string LastActorName(PosingCapability? cap) {
        if(cap == null) return _lastFriendlyName;
        _lastFriendlyName = cap.Entity.FriendlyName;
        return _lastFriendlyName;
    }

    private struct PoseControllerData {
        public bool IsMock;
    }

    private PoseControllerData GetPoseControllerData(PosingCapability? ent) { 
        if(ent is not PosingCapability cap) return mock();

        return mock();

        static PoseControllerData mock() {
            return new PoseControllerData {
                IsMock = true,
            };
        }
    }


    private void SetPoseControllerData(PoseControllerData posedat) {
    }

    private PosingCapability? ActorLastValid(Entity? ent) 
        => selectionService.UpdateLastPosing(ent);

    //
    // service aliases
    //

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

    private Action ExportPose(PosingCapability pose_cap)
        => () => { FileUIHelpers.ShowExportPoseModal(pose_cap); };

    private Action ImportPose(PosingCapability pose_cap) => () => { 
        BoneFilter filter = new(posingService);

        TransformComponents xfmComp = TransformComponents.None;
        if(_chkLoadPosition) xfmComp |= TransformComponents.Position;
        if(_chkLoadRotation) xfmComp |= TransformComponents.Rotation;
        if(_chkLoadScale) xfmComp |= TransformComponents.Scale;

        PoseImporterOptions importerOptions = new(filter, xfmComp, _chkxfmModel);

        TypeFilter typeFilter = new("Poses", typeof(PoseFile));

        // TODO: directory quick pose loading:
        // this call sets: 
        // var lastDirectories = _configurationService.Configuration.Library.LastBrowsePaths;
        // which should be ideal for indexing and switching poses
        LibraryManager.GetWithFilePicker(typeFilter, FilePickerCallback);

        void FilePickerCallback(object r){
            if(r is PoseFile pose) ImportPose(pose_cap, pose, xfmComp);
        }
    };


    private void ImportPose(PosingCapability posecap, PoseFile pose, TransformComponents xfmComp){
        if(_chkLoadExpression == _chkLoadBody) ImportFullPose(posecap, pose);
        if(_chkLoadBody) ImportGesture(posecap, pose, xfmComp);
        if(_chkLoadExpression) ImportExpression(posecap, pose);
    }

    private void ImportFullPose(PosingCapability posecap, PoseFile pose){
        posecap.ImportPose(pose,
                options: posecap.PosingService.DefaultIPCImporterOptions,
                asExpression: false, asBody: false, transformComponents: null,
                freezeOnLoad: _chkfreezeOnLoad, applyModelTransformOverride: _chkxfmModelOverride);
    }

    private void ImportExpression(PosingCapability posecap, PoseFile pose){
        posecap.ImportPose(pose,
                options: null,
                asExpression: true, asBody: false, transformComponents: null,
                freezeOnLoad: _chkfreezeOnLoad, applyModelTransformOverride: null);
    }

    private void ImportGesture(PosingCapability posecap, PoseFile pose, TransformComponents xfmComp){
        posecap.ImportPose(pose,
                options: null,
                asExpression: false, asBody: true, transformComponents: xfmComp,
                freezeOnLoad: _chkfreezeOnLoad, applyModelTransformOverride: _chkxfmModelOverride);
    }


    private void ImportAPose(PosingCapability poseCap) {
        poseCap.LoadResourcesPose("Data.BrioAPose.pose", _chkfreezeOnLoad, asBody: true);
    }

    private void ImportTPose(PosingCapability poseCap) {
        poseCap.LoadResourcesPose("Data.BrioTPose.pose", _chkfreezeOnLoad, asBody: true);
    }
}

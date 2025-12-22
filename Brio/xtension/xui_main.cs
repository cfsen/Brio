using Brio.Config;
using Brio.Core;
using Brio.Entities;
using Brio.Entities.Core;
using Brio.Game.Core;
using Brio.Game.GPose;
using Brio.Game.Scene;
using Brio.MCDF.Game.Services;
using Brio.UI.Controls.Core;
using Brio.UI.Entitites;
using Brio.UI.Theming;
using Brio.UI.Windows;
using Brio.Xtension.ui;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using System;
using System.Numerics;
using static Dalamud.Interface.Utility.Raii.ImRaii;

namespace Brio.Xtension;

// Changes to Brios codebase outside of this file are marked with:
// XT: reason

public class XtensionMain : Window, IDisposable
{
    private readonly SettingsWindow _settingsWindow;
    private readonly UpdateWindow _infoWindow;
    private readonly LibraryWindow _libraryWindow;
    private readonly ConfigurationService _configurationService;
    private readonly EntityManager _entityManager;
    private readonly EntityHierarchyView _entitySelector;
    private readonly SceneService _sceneService;
    private readonly ProjectWindow _projectWindow;
    private readonly GPoseService _gPoseService;
    private readonly AutoSaveService _autoSaveService;
    private readonly HistoryService _groupedUndoService;
    private readonly MCDFService _mCDFService;

    private RootEntityContainers? _rec;
    private XTWidgetSelector? _xwidgets;

    public XtensionMain(
            ConfigurationService configService,
            SettingsWindow settingsWindow,
            UpdateWindow infoWindow,
            LibraryWindow libraryWindow,
            EntityManager entityManager,
            HistoryService groupedUndoService,
            SceneService sceneService,
            GPoseService gPoseService,
            ProjectWindow projectWindow,
            AutoSaveService autoSaveService,
            MCDFService mCDFService
            )
        : base("##brio_xtension_main_window", ImGuiWindowFlags.AlwaysAutoResize, true) 
    {
        Namespace = "bri_xtension_namespace";

        _configurationService = configService;
        _settingsWindow = settingsWindow;
        _libraryWindow = libraryWindow;
        _infoWindow = infoWindow;
        _entityManager = entityManager;
        _gPoseService = gPoseService;
        _groupedUndoService = groupedUndoService;
        _entitySelector = new(_entityManager, _gPoseService, _groupedUndoService);
        _sceneService = sceneService;
        _projectWindow = projectWindow;
        _autoSaveService = autoSaveService;
        _mCDFService = mCDFService;

        // lazy init
        _rec = null;
        _xwidgets = null;

        SizeConstraints = new WindowSizeConstraints {
            MaximumSize = new Vector2(400, 1000),
            MinimumSize = new Vector2(400, 1000),
        };
    }

    public override void Draw(){
        LazyInit();
        _xwidgets?.DrawWidgetSelector();
        _xwidgets?.DrawWidgets();
        // DrawEntitySelection();
    }

    private void LazyInit(){
        if(_rec == null && _entityManager.RootEntity != null) {
            _rec = new RootEntityContainers(_entityManager.RootEntity);

            _xwidgets = new XTWidgetSelector(_rec, _entitySelector);
        }
    }

    private void DrawEntitySelection(){
        if(!_gPoseService.IsGPosing) {
            using(ImRaii.PushColor(ImGuiCol.Text, UIConstants.GizmoRed)) {
                ImGui.Text("Open GPose to use Brio!");
            }
            return;
        }

        if(_entityManager.RootEntity is null) {
            return;
        }
        var rootEntity = _entityManager.RootEntity;

        IEndObject? container = ImRaii.Child("###entity_hierarchy_container",
                new Vector2(-1, ImGui.GetTextLineHeight() * 18f),
                true);
        if(!container.Success) {
            return;
        }

        using(container) {
            _entitySelector.Draw(rootEntity);

            // if(_entityManager.SelectedEntityIds.Count > 1) {
            // 	using var color = ImRaii.PushColor(ImGuiCol.Text, ThemeManager.CurrentTheme.Accent.AccentColor);
            // 	ImGui.Text($"{_entityManager.SelectedEntityIds.Count} selected");
            // }
        }

        // NOTE: this draws all the extra context sensitive UI
        //
        // try
        // {
        // 	EntityHelpers.DrawEntitySection(_entityManager.SelectedEntity);
        // }
        // catch(Exception ex)
        // {
        // 	Brio.Log.Error(ex, $"Failed to draw entity section: [ {_entityManager?.SelectedEntity?.FriendlyName ?? "Unknown"} ] ");
        // }
    }


    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}


using System;
using Brio.Config;
using Brio.Game.Posing;
using Brio.Input;
using Brio.UI.Windows.Specialized;

namespace Brio.Xtension.services;

public class XTServiceDispatcher : IDisposable {
    private readonly PosingService _posingService;
    private readonly ConfigurationService _configService;
    private readonly PosingOverlayWindow _posingOverlay;

    public XTServiceDispatcher(
            PosingService posingService,
            ConfigurationService configurationService,
            PosingOverlayWindow posingOverlayWindow
            ){
        _posingService = posingService;
        _configService = configurationService;
        _posingOverlay = posingOverlayWindow;
    }

    public void HandleKeyboardAction(InputAction action){
        switch(action) {
            case InputAction.Interface_StopCutscene:
                break;
            case InputAction.Interface_StartAllActorsAnimations:
                break;
            case InputAction.Interface_StopAllActorsAnimations:
                break;
            case InputAction.Interface_SelectAllActors:
                break;
            case InputAction.Posing_ToggleOverlay:
                _posingOverlay.IsOpen = !_posingOverlay.IsOpen;
                break;
            case InputAction.Posing_Undo:
                break;
            case InputAction.Posing_Redo:
                break;
            case InputAction.Posing_Esc:
                break;
            case InputAction.Posing_DisableGizmo:
                break;
            case InputAction.Posing_DisableSkeleton:
                break;
            case InputAction.Posing_Translate:
                _posingService.Operation = PosingOperation.Translate;
                break;
            case InputAction.Posing_Rotate:
                _posingService.Operation = PosingOperation.Rotate;
                break;
            case InputAction.Posing_Scale:
                _posingService.Operation = PosingOperation.Scale;
                break;
            case InputAction.Posing_Universal:
                break;
            case InputAction.Posing_ToggleLink:
                break;
            case InputAction.Posing_ToggleWorld:
                break;
            case InputAction.Posing_Freeze:
                break;
            default:
                break;
        }
    }

    public void Dispose() {
        GC.SuppressFinalize(this);
    }
}

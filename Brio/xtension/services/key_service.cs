using Brio.Config;
using Brio.Game.GPose;
using Brio.Input;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;

namespace Brio.Xtension.services;

public class XTInputKeyboardHandler : IDisposable {
    private readonly ConfigurationService _configurationService;
    private readonly GPoseService _gPoseService;
    private readonly IFramework _framework;
    private readonly IKeyState _keyState;

    private readonly List<(int idx, Binding bind)> DirtyBindings = [];
    private readonly List<Binding> Keymap = [];
    private readonly XTServiceDispatcher _dispatcher;

    private ModifierState Modifiers;

    public XTInputKeyboardHandler(
            IKeyState keyState,
            IFramework framework,
            ConfigurationService configurationService,
            GPoseService gPoseService,
            XTServiceDispatcher dispatcher
            ){
        _keyState = keyState;
        _configurationService = configurationService;
        _gPoseService = gPoseService;
        _framework = framework;
        _dispatcher = dispatcher;

        _configurationService.OnConfigurationChanged += Reinitialize;
        _framework.Update += Update;

        Brio.Log.Info("XT input handler started.");
        Initialize();
    }

    public void Update(IFramework fw){
        if(!_gPoseService.IsGPosing) return;

        Modifiers = GetModifierState(_keyState);

        for(int i = 0; i < Keymap.Count; i++) {
            KeyStateChanged(i, Keymap[i], _keyState, DirtyBindings.Add);
        }

        for(int j = 0; j < DirtyBindings.Count; j++){
            Dispatcher(DirtyBindings[j].bind);
            Keymap[DirtyBindings[j].idx] = DirtyBindings[j].bind;
        }
        DirtyBindings.Clear();
    }


    private void KeyStateChanged(int KeymapIdx, Binding bind, IKeyState tickKeys, Action<(int, Binding)> add_dirty){
        if(!ModifiersMatch(bind.Modifiers)) return;

        if(tickKeys[bind.Key] != bind.Keystate) {
            bind.Keystate = tickKeys[bind.Key];
            add_dirty((KeymapIdx, bind));
        }
    }

    private ModifierState GetModifierState(IKeyState tickKeys){
        return new ModifierState {
            Ctrl = tickKeys[VirtualKey.CONTROL],
            Alt = tickKeys[VirtualKey.MENU],
            Shift = tickKeys[VirtualKey.SHIFT],
        };
    }

    private void Dispatcher(Binding bind){
        if(!bind.Keystate) return;
        if(!ModifiersMatch(bind.Modifiers)) return;

        Brio.Log.Info($"XT dispatching: {bind.Key.GetFancyName()} -> {bind.Action}.");
        _dispatcher.HandleKeyboardAction(bind.Action);
    }

    //
    // Helpers
    //


    private HotkeyModifiers ModifiersFromKeyConfig(KeyConfig cfg){
        HotkeyModifiers res = HotkeyModifiers.None;
        if(cfg.RequireCtrl) res ^= HotkeyModifiers.Ctrl;
        if(cfg.RequireAlt) res ^= HotkeyModifiers.Alt;
        if(cfg.RequireShift) res ^= HotkeyModifiers.Shift;
        return res;
    }

    private bool ModifiersMatch(HotkeyModifiers required) {
        HotkeyModifiers current = HotkeyModifiers.None;
        if(Modifiers.Ctrl) current |= HotkeyModifiers.Ctrl;
        if(Modifiers.Alt) current |= HotkeyModifiers.Alt;
        if(Modifiers.Shift) current |= HotkeyModifiers.Shift;
        return current == required;
    }

    //
    // init, dispose
    //

    private void Initialize(){
        Keymap.Clear();
        HashSet<VirtualKey> validKeys = [.. _keyState.GetValidVirtualKeys()];

        foreach((InputAction action, KeyConfig bind) in _configurationService.Configuration.InputManager.KeyBindings) {
            if(!validKeys.Contains(bind.Key)) {
                continue;
            }

            Keymap.Add(new Binding {
                    Key = bind.Key, 
                    Action = action,
                    Keystate = _keyState[bind.Key],
                    Modifiers = ModifiersFromKeyConfig(bind),
                    });
        }
    }

    private void Reinitialize(){
        Brio.Log.Info("XTInputKeyboardHandler reintializing");
        Initialize();
    }

    public void Dispose() {
        _configurationService.OnConfigurationChanged -= Reinitialize;
        _framework.Update -= Update;
        GC.SuppressFinalize(this);
    }

    //
    // DS
    //

    private struct Binding {
        public VirtualKey Key;
        public InputAction Action;
        public bool Keystate;
        public HotkeyModifiers Modifiers;
    }

    private struct ModifierState {
        public bool Ctrl;
        public bool Alt;
        public bool Shift;
    }

    [Flags]
    private enum HotkeyModifiers {
        None = 0,
        Ctrl = 2,
        Alt = 4,
        Shift = 8,
    }
}

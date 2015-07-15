using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jolt
{
    public class SetupPresenter
    {
        /****************************************************************************************************
         * fields
         ****************************************************************************************************/
        private ISetupView    _view;
        private ISetupModel     _model;


        /****************************************************************************************************
         * construction, initialization, destruction, finalization
         ****************************************************************************************************/
        public SetupPresenter(ISetupView view, ISetupModel model)
        {
            _view = view;
            _model = model;

            // initial view values
            _view.Fullscreen = _model.FullScreen;
            _view.VerticalSync = _model.UseVerticalSync;
            _view.SyncRecordMode = _model.SyncRecordMode;
            _view.DeviceDebugMode = _model.DeviceDebugMode;
            _view.UseAudio = _model.UseAudio;
            _view.UseOculus = _model.UseOculus;
            
            // view -> model
            _view.SelectedAdapterChanged += (i) => _model.SelectAdapter(i);
            _view.SelectedOutputChanged += (i) => _model.SelectOutput(i);
            _view.SelectedModeChanged += (i) => _model.SelectMode(i);
            _view.FullscreenChanged += (b) => _model.FullScreen = b;
            _view.VerticalSyncChanged += (b) => _model.UseVerticalSync = b;
            _view.SyncRecordModeChanged += (b) => _model.SyncRecordMode = b;
            _view.DeviceDebugModeChanged += (b) => _model.DeviceDebugMode = b;
            _view.UseAudioChanged += (b) => _model.UseAudio = b;
            _view.UseOculusChanged += (b) => _model.UseOculus = b;
            _view.BassRegistrationEmailChanged += (s) => _model.BassRegistrationEmail = s;
            _view.BassRegistrationKeyChanged += (s) => _model.BassRegistrationKey = s;

            // model -> view
            _model.FullScreenChanged += () => _view.Fullscreen = _model.FullScreen;
            _model.DeviceDebugModeChanged += () => _view.DeviceDebugMode = _model.DeviceDebugMode;
            _model.SyncRecordModeChanged += () => _view.SyncRecordMode = _model.SyncRecordMode;
            _model.UseVerticalSyncChanged += () => _view.VerticalSync = _model.UseVerticalSync;
            _model.UseAudioChanged += () => _view.UseAudio = _model.UseAudio;
            _model.UseOculusChanged += () => _view.UseOculus = _model.UseOculus;
            _model.BassRegistrationEmailChanged += () => _view.BassRegistrationEmail = _model.BassRegistrationEmail;
            _model.BassRegistrationKeyChanged += () => _view.BassRegistrationKey = _model.BassRegistrationKey;
            _model.AdapterChanged += () =>
            {
                _view.SelectedAdapter = _model.AdapterIndex;
                _view.SetAvailableOutputs(_model.GetAvailableOutputs());
            };
            _model.OutputChanged += () =>
            {
                _view.SelectedOutput = _model.OutputIndex;
                _view.SetAvailableModes(_model.GetAvailableModes());
            };
            _model.ModeChanged += () => 
            {
                _view.SelectedMode = _model.ModeIndex;
            };
            _model.SupportedFeatureLevelChanged += () => _view.SetFeatureLevel(_model.SupportedFeatureLevel.ToString());

            _view.SetAvailableAdapters(_model.GetAvailableAdapters());
        }
    }
}

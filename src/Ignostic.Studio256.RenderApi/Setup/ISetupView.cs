using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jolt
{
    public interface ISetupView
    {
        event Action<int>       SelectedAdapterChanged;
        event Action<int>       SelectedOutputChanged;
        event Action<int>       SelectedModeChanged;
        event Action<bool>      FullscreenChanged;
        event Action<bool>      VerticalSyncChanged;
        event Action<bool>      SyncRecordModeChanged;
        event Action<bool>      DeviceDebugModeChanged;
        event Action<bool>      UseAudioChanged;
        event Action<bool>      UseOculusChanged;
        event Action<string>    BassRegistrationEmailChanged;
        event Action<string>    BassRegistrationKeyChanged;
        
        int     SelectedAdapter         { get; set; }
        int     SelectedOutput          { get; set; }
        int     SelectedMode            { get; set; }
        bool    Fullscreen              { get; set; }
        bool    VerticalSync            { get; set; }
        bool    SyncRecordMode          { get; set; }
        bool    DeviceDebugMode         { get; set; }
        bool    UseAudio                { get; set; }
        bool    UseOculus               { get; set; }
        string  BassRegistrationEmail   { get; set; }
        string  BassRegistrationKey     { get; set; }

        void SetAvailableAdapters(string[] items);
        void SetAvailableOutputs(string[] items);
        void SetAvailableModes(string[] items);
        void SetFeatureLevel(string featureLevel);
    }
}

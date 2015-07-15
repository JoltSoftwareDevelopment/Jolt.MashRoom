using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;

namespace Jolt
{
    public interface ISetupModel
    {
        /****************************************************************************************************
         * events
         ****************************************************************************************************/
        event Action AdapterChanged;
        event Action OutputChanged;
        event Action ModeChanged;
        event Action SupportedFeatureLevelChanged;
        event Action FullScreenChanged;
        event Action DeviceDebugModeChanged;
        event Action SyncRecordModeChanged;
        event Action UseVerticalSyncChanged;
        event Action UseAudioChanged;
        event Action UseOculusChanged;
        event Action BassRegistrationEmailChanged;
        event Action BassRegistrationKeyChanged;


        /****************************************************************************************************
         * properties
         ****************************************************************************************************/
        Factory         Factory                 { get; }
        Format          Format                  { get; }
        Adapter         Adapter                 { get; }
        Output          Output                  { get; }
        FeatureLevel?   SupportedFeatureLevel   { get; }
        ModeDescription Mode                    { get; }
        int             AdapterIndex            { get; }
        int             OutputIndex             { get; }
        int             ModeIndex               { get; }
        FeatureLevel?   RequiredFeatureLevel    { get; set; }
        DriverType      DriverType              { get; set; }
        bool            FullScreen              { get; set; }
        int             MultiSampleCount        { get; set; }
        int             MultiSampleQuality      { get; set; }
        bool            DeviceDebugMode         { get; set; }
        bool            SyncRecordMode          { get; set; }
        bool            UseVerticalSync         { get; set; }
        bool            UseAudio                { get; set; }
        bool            UseOculus               { get; set; }
        double          StartTime               { get; set; }
        string          BassRegistrationEmail   { get; set; }
        string          BassRegistrationKey     { get; set; }


        /****************************************************************************************************
         * methods
         ****************************************************************************************************/
        string[] GetAvailableAdapters();
        string[] GetAvailableOutputs();
        string[] GetAvailableModes();
        
        void SelectAdapter(int index);
        void SelectOutput(int index);
        void SelectMode(int index);
    }
}

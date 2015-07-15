using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;
using Ignostic;


namespace Jolt
{
    public class SetupModel : ISetupModel
    {
        /****************************************************************************************************
         * events
         ****************************************************************************************************/
        public event Action AdapterChanged;
        public event Action OutputChanged;
        public event Action ModeChanged;
        public event Action SupportedFeatureLevelChanged;
        public event Action FullScreenChanged;
        public event Action DeviceDebugModeChanged;
        public event Action SyncRecordModeChanged;
        public event Action UseVerticalSyncChanged;
        public event Action UseAudioChanged;
        public event Action UseOculusChanged;
        public event Action BassRegistrationEmailChanged;
        public event Action BassRegistrationKeyChanged;


        /****************************************************************************************************
         * fields
         ****************************************************************************************************/
        private bool    _fullScreen;
        private bool    _deviceDebugMode;
        private bool    _syncRecordMode;
        private bool    _useVerticalSync;
        private bool    _useAudio;
        private bool    _useOculus;
        private string  _bassRegistrationEmail;
        private string  _bassRegistrationKey;


        /****************************************************************************************************
         * properties without code
         ****************************************************************************************************/
        public Factory          Factory                 { get; private set; }
        public Format           Format                  { get; private set; }
        public Adapter          Adapter                 { get; private set; }
        public Output           Output                  { get; private set; }
        public ModeDescription  Mode                    { get; private set; }
        public int              AdapterIndex            { get; private set; }
        public int              OutputIndex             { get; private set; }
        public int              ModeIndex               { get; private set; }
        public int              MultiSampleCount        { get; set; }
        public int              MultiSampleQuality      { get; set; }
        public DriverType       DriverType              { get; set; }
        public double           StartTime               { get; set; }
        public FeatureLevel?    RequiredFeatureLevel    { get; set; }
        public FeatureLevel?    SupportedFeatureLevel   { get; private set; }
#warning TODO Device.GetSupportedFeatureLevel and Device.MultisampleCountMaximum


        /****************************************************************************************************
         * construction, initialization, destruction, finalization
         ****************************************************************************************************/
        public SetupModel()
        {
            Factory = new Factory();
            // TODO determine if ew should use srgb or not
            Format = Format.R8G8B8A8_UNorm;
            //Format = Format.R8G8B8A8_UNorm_SRgb;
            // TODO make sure we have the proper count and quality
            MultiSampleCount = 4;
            MultiSampleQuality = 0;
            RequiredFeatureLevel = FeatureLevel.Level_11_0;
            DriverType = DriverType.Hardware;

            FullScreen = true;
            DeviceDebugMode = false;
            SyncRecordMode = false;
            UseVerticalSync = true;
            UseAudio = true;
            AdapterIndex = -1;
            OutputIndex = -1;
            ModeIndex = -1;
        }


        /****************************************************************************************************
         * properties with code
         ****************************************************************************************************/
        public bool FullScreen
        {
            get { return _fullScreen; }
            set
            {
                if (_fullScreen == value)
                    return;
                _fullScreen = value;
                FullScreenChanged.InvokeIfNotNull();
            }
        }


        public bool DeviceDebugMode
        {
            get { return _deviceDebugMode; }
            set
            {
                if (_deviceDebugMode == value)
                    return;
                _deviceDebugMode = value;
                DeviceDebugModeChanged.InvokeIfNotNull();
            }
        }


        public bool SyncRecordMode
        {
            get { return _syncRecordMode; }
            set
            {
                if (_syncRecordMode == value)
                    return;
                _syncRecordMode = value;
                SyncRecordModeChanged.InvokeIfNotNull();
            }
        }


        public bool UseVerticalSync
        {
            get { return _useVerticalSync; }
            set
            {
                if (_useVerticalSync == value)
                    return;
                _useVerticalSync = value;
                UseVerticalSyncChanged.InvokeIfNotNull();
            }
        }


        public bool UseAudio
        {
            get { return _useAudio; }
            set
            {
                if (_useAudio == value)
                    return;
                _useAudio = value;
                UseAudioChanged.InvokeIfNotNull();
            }
        }


        public bool UseOculus
        {
            get { return _useOculus; }
            set
            {
                if (_useOculus == value)
                    return;
                _useOculus = value;
                UseOculusChanged.InvokeIfNotNull();
            }
        }

        
        public string BassRegistrationEmail
        {
            get { return _bassRegistrationEmail; }
            set
            {
                if (_bassRegistrationEmail == value)
                    return;
                _bassRegistrationEmail = value;
                BassRegistrationEmailChanged.InvokeIfNotNull();
            }
        }


        public string BassRegistrationKey
        {
            get { return _bassRegistrationKey; }
            set
            {
                if (_bassRegistrationKey == value)
                    return;
                _bassRegistrationKey = value;
                BassRegistrationKeyChanged.InvokeIfNotNull();
            }
        }


        /****************************************************************************************************
         * public methods
         ****************************************************************************************************/
        public string[] GetAvailableAdapters()
        {
            return Enumerable
                .Range(0, Factory.GetAdapterCount())
                .Select(i => Factory.GetAdapter(i).Description)
                .Select(d => d.Description)
                .ToArray();
        }


        public string[] GetAvailableOutputs()
        {
            if (Adapter == null)
                return new string[0];

            return Adapter
                .Outputs
                .Select(adapter => adapter.Description.DeviceName)
                .ToArray();
        }


        public string[] GetAvailableModes()
        {
            return GetDisplayModes()
                .Select(mode => string.Format(
                    "{0}x{1}@{2}({3})",
                    mode.Width,
                    mode.Height,
                    (float)mode.RefreshRate.Numerator / mode.RefreshRate.Denominator,
                    mode.ScanlineOrdering))
                .ToArray();
        }


        public void SelectAdapter(int index)
        {
            if (AdapterIndex == index)
                return;

            SelectOutput(-1);

            AdapterIndex = index;
            var count = Factory.GetAdapterCount();
            if (0 <= index && index < count)
            {
                Adapter = Factory.GetAdapter(index);
                using (var device = new Device(Adapter, DeviceCreationFlags.None))
                {
                    SupportedFeatureLevel = device.FeatureLevel;
                }
                Output = null;
                Mode = new ModeDescription();
            }
            else
            {
                Adapter = null;
                SupportedFeatureLevel = null;
                Output = null;
                Mode = new ModeDescription();
            }

            AdapterChanged.InvokeIfNotNull();
            SelectOutput(0);
        }


        public void SelectOutput(int index)
        {
            if (OutputIndex == index)
                return;

            SelectMode(-1);

            var count = Adapter.GetOutputCount();
            if (0 <= index && index < count)
            {
                OutputIndex = index;
                Output = Adapter.GetOutput(index);
                Mode = new ModeDescription();
            }
            else
            {
                OutputIndex = -1;
                Output = null;
                Mode = new ModeDescription();
            }

            OutputChanged.InvokeIfNotNull();
            SelectMode(0);
        }


        public void SelectMode(int index)
        {
            if (ModeIndex == index)
                return;

            var modes = GetDisplayModes();
            var count = modes.Length;
            if (0 <= index && index < count)
            {
                ModeIndex = index;
                Mode = modes[index];
            }
            else
            {
                ModeIndex = -1;
                Mode = new ModeDescription();
            }

            ModeChanged.InvokeIfNotNull();
        }


        public bool TrySelectMode(ModeDescription mode)
        {
            var modes = GetDisplayModes();
            var matchingIndices = Enumerable
                .Range(0, modes.Length)
                .Where(i => mode.Equals(modes[i]))
                .ToArray();
            if (matchingIndices.Length != 1)
                return false;

            SelectMode(matchingIndices[0]);
            return true;
        }


        [Conditional("DEBUG")]
        public void SaveSettings()
        {
            using (var stream = File.Create("settings.xml"))
            using (var xmlWriter = XmlWriter.Create(stream))
            {
                var serializer = new XmlSerializer(typeof(SetupData));
                serializer.Serialize(xmlWriter, new SetupData
                {
                    DeviceDebugMode = this.DeviceDebugMode,
                    SyncRecordMode = this.SyncRecordMode,
                    UseVerticalSync = this.UseVerticalSync,
                    UseAudio = this.UseAudio,
                    UseOculus = this.UseOculus,
                    FullScreen = this.FullScreen,
                    MultiSampleCount = this.MultiSampleCount,
                    MultiSampleQuality = this.MultiSampleQuality,
                    StartTime = this.StartTime,
                    Mode = this.Mode,
                    BaseRegistrationEmail = this.BassRegistrationEmail,
                    BaseRegistrationKey = this.BassRegistrationKey,
                });
                xmlWriter.Flush();
                stream.Flush();
            }
        }


        [Conditional("DEBUG")]
        public void TryLoadSettings()
        {
            var persistedSettings = null as SetupData;
            try
            {
                using (var stream = File.Open("settings.xml", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var xmlReader = XmlReader.Create(stream))
                {
                    var serializer = new XmlSerializer(typeof(SetupData));
                    persistedSettings = (SetupData)serializer.Deserialize(xmlReader);
                }
            }
            catch
            {
                // swallow exception
                return;
            }

            this.DeviceDebugMode = persistedSettings.DeviceDebugMode;
            this.SyncRecordMode = persistedSettings.SyncRecordMode;
            this.UseVerticalSync = persistedSettings.UseVerticalSync;
            this.UseAudio = persistedSettings.UseAudio;
            this.UseOculus = persistedSettings.UseOculus;
            this.FullScreen = persistedSettings.FullScreen;
            this.MultiSampleCount = persistedSettings.MultiSampleCount;
            this.MultiSampleQuality = persistedSettings.MultiSampleQuality;
            this.StartTime = persistedSettings.StartTime;
            this.BassRegistrationEmail = persistedSettings.BaseRegistrationEmail;
            this.BassRegistrationKey = persistedSettings.BaseRegistrationKey;
            TrySelectMode(persistedSettings.Mode); // might fail, but we don't care
        }


        /****************************************************************************************************
         * private methods
         ****************************************************************************************************/
        private ModeDescription[] GetDisplayModes()
        {
            if (Output == null)
                return new ModeDescription[0];

            return Output
                .GetDisplayModeList(Format, DisplayModeEnumerationFlags.Scaling).Where(s => s.Scaling == DisplayModeScaling.Unspecified)
                .Select(mode => new ModeDescription(mode.Width, mode.Height, mode.RefreshRate, mode.Format)
                {
                    ScanlineOrdering = mode.ScanlineOrdering
                })
                .OrderByDescending(mode => mode.Width * mode.Height)
                .ThenByDescending(o => (float)o.RefreshRate.Numerator / o.RefreshRate.Denominator)
                .ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ignostic;

namespace Jolt
{
    public partial class StartupView : Form, ISetupView
    {
        /****************************************************************************************************
         * events
         ****************************************************************************************************/
        public event Action<int> SelectedAdapterChanged;
        public event Action<int> SelectedOutputChanged;
        public event Action<int> SelectedModeChanged;
        public event Action<bool> FullscreenChanged;
        public event Action<bool> VerticalSyncChanged;
        public event Action<bool> SyncRecordModeChanged;
        public event Action<bool> DeviceDebugModeChanged;
        public event Action<bool> UseAudioChanged;
        public event Action<bool> UseOculusChanged;
        public event Action<string> BassRegistrationEmailChanged;
        public event Action<string> BassRegistrationKeyChanged;


        /****************************************************************************************************
         * construction, initialization, destruction, finalization
         ****************************************************************************************************/
        public StartupView(string title)
        {
            InitializeComponent();
            this.Text = title;
            this.comboBoxAdapters.SelectedIndexChanged += (s, a) => SelectedAdapterChanged.InvokeIfNotNull(comboBoxAdapters.SelectedIndex);
            this.comboBoxOutputs.SelectedIndexChanged += (s, a) => SelectedOutputChanged.InvokeIfNotNull(comboBoxOutputs.SelectedIndex);
            this.comboBoxModes.SelectedIndexChanged += (s, a) => SelectedModeChanged.InvokeIfNotNull(comboBoxModes.SelectedIndex);
            this.checkBoxFullscreen.CheckedChanged += (s, a) => FullscreenChanged.InvokeIfNotNull(checkBoxFullscreen.Checked);
            this.checkBoxVerticalSync.CheckedChanged += (s, a) => VerticalSyncChanged.InvokeIfNotNull(checkBoxVerticalSync.Checked);
            this.checkBoxSyncRecord.CheckedChanged += (s, a) => SyncRecordModeChanged.InvokeIfNotNull(checkBoxSyncRecord.Checked);
            this.checkBoxDeviceDebug.CheckedChanged += (s, a) => DeviceDebugModeChanged.InvokeIfNotNull(checkBoxDeviceDebug.Checked);
            this.checkBoxUseAudio.CheckedChanged += (s, a) => UseAudioChanged.InvokeIfNotNull(checkBoxUseAudio.Checked);
            this.checkBoxUseOculus.CheckedChanged += (s, a) => UseOculusChanged.InvokeIfNotNull(checkBoxUseOculus.Checked);
            this.bassRegistrationEmailTextbox.TextChanged += (s, a) => BassRegistrationEmailChanged.InvokeIfNotNull(bassRegistrationEmailTextbox.Text);
            this.bassRegistrationKeyTextbox.TextChanged += (s, a) => BassRegistrationKeyChanged.InvokeIfNotNull(bassRegistrationKeyTextbox.Text);

            //#if !DEBUG
            //{
            //    this.checkBoxFullscreen.Visible = false;
            //    this.checkBoxVerticalSync.Visible = false;
            //    this.checkBoxSyncRecord.Visible = false;
            //    this.checkBoxDeviceDebug.Visible = false;
            //}
            //#endif
        }


        /****************************************************************************************************
         * properties
         ****************************************************************************************************/
        public int SelectedAdapter
        {
            get { return comboBoxAdapters.SelectedIndex; }
            set { comboBoxAdapters.SelectedIndex = value; }
        }


        public int SelectedOutput
        {
            get { return comboBoxOutputs.SelectedIndex; }
            set { comboBoxOutputs.SelectedIndex = value; }
        }


        public int SelectedMode
        {
            get { return comboBoxModes.SelectedIndex; }
            set { comboBoxModes.SelectedIndex = value; }
        }


        public bool Fullscreen
        {
            get { return checkBoxFullscreen.Checked; }
            set { checkBoxFullscreen.Checked = value; }
        }


        public bool VerticalSync
        {
            get { return checkBoxVerticalSync.Checked; }
            set { checkBoxVerticalSync.Checked = value; }
        }


        public bool SyncRecordMode
        {
            get { return checkBoxSyncRecord.Checked; }
            set { checkBoxSyncRecord.Checked = value; }
        }


        public bool DeviceDebugMode
        {
            get { return checkBoxDeviceDebug.Checked; }
            set { checkBoxDeviceDebug.Checked = value; }
        }


        public bool UseAudio
        {
            get { return checkBoxUseAudio.Checked; }
            set { checkBoxUseAudio.Checked = value; }
        }

        
        public bool UseOculus
        {
            get { return checkBoxUseOculus.Checked; }
            set { checkBoxUseOculus.Checked = value; }
        }

        
        public string BassRegistrationEmail 
        { 
            get { return bassRegistrationEmailTextbox.Text; } 
            set { bassRegistrationEmailTextbox.Text = value; } 
        }
        
        
        public string BassRegistrationKey   
        { 
            get { return bassRegistrationKeyTextbox  .Text; } 
            set { bassRegistrationKeyTextbox  .Text = value; } 
        }
        

        /****************************************************************************************************
         * methods
         ****************************************************************************************************/
        public void SetAvailableAdapters(string[] items)
        {
            comboBoxAdapters.Items.Clear();
            comboBoxAdapters.Items.AddRange(items);
            if (items.Length > 0)
                comboBoxAdapters.SelectedIndex = 0;
            comboBoxAdapters.Invalidate();
        }


        public void SetAvailableOutputs(string[] items)
        {
            comboBoxOutputs.Items.Clear();
            comboBoxOutputs.Items.AddRange(items);
            if (items.Length > 0)
                comboBoxOutputs.SelectedIndex = 0;
            comboBoxOutputs.Invalidate();
        }


        public void SetAvailableModes(string[] items)
        {
            comboBoxModes.Items.Clear();
            comboBoxModes.Items.AddRange(items);
            if (items.Length > 0)
                comboBoxModes.SelectedIndex = 0;
            comboBoxModes.Invalidate();
        }


        public void SetFeatureLevel(string featureLevel)
        {
            labelFeatureLevel.Text = featureLevel;
        }
    }
}

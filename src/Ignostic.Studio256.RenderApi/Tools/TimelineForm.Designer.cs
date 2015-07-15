namespace Ignostic.Studio256.RenderApi.Tools
{
    partial class TimelineForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            TimelineModel timelineModel1 = new TimelineModel();
            this.timelineControl1 = new TimelineControl();
            this.SuspendLayout();
            // 
            // timelineControl1
            // 
            this.timelineControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timelineControl1.Location = new System.Drawing.Point(0, 0);
            this.timelineControl1.Model = timelineModel1;
            this.timelineControl1.Name = "timelineControl1";
            this.timelineControl1.Size = new System.Drawing.Size(284, 261);
            this.timelineControl1.TabIndex = 0;
            this.timelineControl1.Text = "timelineControl1";
            this.timelineControl1.TimeOffset = 0F;
            this.timelineControl1.TimeScale = 1F;
            // 
            // TimelineForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.timelineControl1);
            this.Name = "TimelineForm";
            this.Text = "TimelineForm";
            this.ResumeLayout(false);

        }

        #endregion

        private TimelineControl timelineControl1;
    }
}
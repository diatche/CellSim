namespace AI
{
    partial class Env
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
            this.components = new System.ComponentModel.Container();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 25;
            this.timer.Tick += new System.EventHandler(this.timer_Tick_1);
            // 
            // Env
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Name = "Env";
            this.Text = "Diatche AI";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResizeBegin += new System.EventHandler(this.Env_ResizeBegin);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Env_MouseWheel);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Env_MouseUp);
            this.Disposed += new System.EventHandler(this.Env_Disposed);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Env_KeyUp);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Env_MouseMove);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Env_KeyDown);
            this.ResizeEnd += new System.EventHandler(this.Env_ResizeEnd);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Env_MouseDown);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Timer timer;

    }
}


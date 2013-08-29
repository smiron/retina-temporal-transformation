namespace WindowsFormsRetina
{
    partial class SpikeViewer
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
            this.output = new System.Windows.Forms.PictureBox();
            this.Generate = new System.Windows.Forms.Button();
            this.source = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.output)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.source)).BeginInit();
            this.SuspendLayout();
            // 
            // output
            // 
            this.output.Location = new System.Drawing.Point(61, 12);
            this.output.Name = "output";
            this.output.Size = new System.Drawing.Size(1109, 530);
            this.output.TabIndex = 0;
            this.output.TabStop = false;
            // 
            // Generate
            // 
            this.Generate.Location = new System.Drawing.Point(0, 300);
            this.Generate.Name = "Generate";
            this.Generate.Size = new System.Drawing.Size(55, 23);
            this.Generate.TabIndex = 1;
            this.Generate.Text = "button1";
            this.Generate.UseVisualStyleBackColor = true;
            this.Generate.Click += new System.EventHandler(this.LoadSpikes);
            // 
            // source
            // 
            this.source.Location = new System.Drawing.Point(0, 12);
            this.source.Name = "source";
            this.source.Size = new System.Drawing.Size(55, 48);
            this.source.TabIndex = 2;
            this.source.TabStop = false;
            // 
            // SpikeViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 487);
            this.Controls.Add(this.source);
            this.Controls.Add(this.Generate);
            this.Controls.Add(this.output);
            this.Name = "SpikeViewer";
            this.Text = "SpikeViewer";
            ((System.ComponentModel.ISupportInitialize)(this.output)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.source)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox output;
        private System.Windows.Forms.Button Generate;
        private System.Windows.Forms.PictureBox source;
    }
}


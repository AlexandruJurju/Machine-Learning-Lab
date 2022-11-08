namespace Kmeans2
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonStep = new System.Windows.Forms.Button();
            this.buttonFullRun = new System.Windows.Forms.Button();
            this.textBoxPrinting = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonStep
            // 
            this.buttonStep.Location = new System.Drawing.Point(797, 77);
            this.buttonStep.Name = "buttonStep";
            this.buttonStep.Size = new System.Drawing.Size(75, 23);
            this.buttonStep.TabIndex = 0;
            this.buttonStep.Text = "Step";
            this.buttonStep.UseVisualStyleBackColor = true;
            this.buttonStep.MouseClick += new System.Windows.Forms.MouseEventHandler(this.buttonStep_MouseClick);
            // 
            // buttonFullRun
            // 
            this.buttonFullRun.Location = new System.Drawing.Point(927, 77);
            this.buttonFullRun.Name = "buttonFullRun";
            this.buttonFullRun.Size = new System.Drawing.Size(75, 23);
            this.buttonFullRun.TabIndex = 1;
            this.buttonFullRun.Text = "Full Run";
            this.buttonFullRun.UseVisualStyleBackColor = true;
            this.buttonFullRun.MouseClick += new System.Windows.Forms.MouseEventHandler(this.buttonFullRun_MouseClick);
            // 
            // textBoxPrinting
            // 
            this.textBoxPrinting.Location = new System.Drawing.Point(797, 204);
            this.textBoxPrinting.Multiline = true;
            this.textBoxPrinting.Name = "textBoxPrinting";
            this.textBoxPrinting.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxPrinting.Size = new System.Drawing.Size(412, 402);
            this.textBoxPrinting.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(1272, 711);
            this.Controls.Add(this.textBoxPrinting);
            this.Controls.Add(this.buttonFullRun);
            this.Controls.Add(this.buttonStep);
            this.Name = "MainForm";
            this.Text = "Main Form";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button buttonStep;
        private Button buttonFullRun;
        private TextBox textBoxPrinting;
    }
}
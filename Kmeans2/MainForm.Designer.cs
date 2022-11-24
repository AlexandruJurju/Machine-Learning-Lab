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
            this.buttonDrawNeurons = new System.Windows.Forms.Button();
            this.buttonSOMFullRun = new System.Windows.Forms.Button();
            this.buttonDrawOutlines = new System.Windows.Forms.Button();
            this.buttonGeneratePoints = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonStep
            // 
            this.buttonStep.Location = new System.Drawing.Point(756, 94);
            this.buttonStep.Name = "buttonStep";
            this.buttonStep.Size = new System.Drawing.Size(75, 41);
            this.buttonStep.TabIndex = 0;
            this.buttonStep.Text = "Step";
            this.buttonStep.UseVisualStyleBackColor = true;
            this.buttonStep.MouseClick += new System.Windows.Forms.MouseEventHandler(this.buttonStep_MouseClick);
            // 
            // buttonFullRun
            // 
            this.buttonFullRun.Location = new System.Drawing.Point(867, 94);
            this.buttonFullRun.Name = "buttonFullRun";
            this.buttonFullRun.Size = new System.Drawing.Size(75, 41);
            this.buttonFullRun.TabIndex = 1;
            this.buttonFullRun.Text = "Full Run";
            this.buttonFullRun.UseVisualStyleBackColor = true;
            this.buttonFullRun.MouseClick += new System.Windows.Forms.MouseEventHandler(this.buttonFullRun_MouseClick);
            // 
            // textBoxPrinting
            // 
            this.textBoxPrinting.Location = new System.Drawing.Point(756, 250);
            this.textBoxPrinting.Multiline = true;
            this.textBoxPrinting.Name = "textBoxPrinting";
            this.textBoxPrinting.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxPrinting.Size = new System.Drawing.Size(412, 402);
            this.textBoxPrinting.TabIndex = 2;
            // 
            // buttonDrawNeurons
            // 
            this.buttonDrawNeurons.Location = new System.Drawing.Point(756, 159);
            this.buttonDrawNeurons.Name = "buttonDrawNeurons";
            this.buttonDrawNeurons.Size = new System.Drawing.Size(75, 42);
            this.buttonDrawNeurons.TabIndex = 3;
            this.buttonDrawNeurons.Text = "Draw Neurons";
            this.buttonDrawNeurons.UseVisualStyleBackColor = true;
            this.buttonDrawNeurons.Click += new System.EventHandler(this.buttonDrawNeurons_Click);
            // 
            // buttonSOMFullRun
            // 
            this.buttonSOMFullRun.Location = new System.Drawing.Point(867, 159);
            this.buttonSOMFullRun.Name = "buttonSOMFullRun";
            this.buttonSOMFullRun.Size = new System.Drawing.Size(75, 42);
            this.buttonSOMFullRun.TabIndex = 4;
            this.buttonSOMFullRun.Text = "SOM Full Run";
            this.buttonSOMFullRun.UseVisualStyleBackColor = true;
            this.buttonSOMFullRun.Click += new System.EventHandler(this.buttonSOMFullRun_Click);
            // 
            // buttonDrawOutlines
            // 
            this.buttonDrawOutlines.Location = new System.Drawing.Point(983, 94);
            this.buttonDrawOutlines.Name = "buttonDrawOutlines";
            this.buttonDrawOutlines.Size = new System.Drawing.Size(75, 41);
            this.buttonDrawOutlines.TabIndex = 5;
            this.buttonDrawOutlines.Text = "Draw Outlines";
            this.buttonDrawOutlines.UseVisualStyleBackColor = true;
            this.buttonDrawOutlines.Click += new System.EventHandler(this.buttonDrawOutlines_Click);
            // 
            // buttonGeneratePoints
            // 
            this.buttonGeneratePoints.Location = new System.Drawing.Point(756, 36);
            this.buttonGeneratePoints.Name = "buttonGeneratePoints";
            this.buttonGeneratePoints.Size = new System.Drawing.Size(75, 38);
            this.buttonGeneratePoints.TabIndex = 6;
            this.buttonGeneratePoints.Text = "Generate Points";
            this.buttonGeneratePoints.UseVisualStyleBackColor = true;
            this.buttonGeneratePoints.Click += new System.EventHandler(this.buttonGeneratePoints_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(1186, 711);
            this.Controls.Add(this.buttonGeneratePoints);
            this.Controls.Add(this.buttonDrawOutlines);
            this.Controls.Add(this.buttonSOMFullRun);
            this.Controls.Add(this.buttonDrawNeurons);
            this.Controls.Add(this.textBoxPrinting);
            this.Controls.Add(this.buttonFullRun);
            this.Controls.Add(this.buttonStep);
            this.Name = "MainForm";
            this.Text = "Main Form";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button buttonStep;
        private Button buttonFullRun;
        private TextBox textBoxPrinting;
        private Button buttonDrawNeurons;
        private Button buttonSOMFullRun;
        private Button buttonDrawOutlines;
        private Button buttonGeneratePoints;
    }
}
namespace DiscordRPCTool
{
    partial class Form2
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
            this.checkBoxStartUp = new System.Windows.Forms.CheckBox();
            this.textboxAPIKey = new System.Windows.Forms.TextBox();
            this.labelapi = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textboxUserName = new System.Windows.Forms.TextBox();
            this.comboBoxCatImage = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBoxStartUp
            // 
            this.checkBoxStartUp.AutoSize = true;
            this.checkBoxStartUp.Location = new System.Drawing.Point(12, 170);
            this.checkBoxStartUp.Name = "checkBoxStartUp";
            this.checkBoxStartUp.Size = new System.Drawing.Size(112, 16);
            this.checkBoxStartUp.TabIndex = 0;
            this.checkBoxStartUp.Text = "起動時に実行する";
            this.checkBoxStartUp.UseVisualStyleBackColor = true;
            // 
            // textboxAPIKey
            // 
            this.textboxAPIKey.Location = new System.Drawing.Point(122, 33);
            this.textboxAPIKey.Name = "textboxAPIKey";
            this.textboxAPIKey.Size = new System.Drawing.Size(250, 19);
            this.textboxAPIKey.TabIndex = 1;
            // 
            // labelapi
            // 
            this.labelapi.AutoSize = true;
            this.labelapi.Location = new System.Drawing.Point(13, 36);
            this.labelapi.Name = "labelapi";
            this.labelapi.Size = new System.Drawing.Size(91, 12);
            this.labelapi.TabIndex = 2;
            this.labelapi.Text = "Last.fm API Key ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Last.fm User Name";
            // 
            // textboxUserName
            // 
            this.textboxUserName.Location = new System.Drawing.Point(122, 59);
            this.textboxUserName.Name = "textboxUserName";
            this.textboxUserName.Size = new System.Drawing.Size(250, 19);
            this.textboxUserName.TabIndex = 4;
            // 
            // comboBoxCatImage
            // 
            this.comboBoxCatImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCatImage.FormattingEnabled = true;
            this.comboBoxCatImage.Items.AddRange(new object[] {
            "Default Black Cat",
            "Headphone Cat",
            "Dancing Cat"});
            this.comboBoxCatImage.Location = new System.Drawing.Point(122, 116);
            this.comboBoxCatImage.Name = "comboBoxCatImage";
            this.comboBoxCatImage.Size = new System.Drawing.Size(150, 20);
            this.comboBoxCatImage.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "Cat Image";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(257, 226);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "保存する";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxCatImage);
            this.Controls.Add(this.textboxUserName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelapi);
            this.Controls.Add(this.textboxAPIKey);
            this.Controls.Add(this.checkBoxStartUp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form2";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxStartUp;
        private System.Windows.Forms.TextBox textboxAPIKey;
        private System.Windows.Forms.Label labelapi;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textboxUserName;
        private System.Windows.Forms.ComboBox comboBoxCatImage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}
namespace FaceDemo
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.realtimeBtn = new System.Windows.Forms.Button();
            this.pictureBtn = new System.Windows.Forms.Button();
            this.userIdentity = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.videoImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.videoImage)).BeginInit();
            this.SuspendLayout();
            // 
            // realtimeBtn
            // 
            this.realtimeBtn.Location = new System.Drawing.Point(448, 1086);
            this.realtimeBtn.Name = "realtimeBtn";
            this.realtimeBtn.Size = new System.Drawing.Size(169, 39);
            this.realtimeBtn.TabIndex = 1;
            this.realtimeBtn.Text = "实时抽取";
            this.realtimeBtn.UseVisualStyleBackColor = true;
            this.realtimeBtn.Click += new System.EventHandler(this.OnShotExtractClick);
            // 
            // pictureBtn
            // 
            this.pictureBtn.Location = new System.Drawing.Point(623, 1086);
            this.pictureBtn.Name = "pictureBtn";
            this.pictureBtn.Size = new System.Drawing.Size(169, 39);
            this.pictureBtn.TabIndex = 1;
            this.pictureBtn.Text = "照片抽取";
            this.pictureBtn.UseVisualStyleBackColor = true;
            this.pictureBtn.Click += new System.EventHandler(this.OnImageExtractClick);
            // 
            // userIdentity
            // 
            this.userIdentity.Location = new System.Drawing.Point(98, 1091);
            this.userIdentity.Name = "userIdentity";
            this.userIdentity.Size = new System.Drawing.Size(338, 28);
            this.userIdentity.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 1096);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 18);
            this.label1.TabIndex = 3;
            this.label1.Text = "用户标识";
            // 
            // videoImage
            // 
            this.videoImage.Location = new System.Drawing.Point(15, 12);
            this.videoImage.Name = "videoImage";
            this.videoImage.Size = new System.Drawing.Size(1380, 1068);
            this.videoImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.videoImage.TabIndex = 4;
            this.videoImage.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1407, 1137);
            this.Controls.Add(this.videoImage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.userIdentity);
            this.Controls.Add(this.pictureBtn);
            this.Controls.Add(this.realtimeBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "人脸识别测试程序";
            ((System.ComponentModel.ISupportInitialize)(this.videoImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button realtimeBtn;
        private System.Windows.Forms.Button pictureBtn;
        private System.Windows.Forms.TextBox userIdentity;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox videoImage;
    }
}


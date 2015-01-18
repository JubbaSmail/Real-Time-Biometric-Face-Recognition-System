namespace Real_Time_Biometric_Face_Recognition_System
{
    partial class MainFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
            this.pic_Screen = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btn_Shutdown = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pic_color_bar = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.rich_Screen = new System.Windows.Forms.RichTextBox();
            this.btn_Ref = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_mesh = new System.Windows.Forms.ComboBox();
            this.checkBox_last = new System.Windows.Forms.CheckBox();
            this.btnSnapshot = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.combo_Sys_out = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Reg = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btn_Run = new System.Windows.Forms.Button();
            this.txt_modelName = new System.Windows.Forms.TextBox();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.pic_Camera_Tick = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Screen)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_color_bar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Camera_Tick)).BeginInit();
            this.SuspendLayout();
            // 
            // pic_Screen
            // 
            this.pic_Screen.BackColor = System.Drawing.Color.White;
            this.pic_Screen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pic_Screen.Image = global::Real_Time_Biometric_Face_Recognition_System.Properties.Resources.win;
            this.pic_Screen.Location = new System.Drawing.Point(7, 4);
            this.pic_Screen.Name = "pic_Screen";
            this.pic_Screen.Size = new System.Drawing.Size(380, 306);
            this.pic_Screen.TabIndex = 4;
            this.pic_Screen.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btn_Shutdown
            // 
            this.btn_Shutdown.BackgroundImage = global::Real_Time_Biometric_Face_Recognition_System.Properties.Resources.shdwn1;
            this.btn_Shutdown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Shutdown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Shutdown.Enabled = false;
            this.btn_Shutdown.Location = new System.Drawing.Point(518, 254);
            this.btn_Shutdown.Name = "btn_Shutdown";
            this.btn_Shutdown.Size = new System.Drawing.Size(46, 44);
            this.btn_Shutdown.TabIndex = 5;
            this.btn_Shutdown.UseVisualStyleBackColor = true;
            this.btn_Shutdown.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.pic_color_bar);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.rich_Screen);
            this.panel1.Controls.Add(this.btn_Ref);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.pic_Screen);
            this.panel1.Controls.Add(this.btn_Shutdown);
            this.panel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.panel1.Location = new System.Drawing.Point(114, 103);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(570, 315);
            this.panel1.TabIndex = 6;
            this.panel1.Visible = false;
            // 
            // pic_color_bar
            // 
            this.pic_color_bar.Location = new System.Drawing.Point(394, 5);
            this.pic_color_bar.Name = "pic_color_bar";
            this.pic_color_bar.Size = new System.Drawing.Size(25, 300);
            this.pic_color_bar.TabIndex = 10;
            this.pic_color_bar.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::Real_Time_Biometric_Face_Recognition_System.Properties.Resources._lock;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(428, 255);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(33, 44);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // rich_Screen
            // 
            this.rich_Screen.BackColor = System.Drawing.Color.Black;
            this.rich_Screen.Cursor = System.Windows.Forms.Cursors.Default;
            this.rich_Screen.Font = new System.Drawing.Font("BatangChe", 8F);
            this.rich_Screen.ForeColor = System.Drawing.Color.White;
            this.rich_Screen.Location = new System.Drawing.Point(7, 4);
            this.rich_Screen.Name = "rich_Screen";
            this.rich_Screen.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.rich_Screen.Size = new System.Drawing.Size(382, 306);
            this.rich_Screen.TabIndex = 8;
            this.rich_Screen.Text = "";
            // 
            // btn_Ref
            // 
            this.btn_Ref.BackgroundImage = global::Real_Time_Biometric_Face_Recognition_System.Properties.Resources.refresh;
            this.btn_Ref.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_Ref.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Ref.Enabled = false;
            this.btn_Ref.Location = new System.Drawing.Point(466, 253);
            this.btn_Ref.Name = "btn_Ref";
            this.btn_Ref.Size = new System.Drawing.Size(46, 44);
            this.btn_Ref.TabIndex = 7;
            this.btn_Ref.UseVisualStyleBackColor = true;
            this.btn_Ref.Click += new System.EventHandler(this.btn_Ref_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBox_mesh);
            this.groupBox1.Controls.Add(this.checkBox_last);
            this.groupBox1.Controls.Add(this.btnSnapshot);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.combo_Sys_out);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btn_Reg);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.btn_Run);
            this.groupBox1.Controls.Add(this.txt_modelName);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(426, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(138, 244);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Control Buttons";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Mesh:";
            // 
            // comboBox_mesh
            // 
            this.comboBox_mesh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.comboBox_mesh.FormattingEnabled = true;
            this.comboBox_mesh.Items.AddRange(new object[] {
            "ExtriMesh",
            "EntraMesh",
            "HybridMesh"});
            this.comboBox_mesh.Location = new System.Drawing.Point(51, 91);
            this.comboBox_mesh.Name = "comboBox_mesh";
            this.comboBox_mesh.Size = new System.Drawing.Size(81, 21);
            this.comboBox_mesh.TabIndex = 22;
            this.comboBox_mesh.Text = "HybridMesh";
            // 
            // checkBox_last
            // 
            this.checkBox_last.AutoSize = true;
            this.checkBox_last.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBox_last.Location = new System.Drawing.Point(68, 62);
            this.checkBox_last.Name = "checkBox_last";
            this.checkBox_last.Size = new System.Drawing.Size(73, 17);
            this.checkBox_last.TabIndex = 20;
            this.checkBox_last.Text = "SimuMesh";
            this.checkBox_last.UseVisualStyleBackColor = true;
            this.checkBox_last.CheckedChanged += new System.EventHandler(this.checkBox_last_CheckedChanged);
            // 
            // btnSnapshot
            // 
            this.btnSnapshot.BackgroundImage = global::Real_Time_Biometric_Face_Recognition_System.Properties.Resources.snapshot1;
            this.btnSnapshot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSnapshot.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSnapshot.Location = new System.Drawing.Point(14, 16);
            this.btnSnapshot.Name = "btnSnapshot";
            this.btnSnapshot.Size = new System.Drawing.Size(46, 44);
            this.btnSnapshot.TabIndex = 11;
            this.btnSnapshot.UseVisualStyleBackColor = true;
            this.btnSnapshot.Click += new System.EventHandler(this.Snapshot_Background);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(18, 62);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(49, 17);
            this.checkBox1.TabIndex = 18;
            this.checkBox1.Text = "Auto";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 221);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Output:";
            // 
            // combo_Sys_out
            // 
            this.combo_Sys_out.Cursor = System.Windows.Forms.Cursors.Hand;
            this.combo_Sys_out.FormattingEnabled = true;
            this.combo_Sys_out.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.combo_Sys_out.Location = new System.Drawing.Point(70, 218);
            this.combo_Sys_out.Name = "combo_Sys_out";
            this.combo_Sys_out.Size = new System.Drawing.Size(55, 21);
            this.combo_Sys_out.TabIndex = 16;
            this.combo_Sys_out.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 166);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Person Name:";
            // 
            // btn_Reg
            // 
            this.btn_Reg.BackgroundImage = global::Real_Time_Biometric_Face_Recognition_System.Properties.Resources.reg;
            this.btn_Reg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Reg.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Reg.Enabled = false;
            this.btn_Reg.Location = new System.Drawing.Point(80, 118);
            this.btn_Reg.Name = "btn_Reg";
            this.btn_Reg.Size = new System.Drawing.Size(46, 44);
            this.btn_Reg.TabIndex = 14;
            this.btn_Reg.UseVisualStyleBackColor = true;
            this.btn_Reg.Click += new System.EventHandler(this.Capture_Vedio);
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = global::Real_Time_Biometric_Face_Recognition_System.Properties.Resources.save;
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.Location = new System.Drawing.Point(14, 118);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(46, 44);
            this.btnSave.TabIndex = 13;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.Save_Result);
            // 
            // btn_Run
            // 
            this.btn_Run.BackgroundImage = global::Real_Time_Biometric_Face_Recognition_System.Properties.Resources.play;
            this.btn_Run.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Run.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Run.Location = new System.Drawing.Point(80, 16);
            this.btn_Run.Name = "btn_Run";
            this.btn_Run.Size = new System.Drawing.Size(46, 44);
            this.btn_Run.TabIndex = 12;
            this.btn_Run.UseVisualStyleBackColor = true;
            this.btn_Run.Click += new System.EventHandler(this.Run_System);
            // 
            // txt_modelName
            // 
            this.txt_modelName.Location = new System.Drawing.Point(6, 185);
            this.txt_modelName.Name = "txt_modelName";
            this.txt_modelName.Size = new System.Drawing.Size(126, 20);
            this.txt_modelName.TabIndex = 10;
            // 
            // timer2
            // 
            this.timer2.Interval = 2000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // pic_Camera_Tick
            // 
            this.pic_Camera_Tick.Image = global::Real_Time_Biometric_Face_Recognition_System.Properties.Resources.camera;
            this.pic_Camera_Tick.Location = new System.Drawing.Point(349, 427);
            this.pic_Camera_Tick.Name = "pic_Camera_Tick";
            this.pic_Camera_Tick.Size = new System.Drawing.Size(116, 116);
            this.pic_Camera_Tick.TabIndex = 7;
            this.pic_Camera_Tick.TabStop = false;
            this.pic_Camera_Tick.Visible = false;
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Real_Time_Biometric_Face_Recognition_System.Properties.Resources.welcom_window;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.pic_Camera_Tick);
            this.Controls.Add(this.panel1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AIU_CV - Real Time Biometric Face Recognition System";
            this.Load += new System.EventHandler(this.MainFrm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Screen)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_color_bar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Camera_Tick)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pic_Screen;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btn_Shutdown;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Reg;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btn_Run;
        private System.Windows.Forms.Button btnSnapshot;
        public System.Windows.Forms.TextBox txt_modelName;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Button btn_Ref;
        private System.Windows.Forms.RichTextBox rich_Screen;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox combo_Sys_out;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pic_color_bar;
        private System.Windows.Forms.PictureBox pic_Camera_Tick;
        private System.Windows.Forms.CheckBox checkBox_last;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_mesh;
        
    }
}
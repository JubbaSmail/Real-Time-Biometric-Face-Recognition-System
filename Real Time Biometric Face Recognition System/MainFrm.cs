using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Media;
using System.Drawing.Drawing2D;
using Real_Time_Biometric_Face_Recognition_System.Properties;
using SpeechLib; //referring SpeechLib dll


namespace Real_Time_Biometric_Face_Recognition_System
{
    delegate void dely();
    public partial class MainFrm : Form
    {
        private bool g_Sys_Running = false;

        Thread thread1;
        Size scr_size;
        Size Cmr_rsln;
        int counter = 0, timer_counter = 0;
        Image test;
        static string HR_DB = "HR_DB";
        //----------------
        SpeechLib.SpVoice flvoice = new SpeechLib.SpVoice();
        //------------------------------------
        public MainFrm()
        {
            InitializeComponent();
            scr_size = pic_Screen.Size;
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer_counter++;
            if (timer_counter == 1)
            {
                txt_modelName.Text = "Booting...";
                Point temp_loc = this.Location;
                this.BackgroundImageLayout = ImageLayout.Tile;
                this.BackgroundImage = Resources.bck1;
                panel1.Visible = true;
                panel1.Refresh();
                string[] bbb = File.ReadAllLines("boot.ini");
                List<string> booting = new List<string>();
                for (int i = 0; i < bbb.Length; i++)
                {
                    booting.Add(bbb[i]);
                }
                while (booting.Count > 23)
                {
                    rich_Screen.Clear();
                    rich_Screen.Lines = booting.GetRange(0, 23).ToArray();
                    booting.RemoveRange(0, 2);
                    rich_Screen.Refresh();
                    Thread.Sleep(5);
                }
            }
            else if (timer_counter == 2)
            {
                rich_Screen.Dispose();
            }
            else
            {
                timer2.Stop();
                speek("welcome");
                txt_modelName.Text = "Camera Initialization...";
                pic_Screen.Image = Resources.loading1;
                thread1 = new Thread(new ThreadStart(Loading));
                thread1.Start();
            }
        }

        private void Loading()
        {
            Thread.Sleep(1000);
            dely d2 = new dely(Loading_b);
            this.Invoke(d2);
        }

        private void Loading_b()
        {
            HR_DB = "HR_DB";
            c_Image_acquisition.Start(10, this.Handle);
            test = c_Image_acquisition.m_Capture();
            //60 ms to capture;
            if (test != null)
            {
                if (c_Face_Detection.m_Load_Model())
                {
                    c_AIU_CV_Engine_Manager.m_Load_Faces(HR_DB);
                    Cmr_rsln = new Size(test.Width, test.Height);
                    groupBox1.Enabled = true;
                    Application.Idle += ProcessFrame;
                    sound.Stream = Resources.cameraclick_02;
                    txt_modelName.Text = "Ready";
                }
                else
                {
                    txt_modelName.Text = "No models found..";
                    this.Refresh();
                    Thread.Sleep(200);
                    speek("No models found");
                }
            }
            else
            {
                panel1.Visible = true;
                pic_Screen.Image = Resources.disc_camera;
                txt_modelName.Text = "No camera found...";
                this.Refresh();
                Thread.Sleep(200);
                speek("No camera found");
            }
            btn_Ref.Enabled = true;
            btn_Shutdown.Enabled = true;
        }

        private void Run_System(object sender, EventArgs e)
        {
            if (g_Sys_Running)
            {
                btn_Run.BackgroundImage = Resources.play;
                pic_color_bar.Image = null;
                g_Sys_Running = false;
            }
            else
            {
                if (c_AIU_CV_Engine_Manager.m_Load_Faces(HR_DB))
                {
                    btn_Run.BackgroundImage = Resources.Pause;
                    c_Image_acquisition.m_Snapshot_Background();
                    g_Sys_Running = true;
                }
                else
                {
                    txt_modelName.Text = "No persons found...";
                    Thread.Sleep(200);
                    speek("No persons found");
                }
            }
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            counter++;
            if (counter % 10 == 0)
            {
                if (counter % 20 == 0)
                {
                    pic_Camera_Tick.Visible = true;
                    counter = 0;
                }
                else
                {
                    pic_Camera_Tick.Visible = false;
                }
            }
            if (g_Sys_Running)
            {
                int t1 = DateTime.Now.Hour * 3600 * 1000 +
                DateTime.Now.Minute * 60 * 1000 +
                DateTime.Now.Second * 1000 +
                DateTime.Now.Millisecond;
                Bitmap[] res = c_AIU_CV_Engine_Manager.m_Process
                    (combo_Sys_out.Text, comboBox_mesh.Text);
                pic_Screen.Image = m_ResizeImage(res[0], scr_size);
                pic_color_bar.Image = res[1];

                int t2 = DateTime.Now.Hour * 3600 * 1000 +
               DateTime.Now.Minute * 60 * 1000 +
               DateTime.Now.Second * 1000 +
               DateTime.Now.Millisecond;
                txt_modelName.Text = (t2 - t1).ToString() + " MS";
            }
            else
                pic_Screen.Image = m_ResizeImage(c_Image_acquisition.m_Capture(), scr_size);
        }

        private void Snapshot_Background(object sender, EventArgs e)
        {
            txt_modelName.Text = "Snapshoting...";
            c_Image_acquisition.m_Snapshot_Background();
            btn_Reg.Enabled = true;
            txt_modelName.Text = "Ready";
        }

        private void Save_Result(object sender, EventArgs e)
        {
            Directory.CreateDirectory("images");
            string date = "images\\" + getslashedTime();
            pic_Screen.Image.Save(date + "-img.bmp");
        }

        private string getslashedTime()
        {
            DateTime x = DateTime.Now;
            return x.Year.ToString() + "-" + x.Month.ToString() + "-" + x.Day.ToString()
                + "-" + x.Hour.ToString() + "-" + x.Minute.ToString() + "-" + x.Second.ToString();
        }
        //------------------------
        int number_of_Snapshots_for_user = 10;
        public SoundPlayer sound = new SoundPlayer();

        public void m_Register(string model_name)
        {
            string HR_DB = "HR_DB";
            if(checkBox_last.Checked)
                HR_DB = "HR_DB_";
            Directory.CreateDirectory(HR_DB);
            int frame_counter = 0, loop = 0,path_counter = 1;
            FileStream fs = null;
            BinaryWriter wr = null;
            if (model_name.Length > 0)
            {
                while (frame_counter < number_of_Snapshots_for_user &&
                    loop < 50)
                {
                    loop++;
                    s_Face dtctd_fc = new s_Face();
                    pic_Screen.Image = m_ResizeImage(
                        c_AIU_CV_Engine_Manager.m_Register_steps(ref dtctd_fc), scr_size);
                    if (dtctd_fc.Eye1 != Point.Empty &&
                    dtctd_fc.Eye1.X >= dtctd_fc.x &&
                    dtctd_fc.Eye2 != Point.Empty &&
                    dtctd_fc.Eye2.X >= dtctd_fc.x &&
                        dtctd_fc.mesh_mouht_points != null &&
               dtctd_fc.mesh_right_brow_points != null &&
               dtctd_fc.mesh_left_brow_points != null &&
               dtctd_fc.mesh_nose_point != Point.Empty &&
                        loop > 3)
                    {
                        loop = 0;
                        
                        frame_counter++;
                        while (
                        File.Exists(HR_DB + "//" + model_name + "_" + path_counter + ".xml"))
                            path_counter++;
                        fs = new FileStream
                            (HR_DB + "//" + model_name + "_" + path_counter + ".xml",
                            FileMode.Create);
                        wr = new BinaryWriter(fs);
                        if (dtctd_fc.mesh != null)
                        {
                            wr.Write(dtctd_fc.mesh);
                            wr.Write(dtctd_fc.Right_face_shape);
                            wr.Write(dtctd_fc.Left_face_shape);
                            wr.Write(dtctd_fc.hair_Signal);
                            wr.Write(dtctd_fc.Right_brow_shape);
                            wr.Write(dtctd_fc.Left_brow_shape);
                            wr.Write(dtctd_fc.nose_shape);
                            wr.Write(dtctd_fc.mouth_shape);
                            //8 Signals
                            sound.Stop();
                            sound.Play();
                        }
                        if (fs.Length == 0)
                        {
                            wr.Close();
                            fs.Close();
                            File.Delete(HR_DB + "//" + model_name + "_" + frame_counter + ".mdl");
                        }
                        wr.Close();
                        fs.Close();
                    }
                }
                if (loop == 50 && frame_counter == 0)
                {
                    speek("No Person Saved");
                    txt_modelName.Text = "No Person Saved.";
                }
                else if (loop == 50 && frame_counter != 0)
                {
                    speek("Time out");
                    txt_modelName.Text = "Time out";
                }
                else
                {
                    Thread.Sleep(1000);
                    speek("Nice to meet you " + txt_modelName.Text);
                    txt_modelName.Text = txt_modelName.Text + " Saved successfully.";
                    c_AIU_CV_Engine_Manager.m_Load_Faces("HR_DB_");
                }
            }
            else
            {
                txt_modelName.Text = "Please Enter Person Name.";
            }
            btn_Reg.Enabled = false;
        }

        private void Capture_Vedio(object sender, EventArgs e)
        {
            Application.Idle -= ProcessFrame;
            m_Register(txt_modelName.Text);
            Application.Idle += ProcessFrame;
        }
        //------------------------
        private void button1_Click(object sender, EventArgs e)
        {
            speek("good bye");
            this.Visible = false;
            this.Close();
        }

        private void speek(string strMessage)
        {
            if (strMessage.Trim() != null && strMessage.Trim() != "")
                flvoice.Speak(strMessage, SpeechVoiceSpeakFlags.SVSFDefault);
        }

        private Image m_ResizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

        private void btn_Ref_Click(object sender, EventArgs e)
        {
            btn_Shutdown.Enabled = false;
            thread1.Abort();
            Application.Idle -= ProcessFrame;
            txt_modelName.Text = "";
            pic_Screen.Image = Resources.loading1;
            pic_Screen.Refresh();
            if (!thread1.IsAlive)
            {
                thread1 = new Thread(new ThreadStart(Loading));
                thread1.Start();
            }
        }

        private void checkBox_last_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_last.Checked)
                HR_DB = "HR_DB_";
            else
                HR_DB = "HR_DB";
            c_AIU_CV_Engine_Manager.m_Load_Faces(HR_DB);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            string[] all_files = Directory.GetFiles("HR_DB_");
            string[] temp;
            for (int i = 0; i < all_files.Length; i++)
            {
                temp = all_files[i].Split('\\');
                Directory.Move(all_files[i], "HR_DB\\" + temp[temp.Length - 1]);
            }
        }
    }
}
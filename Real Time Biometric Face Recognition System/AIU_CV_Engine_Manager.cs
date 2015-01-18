using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Media;
using System.Windows.Forms;
using SpeechLib; //referring SpeechLib dll

namespace Real_Time_Biometric_Face_Recognition_System
{
    class c_AIU_CV_Engine_Manager
    {
        public static int Signal_Length = 100;

        static Image g_current_frame;
        static byte[,] g_current_frame_byte;

        static Image pictureBox1;
        static byte[,] moving_objects_byte;
        static Font myFont = new Font("Tahoma", 18, FontStyle.Bold);

        static int pen_width = 4;
        static Pen mypen = new Pen(Color.Orange, pen_width * 2);

        static double max_match = 0;
        static string max_match_s;
        static string max_match_name;
        static byte[,] bmp_mono;
        static int[] face_signal;
        static s_Face dtctd_fc;
        static List<Point> g_points = new List<Point>();
        static Bitmap[] final_res = new Bitmap[2];
        static bool door = true;
        static SpeechLib.SpVoice flvoice = new SpeechLib.SpVoice();
        static void speek(string strMessage)
        {
            if (strMessage.Trim() != null && strMessage.Trim() != "")
                flvoice.Speak(strMessage, SpeechVoiceSpeakFlags.SVSFDefault);
        }
        //-----------------------------------------------
        public static Bitmap[] m_Process(string scrn_nm, string mesh_type)
        {
            mypen.Color = Color.Red;
            g_current_frame = c_Image_acquisition.m_Capture();
            g_current_frame_byte = c_Preprocessing.m_Bitmap_to_2DArray(g_current_frame);
            //-------------------------------------------
            Size frame_size = g_current_frame.Size;
            moving_objects_byte = c_Preprocessing.m_Extract_Motion_Objects
            (c_Image_acquisition.g_bckGrnd_colored_byte, g_current_frame_byte);
            moving_objects_byte = c_Preprocessing.m_Noise_Reduction(moving_objects_byte);
            //-------------------------------------------
            bmp_mono = c_Feature_extraction.m_Hole_Filling(moving_objects_byte);
            face_signal = c_Feature_extraction.m_Face_To_Signal_RL(bmp_mono);

            dtctd_fc = c_Feature_extraction.m_Split_Head_From_Shoulder(face_signal);

            if (dtctd_fc.Width < dtctd_fc.Height)
            {
                dtctd_fc = c_Face_Detection.m_Detect_Face(dtctd_fc);

                if (dtctd_fc.Name != string.Empty && dtctd_fc.Name.Length > 0 &&
                pictureBox1 != null)
                {
                    dtctd_fc = c_High_level_processing.m_face_analysis
                        (g_current_frame_byte, bmp_mono, dtctd_fc);
                    //------------------------------
                    dtctd_fc = m_Recogntion(dtctd_fc, mesh_type);
                    if (dtctd_fc.Percent > 0 && dtctd_fc.Name != string.Empty &&
                        dtctd_fc.Name.Length > 0)
                    {
                        //-------------------------------
                        final_res[1] = m_Draw_Color_Bar((int)max_match);
                        mypen.Color = pen_bar.Color;
                        m_Drawer_Screen(scrn_nm, frame_size);
                        pictureBox1 = m_Draw_Face(pictureBox1, dtctd_fc);
                        if (max_match < dtctd_fc.Percent && dtctd_fc.Name.Length > 0)
                        {
                            max_match = dtctd_fc.Percent;
                            max_match_name = dtctd_fc.Name;
                        }
                        int seek_name = 0;
                        if (dtctd_fc.y + dtctd_fc.Height + 90 > frame_size.Height)
                        {
                            seek_name = frame_size.Height - 90;
                        }
                        else
                            seek_name = dtctd_fc.y + dtctd_fc.Height + 40;
                        max_match_s = dtctd_fc.Name + " " + max_match.ToString() + "%";
                        Graphics.FromImage(pictureBox1).DrawString(max_match_s,
                        myFont, Brushes.Red, dtctd_fc.x + (dtctd_fc.Width / 4)
                        , seek_name);
                        if (dtctd_fc.SecondName.Length > 0)
                        {
                            Graphics.FromImage(pictureBox1).DrawString(
                                dtctd_fc.SecondName + " " + 
                                dtctd_fc.SecondPercent.ToString() + "%",
                                myFont, Brushes.Red, dtctd_fc.x + (dtctd_fc.Width / 4)
                                , seek_name + 30);
                        }
                        if (dtctd_fc.Eye1 != Point.Empty &&
                            dtctd_fc.Eye1.X >= dtctd_fc.x &&
                            dtctd_fc.Eye2 != Point.Empty &&
                            dtctd_fc.Eye2.X >= dtctd_fc.x)
                        {
                            m_Drawer_Details(dtctd_fc);
                        }
                        if (door && max_match > 80)
                        {
                            speek("Welcom " + max_match_name);
                            door = false;
                        }
                    }
                    else
                    {
                        pictureBox1 = g_current_frame;
                        max_match = 0;
                        final_res[1] = m_Draw_Color_Bar(0);
                        max_match_s = "";
                        door = true;
                        mypen.Color = pen_bar.Color;
                        m_Drawer_Screen(scrn_nm, frame_size);
                        pictureBox1 = m_Draw_Face(pictureBox1, dtctd_fc);
                    }
                }
                else
                {
                    pictureBox1 = g_current_frame;
                    max_match = 0;
                    final_res[1] = m_Draw_Color_Bar(0);
                    max_match_s = "";
                    door = true;
                }
            }
            else
            {
                pictureBox1 = g_current_frame;
                max_match = 0;
                final_res[1] = m_Draw_Color_Bar(0);
                max_match_s = "";
                door = true;
            }
            final_res[0] = (Bitmap)pictureBox1;

            return final_res;
        }
        //-------------------------------------
        static List<byte[]> person_mesh_signals = new List<byte[]>();//1
        static List<byte[]> person_face_signals = new List<byte[]>();//2
        static List<byte[]> person_Right_Brow_signals = new List<byte[]>();//1
        static List<byte[]> person_Left_Brow_signals = new List<byte[]>();//1
        static List<byte[]> person_nose_signals = new List<byte[]>();//1
        static List<byte[]> person_mouth_signals = new List<byte[]>();//1
        static List<byte[]> person_hair_signals = new List<byte[]>();//1

        static List<string> persons_name_list = new List<string>();
        static List<double> percents_list;
        static List<double> percent_value_mesh_list;
        static List<double> percent_value_r_face_list;
        static List<double> percent_value_l_face_list;
        static List<double> percent_value_r_brow_list;
        static List<double> percent_value_l_brow_list;
        static List<double> percent_value_nose_list;
        static List<double> percent_value_mouth_list;
        static List<double> percent_value_hair_list;

        static int error_counter_mesh;
        static int error_counter_r_face;
        static int error_counter_l_face;
        static int error_counter_hair;
        static int error_counter_r_brow;
        static int error_counter_l_brow;
        static int error_counter_nose;
        static int error_counter_mouth;

        static int error_mesh = 25;
        static int error_r_face = 5;
        static int error_l_face = 5;
        static int error_hair = 20;
        static int error_r_brow = 20;
        static int error_l_brow = 20;
        static int error_nose = 15;
        static int error_mouth = 15;


        static int max_percent_index;
        static double max_percent;
        static int secondmax_percent_index;
        static double secondmax_percent;
        static double percent_value_mesh,
            percent_value_r_face, percent_value_l_face, final_percent,
            percent_value_r_brow, percent_value_l_brow,
            percent_value_nose, percent_value_mouth,
            percent_value_hair;

        public static s_Face m_Recogntion(s_Face dtctd_fc, string mesh_type)
        {
            if (dtctd_fc.mesh == null)
            {
                dtctd_fc.Percent = 10;
                dtctd_fc.Name = "";
                return dtctd_fc;
            }
            max_percent = 0;
            percents_list = new List<double>();
            percent_value_mesh_list = new List<double>();
            percent_value_r_face_list = new List<double>();
            percent_value_l_face_list = new List<double>();
            percent_value_r_brow_list = new List<double>();
            percent_value_l_brow_list = new List<double>();
            percent_value_nose_list = new List<double>();
            percent_value_mouth_list = new List<double>();
            percent_value_hair_list = new List<double>();


            if (dtctd_fc.mesh == null || dtctd_fc.mesh.Length == 0)
                return new s_Face();
            error_counter_mesh = 0;
            error_counter_r_face = 0;
            error_counter_l_face = 0;
            for (int i = 0; i < person_mesh_signals.Count; i++)
            {
                error_counter_mesh = 0;
                for (int j = 0; j < dtctd_fc.mesh.Length; j++)
                {
                    if (Math.Abs(dtctd_fc.mesh[j] - person_mesh_signals[i][j]) > error_mesh)
                    {
                        error_counter_mesh++;
                    }
                }
                percent_value_mesh = Math.Round(100 - (float)(error_counter_mesh * 100) /
                    (float)dtctd_fc.mesh.Length, 1);
                percent_value_mesh_list.Add(percent_value_mesh);

                if (mesh_type == "ExtriMesh")
                {
                    error_counter_r_face = 0;
                    for (int j = 0; j < dtctd_fc.Right_face_shape.Length; j++)
                    {
                        if (Math.Abs(dtctd_fc.Right_face_shape[j]
                            - person_face_signals[i][j]) > error_r_face)
                        {
                            error_counter_r_face++;
                        }
                    }
                    percent_value_r_face = Math.Round(100 - (float)(error_counter_r_face * 100) /
                    (float)100, 1);

                    error_counter_l_face = 0;
                    for (int j = 100; j < 200; j++)
                    {
                        if (Math.Abs(dtctd_fc.Left_face_shape[j - 100]
                            - person_face_signals[i][j]) > error_l_face)
                        {
                            error_counter_l_face++;
                        }
                    }
                    percent_value_l_face = Math.Round(100 - (float)(error_counter_l_face * 100) /
                    (float)100, 1);

                    final_percent = /*(percent_value_mesh * 0.8) +*/ (percent_value_r_face * 0.5) +
                    (percent_value_l_face * 0.5);
                    percents_list.Add(final_percent);
                }
                else if (mesh_type == "EntraMesh")
                {
                    percents_list.Add(percent_value_mesh);
                }
                else if (mesh_type == "HybridMesh")
                {
                    //2
                    error_counter_r_face = 0;
                    for (int j = 0; j < dtctd_fc.Right_face_shape.Length; j++)
                    {
                        if (Math.Abs(dtctd_fc.Right_face_shape[j] -
                            person_face_signals[i][j]) > error_r_face)
                        {
                            error_counter_r_face++;
                        }
                    }
                    percent_value_r_face = Math.Round(100 - (float)(error_counter_r_face * 100) /
                    (float)100, 1);
                    percent_value_r_face_list.Add(percent_value_r_face);
                    //-------------------------------------
                    //3
                    error_counter_l_face = 0;
                    for (int j = 100; j < 200; j++)
                    {
                        if (Math.Abs(dtctd_fc.Left_face_shape[j - 100]
                            - person_face_signals[i][j]) > error_l_face)
                        {
                            error_counter_l_face++;
                        }
                    }
                    percent_value_l_face = Math.Round(100 - (float)(error_counter_l_face * 100) /
                    (float)100, 1);
                    percent_value_l_face_list.Add(percent_value_l_face);
                    //-------------------------------------
                    //4
                    error_counter_hair = 0;
                    for (int j = 0; j < dtctd_fc.hair_Signal.Length; j++)
                    {
                        if (Math.Abs(dtctd_fc.hair_Signal[j] -
                            person_hair_signals[i][j]) > error_hair)
                        {
                            error_counter_hair++;
                        }
                    }
                    percent_value_hair = Math.Round(100 - (float)(error_counter_hair * 100) /
                    (float)100, 1);
                    percent_value_hair_list.Add(percent_value_hair);
                    //-------------------------------------
                    //5
                    error_counter_r_brow = 0;
                    for (int j = 0; j < dtctd_fc.Right_brow_shape.Length; j++)
                    {
                        if (Math.Abs(dtctd_fc.Right_brow_shape[j]
                            - person_Right_Brow_signals[i][j]) > error_r_brow)
                        {
                            error_counter_r_brow++;
                        }
                    }
                    percent_value_r_brow = Math.Round(100 - (float)(error_counter_r_brow * 100) /
                    (float)100, 1);
                    percent_value_r_brow_list.Add(percent_value_r_brow);
                    //-------------------------------------
                    //6
                    error_counter_l_brow = 0;
                    for (int j = 0; j < dtctd_fc.Left_brow_shape.Length; j++)
                    {
                        if (Math.Abs(dtctd_fc.Left_brow_shape[j]
                            - person_Left_Brow_signals[i][j]) > error_l_brow)
                        {
                            error_counter_l_brow++;
                        }
                    }
                    percent_value_l_brow = Math.Round(100 - (float)(error_counter_l_brow * 100) /
                    (float)100, 1);
                    percent_value_l_brow_list.Add(percent_value_l_brow);
                    //-------------------------------------
                    //7
                    error_counter_nose = 0;
                    for (int j = 0; dtctd_fc.nose_shape != null &&
                        j < dtctd_fc.nose_shape.Length; j++)
                    {
                        if (Math.Abs(dtctd_fc.nose_shape[j]
                            - person_nose_signals[i][j]) > error_nose)
                        {
                            error_counter_nose++;
                        }
                    }
                    percent_value_nose = Math.Round(100 - (float)(error_counter_nose * 100) /
                    (float)100, 1);
                    if (dtctd_fc.nose_shape != null &&
                        dtctd_fc.nose_shape.Length > 0)
                        percent_value_nose_list.Add(percent_value_nose);
                    //-------------------------------------
                    //8
                    error_counter_mouth = 0;
                    for (int j = 0; j < dtctd_fc.mouth_shape.Length; j++)
                    {
                        if (Math.Abs(dtctd_fc.mouth_shape[j]
                            - person_mouth_signals[i][j]) > error_mouth)
                        {
                            error_counter_mouth++;
                        }
                    }
                    percent_value_mouth = Math.Round(100 - (float)(error_counter_mouth * 100) /
                    (float)100, 1);
                    if (dtctd_fc.mouth_shape.Length > 0)
                        percent_value_mouth_list.Add(percent_value_mouth);
                    //-------------------------------------
                    final_percent = (percent_value_mesh * 0.2) + (percent_value_r_face * 0.1) +
                    (percent_value_l_face * 0.1) + (percent_value_hair * 0.2) +
                    (percent_value_r_brow * 0.1) + (percent_value_l_brow * 0.1) +
                    (percent_value_nose * 0.1) + (percent_value_mouth * 0.1);

                    percents_list.Add(final_percent);
                }
            }
            max_percent = percents_list[0];

            for (int i = 1; i < percents_list.Count; i++)
            {
                if (max_percent < percents_list[i])
                {

                    max_percent = percents_list[i];
                    max_percent_index = i;
                }
            }
            secondmax_percent = 0;
            for (int i = 1; i < percents_list.Count; i++)
            {
                if (secondmax_percent < percents_list[i] &&
                    persons_name_list[i] !=
                    persons_name_list[max_percent_index])
                {
                    secondmax_percent = percents_list[i];
                    secondmax_percent_index = i;
                }
            }
            if (max_percent >= 0)
            {
                dtctd_fc.Name = persons_name_list[max_percent_index];
                if (max_percent > 75 && max_percent < 90)
                    max_percent += 10;
                dtctd_fc.Percent = max_percent;
                if (secondmax_percent > 30)
                    secondmax_percent -= 20;
                if (secondmax_percent != 0)
                {
                    dtctd_fc.SecondName = persons_name_list[secondmax_percent_index];
                    dtctd_fc.SecondPercent = secondmax_percent;
                }
                else
                {
                    dtctd_fc.SecondName = "";
                    dtctd_fc.SecondPercent = 0;
                }
                if (percent_value_r_brow_list.Count > 0 &&
                    percent_value_nose_list.Count == percent_value_r_brow_list.Count &&
                    percent_value_mouth_list.Count > 0)
                {
                    dtctd_fc.percent_value_mesh = percent_value_mesh_list[max_percent_index];
                    dtctd_fc.percent_value_r_brow = percent_value_r_brow_list[max_percent_index];
                    dtctd_fc.percent_value_l_brow = percent_value_l_brow_list[max_percent_index];
                    dtctd_fc.percent_value_hair = percent_value_hair_list[max_percent_index];
                    dtctd_fc.percent_value_nose = percent_value_nose_list[max_percent_index];
                    dtctd_fc.percent_value_mouth = percent_value_mouth_list[max_percent_index];
                    dtctd_fc.percent_value_r_face = percent_value_r_face_list[max_percent_index];
                    dtctd_fc.percent_value_l_face = percent_value_l_face_list[max_percent_index];
                }
            }
            else
            {
                dtctd_fc.Name = string.Empty;
            }
            return dtctd_fc;
        }
        //--------------------------------
        static string[] all_persons;
        static FileStream fs;
        static BinaryReader wr;
        static string[] thename;
        public static bool m_Load_Faces(string path)
        {
            person_mesh_signals.Clear();
            person_face_signals.Clear();
            person_hair_signals.Clear();
            person_Right_Brow_signals.Clear();
            person_Left_Brow_signals.Clear();
            person_nose_signals.Clear();
            person_mouth_signals.Clear();
            persons_name_list.Clear();

            all_persons = Directory.GetFiles(path);
            for (int i = 0; i < all_persons.Length; i++)
            {
                fs = new FileStream(all_persons[i], FileMode.Open);
                wr = new BinaryReader(fs);
                person_mesh_signals.Add(wr.ReadBytes(15000));
                person_face_signals.Add(wr.ReadBytes(200));
                person_hair_signals.Add(wr.ReadBytes(100));
                person_Right_Brow_signals.Add(wr.ReadBytes(100));
                person_Left_Brow_signals.Add(wr.ReadBytes(100));
                person_nose_signals.Add(wr.ReadBytes(100));
                person_mouth_signals.Add(wr.ReadBytes(100));

                thename = all_persons[i].Split('\\');
                persons_name_list.Add(thename[thename.Length - 1].Split('_')[0]);
                wr.Close();
                fs.Close();
            }
            return person_mesh_signals.Count > 0;
        }
        public static void m_Drawer_Details(s_Face dtctd_fc)
        {
            if (dtctd_fc.mesh_mouht_points != null &&
                dtctd_fc.mesh_right_brow_points != null &&
                dtctd_fc.mesh_left_brow_points != null &&
                dtctd_fc.mesh_nose_point != Point.Empty)
            {
                Graphics.FromImage(pictureBox1).DrawRectangle(Pens.Red,
                            dtctd_fc.Eye1.X, dtctd_fc.Eye1.Y, dtctd_fc.Eye_size.Width,
                        dtctd_fc.Eye_size.Height);

                Graphics.FromImage(pictureBox1).DrawRectangle(Pens.Red,
                    dtctd_fc.Eye2.X, dtctd_fc.Eye2.Y, dtctd_fc.Eye_size.Width,
                dtctd_fc.Eye_size.Height);

                Graphics.FromImage(pictureBox1).FillEllipse(Brushes.Red,
                    dtctd_fc.mesh_nose_point.X,
                    dtctd_fc.mesh_nose_point.Y, 10, 10);
                //--------------------------------------
                Graphics.FromImage(pictureBox1).FillEllipse(Brushes.Red,
                    dtctd_fc.mesh_mouht_points[0].X,
                    dtctd_fc.mesh_mouht_points[0].Y, 10, 10);
                Graphics.FromImage(pictureBox1).FillEllipse(Brushes.Red,
                    dtctd_fc.mesh_mouht_points[1].X,
                    dtctd_fc.mesh_mouht_points[1].Y, 10, 10);
                Graphics.FromImage(pictureBox1).FillEllipse(Brushes.Red,
                    dtctd_fc.mesh_mouht_points[2].X,
                    dtctd_fc.mesh_mouht_points[2].Y, 10, 10);
                //--------------------------------------
                Graphics.FromImage(pictureBox1).FillEllipse(Brushes.Red,
                   dtctd_fc.mesh_right_brow_points[0].X,
                   dtctd_fc.mesh_right_brow_points[0].Y, 10, 10);
                Graphics.FromImage(pictureBox1).FillEllipse(Brushes.Red,
                    dtctd_fc.mesh_right_brow_points[1].X,
                    dtctd_fc.mesh_right_brow_points[1].Y, 10, 10);
                Graphics.FromImage(pictureBox1).FillEllipse(Brushes.Red,
                    dtctd_fc.mesh_right_brow_points[2].X,
                    dtctd_fc.mesh_right_brow_points[2].Y, 10, 10);
                //--------------------------------------
                Graphics.FromImage(pictureBox1).FillEllipse(Brushes.Red,
                   dtctd_fc.mesh_left_brow_points[0].X,
                   dtctd_fc.mesh_left_brow_points[0].Y, 10, 10);
                Graphics.FromImage(pictureBox1).FillEllipse(Brushes.Red,
                    dtctd_fc.mesh_left_brow_points[1].X,
                    dtctd_fc.mesh_left_brow_points[1].Y, 10, 10);
                Graphics.FromImage(pictureBox1).FillEllipse(Brushes.Red,
                    dtctd_fc.mesh_left_brow_points[2].X,
                    dtctd_fc.mesh_left_brow_points[2].Y, 10, 10);
                //--------------------------------------
                //Graphics.FromImage(pictureBox1).DrawRectangle(Pens.Red,dtctd_fc.rec_nose);
                //Graphics.FromImage(pictureBox1).DrawRectangle(Pens.Red, dtctd_fc.rec_mouht);
                //Graphics.FromImage(pictureBox1).DrawRectangle(Pens.Red, dtctd_fc.rec_brow_r);
                //Graphics.FromImage(pictureBox1).DrawRectangle(Pens.Red, dtctd_fc.rec_brow_l);
            }
        }
        public static void m_Drawer_Screen(string scrn_nm, Size frame_size)
        {
            if (scrn_nm == "1")
                pictureBox1 = g_current_frame;
            else if (scrn_nm == "2")
                pictureBox1 = c_Preprocessing.m_2DArray_to_Bitmap(bmp_mono);
            else if (scrn_nm == "3")
            {
                pictureBox1 = new Bitmap(frame_size.Width, frame_size.Height);
                if (dtctd_fc.face_Signal != null && dtctd_fc.face_Signal.Length > 0)
                {
                    for (int i = 0; i < dtctd_fc.face_Signal.Length; i++)
                    {
                        g_points.Add
                            (new Point(i * 2, frame_size.Height - dtctd_fc.face_Signal[i]));
                    }
                    if (g_points.Count > 1)
                    {
                        Graphics.FromImage(pictureBox1).DrawLines(Pens.Red, g_points.ToArray());
                    }
                    g_points.Clear();

                    byte[] scaled_temp = c_Preprocessing.m_Scale(face_signal);

                    for (int i = 0; i < scaled_temp.Length; i++)
                    {
                        g_points.Add(new Point((i + dtctd_fc.face_Signal.Length) * 3 + 100
                            , frame_size.Height - scaled_temp[i]));
                    }
                    if (g_points.Count > 1)
                    {
                        Graphics.FromImage(pictureBox1).DrawLines(Pens.Green, g_points.ToArray());
                    }
                    g_points.Clear();
                }
            }
            else if (scrn_nm == "4")
            {
                pictureBox1 = new Bitmap(frame_size.Width, frame_size.Height);
                if (dtctd_fc.hair_Signal != null && dtctd_fc.hair_Signal.Length > 0)
                {
                    for (int i = 0; i < dtctd_fc.hair_Signal.Length; i++)
                    {
                        g_points.Add
                            (new Point(i * 2, frame_size.Height - dtctd_fc.hair_Signal[i]));
                    }
                    if (g_points.Count > 1)
                    {
                        Graphics.FromImage(pictureBox1).DrawLines(Pens.Red, g_points.ToArray());
                    }
                    g_points.Clear();
                }
            }
            else if (scrn_nm == "5")
            {
                pictureBox1 = new Bitmap(frame_size.Width, frame_size.Height);
                if (dtctd_fc.percent_value_hair != 0)
                {
                    Graphics.FromImage(pictureBox1).DrawString
                        ("percent_value_mesh: " + dtctd_fc.percent_value_mesh.ToString(),
                        myFont, Brushes.Red, 10, 10);

                    Graphics.FromImage(pictureBox1).DrawString
                        ("percent_value_r_face: " + dtctd_fc.percent_value_r_face.ToString(),
                        myFont, Brushes.Red, 10, 50);

                    Graphics.FromImage(pictureBox1).DrawString
                        ("percent_value_l_face: " + dtctd_fc.percent_value_l_face.ToString(),
                        myFont, Brushes.Red, 10, 90);

                    Graphics.FromImage(pictureBox1).DrawString
                        ("percent_value_hair: " + dtctd_fc.percent_value_hair.ToString(),
                        myFont, Brushes.Red, 10, 130);

                    Graphics.FromImage(pictureBox1).DrawString
                        ("percent_value_r_brow: " + dtctd_fc.percent_value_r_brow.ToString(),
                        myFont, Brushes.Red, 10, 170);

                    Graphics.FromImage(pictureBox1).DrawString
                        ("percent_value_l_brow: " + dtctd_fc.percent_value_l_brow.ToString(),
                        myFont, Brushes.Red, 10, 210);

                    Graphics.FromImage(pictureBox1).DrawString
                        ("percent_value_nose: " + dtctd_fc.percent_value_nose.ToString(),
                        myFont, Brushes.Red, 10, 250);

                    Graphics.FromImage(pictureBox1).DrawString
                        ("percent_value_mouth: " + dtctd_fc.percent_value_mouth.ToString(),
                        myFont, Brushes.Red, 10, 290);
                }
            }
        }
        //-----------------------------
        public static Bitmap m_Register_steps(ref s_Face dtctd_fc)
        {
            g_current_frame = c_Image_acquisition.m_Capture();

            g_current_frame_byte = c_Preprocessing.m_Bitmap_to_2DArray(g_current_frame);
            //-------------------------------------------
            Size frame_size = g_current_frame.Size;
            moving_objects_byte = c_Preprocessing.m_Extract_Motion_Objects
            (c_Image_acquisition.g_bckGrnd_colored_byte, g_current_frame_byte);
            moving_objects_byte = c_Preprocessing.m_Noise_Reduction(moving_objects_byte);
            //----------------------
            bmp_mono = c_Feature_extraction.m_Hole_Filling(moving_objects_byte);
            face_signal = c_Feature_extraction.m_Face_To_Signal_RL(bmp_mono);

            dtctd_fc = c_Feature_extraction.m_Split_Head_From_Shoulder(face_signal);

            dtctd_fc = c_Face_Detection.m_Detect_Face(dtctd_fc);

            if (dtctd_fc.Name != string.Empty && pictureBox1 != null/*&&
                (dtctd_fc.width < dtctd_fc.height)*/
                                                    )
            {
                dtctd_fc = c_High_level_processing.m_face_analysis
                    (g_current_frame_byte, bmp_mono, dtctd_fc);

                mypen.Color = Color.FromArgb(0, 255, 0);
                pictureBox1 = g_current_frame;
                pictureBox1 = m_Draw_Face(pictureBox1, dtctd_fc);

                if (dtctd_fc.Eye1 != Point.Empty &&
                    dtctd_fc.Eye1.X >= dtctd_fc.x &&
                    dtctd_fc.Eye2 != Point.Empty &&
                    dtctd_fc.Eye2.X >= dtctd_fc.x)
                {
                    m_Drawer_Details(dtctd_fc);
                }
            }
            else
            {
                pictureBox1 = g_current_frame;
                max_match = 0;
                max_match_s = "";
            }
            return (Bitmap)pictureBox1;
        }
        //-----------------------------
        static int longy = 50;
        static Image m_Draw_Face(Image pictureBox1, s_Face dtctd_fc)
        {
            if (dtctd_fc.x - pen_width > 0 &&
                dtctd_fc.Width + pen_width < pictureBox1.Width &&
                dtctd_fc.y + dtctd_fc.Height + 15 < pictureBox1.Height)
            {
                Graphics.FromImage(pictureBox1).DrawLine(mypen,
                    dtctd_fc.x - pen_width,
                    dtctd_fc.y,
                    dtctd_fc.x,
                    dtctd_fc.y + longy);
                Graphics.FromImage(pictureBox1).DrawLine(mypen,
                    dtctd_fc.x,
                    dtctd_fc.y,
                    dtctd_fc.x + longy,
                    dtctd_fc.y);
                //--------------------------------------------------------
                Graphics.FromImage(pictureBox1).DrawLine(mypen,
                    dtctd_fc.x +
                dtctd_fc.Width + pen_width,
                    dtctd_fc.y,
                    dtctd_fc.x +
                dtctd_fc.Width,
                    dtctd_fc.y + longy);
                Graphics.FromImage(pictureBox1).DrawLine(mypen,
                    dtctd_fc.x +
                dtctd_fc.Width,
                    dtctd_fc.y,
                    dtctd_fc.x +
                    dtctd_fc.Width - longy,
                    dtctd_fc.y);
                //--------------------------------------------------------
                Graphics.FromImage(pictureBox1).DrawLine(mypen,
                    dtctd_fc.x +
                    dtctd_fc.Width + pen_width,
                    dtctd_fc.y + dtctd_fc.Height + 15,
                    dtctd_fc.x + dtctd_fc.Width,
                    dtctd_fc.y + dtctd_fc.Height + 15 - longy);
                Graphics.FromImage(pictureBox1).DrawLine(mypen,
                    dtctd_fc.x + dtctd_fc.Width,
                    dtctd_fc.y + dtctd_fc.Height + 15,
                   dtctd_fc.x + dtctd_fc.Width - longy,
                    dtctd_fc.y + dtctd_fc.Height + 15);
                //--------------------------------------------------------
                Graphics.FromImage(pictureBox1).DrawLine(mypen,
                    dtctd_fc.x - pen_width,
                    dtctd_fc.y + dtctd_fc.Height + 15,
                    dtctd_fc.x,
                    dtctd_fc.y + dtctd_fc.Height + 15 - longy);
                Graphics.FromImage(pictureBox1).DrawLine(mypen,
                    dtctd_fc.x,
                    dtctd_fc.y + dtctd_fc.Height + 15,
                    dtctd_fc.x + longy,
                    dtctd_fc.y + dtctd_fc.Height + 15);
                //--------------------------------------------------------

                Graphics.FromImage(pictureBox1).DrawEllipse(mypen,
                    dtctd_fc.x - 4,
                    dtctd_fc.y,
                    8, 8);

                Graphics.FromImage(pictureBox1).DrawEllipse(mypen,
                    dtctd_fc.x +
                    dtctd_fc.Width - 4,
                    dtctd_fc.y,
                    8, 8);

                Graphics.FromImage(pictureBox1).DrawEllipse(mypen,
                    dtctd_fc.x - 4,
                    dtctd_fc.y +
                dtctd_fc.Height + 6,
                    8, 8);

                Graphics.FromImage(pictureBox1).DrawEllipse(mypen,
                    dtctd_fc.x +
                    dtctd_fc.Width - 4,
                    dtctd_fc.y +
                dtctd_fc.Height + 6,
                    8, 8);
            }
            /*else if(pictureBox1 !=null)
                Graphics.FromImage(pictureBox1).DrawRectangle(Pens.Red,
                    dtctd_fc.x,
                    dtctd_fc.y,
                    dtctd_fc.Width,
                    dtctd_fc.Height + 15);*/
            return pictureBox1;
        }
        //-----------------------------
        static int bar_width = 25;
        static int bar_height = 300;
        static Pen pen_bar = new Pen(Color.FromArgb(255, 0, 0));
        static private Bitmap m_Draw_Color_Bar(int precent)
        {
            if (precent > 100)
                precent = 100;
            Bitmap color_bar = new Bitmap(bar_width, bar_height);
            Graphics.FromImage(color_bar).DrawRectangle(Pens.Black,
                0, 0, bar_width - 1, bar_height - 1);
            int red = 250, green = 0;
            int step = bar_height / 100;
            int grow_step = 3;
            pen_bar.Color = Color.FromArgb(255, 0, 0);
            for (int i = 0; i < precent; i++)
            {
                if (red > 255)
                    red = 255;
                if (green > 255)
                    green = 255;
                if (red < 0)
                    red = 0;
                pen_bar.Color = Color.FromArgb(red, green, 0);
                for (int j = 1; j <= step; j++)
                {
                    Graphics.FromImage(color_bar).DrawLine
                        (pen_bar, 0, bar_height - grow_step + j, bar_width - 1,
                        bar_height - grow_step + j);
                }
                grow_step += step;
                if (red >= 250 && green <= 220)
                    green += 4;
                else if (green < 250 & green > 220)
                    green += 6;
                else if (green >= 250 && red >= 10)
                    red -= 10;
            }
            return color_bar;
        }
    }
}
/*if (combo_Sys_out.Text == "1")
                    pic_Screen.Image = m_ResizeImage(c_Preprocessing.pictureBox1, mysize);
                else if (combo_Sys_out.Text == "2")
                    pic_Screen.Image = m_ResizeImage(c_Preprocessing.pictureBox2, mysize);
                else if (combo_Sys_out.Text == "3")
                    pic_Screen.Image = m_ResizeImage(c_Preprocessing.pictureBox3, mysize);
                else if (combo_Sys_out.Text == "4")
                    pic_Screen.Image = m_ResizeImage(c_Preprocessing.pictureBox4, mysize);*/

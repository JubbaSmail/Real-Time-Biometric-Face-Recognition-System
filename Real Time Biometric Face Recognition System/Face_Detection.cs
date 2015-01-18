using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace Real_Time_Biometric_Face_Recognition_System
{
    class c_Face_Detection
    {
        static List<byte[]> model_lines = new List<byte[]>();
        static List<string> model_name = new List<string>();
        static List<double> percent;
        static byte[] Scaled_Signal;
        static int error_counter;
        static double max_percent;
        public static s_Face m_Detect_Face(s_Face dtctd_fc)
        {
            if (dtctd_fc.face_Signal == null)
                return dtctd_fc;
            percent = new List<double>();
            Scaled_Signal = dtctd_fc.face_Signal;
            if (Scaled_Signal == null || Scaled_Signal.Length == 0)
                return new s_Face();
            
            error_counter = 0;
            for (int i = 0; i < model_lines.Count; i++)
            {
                error_counter = 0;
                for (int j = 0; j < Scaled_Signal.Length; j++)
                {
                    if (Math.Abs(Scaled_Signal[j] - model_lines[i][j]) > 3)
                    {
                        error_counter++;
                    }
                }
                percent.Add(Math.Round(100 - (float)(error_counter * 100) /
                    (float)Scaled_Signal.Length, 1));
            }
            max_percent = percent[0];
            for (int i = 1; i < percent.Count; i++)
            {
                if (max_percent < percent[i])
                    max_percent = percent[i];
            }
            if (max_percent >= 5 &&
               dtctd_fc.Width > 0 && dtctd_fc.Height > 0)
            {
                dtctd_fc.Name = model_name[percent.IndexOf(max_percent)];
                dtctd_fc.Percent = max_percent;
            }
            else
            {
                dtctd_fc.Name = string.Empty;
            }
            return dtctd_fc;
        }
        //--------------------------------
        static string[] all_models;
        static FileStream fs;
        static BinaryReader wr;
        static string[] thename;
        public static bool m_Load_Model()
        {
            all_models = Directory.GetFiles("Models_DB");
            byte[] model_item;
            for (int i = 0; i < all_models.Length; i++)
            {
                fs = new FileStream(all_models[i], FileMode.Open);
                wr = new BinaryReader(fs);
                model_item = wr.ReadBytes(c_AIU_CV_Engine_Manager.Signal_Length);
                model_lines.Add(model_item);
                thename = all_models[i].Split('\\');
                model_name.Add(thename[thename.Length - 1].Split('_')[0]);
                wr.Close();
                fs.Close();
            }
            return model_lines.Count > 0;
        }
    }
}
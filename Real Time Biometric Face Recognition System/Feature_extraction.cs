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

namespace Real_Time_Biometric_Face_Recognition_System
{
    class c_Feature_extraction
    {
        static int g_head_Height = 0;
        static int g_head_Position_y = 0;
        static int g_head_Position_x = 0;
        static int g_head_width = 0;
        static List<int> g_lines = new List<int>();
        static List<int> g_lines_0sCounter = new List<int>();

        static int[] tophat = new int[32] 
        {-16,-15,-14,-13,-12,-11,-10,-9, -8, -7, -6, -5, -4, -3, -2, -1,
            1, 2, 3, 4, 5, 6, 7, 8,9,10,11,12,13,14,15,16 };
        //-----------------------------------------------
        public static byte[,] m_Hole_Filling(byte[,] pic)
        {
            if (pic == null)
                return null;
            bool firsthit;
            int wIndex = 0, bIndex = 0;
            byte[,] temp = new byte[pic.GetLength(0), pic.GetLength(1)];
            for (int j = 0; j < pic.GetLength(1); j++)
            {
                firsthit = true;
                wIndex = 0; bIndex = 0;
                for (int i = 0; i < pic.GetLength(0); i++)
                {
                    if (firsthit && pic[i, j] == 0)
                    {
                        firsthit = false;
                        wIndex = i;
                    }
                    else if (!firsthit && pic[i, j] == 0)
                    {
                        bIndex = i;
                    }
                    else
                        temp[i, j] = 255;
                    /*if (j > 1 && j < pic.GetLength(1) - 1 && i > 4 && i < pic.GetLength(0) - 4 &&
                        firsthit && pic[i, j] == 0 && pic[i - 1, j] == 0 &&
                        pic[i - 2, j] == 0 && pic[i - 3, j] == 0 && pic[i - 4, j] == 0&&
                        pic[i + 1, j] == 0 &&
                        pic[i + 2, j] == 0 && pic[i + 3, j] == 0 && pic[i + 4, j] == 0 &&
                        pic[i - 1, j - 1] == 0 && pic[i + 1, j + 1] == 0 &&
                        pic[i - 1, j + 1] == 0 && pic[i + 1, j - 1] == 0)
                    {
                        firsthit = false;
                        wIndex = i;
                    }
                    else if (j > 1 && j < pic.GetLength(1) - 1 && i > 4 && i < pic.GetLength(0) - 4
                        && !firsthit && pic[i, j] == 0 && pic[i - 1, j] == 0 &&
                        pic[i - 2, j] == 0 && pic[i - 3, j] == 0 && pic[i - 4, j] == 0 &&
                        pic[i + 1, j] == 0 &&
                        pic[i + 2, j] == 0 && pic[i + 3, j] == 0 && pic[i + 4, j] == 0 &&
                        pic[i - 1, j - 1] == 0 && pic[i + 1, j + 1] == 0 &&
                        pic[i - 1, j + 1] == 0 && pic[i + 1, j - 1] == 0)
                    {
                        bIndex = i;
                    }*/
                }
                for (int i = wIndex; i <= bIndex && bIndex > 0; i++)
                {
                    temp[i, j] = 0;
                }
            }
            return temp;
        }
        //--------------------------------
        static bool firsthit;
        public static int[] m_Face_To_Signal_RL(byte[,] pic)
        {
            if (pic == null)
                return null;
            g_lines.Clear();
            g_lines_0sCounter.Clear();
            int index = 0, wIndex = 0, bIndex = 0, pixel_counter = 0;
            bool firsthit;
            for (int j = 0; j < pic.GetLength(1); j++)
            {
                firsthit = true;
                wIndex = 0; bIndex = 0;
                for (int i = 0; i < pic.GetLength(0); i++)
                {
                    if (pic[i, j] == 0)
                    {
                        if (firsthit)
                        {
                            firsthit = false;
                            wIndex = i;
                        }
                        else if (!firsthit)
                        {
                            bIndex = i;
                        }
                    }
                }
                if (bIndex > wIndex)
                {
                    pixel_counter = (bIndex - wIndex);
                    g_lines.Add(pixel_counter);
                    g_lines_0sCounter.Add((wIndex - index));
                }
                else
                {
                    g_lines.Add(0);
                    g_lines_0sCounter.Add(0);
                }
            }
            return m_Translation_to_coordinates(g_lines);
        }

        public static void m_Face_To_Signal_UD(Bitmap pic, int p_bmp_width, int p_bmp_height)
        {
            /*byte[] bmp = m_extract_face(pic, p_bmp_width, p_bmp_height);
            if (bmp.Length > 0)
            {
                bool firsthit;
                int index = 0, wIndex = 0, bIndex = 0, height_counter = 0, pixel_counter = 0;
                int w_width = c_Feature_extraction.g_head_width * 3,
                w_height = c_Feature_extraction.g_cut_Line * 3;
                for (int i = 0; i < w_width; i += 3)
                {
                    index = i;
                    height_counter = 0;
                    firsthit = true;
                    wIndex = 0;
                    bIndex = 0;
                    do
                    {
                        if (firsthit && bmp[index] == 0)
                        {
                            firsthit = false;
                            wIndex = index;
                        }
                        else if (!firsthit && bmp[index] == 0)
                        {
                            bIndex = index;
                        }
                        index += w_width;
                        height_counter += 3;
                    } while (height_counter % (w_height - 3) != 0);
                    if (bIndex > wIndex)
                    {
                        pixel_counter = (bIndex - wIndex) / w_width;
                        g_lines.Add(pixel_counter);
                    }
                    else
                        g_lines.Add(0);
                }
            }*/
        }

        public static int[] m_Translation_to_coordinates(List<int> face_signal)
        {
            //Transfer to 0,0
            g_head_Position_y = 0;
            while (g_head_Position_y < face_signal.Count && face_signal[g_head_Position_y] == 0)
            {
                g_head_Position_y++;
            }
            if (g_head_Position_y == face_signal.Count)
            {
                g_head_Position_y = 0;
                face_signal.Clear();
            }
            else
                face_signal.RemoveRange(0, g_head_Position_y);
            return face_signal.ToArray();
        }
        //----------------------------
        public static s_Face m_Split_Head_From_Shoulder(int[] face_signal)
        {
            if (face_signal == null)
                return new s_Face();
            g_head_Height = 0;
            int min = int.MaxValue;
            int y1 = 0, y2 = 0;
            for (int i = (tophat.Length / 2) + (face_signal.Length / 4)
                ; i < face_signal.Length - (tophat.Length / 2); i++)
            {
                y1 = 0; y2 = 0;
                for (int j = 0; j < tophat.Length / 2 - 1; j++)
                {
                    y1 += face_signal[i + tophat[j]] - face_signal[i + tophat[j] + 1];
                }
                for (int j = tophat.Length / 2; j < tophat.Length - 1; j++)
                {
                    y2 += face_signal[i + tophat[j]] - face_signal[i + tophat[j] + 1];
                }
                if (y1 > -1 && y2 < -1)
                {
                    if (min > face_signal[i])
                    {
                        min = face_signal[i];
                        g_head_Height = i;
                    }
                }
            }
            if (g_head_Height == 0)
            {
                g_head_Height = face_signal.Length;
            }

            if (face_signal.Length > 0 && face_signal.Length - g_head_Height > 0)
            {
                g_head_width = 0;
                for (int i = 0; i < g_head_Height; i++)
                {
                    if (g_head_width < face_signal[i])
                    {
                        g_head_width = face_signal[i];
                        g_head_Position_x = g_lines_0sCounter[g_head_Position_y + i];
                    }
                }
            }
            //g_head_Height = (int)(g_head_width * 1.0 / 0.618);
            //golden ration on human face
            if (face_signal.Length > 0 && face_signal.Length >= g_head_Height)
            {
                if (g_head_width % 4 != 0)
                    g_head_width -= g_head_width % 4;
                int[] temp_Signal = new int[g_head_Height];
                for (int i = 0; i < temp_Signal.Length; i++)
                {
                    temp_Signal[i] = face_signal[i];
                }
                byte[] Scaled_Signal = c_Preprocessing.m_Scale(temp_Signal);
                return new s_Face(g_head_Position_x,g_head_Position_y,
                    g_head_width, g_head_Height, Scaled_Signal);
            }
            else
                return new s_Face();
        }
        //-----------------------------------------
    }
}
/*static List<Point> marker_point = new List<Point>();
       public static Point m_marker_point_selection
           (byte[,] wrkng_points, byte[,] moving_objects_b, Size frame_size)
       {
           //marker point X is the min value & Y is the max value
           bool first_tic = true;
           marker_point.Clear();
           int min = 0, max = 0;
           byte[] histogram = new byte[256];
           int ten_percent = (int)(0.1 * moving_objects_b.GetLength(0));
           for (int j = 0; marker_point.Count < 10 && j < moving_objects_b.GetLength(1); j += 4)
           {
               first_tic = true;
               for (int i = 0; i < moving_objects_b.GetLength(0); i++)
               {
                   if (moving_objects_b[i, j] == 0)
                   {
                       histogram[wrkng_points[i, j]]++;
                       if (first_tic)
                       {
                           first_tic = false;
                           min = max = wrkng_points[i, j];
                       }
                       else if (!first_tic)
                       {
                           if (min > wrkng_points[i, j])
                               min = wrkng_points[i, j];
                           if (max < wrkng_points[i, j])
                               max = wrkng_points[i, j];
                       }
                   }
               }
               if (min > 0 && max > 0)
               {
                   if (histogram[min] <= ten_percent)
                   {
                       while (min < max)
                       {
                           min++;
                           if (histogram[min] > ten_percent)
                               break;
                       }
                       if (histogram[min] < ten_percent)
                           min = 0;
                   }
                   if (histogram[max] <= ten_percent)
                   {
                       while (max > min)
                       {
                           max--;
                           if (histogram[max] > ten_percent)
                               break;
                       }
                       if (histogram[max] < ten_percent)
                           max = 0;
                   }

               }
               if (min > 0 && max > 0)
                   marker_point.Add
                           (new Point(min, max));
           }
           int avg_min = 0, avg_max = 0;
           for (int i = 0; i < marker_point.Count; i++)
           {
               avg_min += marker_point[i].X;
               avg_max += marker_point[i].Y;
           }
           if (marker_point.Count > 0)
           {
               avg_min /= marker_point.Count;
               avg_max /= marker_point.Count;
           }
           return new Point(avg_min, avg_max);
       }

       static List<c_My_Group> united_Groups = new List<c_My_Group>();
       static myGroup_Compare myComparable_group = new myGroup_Compare();
       static c_My_Group crnt_Group;
       static int[] indexI;
       static Size frm_sz;
       private static byte[,] m_Grouping(Size _frm_sz, int seek_error)
       {
           frm_sz = _frm_sz;
           united_Groups.Clear();
           indexI = new int[seek_error * 2 + 1];
           int index_Value = -seek_error;
           for (int i = 0; i < indexI.Length; i++)
           {
               indexI[i] = index_Value++;
           }
           for (int j = 0; j < moving_objects_b.GetLength(1); j++)
           {
               for (int i = 0; i < moving_objects_b.GetLength(0); i++)
               {
                   if (moving_objects_b[i, j] == 0)
                   {
                       crnt_Group = new c_My_Group();
                       c_My_point cancer_cell = new c_My_point(i, j);
                       crnt_Group.points.Add(cancer_cell);
                       moving_objects_b[i, j] = 255;
                       m_neighbourhood(cancer_cell);
                       if (crnt_Group.Count > 250)
                           united_Groups.Add(crnt_Group);
                   }
               }
           }
           united_Groups.Sort(myComparable_group);
           byte[,] result = new byte[frm_sz.Width, frm_sz.Height];
           for (int j = 0; j < moving_objects_b.GetLength(1); j++)
           {
               for (int i = 0; i < moving_objects_b.GetLength(0); i++)
               {
                   result[i, j] = 255;
               }
           }
           if (united_Groups.Count > 0)
           {
               for (int i = 0; i < united_Groups[0].Count; i++)
               {
                   result[united_Groups[0].points[i].x,
                       united_Groups[0].points[i].y] = 0;
               }
           }
           return result;
       }
       //--------------------------------
        
       private static void m_neighbourhood(c_My_point cancer_cell)
       {
           Stack<c_My_point> my_Stack = new Stack<c_My_point>();
           my_Stack.Push(cancer_cell);
           while (my_Stack.Count > 0)
           {
               cancer_cell = my_Stack.Pop();
               if (cancer_cell.x < frm_sz.Width - 1 && cancer_cell.x > 0
                   && cancer_cell.y < frm_sz.Height - 1 && cancer_cell.y > 0)
               {
                   for (int w = 0; w < indexI.Length; w++)
                   {
                       for (int q = 0; q < indexI.Length; q++)
                       {
                           if (moving_objects_b[cancer_cell.x + indexI[w],
                               cancer_cell.y + indexI[q]] == 0)
                           {
                               moving_objects_b[cancer_cell.x + indexI[w],
                                   cancer_cell.y + indexI[q]] = 255;

                               my_Stack.Push(new c_My_point(cancer_cell.x + indexI[w],
                                       cancer_cell.y + indexI[q]));
                               crnt_Group.points.Add(my_Stack.Peek());
                           }
                       }
                   }
               }
           }
       }*/
//----------------------------------------
/*public static Point[] m_marker_point_selection
    (byte[,] moving_objects_b, Size frame_size)
{
    bool first_tic = true;
    marker_point.Clear();
    int f_index, l_index;
    for (int j = 0; marker_point.Count < 10 && j < moving_objects_b.GetLength(1); j += 20)
    {
        first_tic = true;
        f_index = l_index = 0;
        for (int i = 0; i < moving_objects_b.GetLength(0); i++)
        {
            if (first_tic && moving_objects_b[i, j] == 0)
            {
                first_tic = false;
                f_index = i;
            }
            else if (!first_tic && moving_objects_b[i, j] == 0)
            {
                l_index = i;
            }
        }
        if (f_index != 0 && l_index != 0 && l_index - f_index >= 20)
        {
            marker_point.Add
                (new Point(f_index + 15, j));
            marker_point.Add
                (new Point(l_index - 15, j));
        }
    }
    return marker_point.ToArray();
}*/
/*static byte[,] wrkng_points;
    static byte[,] moving_objects_b;
    public static byte[] m_Hair_Stem_Extraction
        (Bitmap crt_frame, byte[] moving_objects, Size frame_size)
    {
        wrkng_points = m_1D_to_2D_array(
            c_Preprocessing.m_BmpToBytes_mono(crt_frame), frame_size.Width);

        moving_objects_b = m_1D_to_2D_array(moving_objects, frame_size.Width);

        return null;
        Point mrkr_point =
            m_marker_point_selection(wrkng_points,moving_objects_b, frame_size);
        moving_objects_b = m_threshold(mrkr_point, wrkng_points, moving_objects_b);
        Bitmap temp = (Bitmap)c_Preprocessing.m_Noise_Reduction_4X4
            (c_Preprocessing.m_BytesToBmp_mono(m_2D_to_1D_array(moving_objects_b),frame_size));

        moving_objects_b = m_1D_to_2D_array(
            c_Preprocessing.m_BmpToBytes_mono(temp), frame_size.Width);

        return m_2D_to_1D_array(m_Grouping(frame_size, 1));
    }*/
/*private static byte[,] m_threshold
           (Point threshold_point, byte[,] wrkng_points, byte[,] moving_objects_b)
       {
           for (int j = 0; j < wrkng_points.GetLength(1); j++)
           {
               for (int i = 0; i < wrkng_points.GetLength(0); i++)
               {
                   if (moving_objects_b[i, j] == 0)
                   {
                       if (wrkng_points[i, j] >= threshold_point.X &&
                           wrkng_points[i, j] <= threshold_point.Y)
                           moving_objects_b[i, j] = 255;
                   }
               }
           }
           return moving_objects_b;
       }*/
/*private static byte[,] m_1D_to_2D_array(byte[] pic, int width)
        {
            byte[,] result = new byte[width, pic.Length / width];
            int index = 0;
            for (int i = 0; i < result.GetLength(1); i++)
            {
                for (int j = 0; j < width; j++)
                {
                    result[j, i] = pic[index++];
                }
            }
            return result;
        }

        private static byte[] m_2D_to_1D_array(byte[,] pic)
        {
            byte[] result = new byte[pic.GetLength(0) * pic.GetLength(1)];
            int index = 0;
            for (int i = 0; i < pic.GetLength(1); i++)
            {
                for (int j = 0; j < pic.GetLength(0); j++)
                {
                    result[index++] = pic[j, i];
                }
            }
            return result;
        }*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Real_Time_Biometric_Face_Recognition_System
{
    class c_High_level_processing
    {
        static int eye_width, eye_height;
        static myPoint_Compare myComparable_point = new myPoint_Compare();
        static myGroup_Compare myComparable_group = new myGroup_Compare();
        static Point_Compare myComparable_Point = new Point_Compare();
        static int stack_counter;
        //--------------------------------------------------
        static Point old_eye1, old_eye2, old_face_position,
            old_eye_face_position1, old_eye_face_position2;
        static Size old_face_size, old_eye_size;
        public static s_Face m_face_analysis(byte[,] pic, byte[,] moving_object, s_Face dtctd_fc)
        {
            eyes_Location.Clear();
            int x = dtctd_fc.x;
            int y = dtctd_fc.y;
            int width = dtctd_fc.Width;
            int height = dtctd_fc.Height;
            if (width < height && width > 8)
            {
                byte[,] face_area = m_Crop_area(pic, x, y, ref width, height);
                byte[,] face_area_moving_object =
                    m_Crop_area(moving_object, x, y, ref width, height);
                
                //-----------------------------------------
                x = face_area.GetLength(0) / 6;
                y = face_area.GetLength(1) / 3;
                width = face_area.GetLength(0) - (face_area.GetLength(0) / 3);
                height = face_area.GetLength(1) / 3;

                byte[,] eyes_area = m_Crop_area(face_area, x, y, ref width, height);
                dtctd_fc = m_eye_detection
                    (dtctd_fc, eyes_area,
                    new Size(face_area.GetLength(0), face_area.GetLength(1)));
                //-----------------------------------
                if ((dtctd_fc.Eye1 == Point.Empty ||
                    dtctd_fc.Eye2 == Point.Empty ||
                    dtctd_fc.Eye1.X == 0 || dtctd_fc.Eye2.X == 0)
                    && old_eye1.X != 0 && old_eye2.X != 0
                    && Math.Abs(old_face_position.X - dtctd_fc.x) < 10
                    && Math.Abs(old_face_position.Y - dtctd_fc.y) < 10)
                {
                    dtctd_fc.Eye1 = old_eye1;
                    dtctd_fc.Eye2 = old_eye2;
                    dtctd_fc.Eye1_face_position = old_eye_face_position1;
                    dtctd_fc.Eye2_face_position = old_eye_face_position2;
                    dtctd_fc.Eye_size = old_eye_size;
                }
                Point p1 = new Point(dtctd_fc.Eye1_face_position.X,
                        dtctd_fc.Eye1_face_position.Y);
                Point p2 = new Point(dtctd_fc.Eye2_face_position.X,
                    dtctd_fc.Eye2_face_position.Y);

                if (dtctd_fc.Eye1 != Point.Empty &&
                    dtctd_fc.Eye2 != Point.Empty &&
                    dtctd_fc.Eye1.X != 0 && dtctd_fc.Eye1.Y != 0 &&
                    dtctd_fc.Eye2.X != 0 && dtctd_fc.Eye2.Y != 0
                    && p1.X > 0 && p1.Y > 0 && p2.X > 0 && p2.Y > 0)
                {
                    old_face_position = new Point(dtctd_fc.x, dtctd_fc.y);
                    old_face_size = new Size(dtctd_fc.Width, dtctd_fc.Height);
                    old_eye1 = dtctd_fc.Eye1;
                    old_eye2 = dtctd_fc.Eye2;
                    old_eye_face_position1 = dtctd_fc.Eye1_face_position;
                    old_eye_face_position2 = dtctd_fc.Eye2_face_position;
                    old_eye_size = dtctd_fc.Eye_size;

                    x = dtctd_fc.Eye1_face_position.X - dtctd_fc.Eye_size.Width/2;
                    y = dtctd_fc.Eye1_face_position.Y - 3 * dtctd_fc.Eye_size.Height;
                    width = dtctd_fc.Eye2_face_position.X +
                        dtctd_fc.Eye_size.Width - dtctd_fc.Eye1_face_position.X;
                    height = (int)(width * 1.5);
                       
                    if (x + width > dtctd_fc.Width)
                        width = dtctd_fc.Width - x;
                    if (y + height > dtctd_fc.Height)
                        height = dtctd_fc.Height - y;
                    if (width % 4 != 0)
                    {
                        width -= width % 4;
                    }
                    byte[,] mesh =
                    m_Crop_area(face_area, x, y, ref width, height);
                    if (mesh != null)
                    {
                        dtctd_fc.mesh =
                            c_Preprocessing.m_BmpToBytes_mono
                            ((Bitmap)m_ResizeImage(c_Preprocessing.m_2DArray_to_Bitmap(mesh)));
                    }
                    m_mouth_nose_brow_stuff
                        (ref dtctd_fc, face_area, face_area_moving_object, p1, p2);
                    //----------------------------------
                }
                else
                {
                    dtctd_fc.Eye1 = Point.Empty;
                    dtctd_fc.Eye2 = Point.Empty;
                    old_eye1 = Point.Empty;
                    old_eye2 = Point.Empty;
                }
                //Right face shape
                x = dtctd_fc.Width / 2;
                y = 0;
                width = dtctd_fc.Width / 2;
                height = dtctd_fc.Height;
                byte[,] Right_face_area_m_object = 
                    m_Crop_area(face_area_moving_object, x, y, ref width, height);
                dtctd_fc.Right_face_shape = m_extract_object_shape(Right_face_area_m_object);
                
                //-------------------------------------
                //Left face shape
                x = 0;
                y = 0;
                width = dtctd_fc.Width / 2;
                height = dtctd_fc.Height;

                byte[,] Left_face_area_m_object = 
                    m_Crop_area(face_area_moving_object, x, y, ref width, height);
                dtctd_fc.Left_face_shape = m_extract_object_shape(Left_face_area_m_object);
                //-------------------------------------
            }
            return dtctd_fc;
        }
        //--------------------------------------
        private static Image m_ResizeImage(Image imgToResize)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            /*nPercentW = ((float)size.Width / (float)sourceWidth);
            /nPercentH = ((float)size.Height / (float)sourceHeight);
            
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;*/

            int destWidth = 100;//(int)(sourceWidth * nPercent);
            int destHeight = 150;// (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }
        //------------------------------------
        static void m_mouth_nose_brow_stuff
            (ref s_Face dtctd_fc, byte[,] face_area, byte[,] face_area_moving_object,
            Point p1, Point p2)
        {
            int x = dtctd_fc.Eye1_face_position.X;
            int y = 0;
            int width = dtctd_fc.Eye2_face_position.X + dtctd_fc.Eye_size.Width
                - dtctd_fc.Eye1_face_position.X;
            int height = dtctd_fc.Height / 3;

            byte[,] hair_area = m_Crop_area(face_area, x, y, ref width, height);
            
            if (hair_area != null)
            {
                byte[,] hair_area_m_object = 
                    m_Crop_area(face_area_moving_object, x, y, ref width, height);
                
                dtctd_fc.hair_Signal = c_Preprocessing.m_Scale(
                    m_Hair_Stem_Extraction(hair_area, hair_area_m_object));
            }
            //----------------------------------
            //mouth_detection

            c_face_Organ mouth = m_get_mouth(p1, p2, face_area, 0.2);
            
            if (mouth != null && mouth.rectangle.Width != 0)
            {
                mouth.rectangle.X += dtctd_fc.x;
                mouth.rectangle.Y += dtctd_fc.y;

                dtctd_fc.rec_mouht = mouth.rectangle;
                mouth = m_oragn_Grouping(mouth, 1);
                dtctd_fc.mouth_shape = m_extract_object_shape(mouth.bitmap_Array);
                if (mouth.Mid_point.X > 0 && mouth.Mid_point.Y > 0)
                {
                    mouth.Mid_point.X += mouth.rectangle.X;
                    mouth.Mid_point.Y += mouth.rectangle.Y;

                    mouth.Left_point.X += mouth.rectangle.X;
                    mouth.Left_point.Y += mouth.rectangle.Y;

                    mouth.Right_point.X += mouth.rectangle.X;
                    mouth.Right_point.Y += mouth.rectangle.Y;

                    dtctd_fc.mesh_mouht_points = new Point[3] { mouth.Left_point, mouth.Mid_point, mouth.Right_point };
                }

            }
            //----------------------------------
            c_face_Organ nose = null;
            if (mouth != null)
                nose = m_get_nose(p1, p2, mouth.rectangle.Y - dtctd_fc.y, mouth.rectangle.Height, face_area, dtctd_fc.Eye_size, 0.5);
            if (nose != null && nose.rectangle.Width != 0)
            {
                nose.rectangle.X += dtctd_fc.x;
                nose.rectangle.Y += dtctd_fc.y;

                dtctd_fc.rec_nose = nose.rectangle;
                nose = m_oragn_Grouping(nose, 1);
                dtctd_fc.nose_shape = m_extract_object_shape(nose.bitmap_Array);
                if (nose.Mid_point.X > 0 && nose.Mid_point.Y > 0)
                {
                    nose.Mid_point.X += nose.rectangle.X;
                    nose.Mid_point.Y += nose.rectangle.Y;
                    dtctd_fc.mesh_nose_point = nose.Mid_point;
                }
            }
            //----------------------------------
            c_face_Organ eye_brow_r =
                m_get_R_eyebrow(p1, p2, face_area, dtctd_fc.Eye_size, 0.3);
            if (eye_brow_r != null && eye_brow_r.rectangle.Width != 0)
            {
                eye_brow_r.rectangle.X += dtctd_fc.x;
                eye_brow_r.rectangle.Y += dtctd_fc.y;
                dtctd_fc.rec_brow_r = eye_brow_r.rectangle;

                eye_brow_r = m_oragn_Grouping(eye_brow_r, 1);

                dtctd_fc.Right_brow_shape = m_extract_object_shape(eye_brow_r.bitmap_Array);

                if (eye_brow_r.Mid_point.X > 0 && eye_brow_r.Mid_point.Y > 0)
                {
                    eye_brow_r.Mid_point.X += eye_brow_r.rectangle.X;
                    eye_brow_r.Mid_point.Y += eye_brow_r.rectangle.Y;

                    eye_brow_r.Left_point.X += eye_brow_r.rectangle.X;
                    eye_brow_r.Left_point.Y += eye_brow_r.rectangle.Y;

                    eye_brow_r.Right_point.X += eye_brow_r.rectangle.X;
                    eye_brow_r.Right_point.Y += eye_brow_r.rectangle.Y;

                    dtctd_fc.mesh_right_brow_points = new Point[3] { eye_brow_r.Left_point, eye_brow_r.Mid_point, eye_brow_r.Right_point };
                }

            }
            //----------------------------------
            c_face_Organ eye_brow_l =
                m_get_L_eyebrow(p1, p2, face_area, dtctd_fc.Eye_size, 0.3);
            if (eye_brow_l != null && eye_brow_l.rectangle.Width != 0)
            {
                eye_brow_l.rectangle.X += dtctd_fc.x;
                eye_brow_l.rectangle.Y += dtctd_fc.y;
                dtctd_fc.rec_brow_l = eye_brow_l.rectangle;

                eye_brow_l = m_oragn_Grouping(eye_brow_l, 1);
                dtctd_fc.Left_brow_shape = m_extract_object_shape(eye_brow_l.bitmap_Array);

                if (eye_brow_l.Mid_point.X > 0 && eye_brow_l.Mid_point.Y > 0)
                {
                    eye_brow_l.Mid_point.X += eye_brow_l.rectangle.X;
                    eye_brow_l.Mid_point.Y += eye_brow_l.rectangle.Y;

                    eye_brow_l.Left_point.X += eye_brow_l.rectangle.X;
                    eye_brow_l.Left_point.Y += eye_brow_l.rectangle.Y;

                    eye_brow_l.Right_point.X += eye_brow_l.rectangle.X;
                    eye_brow_l.Right_point.Y += eye_brow_l.rectangle.Y;

                    dtctd_fc.mesh_left_brow_points = new Point[3] { eye_brow_l.Left_point, eye_brow_l.Mid_point, eye_brow_l.Right_point };
                }

            }
        }
        //--------------------------------------
        static byte[] m_extract_object_shape(byte[,] pic_binary)
        {
            if (pic_binary == null)
                return null;
            return c_Preprocessing.m_Scale(c_Feature_extraction.m_Face_To_Signal_RL(pic_binary));
        }
        //--------------------------------------
        static List<int> eye_pos = new List<int>();
        //static byte[,] eyes;
        static byte[,] pic_2D;
        static Size eye_size = new Size();
        static double com_length;
        static c_My_point[] min_point;
        static List<c_My_point> face_list = new List<c_My_point>();
        static byte[] eye_reg;
        static byte[,] face_3D;
        static byte[,] result;
        static List<c_My_Group> mark_points_group;
        static List<c_My_pieces> pieces = new List<c_My_pieces>();
        static Point mid_point;
        static byte[,] one_piece;
        static List<Point> eyes_Location = new List<Point>();

        public static s_Face m_eye_detection
            (s_Face dtctd_fc, byte[,] eye_area, Size face_area)
        {
            eye_pos.Clear();
            face_list.Clear();
            pieces.Clear();
            //eyes = eye_area;
            pic_2D = eye_area;

            eye_width = (face_area.Width * 30) / 180;
            eye_height = (face_area.Height * 32) / 540;

            /*if (eye_width < 15 || eye_height < 6)
                return dtctd_fc;*/

            if (eye_width % 4 != 0)
                eye_width = (eye_width / 4) * 4;

            eye_size.Width = eye_width;
            eye_size.Height = eye_height;
            ///////////////////////////////////////////////////
            face_3D = eye_area;

            result = new byte[face_3D.GetLength(0), face_3D.GetLength(1)];
            for (int i = 0; i < face_3D.GetLength(0); i++)
            {
                for (int j = 0; j < face_3D.GetLength(1); j++)
                {
                    face_list.Add(new c_My_point(i, j, face_3D[i, j]));
                    result[i, j] = face_3D[i, j];
                }
            }
            face_list.Sort(myComparable_point);
            com_length = 0.5 * face_list.Count / 100;
            min_point = face_list.GetRange(0, (int)com_length).ToArray();
            mark_points_group = m_Grouping(min_point, 0, 10);
            ///////////////////////////////////////////////////
            int a = eye_width / 4;
            int b = eye_height / 4;
            int[] w_cut = new int[5] { 0, a, -a, 0, 0 };
            int[] h_cut = new int[5] { 0, 0, 0, b, -b };

            int temp_i = 0, temp_j = 0;
            for (int i = 0; i < mark_points_group.Count; i++)
            {
                mid_point = mark_points_group[i].Mid_Point;
                mid_point.X -= eye_width / 2;
                mid_point.Y -= eye_height / 2;
                if (mid_point.X < a)
                    mid_point.X = a;
                if (mid_point.Y < b)
                    mid_point.Y = b;

                for (int j = 0; j < w_cut.Length; j++)
                {
                    temp_i = 0; temp_j = 0;
                    one_piece = new byte[eye_width, eye_height];
                    for (int q = mid_point.X + w_cut[j];
                          q < result.GetLength(0) && q < mid_point.X + w_cut[j] + eye_width; q++)
                    {
                        temp_j = 0;
                        for (int w = mid_point.Y + h_cut[j];
                            w < result.GetLength(1) &&
                            w < mid_point.Y + h_cut[j] + eye_height; w++)
                        {
                            one_piece[temp_i, temp_j] = result[q, w];
                            temp_j++;
                        }
                        temp_i++;
                    }
                    pieces.Add(new c_My_pieces(mid_point.X + w_cut[j],
                        mid_point.Y + h_cut[j], one_piece));
                }
            }

            for (int i = 0; i < pieces.Count; i++)
            {
                if (m_3D_eye(pieces[i].points))
                {
                    eyes_Location.Add(new Point(pieces[i].x, pieces[i].y));
                    //eye_pos.Add(pieces[i].x);
                    //eye_pos.Add(pieces[i].y);
                }
            }
            /////////
            eyes_Location.Sort(myComparable_Point);
            int x = face_area.Width / 6;
            int y = face_area.Height / 3;
            /*if (eyes_Location.Count == 1)
            {
                eyes_Location[0] = new Point(eyes_Location[0].X +
                    dtctd_fc.x + x, eyes_Location[0].Y + dtctd_fc.y + y);
                dtctd_fc.eye1 = eyes_Location[0];
            }*/
            if (eyes_Location.Count > 1)
            {
                if (Math.Abs(eyes_Location[0].X - eyes_Location[eyes_Location.Count - 1].X)
                    > eye_width &&
                    Math.Abs(eyes_Location[0].Y - eyes_Location[eyes_Location.Count - 1].Y)
                    < eye_height &&
                    eyes_Location[0].X > 0 && eyes_Location[0].Y > 0 &&
                    eyes_Location[eyes_Location.Count - 1].X > 0 &&
                    eyes_Location[eyes_Location.Count - 1].Y > 0)
                {
                    dtctd_fc.Eye_size = new Size(eye_width, eye_height);
                    dtctd_fc.Eye1_face_position =
                        new Point(eyes_Location[0].X + x + (eye_width / 2),
                            eyes_Location[0].Y + y + (eye_height / 2));
                    dtctd_fc.Eye2_face_position =
                        new Point(eyes_Location[eyes_Location.Count - 1].X + x + (eye_width / 2),
                            eyes_Location[eyes_Location.Count - 1].Y + y + (eye_height / 2));

                    eyes_Location[0] = new Point(eyes_Location[0].X + dtctd_fc.x + x,
                    eyes_Location[0].Y + dtctd_fc.y + y);
                    eyes_Location[1] = new Point(eyes_Location[eyes_Location.Count - 1].X +
                        dtctd_fc.x + x,
                    eyes_Location[eyes_Location.Count - 1].Y
                    + dtctd_fc.y + y);

                    dtctd_fc.Eye1 = eyes_Location[0];
                    dtctd_fc.Eye2 = eyes_Location[1];
                }
                else
                    eyes_Location.Clear();
            }
            return dtctd_fc;
        }
        //----------------------------------
        static List<c_My_point> eye_list = new List<c_My_point>();
        //static byte[,] result;
        //static c_My_point[] min_point;
        static c_My_point[] max_point;
        static bool eye_exist;
        //static int com_length;

        public static bool m_3D_eye(byte[,] eye_3D)
        {
            eye_list.Clear();
            result = new byte[eye_3D.GetLength(0), eye_3D.GetLength(1)];
            for (int i = 0; i < eye_3D.GetLength(0); i++)
            {
                for (int j = 0; j < eye_3D.GetLength(1); j++)
                {
                    eye_list.Add(new c_My_point(i, j, eye_3D[i, j]));
                    result[i, j] = eye_3D[i, j];
                }
            }
            eye_list.Sort(myComparable_point);
            int com_length = 40 * eye_list.Count / 100;
            min_point = eye_list.GetRange(0, com_length).ToArray();
            max_point = eye_list.GetRange
                (eye_list.Count - com_length, com_length).ToArray();
            //To ensure that blacked eye deos not detected
            eye_exist = false;
            if (min_point.Length > 0 && max_point.Length > 0 &&
                Math.Abs(min_point[min_point.Length - 1].value - max_point[0].value) > 2)
            {
                eye_exist =
                    m_eye_condition(min_point, max_point, eye_height / 2, eye_3D.GetLength(0));
            }
            return eye_exist;
        }
        //---------------------
        static List<c_My_Group> minGroups;
        private static bool m_eye_condition
            (c_My_point[] min_point, c_My_point[] max_point
            , int mean_Line, int eye_width)
        {
            minGroups = m_Grouping(min_point, mean_Line + (mean_Line / 4), 1);
            minGroups.Sort(myComparable_group);
            if (minGroups.Count > 0)
            {
                Rectangle min_rec = minGroups[0].get_Rectangle();
                if (min_rec.Width <= eye_width * 13 / 40)
                    return false;

                List<c_My_point> choosen_maxy = new List<c_My_point>();
                for (int i = 0; i < max_point.Length; i++)
                {
                    if (max_point[i].x < min_rec.X || max_point[i].x > min_rec.X + min_rec.Width)
                        choosen_maxy.Add(max_point[i]);
                }
                List<c_My_Group> maxGroups = m_Grouping(choosen_maxy.ToArray(),
                    mean_Line + (mean_Line / 4), 1);
                if (maxGroups.Count > 0 && minGroups.Count > 0)
                //&& maxGroups[0].points[0].value > minGroups[0].points[0].value + 10)
                {
                    maxGroups.Sort(myComparable_group);
                    if (maxGroups.Count >= 2 && maxGroups[0].Count > 5 &&
                        maxGroups[1].Count > 10 && minGroups[0].Count > 10 &&
                        min_rec.Width > 0 && min_rec.Height > 0)
                    {
                        Rectangle max_rec1 = maxGroups[0].get_Rectangle();
                        Rectangle max_rec2 = maxGroups[1].get_Rectangle();
                        if ((Math.Abs(max_rec1.Y - max_rec2.Y) <= 7) &&
                            (Math.Abs(max_rec1.Y - min_rec.Y) <= 10) &&
                            (Math.Abs(max_rec2.Y - min_rec.Y) <= 10) &&
                            ((max_rec1.X > max_rec2.X &&
                            max_rec1.X > min_rec.X &&
                            min_rec.X > max_rec2.X &&
                            max_rec1.X > eye_width / 2 &&
                            max_rec2.X < eye_width / 2) ||

                        (max_rec2.X > max_rec1.X &&
                            max_rec2.X > min_rec.X &&
                            min_rec.X > max_rec1.X &&
                            max_rec2.X > eye_width / 2 &&
                            max_rec1.X < eye_width / 2)))
                        {
                            //eye_min_value.Add(minGroups[0].points[0].value);
                            return true;
                        }

                    }
                }
            }
            return false;
        }
        //--------------------------
        static List<c_My_Group> united_Groups = new List<c_My_Group>();
        static c_My_Group crnt_Group;

        private static List<c_My_Group> m_Grouping
            (c_My_point[] type_point, int mean_Line, int error)
        {
            stack_counter = 0;
            united_Groups.Clear();

            bool vitamin_O = mean_Line == 0;
            for (int i = 0; i < type_point.Length; i++)
            {
                if (!type_point[i].grouped && (vitamin_O || type_point[i].y == mean_Line))
                {
                    stack_counter = 0;
                    crnt_Group = new c_My_Group();
                    crnt_Group.points.Add(type_point[i]);
                    type_point[i].grouped = true;
                    m_neighbourhood(crnt_Group, type_point, type_point[i], error);
                    united_Groups.Add(crnt_Group);
                }
            }
            return united_Groups;
        }

        private static void m_neighbourhood(c_My_Group crnt_Group,
            c_My_point[] type_points, c_My_point cur_point, int error)
        {
            stack_counter++;
            if (stack_counter > 500)
                return;
            for (int i = 0; i < type_points.Length; i++)
            {
                if (stack_counter > 500)
                    return;
                if (!type_points[i].grouped)
                {
                    if (Math.Abs(cur_point.x - type_points[i].x) <= error &&
                        Math.Abs(cur_point.y - type_points[i].y) <= error)
                    {
                        crnt_Group.points.Add(type_points[i]);
                        type_points[i].grouped = true;
                        m_neighbourhood(crnt_Group, type_points, type_points[i], error);
                    }
                }
            }
        }
        //------------------------------------------
        static byte[,] wrkng_points;
        static byte[,] moving_objects_b;

        public static int[] m_Hair_Stem_Extraction
            (byte[,] crt_frame, byte[,] moving_objects_mono)
        {
            wrkng_points = crt_frame;

            moving_objects_b = moving_objects_mono;

            return m_hair_Edge(wrkng_points, moving_objects_b);
        }

        static int[] kernel = new int[21] { -10, -9, -8, -7, -6, -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        private static int[] m_hair_Edge(byte[,] wrkng_points, byte[,] moving_objects_b)
        {
            List<Point>[] maxi = new List<Point>[wrkng_points.GetLength(0)];
            List<int> signal = new List<int>();
            int avg_1 = 0, avg_2 = 0, max = 0, index = 0;
            int[] hair_line = new int[wrkng_points.GetLength(0)];
            bool flag_hair_area_start = false;
            for (int i = 1; i < wrkng_points.GetLength(0) - 1; i++)
            {
                maxi[i] = new List<Point>();
                avg_1 = 0; avg_2 = 0; max = 0; index = 0;
                flag_hair_area_start = false;
                for (int j = (kernel.Length - 1) / 2;
                    j < wrkng_points.GetLength(1) - ((kernel.Length - 1) / 2); j++)
                {
                    for (int q = 0; q < kernel.Length / 2; q++)
                    {
                        if (!flag_hair_area_start &&
                            moving_objects_b[i, j + kernel[q]] == 0 &&
                            moving_objects_b[i - 1, j + kernel[q]] == 0
                            && moving_objects_b[i + 1, j + kernel[q]] == 0)
                        {
                            if (j + 15 < wrkng_points.GetLength(1) - ((kernel.Length - 1) / 2))
                                j += 15;
                            flag_hair_area_start = true;
                        }
                        if (moving_objects_b[i, j + kernel[q]] == 0 &&
                            moving_objects_b[i - 1, j + kernel[q]] == 0
                            && moving_objects_b[i + 1, j + kernel[q]] == 0
                            && flag_hair_area_start)
                        {
                            avg_1 += (wrkng_points[i - 1, j + kernel[q]] +
                                wrkng_points[i, j + kernel[q]] +
                                wrkng_points[i + 1, j + kernel[q]]) / 3;
                        }
                        else
                        {
                            avg_1 = 0;
                            break;
                        }
                    }
                    avg_1 /= (kernel.Length / 2);
                    for (int q = kernel.Length / 2; q < kernel.Length; q++)
                    {
                        if (moving_objects_b[i, j + kernel[q]] == 0 &&
                            moving_objects_b[i - 1, j + kernel[q]] == 0
                            && moving_objects_b[i + 1, j + kernel[q]] == 0)
                        {
                            avg_2 += (wrkng_points[i - 1, j + kernel[q]] +
                                wrkng_points[i, j + kernel[q]] +
                                wrkng_points[i + 1, j + kernel[q]]) / 3;
                        }
                        else
                        {
                            avg_2 = 0;
                            break;
                        }
                    }
                    avg_2 /= (kernel.Length / 2);
                    if (avg_1 != 0 && avg_2 != 0)
                    {
                        if (max < avg_2 - avg_1)
                        {
                            max = avg_2 - avg_1;
                            index = j;
                            maxi[i].Add(new Point(index, max));
                        }
                    }
                }
                if (index != 0)
                    signal.Add(wrkng_points.GetLength(1) - index);
            }
            return signal.ToArray();
        }
        //-------------------------------------------
        public static double m_calc_c_Ratio(double a, double b)
        {
            //Golden Ratio
            // (a+b)/a = a/b = a|--------------|b------|
            return 0.618 * b + 0.382 * a;
        }

        public static byte[,] m_adaptive_threshold_4_rec
            (byte[,] work_area, Rectangle work_area_Bounds, double limt)
        {
            //limit 0->1 %
            int x = work_area_Bounds.X,
                y = work_area_Bounds.Y,
                w = work_area_Bounds.Width,
                h = work_area_Bounds.Height;
            if (!(work_area_Bounds.X >= 0 && work_area_Bounds.Y >= 0 &&
                work_area_Bounds.Width > 0 && work_area_Bounds.Height > 0))
                return null;
            byte[,] result_area = new byte[w, h];
            if (x > 0 && y > 0)
            {
                byte[] histogram = new byte[256];
                for (int i = x; i < x + w && i < work_area.GetLength(0); i++)
                {
                    for (int j = y; j < y + h && j < work_area.GetLength(1); j++)
                    {
                        histogram[work_area[i, j]]++;
                    }
                }
                byte threshold_value = 0;
                int threshold_counter = 0;
                for (byte i = 0; i < histogram.Length - 1; i++)
                {
                    if (threshold_counter < w * h * limt)
                        threshold_counter += histogram[i];
                    else
                    {
                        threshold_value = i;
                        break;
                    }
                }

                for (int i = x; i < x + w && i < work_area.GetLength(0); i++)
                {
                    for (int j = y; j < y + h && j < work_area.GetLength(1); j++)
                    {
                        if (work_area[i, j] < threshold_value)
                            result_area[i - x, j - y] = 0;
                        else
                            result_area[i - x, j - y] = 255;

                    }
                }
            }
            return result_area;
        }

        public static c_face_Organ
            m_get_mouth(Point p1, Point p2, byte[,] face_area_array, double error)
        {
            int distance_eyes = p2.X - p1.X;
            int d = distance_eyes;
            //d : the distant between the eye and the mouth.
            Rectangle mouth_rectangle = new Rectangle(p1.X, p1.Y + d, distance_eyes,
                (int)(d * 0.381));
            if (!(mouth_rectangle.X >= 0 && mouth_rectangle.Y >= 0 &&
                mouth_rectangle.Width > 0 && mouth_rectangle.Height > 0))
                return null;
            int rest = mouth_rectangle.Width % 4;
            mouth_rectangle.Width -= rest;
            byte[,] mouth_area = m_adaptive_threshold_4_rec
                (face_area_array, mouth_rectangle, error);
            //get the pic of the mouth 
            //------------------------------------------------
            c_face_Organ o = new c_face_Organ(mouth_rectangle, mouth_area);
            return o;
        }

        public static c_face_Organ m_get_nose
            (Point p1, Point p2, int mouth_y, int mouth_Height,
            byte[,] face_area_array, Size size_of_the_eye, double error)
        {
            int nose_y = (int)m_calc_c_Ratio(p1.Y, mouth_y);
            Rectangle nose_rectangle = new Rectangle
                (p1.X + (size_of_the_eye.Width / 2),
                nose_y,
                p2.X - p1.X - (size_of_the_eye.Width), mouth_Height * 2 / 3);

            if (!(nose_rectangle.X >= 0 && nose_rectangle.Y >= 0 &&
                nose_rectangle.Width > 0 && nose_rectangle.Height > 0))
                return null;

            int rest = nose_rectangle.Width % 4;
            nose_rectangle.Width -= rest;
            face_area_array = m_adaptive_threshold_4_rec(face_area_array, nose_rectangle, error);
            return new c_face_Organ(nose_rectangle, face_area_array);
        }

        public static c_face_Organ m_get_L_eyebrow
            (Point p1, Point p2, byte[,] face_area_array, Size size_of_the_eye, double error)
        {
            Rectangle l_eyebrow_rectangle =
                new Rectangle(p1.X - (size_of_the_eye.Width / 2),
                    p1.Y - (3 * size_of_the_eye.Height),
                    (int)(size_of_the_eye.Width * 1.5),
                     2 * size_of_the_eye.Height + size_of_the_eye.Height / 3);
            if (!(l_eyebrow_rectangle.X >= 0 && l_eyebrow_rectangle.Y >= 0 &&
                l_eyebrow_rectangle.Width > 0 && l_eyebrow_rectangle.Height > 0))
                return null;
            int rest = l_eyebrow_rectangle.Width % 4;
            l_eyebrow_rectangle.Width -= rest;
            byte[,] bt1 = m_adaptive_threshold_4_rec(face_area_array, l_eyebrow_rectangle, error);
            //get the pic of the mouth 
            c_face_Organ o = new c_face_Organ(l_eyebrow_rectangle, bt1);
            return o;
        }

        public static c_face_Organ m_get_R_eyebrow
            (Point p1, Point p2, byte[,] face_area_array, Size size_of_the_eye, double error)
        {
            Rectangle R_eyebrow_rectangle =
               new Rectangle(p2.X - size_of_the_eye.Width,
                    p2.Y - (3 * size_of_the_eye.Height),
                    (int)(size_of_the_eye.Width * 1.5),
                     2 * size_of_the_eye.Height + size_of_the_eye.Height / 3);
            if (!(R_eyebrow_rectangle.X >= 0 && R_eyebrow_rectangle.Y >= 0 &&
                R_eyebrow_rectangle.Width > 0 && R_eyebrow_rectangle.Height > 0))
                return null;
            int rest = R_eyebrow_rectangle.Width % 4;
            R_eyebrow_rectangle.Width -= rest;
            byte[,] bt1 = m_adaptive_threshold_4_rec(face_area_array, R_eyebrow_rectangle, error);
            //get the pic of the mouth 
            c_face_Organ o = new c_face_Organ(R_eyebrow_rectangle, bt1);
            return o;
        }
        //-------------------------------------------
        static List<c_My_Group> oragn_united_Groups = new List<c_My_Group>();
        static c_My_Group oragn_crnt_Group;
        static int[] indexI;
        static Size frm_sz;
        private static c_face_Organ m_oragn_Grouping
            (c_face_Organ organ, int seek_error)
        {
            byte[,] moving_objects_oragn_b = organ.bitmap_Array;
            frm_sz = organ.rectangle.Size;
            oragn_united_Groups.Clear();
            indexI = new int[seek_error * 2 + 1];
            int index_Value = -seek_error;
            for (int i = 0; i < indexI.Length; i++)
            {
                indexI[i] = index_Value++;
            }
            for (int j = 0; j < moving_objects_oragn_b.GetLength(1); j++)
            {
                for (int i = 0; i < moving_objects_oragn_b.GetLength(0); i++)
                {
                    if (moving_objects_oragn_b[i, j] == 0)
                    {
                        oragn_crnt_Group = new c_My_Group();
                        c_My_point cancer_cell = new c_My_point(i, j);
                        oragn_crnt_Group.points.Add(cancer_cell);
                        moving_objects_oragn_b[i, j] = 255;
                        m_oragn_neighbourhood(moving_objects_oragn_b, cancer_cell);
                        if (oragn_crnt_Group.Count > 250)
                            oragn_united_Groups.Add(oragn_crnt_Group);
                    }
                }
            }
            oragn_united_Groups.Sort(myComparable_group);
            byte[,] result = new byte[frm_sz.Width, frm_sz.Height];
            for (int j = 0; j < moving_objects_oragn_b.GetLength(1); j++)
            {
                for (int i = 0; i < moving_objects_oragn_b.GetLength(0); i++)
                {
                    result[i, j] = 255;
                }
            }
            if (oragn_united_Groups.Count > 0)
            {
                for (int i = 0; i < oragn_united_Groups[0].Count; i++)
                {
                    result[oragn_united_Groups[0].points[i].x,
                        oragn_united_Groups[0].points[i].y] = 0;
                }
            }
            if (oragn_united_Groups.Count > 0)
            {
                organ.bitmap_Array = result;
                organ.Mid_point = oragn_united_Groups[0].Mid_Point;
                Point[] temp_l_r = oragn_united_Groups[0].Left_Right_Point;
                organ.Left_point = temp_l_r[0];
                organ.Right_point = temp_l_r[1];
            }
            return organ;
        }
        //--------------------------------
        private static void m_oragn_neighbourhood(byte[,] moving_objects_oragn_b, c_My_point cancer_cell)
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
                            if (moving_objects_oragn_b[cancer_cell.x + indexI[w],
                                cancer_cell.y + indexI[q]] == 0)
                            {
                                moving_objects_oragn_b[cancer_cell.x + indexI[w],
                                    cancer_cell.y + indexI[q]] = 255;

                                my_Stack.Push(new c_My_point(cancer_cell.x + indexI[w],
                                        cancer_cell.y + indexI[q]));
                                oragn_crnt_Group.points.Add(my_Stack.Peek());
                            }
                        }
                    }
                }
            }
        }
        //-------------------------------------------
        private static byte[,] m_1D_to_2D_array(byte[] pic, int width)
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
        }

        private static byte[,] m_Crop_area
            (byte[,] pic, int x, int y, ref int width, int height)
        {
            if (pic.GetLength(0) > 0 && pic.GetLength(1) > 0)
            {
                if (x < 0 || y < 0 ||
                    (x + width > pic.GetLength(0) ||
                    y + height > pic.GetLength(1)))
                    return null;
                int p_bmp_width = pic.GetLength(0);
                int p_bmp_height = pic.GetLength(1);
                
                if (width % 4 != 0)
                    width -= (width % 4);

                byte[,] bmp2 = new byte[width, height];
                int indexj = 0, indexi = 0;
                for (int j = y; j < height + y; j++)
                {
                    for (int i = x; i < width + x; i++)
                    {
                        bmp2[indexi, indexj] = pic[i, j];
                        indexi++;
                    }
                    indexj++;
                    indexi = 0;
                }
                return bmp2;
            }
            else
                return new byte[0,0];
        }
    }

    class c_My_pieces
    {
        public byte[,] points;
        public int x, y;

        public c_My_pieces(int _x, int _y, byte[,] p)
        {
            x = _x;
            y = _y;
            points = p;
        }
    }

    class c_My_Group
    {
        public List<c_My_point> points = new List<c_My_point>();

        public int Count
        {
            get
            {
                return points.Count;
            }
        }

        public Point Mid_Point
        {
            get
            {
                int[] temp_x = this.get_min_max_X();
                int[] temp_y = this.get_min_max_Y();
                return new Point((temp_x[0] + temp_x[1]) / 2,
                    (temp_y[0] + temp_y[1]) / 2);
            }
        }

        public Point[] Left_Right_Point
        {
            get
            {
                Point[] l_r = new Point[2];
                Point mid = Mid_Point;
                int minX = points[0].x, maxX = points[0].x;
                for (int i = 1; i < points.Count; i++)
                {
                    if (minX > points[i].x && points[i].y == mid.Y)
                        minX = points[i].x;
                    if (maxX < points[i].x && points[i].y == mid.Y)
                        maxX = points[i].x;
                }
                return new Point[2] { new Point(minX, mid.Y), new Point(maxX, mid.Y) };
            }
        }

        public Point Right_Point
        {
            get
            {
                int[] temp_x = this.get_min_max_X();
                int[] temp_y = this.get_min_max_Y();
                return new Point((temp_x[0] + temp_x[1]) / 2,
                    (temp_y[0] + temp_y[1]) / 2);
            }
        }

        public int[] get_min_max_X()
        {
            int min = points[0].x, max = points[0].x;
            for (int i = 1; i < points.Count; i++)
            {
                if (min > points[i].x)
                    min = points[i].x;
                if (max < points[i].x)
                    max = points[i].x;
            }
            return new int[2] { min, max };
        }

        public int[] get_min_max_X(int mean_line)
        {
            int min = mean_line * 5, max = 0;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].y == mean_line)
                {
                    if (min > points[i].x)
                        min = points[i].x;
                    if (max < points[i].x)
                        max = points[i].x;
                }
            }
            return new int[2] { min, max };
        }

        public int[] get_min_max_Y()
        {
            int min = points[0].y, max = points[0].y;
            for (int i = 1; i < points.Count; i++)
            {
                if (min > points[i].y)
                    min = points[i].y;
                if (max < points[i].y)
                    max = points[i].y;
            }
            return new int[2] { min, max };
        }

        public int get_Center()
        {
            int[] start_end = get_min_max_X();
            List<c_My_point> histo = new List<c_My_point>();
            c_My_point temp;
            int counter = 0;
            for (int j = start_end[0]; j <= start_end[1]; j++)
            {
                counter = 0;
                temp = new c_My_point();
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i].x == j)
                    {
                        counter++;
                    }
                }
                temp.y = counter;
                temp.x = j;
                histo.Add(temp);
            }
            int max_y_length = histo[0].y;
            for (int i = 1; i < histo.Count; i++)
            {
                if (max_y_length < histo[i].y)
                    max_y_length = histo[i].y;
            }
            int temp_x_min = int.MaxValue, temp_x_max = 0;
            for (int i = histo.Count - 1; i >= 0; i--)
            {
                if (max_y_length == histo[i].y)
                {
                    if (temp_x_max < histo[i].x)
                        temp_x_max = histo[i].x;
                    if (temp_x_min > histo[i].x)
                        temp_x_min = histo[i].x;
                }
            }
            return (temp_x_max + temp_x_min) / 2;
        }

        public Rectangle get_Rectangle()
        {
            int[] p_y = get_min_max_Y();
            int width = p_y[1] - p_y[0];
            int xx = get_Center() - (width / 2);
            if (xx < 0)
                xx = 0;
            return new Rectangle(xx, p_y[0], width, width);
        }
    }

    class c_My_point
    {
        public int x, y;
        public byte value;
        public bool grouped;
        public c_My_point() { }
        public c_My_point(int _x, int _y)
        {
            x = _x;
            y = _y;
            grouped = false;
        }
        public c_My_point(int _x, int _y, byte _value)
        {
            x = _x;
            y = _y;
            value = _value;
            grouped = false;
        }
    }

    class myPoint_Compare : IComparer<c_My_point>
    {
        public int Compare(c_My_point p1, c_My_point p2)
        {
            if (p1.value >= p2.value)
                return 1;
            else
                return -1;
        }
    }

    class Point_Compare : IComparer<Point>
    {
        public int Compare(Point p1, Point p2)
        {
            if (p1.X >= p2.X)
                return 1;
            else
                return -1;
        }
    }

    class myGroup_Compare : IComparer<c_My_Group>
    {
        public int Compare(c_My_Group g1, c_My_Group g2)
        {
            if (g1.Count <= g2.Count)
                return 1;
            else
                return -1;
        }
    }

    public class c_face_Organ
    {
        public Rectangle rectangle;
        public byte[,] bitmap_Array;
        public Point Mid_point;
        public Point Left_point;
        public Point Right_point;
        public c_face_Organ(Rectangle _rectangle, byte[,] _bitmap_Array)
        {
            this.bitmap_Array = _bitmap_Array;
            rectangle = _rectangle;
        }
    }

    struct s_Face
    {
        public int x, y, Width, Height;
        public string Name;
        public double Percent;

        public string SecondName;
        public double SecondPercent;

        public Size Eye_size;
        public Point Eye1;
        public Point Eye2;
        public Point Eye1_face_position;
        public Point Eye2_face_position;
        public byte[] mesh;
        public byte[] face_Signal;
        public byte[] hair_Signal;

        public byte[] Right_face_shape;
        public byte[] Left_face_shape;

        public byte[] Right_brow_shape;
        public byte[] Left_brow_shape;

        public byte[] nose_shape;

        public byte[] mouth_shape;

        public Point mesh_nose_point;
        public Point[] mesh_mouht_points;
        public Point[] mesh_right_brow_points;
        public Point[] mesh_left_brow_points;

        public Rectangle rec_mouht;
        public Rectangle rec_nose;
        public Rectangle rec_brow_r;
        public Rectangle rec_brow_l;

        public double percent_value_mesh,
            percent_value_r_face, percent_value_l_face, final_percent,
            percent_value_r_brow, percent_value_l_brow,
            percent_value_nose, percent_value_mouth,
            percent_value_hair;

        public s_Face(int _x, int _y, int _width,
            int _height, byte[] _face_signal)
        {
            x = _x;
            y = _y;
            Width = _width;
            Height = _height;
            Name = string.Empty;
            Percent = 0;
            Eye_size = Size.Empty;
            Eye1 = Point.Empty;
            Eye2 = Point.Empty;
            face_Signal = _face_signal;

            hair_Signal = null;
            Right_face_shape = null;
            Left_face_shape = null;

            Right_brow_shape = null;
            Left_brow_shape = null;

            nose_shape = null;

            mouth_shape = null;

            mesh = null;

            mesh_nose_point = Point.Empty;
            mesh_mouht_points = new Point[3];
            mesh_right_brow_points = new Point[3];
            mesh_left_brow_points = new Point[3];
            Eye1_face_position = Point.Empty;
            Eye2_face_position = Point.Empty;

            rec_mouht = Rectangle.Empty;
            rec_nose = Rectangle.Empty; 
            rec_brow_r = Rectangle.Empty; 
            rec_brow_l = Rectangle.Empty;

            SecondName = string.Empty;
            SecondPercent = 0;


            percent_value_mesh = 0;
            percent_value_r_face = 0;
            percent_value_l_face = 0;
            final_percent = 0;
            percent_value_r_brow = 0;
            percent_value_l_brow = 0;
            percent_value_nose = 0;
            percent_value_mouth = 0;
            percent_value_hair = 0;
        }
    }
}
/*
 * public static int[] eye_detection(Bitmap pic)
        {
            byte[] bmp = c_Preprocessing.m_BmpToBytes(pic);
            int width = pic.Width;
            int index = 0;
            List<byte> mins = new List<byte>();
            List<int> min_indexs = new List<int>();
            List<int> match_indexs = new List<int>();
            byte min;
            int min_index;
            while (index < bmp.Length)
            {
                min = bmp[index];
                min_index = 0;
                for (int i = 0; i < width * 3; i += 3)
                {
                    if (min > bmp[index + i])
                    {
                        min = bmp[index + i];
                        min_index = i;
                    }
                }
                mins.Add(min);
                min_indexs.Add(min_index);
                index += width * 3;
            }

            int temp;
            int eye_index = 0;
            int eye_index_x = 0;
            int eye_index_y = 0;
            int max = 0;
            width *= 3;
            match_indexs.Clear();
            for (int i = 0; i < mins.Count; i++)
            {
                for (int j = 0; j < pointer.Length; j++)
                {
                    if (-pointer[j] + min_indexs[i] > 0 &&
                        pointer[j] + min_indexs[i] < width)
                    {
                        temp = bmp[(i * width) + min_indexs[i]];

                        if (max < Math.Abs(temp - bmp[(i * width) + min_indexs[i] - pointer[j]]) ||
                            max < Math.Abs(temp - bmp[(i * width) + min_indexs[i] + pointer[j]]))
                        {
                            max = Math.Max(Math.Abs(temp - bmp[(i * width) + min_indexs[i] - pointer[j]]),
                            Math.Abs(temp - bmp[(i * width) + min_indexs[i] + pointer[j]]));
                            eye_index = (i * width) + min_indexs[i];
                            eye_index_x = min_indexs[i] / 3;
                            eye_index_y = i;
                            match_indexs.Add(min_indexs[i]);
                            bmp[(i * width) + min_indexs[i]] = 255;
                            bmp[(i * width) + min_indexs[i] + 1] = 0;
                            bmp[(i * width) + min_indexs[i] + 2] = 0;
                        }
                    }
                    else
                        break;
                }
            }
            bmp[eye_index] = 0;
            bmp[eye_index + 1] = 0;
            bmp[eye_index + 2] = 255;
            /*c_Preprocessing.m_BytesToBmp(bmp, pic.Size).Save
                ("detected_eye" + (coooooooot++).ToString() + "_" + match_indexs.Count.ToString() + ".bmp");
            return new int[2] { eye_index_x, eye_index_y };
        }
*/
/*
 * public static int[] eye_detection_old(Bitmap pic)
        {
            byte[] eyes = c_Preprocessing.m_BmpToBytes_mono(pic);
            for (int i = 0; i < eyes.Length; i++)
            {
                if (eyes[i] == 255)
                    eyes[i] = 254;
            }
            byte[] single_line = new byte[pic.Width];
            int index = 0;
            while (index < eyes.Length)
            {
                for (int i = 0; i < single_line.Length; i++)
                {
                    single_line[i] = eyes[index + i];
                }
                int[] eye_loca = extract_eyes_signal(single_line);
                for (int w = 0; w < eye_loca.Length; w++)
                {
                    eyes[index + eye_loca[w]] = 255;
                }
                index += single_line.Length;
            }
            byte[] final_eye = new byte[eyes.Length * 3];
            for (int i = 0; i < eyes.Length; i++)
            {
                if (eyes[i] != 255)
                {
                    final_eye[i * 3] = eyes[i];
                    final_eye[i * 3 + 1] = eyes[i];
                    final_eye[i * 3 + 2] = eyes[i];
                }
                else
                {
                    final_eye[i * 3] = 0;
                    final_eye[i * 3 + 1] = 0;
                    final_eye[i * 3 + 2] = 255;
                }
            }
            c_Preprocessing.m_BytesToBmp(final_eye, pic.Size).Save("final.bmp");
            return null;
        }

        private static int[] extract_eyes_signal(byte[] single_line)
        {
            int eye_width = single_line.Length / 4;
            short[] signs = new short[single_line.Length];
            for (int i = 0; i < signs.Length - 1; i++)
            {
                signs[i] = (short)(single_line[i] - single_line[i + 1]);
                if (signs[i] > 1)
                    signs[i] = 1;
                else if (signs[i] < -1)
                    signs[i] = -1;
                else
                    signs[i] = 0;
            }
            signs[signs.Length - 1] = signs[signs.Length - 2];

            List<int> s_and_e = check_CFG(signs);
            List <int>eye_location=new List<int>();
            for (int i = 0; i < s_and_e.Count; i+=7)
            {
                eye_location.Add((s_and_e[i] + s_and_e[i+1]) / 2);
            }
            return eye_location.ToArray();
        }

        private static List<int> check_CFG(short[] signs)
        {
            int[] indxer = new int[3] { 0, 1, 2 };
            for (int i = 0; i < signs.Length; i++)
            {
                for (int j = 0; j < indxer.Length; j++)
                {

                }

            }
            List<int> s_and_e = new List<int>();
            int s_pointer = 0, e_pointer = 0;
            int a, b, c, d, e;
            a = b = c = d = e = 0;
            int index=0;
            while (index < signs.Length)
            {
                if (signs[index] == 1)
                {
                    s_pointer = index;
                    while (index < signs.Length-1 && signs[index] == 1)
                    {
                        a++;
                        index++;
                    }
                    if (signs[index] == -1)
                    {
                        while (index < signs.Length - 1 && signs[index] == -1)
                        {
                            b++;
                            index++;
                        }
                        if (signs[index] == 0)
                        {
                            while (index < signs.Length - 1 && signs[index] == 0)
                            {
                                c++;
                                index++;
                            }
                            if (signs[index] == +1)
                            {
                                while (index < signs.Length - 1 && signs[index] == +1)
                                {
                                    d++;
                                    index++;
                                }
                                if (signs[index] == -1)
                                {
                                    while (index < signs.Length - 1 && signs[index] == -1)
                                    {
                                        e++;
                                        index++;
                                    }
                                    e_pointer = index-1;
                                    s_and_e.Add(s_pointer);
                                    s_and_e.Add(e_pointer);
                                    s_and_e.Add(a);
                                    s_and_e.Add(b);
                                    s_and_e.Add(c);
                                    s_and_e.Add(d);
                                    s_and_e.Add(e);
                                }
                                else
                                {
                                    a = b = c = d = e = 0;
                                    index++;
                                }
                            }
                            else
                            {
                                a = b = c = d = e = 0;
                                index++;
                            }
                        }
                        else
                        {
                            a = b = c = d = e = 0;
                            index++;
                        }
                    }
                    else
                    {
                        a = b = c = d = e = 0;
                        index++;
                    }
                }
                else
                    index++;
            }
            return s_and_e;
        }*/
/*
 * face_area.Save("face.bmp");
            x = width/8;
            y = face_area.Height / 3;
            width = (width / 2) - (width / 8);
            height = y;
            Bitmap eye1 = c_Preprocessing.m_BytesToBmp(
                m_extract_area(face_area, x, y, ref width, height),
                new Size(width, height));
            x = face_area.Width/2;
            Bitmap eye2 = c_Preprocessing.m_BytesToBmp(
                m_extract_area(face_area, x, y, ref width, height),
                new Size(width, height));
            //eye1.Save("eye1.bmp");
            //eye2.Save("eye2.bmp");
            
            eye1_index = eye_detection(eye1);
            eye1_index[0] += (width / 8) + c_Feature_extraction.g_head_Position_x;
            eye1_index[1] += (face_area.Height / 3) + c_Feature_extraction.g_head_Position_y;
            
            eye2_index = eye_detection(eye2);
            eye2_index[0] += (face_area.Width / 2) + c_Feature_extraction.g_head_Position_x;
            eye2_index[1] += (face_area.Height / 3) + c_Feature_extraction.g_head_Position_y;
*/
/*
 for (int q = 0; q < w_cut.Length; q++)
            {
                List<c_My_pieces> pieces =
                    m_to_parts(w_cut[q], h_cut[q], pic_2D, eye_width, eye_height);
                for (int i = 0; i < pieces.Count; i++)
                {
                    if (eye_counter == 103 - 1)
                    {
                    }
                    if (m_3D_eye(pieces[i].points))
                    {
                        eye_pos.Add(pieces[i].x);
                        eye_pos.Add(pieces[i].y);
                        //c_Preprocessing.m_BytesToBmp_mono(m_2D_to_1D_array(pieces[i].points), eye_size).Save("match\\pieces" + eye_counter.ToString() + ".bmp");
                    }
                    eye_counter++;
                    //c_Preprocessing.m_BytesToBmp_mono(m_2D_to_1D_array(pieces[i].points), eye_size).Save("test\\pieces" + eye_counter.ToString() + ".bmp");
                }
            }*/
/*
 * private static List<c_My_Group> m_single_Grouping(c_My_point[] min_point)
        {
            stack_counter = 0;
            groupCounter = 0;
            eye_Groups = new List<c_My_Group>();
            for (int i = 0; i < min_point.Length; i++)
            {
                if (!min_point[i].grouped)
                {
                    stack_counter = 0;
                    eye_Groups.Add(new c_My_Group(Group_Type.down));
                    eye_Groups[groupCounter].points.Add(min_point[i]);
                    min_point[i].grouped = true;
                    m_neighbourhood_single(min_point, min_point[i]);
                    groupCounter++;
                }
            }
            return eye_Groups;
        }

        private static void m_neighbourhood_single(c_My_point[] type_points, c_My_point cur_point)
        {
            stack_counter++;
            if (stack_counter > 500)
                return;
            for (int i = 0; i < type_points.Length; i++)
            {
                if (!type_points[i].grouped)
                {
                    if (stack_counter > 500)
                        return;
                    if (Math.Abs(cur_point.x - type_points[i].x) <= 10 &&
                        Math.Abs(cur_point.y - type_points[i].y) <= 10)
                    {
                        eye_Groups[groupCounter].points.Add(type_points[i]);
                        type_points[i].grouped = true;
                        m_neighbourhood_single(type_points, type_points[i]);
                    }
                }
            }
        }*/
/*
 * private static List<c_My_pieces> m_convert_to_parts
            (int s_I, int s_J, byte[,] pic_2D, int eye_width, int eye_height)
        {
            pieces.Clear();
            byte[,] single = new byte[eye_width, eye_height];
            for (int i = s_I; i + eye_width < pic_2D.GetLength(0); i += eye_width)
            {
                for (int j = s_J; j + eye_height < pic_2D.GetLength(1); j += eye_height)
                {
                    single = new byte[eye_width, eye_height];
                    for (int q = 0; q < eye_width; q++)
                    {
                        for (int w = 0; w < eye_height; w++)
                        {
                            single[q, w] = pic_2D[i + q, j + w];
                        }
                    }
                    pieces.Add(new c_My_pieces(i, j, single));
                }
            }
            return pieces;
        }*/

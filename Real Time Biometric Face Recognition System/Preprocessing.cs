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
    class c_Preprocessing
    {
        static Font myFont = new Font("Tahoma", 12, FontStyle.Bold);

        static byte[,] g_bckGrnd_colored_bytes;
        //--------------------------------------------------------------
        static Image frame1;
        static bool flag1 = false;
        
        //------------------------------------------
        
        public static byte[,] m_Extract_Motion_Objects(byte[,] bckgrnd, byte[,] crrnt_frm)
        {
            if (bckgrnd == null || crrnt_frm == null)
                return null;
            g_bckGrnd_colored_bytes = new byte[crrnt_frm.GetLength(0),
                crrnt_frm.GetLength(1)];
            for (int j = 0; j < crrnt_frm.GetLength(1); j++)
            {
                for (int i = 0; i < crrnt_frm.GetLength(0); i++)
                {
                    if (Math.Abs(bckgrnd[i, j] - crrnt_frm[i, j])
                    > c_Image_acquisition.g_Difference[i,j] + 15)
                    {
                        g_bckGrnd_colored_bytes[i,j] = 0;
                    }
                    else
                    {
                        g_bckGrnd_colored_bytes[i,j] = 255;
                    }
                }
            }
             return g_bckGrnd_colored_bytes;
        }
        //---------------------------------
        static byte[,] SE = new byte[5, 5] { 
        {1, 1, 1, 1 ,1},
        {1, 1, 1, 1 ,1},
        {1, 1, 1, 1 ,1},
        {1, 1, 1, 1 ,1},
        {1, 1, 1, 1 ,1}};
        public static byte[,] m_Noise_Reduction(byte[,] pic)
        {
            if (pic == null)
                return null;
            return m_Bitmap_to_2DArray(m_binary_Erosion(m_2DArray_to_Bitmap(pic), SE));
        }
        //--------------------------------
        static int Max_Model = 100;
        static double scale_w_factor;
        static double scale_h_factor;

        public static byte[] m_Scale(int[] Signal_to_scale)
        {
            if (Signal_to_scale != null && Signal_to_scale.Length != 0)
            {
                int Model_Length = c_AIU_CV_Engine_Manager.Signal_Length;

                int Max_Signal = Signal_to_scale[0];

                for (int i = 1; i < Signal_to_scale.Length; i++)
                {
                    if (Max_Signal < Signal_to_scale[i])
                        Max_Signal = Signal_to_scale[i];
                }
                scale_w_factor = (double)Model_Length / (double)Signal_to_scale.Length;
                scale_h_factor = (double)Max_Model / (double)Max_Signal;
                byte[] Scaled_Signal = new byte[Model_Length];
                for (int i = 0; i < Scaled_Signal.Length - scale_w_factor; i++)
                {
                    Scaled_Signal[i] = (byte)
                        (Signal_to_scale[(int)Math.Round(i * (1 / scale_w_factor))] * scale_h_factor);
                }
                //else out side boundary of array will occur
                for (int i = (int)(Scaled_Signal.Length - scale_w_factor);
                    i < Scaled_Signal.Length; i++)
                {
                    Scaled_Signal[i] = (byte)(Signal_to_scale[Signal_to_scale.Length - 1] * scale_h_factor);
                }
                return Scaled_Signal;
            }
            return new byte[0];
        }
        //----------------------------------
        static BitmapData bData;
        static int byteCount;
        static byte[] bmpBytes;

        public static byte[,] m_Bitmap_to_2DArray(Image bmp)
        {
            return m_1D_to_2D_array(m_BmpToBytes_mono((Bitmap)bmp), bmp.Width);
        }

        public static Bitmap m_2DArray_to_Bitmap(byte[,] bmp)
        {
            if (bmp == null)
                return null;
            return m_BytesToBmp_mono(m_2D_to_1D_array(bmp),new Size(bmp.GetLength(0),bmp.GetLength(1)));
        }

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

        public static unsafe byte[] m_BmpToBytes(Bitmap bmp)
        {
            if (bmp.Width % 4 == 0)
            {
                // Make sure that the width of the bmp
                //is dividable by 4
                //B,G,R
                bData = bmp.LockBits(new Rectangle(new Point(), bmp.Size),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format24bppRgb);
                // number of bytes in the bitmap
                byteCount = bData.Stride * bmp.Height;
                bmpBytes = new byte[byteCount];

                // Copy the locked bytes from memory
                Marshal.Copy(bData.Scan0, bmpBytes, 0, byteCount);
                // don't forget to unlock the bitmap!!
                bmp.UnlockBits(bData);
                return bmpBytes;
            }
            else
            {
                MessageBox.Show("The Picture width isn't dividable by 4;", "Error", 0, MessageBoxIcon.Error);
                return new byte[0];
            }
        }
        //-------------------------
        static byte[] mono;
        static int index;
        public static unsafe byte[] m_BmpToBytes_mono(Bitmap bmp)
        {
            if (bmp == null)
                return null;
            if (bmp.Width % 4 == 0)
            {
                // Make sure that the width of the bmp
                //is dividable by 4
                //B,G,R
                bData = bmp.LockBits(new Rectangle(new Point(), bmp.Size),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format24bppRgb);
                // number of bytes in the bitmap
                byteCount = bData.Stride * bmp.Height;
                bmpBytes = new byte[byteCount];

                // Copy the locked bytes from memory
                Marshal.Copy(bData.Scan0, bmpBytes, 0, byteCount);
                // don't forget to unlock the bitmap!!
                bmp.UnlockBits(bData);
                mono = new byte[bmpBytes.Length / 3];
                index = 0;
                for (int i = 0; i < bmpBytes.Length; i += 3)
                {
                    mono[index++] = (byte)((bmpBytes[i] + bmpBytes[i + 1] + bmpBytes[i + 2]) / 3);
                }
                return mono;
            }
            else
            {
                MessageBox.Show("The Picture width isn't dividable by 4;", "Error", 0, MessageBoxIcon.Error);
                return new byte[0];
            }
        }
        static Bitmap bmp;
        public static unsafe Bitmap m_BytesToBmp(byte[] bmpBytes, Size imageSize)
        {
            if (bmpBytes == null)
                return null;
            if (imageSize.Width % 4 == 0)
            {
                bmp = new Bitmap(imageSize.Width, imageSize.Height);

                bData = bmp.LockBits(new Rectangle(new Point(), bmp.Size),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format24bppRgb);
                // Copy the bytes to the bitmap object
                Marshal.Copy(bmpBytes, 0, bData.Scan0, bmpBytes.Length);
                bmp.UnlockBits(bData);
                return bmp;
            }
            else
            {
                MessageBox.Show("The Picture width isn't dividable by 4;", "Error", 0, MessageBoxIcon.Error);
                return null;
            }
        }
        //----------------------------
        static byte[] temp;
        static Bitmap bmp_mono;
        public static unsafe Bitmap m_BytesToBmp_mono(byte[] bmpBytes, Size imageSize)
        {
            if (imageSize.Width % 4 == 0)
            {
                temp = new byte[bmpBytes.Length * 3];
                int index = 0;
                for (int i = 0; i < temp.Length; i += 3)
                {
                    temp[i] = bmpBytes[index];
                    temp[i + 1] = bmpBytes[index];
                    temp[i + 2] = bmpBytes[index++];
                }
                bmp_mono = new Bitmap(imageSize.Width, imageSize.Height);

                bData = bmp_mono.LockBits(new Rectangle(new Point(), bmp_mono.Size),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format24bppRgb);
                // Copy the bytes to the bitmap object
                Marshal.Copy(temp, 0, bData.Scan0, temp.Length);
                bmp_mono.UnlockBits(bData);
                return bmp_mono;
            }
            else
            {
                MessageBox.Show("The Picture width isn't dividable by 4;", "Error", 0, MessageBoxIcon.Error);
                return null;
            }
        }
        //--------------------------
        static Bitmap tempbmp1;
        static BitmapData data2;
        static BitmapData data;
        public static Image m_binary_Erosion(Bitmap bmp, byte[,] sel)
        {
            tempbmp1 = (Bitmap)bmp.Clone();
            data2 = tempbmp1.LockBits(new Rectangle(0, 0, tempbmp1.Width, tempbmp1.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            byte[,] sElement = sel;

            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                byte* tptr = (byte*)data2.Scan0;

                ptr += data.Stride + 3;
                tptr += data.Stride + 3;

                int remain = data.Stride - data.Width * 3;

                for (int i = 1; i < data.Height - 1; i++)
                {
                    for (int j = 1; j < data.Width - 1; j++)
                    {
                        if (ptr[0] != 255)
                        {
                            byte* temp = ptr - data.Stride - 3;
                            bool condition = false;
                            for (int k = 0; k < 3; k++)
                            {
                                for (int l = 0; l < 3; l++)
                                {
                                    if (sElement[k, l] == 1 && temp[0] == 255)
                                    {
                                        condition = true;
                                    }
                                    temp += 3;
                                }
                                temp += data.Stride - 9;
                            }
                            if (condition)
                            {
                                tptr[0] = tptr[1] = tptr[2] = 255;
                            }
                        }
                        ptr += 3;
                        tptr += 3;
                    }
                    ptr += remain + 6;
                    tptr += remain + 6;
                }
            }
            bmp.UnlockBits(data);
            tempbmp1.UnlockBits(data2);
            return tempbmp1;
        }
    }
}
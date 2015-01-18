using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;


namespace Real_Time_Biometric_Face_Recognition_System
{
    public class c_Image_acquisition
    {
        // property variables
        static int m_TimeToCapture_milliseconds = 100;
        static int m_Width = 720;
        static int m_Height = 576;
        static int mCapHwnd;
        static ulong m_FrameNumber = 10;
        //----------------------------------------

        // global variables to make the video capture go faster
        static IDataObject tempObj;
        static Image tempImg;

        public static byte[,] g_Difference;
        public static Image g_bckGrnd_colored_img;
        public static byte[,] g_bckGrnd_colored_byte;
        //public static byte[] g_Difference2;
        //public static Image g_bckGrnd_colored_img2;
        static List<byte[,]> g_bckGrnd_bytes = new List<byte[,]>(20);
        //--------------------------------------------------
        #region API Declarations

        [DllImport("user32", EntryPoint = "SendMessage")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        [DllImport("avicap32.dll", EntryPoint = "capCreateCaptureWindowA")]
        public static extern int capCreateCaptureWindowA(string lpszWindowName, int dwStyle, int X, int Y, int nWidth, int nHeight, int hwndParent, int nID);

        [DllImport("user32", EntryPoint = "OpenClipboard")]
        public static extern int OpenClipboard(int hWnd);

        [DllImport("user32", EntryPoint = "EmptyClipboard")]
        public static extern int EmptyClipboard();

        [DllImport("user32", EntryPoint = "CloseClipboard")]
        public static extern int CloseClipboard();

        #endregion

        #region API Constants

        public static uint WM_USER = 1024;

        public static uint WM_CAP_CONNECT = 1034;
        public static uint WM_CAP_DISCONNECT = 1035;
        public static uint WM_CAP_GET_FRAME = 1084;
        public static uint WM_CAP_COPY = 1054;

        public static uint WM_CAP_START = WM_USER;

        public static uint WM_CAP_DLG_VIDEOFORMAT = WM_CAP_START + 41;
        public static uint WM_CAP_DLG_VIDEOSOURCE = WM_CAP_START + 42;
        public static uint WM_CAP_DLG_VIDEODISPLAY = WM_CAP_START + 43;
        public static uint WM_CAP_GET_VIDEOFORMAT = WM_CAP_START + 44;
        public static uint WM_CAP_SET_VIDEOFORMAT = WM_CAP_START + 45;
        public static uint WM_CAP_DLG_VIDEOCOMPRESSION = WM_CAP_START + 46;
        public static uint WM_CAP_SET_PREVIEW = WM_CAP_START + 50;

        #endregion

        #region NOTES

        /*
		 * If you want to allow the user to change the display size and 
		 * color format of the video capture, call:
		 * SendMessage (mCapHwnd, WM_CAP_DLG_VIDEOFORMAT, 0, 0);
		 * You will need to requery the capture device to get the new settings
		*/

        #endregion

        public int TimeToCapture_milliseconds
        {
            get
            { return m_TimeToCapture_milliseconds; }

            set
            { m_TimeToCapture_milliseconds = value; }
        }

        public int CaptureHeight
        {
            get
            { return m_Height; }

            set
            { m_Height = value; }
        }

        public int CaptureWidth
        {
            get
            { return m_Width; }

            set
            { m_Width = value; }
        }

        public ulong FrameNumber
        {
            get
            { return m_FrameNumber; }

            set
            { m_FrameNumber = value; }
        }

        //-------------------------------------------------
        public static bool Start(ulong FrameNum, IntPtr handle)
        {
            try
            {
                // for safety, call stop, just in case we are already running
                Stop();
                Clipboard.Clear();
                // setup a capture window
                mCapHwnd = capCreateCaptureWindowA("WebCap", 0, 0, 0, m_Width, m_Height, handle.ToInt32(), 0);
                // connect to the capture device
                
                SendMessage(mCapHwnd, WM_CAP_CONNECT, 0, 0);
                SendMessage(mCapHwnd, WM_CAP_SET_PREVIEW, 0, 0);
                SendMessage(mCapHwnd, WM_CAP_DLG_VIDEOSOURCE, 0, 0);
                // set the frame number
                m_FrameNumber = FrameNum;
                // set the timer information
                return true;
            }
            catch (Exception excep)
            {
                MessageBox.Show
                    ("An error ocurred while starting the video capture. Check that your webcamera is connected properly and turned on.\r\n\n" 
                    + excep.Message);
                Stop();
                return false;
            }
        }

        public static void Stop()
        {
            SendMessage(mCapHwnd, WM_CAP_DISCONNECT, 0, 0);
        }

        //--------------------------------------------------
       
        public static Image m_Capture()
        {
            try
            {
                // get the next frame;
                SendMessage(mCapHwnd, WM_CAP_GET_FRAME, 0, 0);

                // copy the frame to the clipboard
                SendMessage(mCapHwnd, WM_CAP_COPY, 0, 0);

                // paste the frame into the event args image

                // get from the clipboard
                tempObj = Clipboard.GetDataObject();
                tempImg = (System.Drawing.Bitmap)tempObj.GetData(System.Windows.Forms.DataFormats.Bitmap);
                //int t1 = DateTime.Now.Millisecond;
                GC.Collect();
                //int t2 = DateTime.Now.Millisecond;
                //int t = t2 - t1;
                /*
                * For some reason, the API is not resizing the video
                * feed to the width and height provided when the video
                * feed was started, so we must resize the image here
                */
                //x.WebCamImage = tempImg.GetThumbnailImage(m_Width, m_Height, null, System.IntPtr.Zero);
            }
            catch (Exception excep)
            {
                MessageBox.Show("An error ocurred while capturing the video image. The video capture will now be terminated.\r\n\n" + excep.Message);
                Stop(); // stop the process
            }
            return tempImg;
        }

        public static void m_Snapshot_Background()
        {
            g_bckGrnd_bytes.Clear();
            g_bckGrnd_colored_img = m_Capture();
            g_bckGrnd_colored_byte = c_Preprocessing.m_Bitmap_to_2DArray(g_bckGrnd_colored_img);

            for (int a = 0; a < g_bckGrnd_bytes.Capacity; a++)
            {
                g_bckGrnd_bytes.Add(c_Preprocessing.m_Bitmap_to_2DArray((Bitmap)m_Capture()));
            }

            g_Difference = new byte[g_bckGrnd_colored_byte.GetLength(0),
                g_bckGrnd_colored_byte.GetLength(1)];
            byte min, max;
            
            for (int j = 0; j < g_Difference.GetLength(1); j++)
            {
                for (int i = 0; i < g_Difference.GetLength(0); i++)
                {
                    min = g_bckGrnd_bytes[0][i,j];
                    max = g_bckGrnd_bytes[0][i,j];
                    for (int q = 1; q < g_bckGrnd_bytes.Capacity; q++)
                    {
                        if (min > g_bckGrnd_bytes[q][i,j])
                            min = g_bckGrnd_bytes[q][i,j];
                        if (max < g_bckGrnd_bytes[q][i,j])
                            max = g_bckGrnd_bytes[q][i,j];
                    }
                    g_Difference[i,j] = (byte)(max - min);
                }
            }
            g_bckGrnd_bytes.Clear();
            Program.main_frm.txt_modelName.Text = "Ready";
        }

        /*public static void m_Snapshot_Background_2()
        {
            g_bckGrnd.Clear();
            g_bckGrnd_colored_img2 = m_Capture();
            g_bckGrnd_bytes = new List<byte[]>(g_bckGrnd.Capacity);

            for (int a = 0; a < g_bckGrnd.Capacity; a++)
            {
                g_bckGrnd.Add(m_Capture());
                g_bckGrnd_bytes.Add(c_Preprocessing.m_BmpToBytes((Bitmap)g_bckGrnd[a]));
            }

            g_Difference2 = new byte[g_bckGrnd_bytes[0].Length];
            byte min, max;
            for (int i = 0; i < g_Difference2.Length; i++)
            {
                min = g_bckGrnd_bytes[0][i];
                max = g_bckGrnd_bytes[0][i];
                for (int j = 1; j < g_bckGrnd_bytes.Capacity; j++)
                {
                    if (min > g_bckGrnd_bytes[j][i])
                        min = g_bckGrnd_bytes[j][i];
                    if (max < g_bckGrnd_bytes[j][i])
                        max = g_bckGrnd_bytes[j][i];
                }
                g_Difference2[i] = (byte)(max - min);
            }
            g_bckGrnd_bytes.Clear();
            g_bckGrnd.Clear();
        }*/

        public static bool m_Is_Similar(Image p1,Image p2)
        {

            byte[] pic1 = c_Preprocessing.m_BmpToBytes_mono((Bitmap)p1);
            byte[] pic2 = c_Preprocessing.m_BmpToBytes_mono((Bitmap)p2);
            int error_counter = 0;
            for (int i = 0; i < pic1.Length; i++)
            {
                if (pic1[i] - pic2[i] > 4)
                    error_counter++;
            }
            int eee = error_counter * 100 / pic1.Length;
            if (eee > 4)
                return false;
            else
                return true;
        }
    }
}
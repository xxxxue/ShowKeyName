using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShowKeyName
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                // 处理异常
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += (_, ex) => HandleException((Exception)ex.Exception);
                AppDomain.CurrentDomain.UnhandledException += (_, ex) => HandleException((Exception)ex.ExceptionObject);


                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

        /// <summary>
        /// 处理异常
        /// -------
        /// 注意: 子线程中的异常,
        /// 需要先在子线程中用try catch 捕获,
        /// 再用 this.BeginInvoke(() => throw e); 
        /// 抛给主线程,才能在这里捕获到....
        /// </summary>
        /// <param name="error"></param>
        static void HandleException(Exception error)
        {
            string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Error.txt");
            string strDateInfo = "异常：" + DateTime.Now.ToString() + "\r\n";
            var str = $"{strDateInfo}Application UnhandledException:{error.Message};\n\r堆栈信息:{error.StackTrace}\r\n";
            File.AppendAllText(_filePath, str);

            MessageBox.Show(str, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

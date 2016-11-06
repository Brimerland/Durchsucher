using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GdaTools
{
    public class FileTools
    {
        [DllImport("shell32.dll", EntryPoint = "ShellExecute")]
        public static extern long ShellExecute(int hwnd, string cmd, string file, string param1, string param2, int swmode);

        public static void OpenInExplorer(String fileName)
        {
            ShellExecute(0, "open", "explorer.exe", string.Format("/select,\"{0}\"", fileName), "", 5);
        }

        public static void OpenInDefaultApplication(string fileName)
        {
            ShellExecute(0, "open", fileName, "", "", 5);
        }
    }
}

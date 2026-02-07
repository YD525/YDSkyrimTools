using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace YDSkyrimTools.xTranslatorManager
{
    public class ProcessHelper
    {
        public static Process TryGetGameProcess(Process[] Array)
        {
            foreach (var Get in Array)
            {
                string ProcessPath = null;
                StringBuilder NewGetPath = new StringBuilder(255);

                if (WinApi.KrnGetProcessPath(uint.Parse(Get.Id.ToString()), NewGetPath))
                {
                    ProcessPath = NewGetPath.ToString();
                }

                try
                {
                    string GetPath = ProcessPath;
                    GetPath = GetPath.Substring(0, GetPath.LastIndexOf(@"\"));

                    foreach (var GetFile in Directory.GetFiles(GetPath))
                    {
                        if (File.Exists(GetFile))
                        {
                            string GetFileName = GetFile.Substring(GetFile.LastIndexOf(@"\") + @"\".Length);

                            if (GetFileName.StartsWith("steam_api"))
                            {
                                return Get;
                            }
                        }
                    }
                }
                catch (Exception Ex)
                {
                    var GetEx = Ex;
                }
            }

            return null;
        }
        public static string GetPorgramName(string FullPath, bool NoSuffix = true)
        {
            if (FullPath.Contains(@"\") && FullPath.Contains("."))
            {
                if (NoSuffix)
                {
                    string NextPath = FullPath.Substring(FullPath.LastIndexOf(@"\") + @"\".Length);
                    return NextPath.Substring(0, NextPath.LastIndexOf("."));
                }
                else
                {
                    return FullPath.Substring(FullPath.LastIndexOf(@"\") + @"\".Length);
                }

            }
            return string.Empty;
        }
        public static bool CheckProcess(string ProcessName)
        {
            foreach (var GetPrItem in Process.GetProcesses())
            {
                if (GetPrItem.ProcessName.ToLower().Equals(ProcessName.ToLower()))
                {
                    return true;
                }

            }
            return false;
        }

        public static int RunProcess(string FileName, string Param)
        {
            int Return = -1;

            Process NewProcess = new Process();
            ProcessStartInfo NewProcessStartInfo = new ProcessStartInfo(FileName, Param);
            NewProcessStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            NewProcess.StartInfo = NewProcessStartInfo;
            NewProcess.Start();

            return Return;
        }
    }
}

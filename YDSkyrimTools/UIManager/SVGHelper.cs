
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using YDSkyrimTools.ConvertManager;
using YDSkyrimTools.SkyrimModManager;

namespace YDSkyrimTools.UIManager
{
    public class SVGHelper
    {

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int UrlCreateFromPath(string path, StringBuilder url, ref int urlLength, int reserved);
       
        public static Uri FileUrlFromPath(string path)
        {
            const string prefix = @"\\";

            const string extended = @"\\?\";

            const string extendedUnc = @"\\?\UNC\";

            const string device = @"\\.\";

            const StringComparison comp = StringComparison.Ordinal;

            if (path.StartsWith(extendedUnc, comp))

            {

                path = prefix + path.Substring(extendedUnc.Length);

            }
            else if (path.StartsWith(extended, comp))

            {

                path = prefix + path.Substring(extended.Length);

            }
            else if (path.StartsWith(device, comp))

            {

                path = prefix + path.Substring(device.Length);

            }

            int len = 1;

            var buffer = new StringBuilder(len);

            int result = UrlCreateFromPath(path, buffer, ref len, 0);

            if (len == 1) Marshal.ThrowExceptionForHR(result);

            buffer.EnsureCapacity(len);

            result = UrlCreateFromPath(path, buffer, ref len, 0);

            if (result == 1) throw new ArgumentException("Argument is not a valid path.", "path");

            Marshal.ThrowExceptionForHR(result);

            return new Uri(buffer.ToString());

        }

        public static string GetCurrentPath()
        {
            return DeFine.GetFullPath(@"\");
        }


        public static Uri ShowSvg(ICOResources Ico)
        {
            try
            {
                string LocalPath = GetCurrentPath() + DeFine.ResourcesPath + Ico.ToString() + ".svg";
                if (File.Exists(LocalPath))
                {
                    return FileUrlFromPath(LocalPath);
                }
            }
            catch { return new Uri(""); }
            return new Uri("");
        }

        public static Uri ShowSvg(string IcoName)
        {
            try
            {
                string LocalPath = GetCurrentPath() + DeFine.ResourcesPath + IcoName + ".svg";
                if (File.Exists(LocalPath))
                {
                    return FileUrlFromPath(LocalPath);
                }
            }
            catch { return new Uri(""); }
            return new Uri("");
        }


        public static void ShowPic(Image Pic, string SourceName)
        {
            string LocalPath = GetCurrentPath() + DeFine.ResourcesPath + @"PicPath\" + SourceName;

            if (File.Exists(LocalPath))
            {
                UIHelper.SecurityUIThread(Pic, new Action(() =>
                {
                    Pic.Source = ConvertHelper.BytesToBitmapImage(DataHelper.GetBytesByFilePath(LocalPath));
                }));
            }
        }

        public static string LoadImagePathCache = "";

        public static void ShowPicExtend(Image Pic, string SourceName)
        {
            string LocalPath = DeFine.GetFullPath(DeFine.ResourcesPath + @"PNG\" + SourceName);

            if (!LoadImagePathCache.Equals(LocalPath))
            {
                LoadImagePathCache = LocalPath;

                if (File.Exists(LoadImagePathCache))
                {
                    UIHelper.SecurityUIThread(Pic, new Action(() =>
                    {
                        Pic.Source = ConvertHelper.BytesToBitmapImage(DataHelper.GetBytesByFilePath(LocalPath));
                    }));
                }
            }
        }

        public static byte[] ShowPic(string SourceName)
        {
            string LocalPath = GetCurrentPath() + DeFine.ResourcesPath + @"PicPath\" + SourceName;
            return DataHelper.GetBytesByFilePath(LocalPath);
        }


    }
    public enum ICOResources
    {
        Null = 0, Baidu = 1, Code = 2, Conjunction = 3, Div = 4, Google = 5, Languages = 6, YouDao = 7, Locker = 8, UnLocker = 9, TransICO = 10, MO2ICO = 11 , Keep = 12, Stop = 13, End = 15, YellowState = 16, SettingICO = 17
    }
}

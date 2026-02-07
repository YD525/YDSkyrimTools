using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace YDSkyrimTools.DataManager
{
    public class TimeHelper
    {
        /// <summary>
        ///  时间戳[10|13]转为C#格式时间       
        /// </summary>
        public static DateTime StampToDateTime(string stamp)
        {
            try
            {
                DateTime StartDateTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
                if (stamp.Length == 10) StartDateTime.AddSeconds(long.Parse(stamp));
                return StartDateTime.AddMilliseconds(long.Parse(stamp));
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }



        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long DateTimeToStamp(System.DateTime time, int length = 13)
        {
            try
            {
                long ts = ConvertDateTimeTolong(time);
                return long.Parse(ts.ToString().Substring(0, length));
            }
            catch { return 0; }
        }



        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        private static long ConvertDateTimeTolong(DateTime DateTime)
        {
            try
            {
                return (DateTime.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary> 
        /// 获取时间戳 
        /// </summary> 
        /// <returns>UTC</returns> 
        public static long GetTimeStamp()
        {
            return DateTimeToStamp(DateTime.Now);
        }

 

        /// <summary>
        /// 转换时间戳为C#时间
        /// </summary>
        /// <param name="timeStamp">时间戳 单位：毫秒</param>
        /// <returns>C#时间</returns>
        public static DateTime ConvertTimeStampToDateTime(long TimeStamp)
        {
            DateTime StartTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime DT = StartTime.AddSeconds(TimeStamp);
            return DT;
        }

      
        /// <summary>
        /// 连接时间服务器
        /// </summary>
        /// <param name="socket">服务器接口</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns></returns>
        private static bool TryConnectToTimeServer(out Socket socket, out DateTime startTime, out string errorMsg)
        {
            string[] timeHosts = { "time-b.nist.gov" };

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建Socket
            socket.ReceiveTimeout = 10 * 1000;//设置超时时间
            errorMsg = string.Empty;
            startTime = DateTime.Now;

            // 遍历时间服务器列表
            foreach (string strHost in timeHosts)
            {
                try
                {
                    // 记录开始的时间
                    startTime = DateTime.Now;

                    var iphostinfo = Dns.GetHostEntry(strHost);
                    var ip = iphostinfo.AddressList[0];
                    //建立IPAddress对象与端口，创建IPEndPoint节点:
                    int port = 13;
                    var ipe = new IPEndPoint(ip, port);
                    //连接到服务器
                    socket.Connect(ipe);
                    // 如果连接到服务器就跳出
                    if (socket.Connected) break;
                }
                catch (Exception ex)
                {
                    errorMsg = $"时间服务器连接失败！\r\n错误信息：{ex.Message}系统提示";
                }
            }
            return socket.Connected;
        }


        /// <summary>
        /// 从服务器接收数据
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        private static StringBuilder ReceiveMessageFromServer(Socket socket)
        {
            //SOCKET同步接受数据
            byte[] receiveBytes = new byte[1024];
            int nBytes, nTotalBytes = 0;
            StringBuilder sb = new StringBuilder();
            System.Text.Encoding encoding = Encoding.UTF8;

            while ((nBytes = socket.Receive(receiveBytes, 0, 1024, SocketFlags.None)) > 0)
            {
                nTotalBytes += nBytes;
                sb.Append(encoding.GetString(receiveBytes, 0, nBytes));
            }

            return sb;
        }

        /// <summary>
        /// 更新系统时间
        /// </summary>
        /// <returns>更新结果</returns>
        public static string UpdateSystemTime()
        {
            try
            {
                var connected = TryConnectToTimeServer(out Socket socket, out var startTime, out string errorMsg);
                if (connected)
                {
                    var receivedMsg = ReceiveMessageFromServer(socket);
                    socket.Close();
                    //切割字符串
                    string[] receiveMsgList = receivedMsg.ToString().Split(' ');
                    if (receiveMsgList.Length >= 3)
                    {
                        var dateTimeValue = receiveMsgList[1] + " " + receiveMsgList[2];
                        SetLocalTime(startTime, dateTimeValue);
                    }
                }
                else
                {
                    return errorMsg;
                }
            }
            catch (Exception e)
            {
                return $"函数{nameof(UpdateSystemTime)}执行异常，{e.Message}";
            }
            return "时间已同步";
        }
        /// <summary>
        /// 设置系统时间
        /// </summary>
        /// <param name="startTime">请求服务器时的开始时间</param>
        /// <param name="dateTimeValue">服务器返回的时间</param>
        private static void SetLocalTime(DateTime startTime, string dateTimeValue)
        {
            // 得到开始到现在所消耗的时间
            TimeSpan k = DateTime.Now - startTime;
            // 减去中途消耗的时间
            DateTime updatedUtcTime = Convert.ToDateTime(dateTimeValue).Subtract(-k);

            //处置北京时间 +8时
            var updatedTime = updatedUtcTime.AddHours(8);

            //转换System.DateTime到SystemTime
            SystemTime systemTime = new SystemTime();
            systemTime.FromDateTime(updatedTime);

            //调用Win32 API设置系统时间
            Win32API.SetLocalTime(ref systemTime);
        }
    }

    /// <summary>
    /// 系统时间帮助类
    /// </summary>
    public struct SystemTime
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;

        /// <summary>
        /// 从System.DateTime转换。
        /// </summary>
        /// <param name="time">System.DateTime类型的时间。</param>
        public void FromDateTime(DateTime time)
        {
            wYear = (ushort)time.Year;
            wMonth = (ushort)time.Month;
            wDayOfWeek = (ushort)time.DayOfWeek;
            wDay = (ushort)time.Day;
            wHour = (ushort)time.Hour;
            wMinute = (ushort)time.Minute;
            wSecond = (ushort)time.Second;
            wMilliseconds = (ushort)time.Millisecond;
        }
        /// <summary>
        /// 转换为System.DateTime类型。
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);
        }
        /// <summary>
        /// 静态方法。转换为System.DateTime类型。
        /// </summary>
        /// <param name="time">SYSTEMTIME类型的时间。</param>
        /// <returns></returns>
        public static DateTime ToDateTime(SystemTime time)
        {
            return time.ToDateTime();
        }
    }

    /// <summary>
    /// 系统更新时间DLL
    /// </summary>
    public class Win32API
    {
        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SystemTime Time);
        [DllImport("Kernel32.dll")]
        public static extern void GetLocalTime(ref SystemTime Time);
    }
}

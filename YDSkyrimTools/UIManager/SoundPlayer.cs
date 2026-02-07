using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YDSkyrimTools.UIManager
{
    public class SoundPlayer
    {
        public object LockEffect = new object();
        public void PlaySound(Sounds OneSound,bool IsFromCG=false)
        {
            if (DeFine.GlobalLocalSetting.PlaySound)
            {
                if (!IsFromCG)
                {
                  
                }

                new Thread(() =>
                {
                    string AutoStr = "常陆";
                    if (new Random(Guid.NewGuid().GetHashCode()).Next(0, 100) >= 50)
                    {
                        AutoStr = "";
                    }
                    MediaPlayer NewPlayer = new MediaPlayer();
                    NewPlayer.Open(new Uri(DeFine.GetFullPath(DeFine.ResourcesPath) + @"\Wav\" + OneSound.ToString() + AutoStr + ".wav"));
                    NewPlayer.Play();
                    Thread.Sleep(5000);
                    NewPlayer.Close();
                }).Start();
            }
        }
    }

    public enum Sounds
    { 
      Null=0, 停止监控进程 =1, 实时信息=2, 当前字典=3, 挂起事务 = 4, 是否要确认 = 5, 显示悬浮层 = 6, 更多功能选中语音 = 7, 检测到进程 = 8, 第一阶段完成 = 9, 第二阶段完成 = 10, 终止线程 = 11, 继续执行事务 = 12, 英汉互译选中语音 = 13, 退出程序确认 = 14, 隐藏悬浮层 = 15, 开始执行翻译 = 16, 发现错误 = 17, 决定窗口 = 18, Hentai = 19
    }
}

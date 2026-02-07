using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YDSkyrimTools.ConvertManager;
using YDSkyrimTools.SkyrimModManager;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace YDSkyrimTools.TranslateCore
{
    public class WordProcess
    {
        public delegate void TranslateMsg(string EngineName, string Text, string Result);

        public static TranslateMsg SendTranslateMsg;

        public bool CheckTranslate(string Source)
        {
            foreach (var Get in new Char[26] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'g', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' })
            {
                if (Source.Contains(Get.ToString().ToLower()))
                {
                    return true;
                }
                if (Source.Contains(Get.ToString().ToUpper()))
                {
                    return true;
                }
            }
            return false;
        }
        public void FormatText(ref string Str)
        {
            Str = Str.Replace("。", ".").Replace("，", ",").Replace("(", " ( ").Replace(")", " ) ").Replace("（", " ( ").Replace("）", " ) ").Replace("【", "[").Replace("】", "]").Replace("《", "<").Replace("》", ">").Replace("‘", " ‘ ").Replace("'", " ‘ ");
        }

        public string QuickTranslate(string OneWord, BDLanguage From, BDLanguage To, int WordType = 0)
        {
            string SqlOrder = "Select [Result] From Languages Where [From] = '{0}' And [To] = '{1}' And Type = '{2}' And Text = '{3}' COLLATE NOCASE";

            string TranslateWord = ConvertHelper.ObjToStr(DeFine.GlobalDB.ExecuteScalar(string.Format(SqlOrder, (int)From, (int)To, WordType, OneWord)));

            if (TranslateWord.Trim().Length > 0)
            {
                FormatText(ref TranslateWord);
                return "（" + TranslateWord.Trim().Replace("\r\n", "") + "）";
            }
            else
            {
                return null;
            }
        }
        public string GetTranslate(ref List<EngineProcessItem> EngineMsgs,string OneWord, BDLanguage From, BDLanguage To,int WordType = 0)
        {
            string TempLine = "";
            string RichText = "";

            for (int i = 0; i < OneWord.Length; i++)
            {
                string Char = OneWord.Substring(i, 1);
                        
                if (Char == "," || Char == "." || Char == "/" || Char == "<" || Char == ">" || Char == "|" || Char == "!" || Char == "\"" || Char == "'")
                {
                    string GetResult = QuickTranslate(TempLine, From, To, WordType);

                    if (GetResult != null)
                    {
                        EngineMsgs.Add(new EngineProcessItem("Languages", TempLine, GetResult, 1));
                    }

                    if (GetResult != null)
                    {
                        TempLine = GetResult;

                        RichText += TempLine + Char;
                        TempLine = string.Empty;
                    }
                    else
                    {
                        RichText += TempLine + Char;
                        TempLine = string.Empty;
                    }
                }
                else
                {
                    TempLine += Char;
                }
            }


            if (TempLine.Trim().Length > 0)
            {
                string GetResult = QuickTranslate(TempLine, From, To, WordType);

                if (GetResult != null)
                {
                    EngineMsgs.Add(new EngineProcessItem("Languages", TempLine, GetResult, 2));
                }

                if (GetResult != null)
                {
                    RichText += GetResult;
                }
                else
                {
                    RichText += TempLine;
                }
            }


            return RichText.Trim();
        }

        public string ProcessWordGroups(ref List<EngineProcessItem> EngineMsgs,string Content, BDLanguage From, BDLanguage To)
        {
            if (!DeFine.PhraseEngineUsing) return Content;
            int MaxLength = 2;

            string RichText = "";

            string WordGroup = "";

            string TempStr = "";

            foreach (var Get in Content.Split(' '))
            {
                if (Get.Trim().Length > 0)
                {
                    if (MaxLength > 0)
                    {
                        MaxLength--;
                        WordGroup += Get + " ";
                        TempStr += Get + " ";
                    }

                    if (MaxLength == 0)
                    {
                        MaxLength = 2;

                        var Result = QuickTranslate(WordGroup.Trim(), From, To, 1);

                        if (Result != null)
                        {
                            EngineMsgs.Add(new EngineProcessItem("Languages", WordGroup.Trim(), Result, 0));
                        }  

                        if (Result != null)
                        {
                            RichText += Result + " ";
                        }
                        else
                        {
                            RichText += WordGroup + " ";
                        }
                      

                        WordGroup = string.Empty;
                        TempStr = string.Empty;
                    }
                }
            }

            if (TempStr.Trim().Length > 0)
            {
                RichText += TempStr;
                TempStr = string.Empty;
            }

           return RichText.Trim();
        }


        public bool HasChinese(string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
        }

        public bool HasEnglish(string str)
        {
            char[] Chars = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
           foreach (var Get in Chars)
           {
                if (str.Contains(Get.ToString().ToUpper()))
                {
                    return true;
                }
                if (str.Contains(Get.ToString().ToLower()))
                {
                    return true;
                }
           }
            return false;
        }
        public int GetEnglishCount(string str)
        {
            int EngCount = 0;

            char[] Chars = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            for(int i=0;i<str.Length;i++)
            {
                string GetChar = str.Substring(i,1);
                foreach (var Get in Chars)
                {
                    if (GetChar.Equals(Get.ToString().ToUpper()))
                    {
                        EngCount++;
                    }
                    if (GetChar.Equals(Get.ToString().ToLower()))
                    {
                        EngCount++;
                    }
                }
               
            }
            return EngCount;
        }
        public string ProcessContent(string Content)
        {
            Content = Content.Replace("\u200b", "").Replace("\u200B", "").Replace("他妈的", "该死的").Replace("去精液", "射精液").Replace("辣味", "热乎乎的").Replace("负载", "正在").Replace("不想精液", "不想让精液").Replace("快点精液", "快点射精液").Replace("上帝", "天啊").Replace("肉棒in", "肉棒在").Replace("precum", "先走汁").Replace("还不精液", "还不射精液").Replace("烟蒂", "臀部").Replace("厚的", "浓稠的").Replace("轴", "阳物");

            if (Content == "Fuck")
            {
                Content = "做爱";
            }
            if (Content == "Fucking")
            {
                Content = "性交";
            }
            if (Content == "fuck")
            {
                Content = "做爱";
            }
            if (Content == "fucking")
            {
                Content = "性交";
            }

            if (Content == "welcome")
            {
                Content = "欢迎";
            }
            else
            if (Content == "welcoming")
            {
                Content = "受欢迎的";
            }
            else
            if (Content == "welcomes")
            {
                Content = "迎接";
            }
            else
            if (Content == "Clamping down on the invading金属parts, I squeal with a mix of pain and pleasure.")
            {
                Content = "肉穴夹住侵入的金属部分，我发出痛苦和快乐交织的尖叫。";
            }
            else
            if (Content == "A shiver of desire and a tremor of fear run通过you.‘I want you里面of me‘, you whisper.")
            {
                Content = "欲望的颤抖和性饥渴从你身边经过。“我想要你在我里面大量的内射”，你低声说。";
            }
            else
            if (Content == "P-please... use my lewd身体any way you please... every opening aches to be 填满...")
            {
                Content = "拜托…请你使用我那淫乱的身体吧,随便你怎么用……每个洞都渴望被填满...";
            }
            else
            if (Content == "Oh Gods... nghhhhhh! Ughhh... I must have at least a foot of horsecock up my(919)now...")
            {
                Content = "天哪……呜呜呜！ 呃……我的（919）现在至少有一英尺高了……";
            }
            else
            if (Content == "Nghhh! Ughh! He‘s really slamming my(919)hard!")
            {
                Content = "啊! 啊! 他真的在猛烈抨击我的 (919)！";
            }
            else
            if (Content == "Aah... ahhh... what am I doing? What would my friends think of me, if they saw me like this?")
            {
                Content = "啊……啊……我在做什么？ 如果我的朋友看到我这样，他们会怎么看我？";
            }
            else
            if (Content == "Nghhh! Ughh! they‘re really slamming(919)‘s(929)s hard!")
            {
                Content = "唔！ 啊！ 他们真的用力猛击 (919) 的 (929)！";
            }
            else
            if (Content == "((919))Oh oh fuck! Oh it‘s too big! Ow ow ow! Pleeeeassse get off me!")
            {
                Content = "((919))哦该死的！ 哦，它太大了！ 呜呜呜！ 请放开我！";
            }
            else
            if (Content == "You squeal and moan around the(919)in your throat as the wolf‘s(929)(939)bumps at your cervix with each thrust.")
            {
                Content = "当狼的 (929)(939) 每次推力撞击您的子宫颈时，您会在喉咙中的 (919) 周围尖叫和呻吟。";
            }
            else
            if (Content == "Ungghhh, ungghhh, unggghhh, unggghhh, unggghhh, ungghhh,(919), oh Gods, fuck, fuck, fuck, fuck...")
            {
                Content = "好热,好热!,饥渴!,受不了!!,想被上!!!,(919),天啊,做爱,做爱,做爱,做爱...";
            }
            else
            if (Content == "Oh Gods... nghhhhhh! Ughhh... You must have at least a foot of horsecock up your(919)now...")
            {
                Content = "天哪……呜呜呜！ 呃……你的（919）现在至少有一英尺高了……";
            }
            else
            if (Content == "The(919)in my mouth‘s musk is as thick as the(929)that is punishing me from behind.")
            {
                Content = "我嘴里的(919) 和从我屁股后面惩罚我的(929)一样浓。";
            }
            else
            if (Content == "((919))Oh oh fuck! Oh it‘s too big! Ow ow ow ow! Pleeeeassse get off me!")
            {
                Content = "((919))哦该死的！ 哦，它实在太大了！ 呜呜呜！ 请放开我！";
            }
            else
            if (Content == "The perverse thought of being knocked up by a monster is arousing.(919)‘s cumming!")
            {
                Content = "被怪物插进去的离奇念头真刺激。(919)高潮了！";
            }
            else
            if (Content == "Oh my God, he‘s(919)in you! You‘re cumming so hard! Ooooooooooooooohhhhhhhhhhhhhhhhhh FUCK!!!!!")
            {
                Content = "哦 天啊,他的(919)在你的里面！他射得太多了肯定会怀孕的！ 呜呜呜呜呜！！！！！！";
            }
            else
            if (Content == "Ohhh! Please don‘t.... huuugh... you can‘t do this! Aahhhh! I can‘t do this!")
            {
                Content = "哦！ 请不要……呵呵……你不能这样做！ 啊啊啊！ 我做不到！";
            }
            else
            if (Content == "You‘re cumming with the horse! How shameful!")
            {
                Content = "马的肉棒让你高潮了！ 多么的可耻！";
            }
            else
            if (Content == "((919))Ungghhh, unggghhh, unggghhh, unggghhh, ungghhh,(929), oh Gods, fuck, fuck, fuck, fuck...")
            {
                Content = "(919)好热,好热!,饥渴!,受不了!!,(929),天啊,做爱,做爱,做爱,做爱...";
            }
            else
            if (Content == "Ugh! Oh Gods, I didn‘t say you could do that! Ahh ah! Well... it does feel pretty good...")
            {
                Content = "啊! 天哪，我没说你能做到！ 啊啊啊！ 嗯……感觉还不错……";
            }
            else
            if (Content == "(919)trembles nervously. Is she really going to do this?")
            {
                Content = "(919)紧张地发抖。她真的要这么做吗？";
            }
            else
            if (Content == "Gods! Ahhh! It‘s so big it‘s stretching me! Go slow, please!")
            {
                Content = "天！啊！ 它太大了，它把我拉长了！ 请慢慢来！";
            }
            else
            if (Content == "((919))Ah, ah, ah, ah, Gods, are all bandits as virile as you?")
            {
                Content = "(919)啊!啊!啊,啊,啊,啊,啊,天啊,土匪都像你这么有男子气概吗？";
            }
            else
            if (Content == "Nghhh! Ughh! He‘s really slamming your(919)hard!")
            {
                Content = "啊! 啊! 他真的在猛烈抨击你的(919)！";
            }
            else
            if (Content == "Wow, you‘ve done this before, haven‘t you? he asks, sliding in. You flush with embarrassment. Yes, you gasp.")
            {
                Content = "哇,你刚刚被人玩过屁股了,不是吗? 他问罢滑入了进去. 你尴尬地脸红了. 是的,你倒吸一口凉气。";
            }
            else
            if (Content == "Loud slapping echoes as his hips pound进入me ass,然后是我快乐的尖叫.")
            {
                Content = "当他的臀部敲打我的屁股时响亮的拍打声回响,然后是我快乐的尖叫.";
            }
            else
            if (Content == "Lost to my 性高潮,我短暂地记录了周围的噪音,低语,喘息.我被监视了吗？")
            {
                Content = "忍不住就潮吹了,我短暂地记录了周围的声音,低语,喘息.我公开发情被看到了吗？";
            }
            else
            if (Content == "兴奋得脸红头晕,我尖叫； Tell me I‘m the best fuck you‘ve ever had!")
            {
                Content = "兴奋得脸红头晕,我尖叫；告诉我我是你有过的最好的该死的！";
            }
            else
            if (Content == "It‘s so dangerous and exciting that I immediately 精液,大声呻吟.")
            {
                Content = "大量的精液它是如此危险和令人兴奋的,以至于我立即大声呻吟";
            }
            else
            if (Content == "卸下我身上的（919）... I爱feeling you精液in my(929)...")
            {
                Content = "卸下我身上的(919)... 我爱感受你的精液在我的(929)里面...";
            }
            else
            if (Content == "(919)rides me steadily and I feel my性高潮building.几乎there...")
            {
                Content = "(919)稳稳的骑在我身上,再让我性高潮,几乎那里已经...";
            }
            else
            if (Content == "我有两个潮吹(919)pumme的提示")
            {
                Content = "我有两个潮吹(919)痉挛的提示";
            }
            else
            if (Content == "开枪吧!我想精液而得到creampied !")
            {
                Content = "快点射进来我忍不了了!我想被中出大量的精液!";
            }
            else
            if (Content == "I‘m nude and bouncing on(919)‘s(929).我脸上有（939）和（949）.")
            {
                Content = "我是赤裸和弹跳(919)的(929).我脸上有（939）和（949）.";
            }
            else
            if (Content == "My heart flutters in my 胸部. He‘s getting close, soon he‘s going to get me 怀孕的...")
            {
                Content = "我的心脏剧烈的跳动在我的胸部.他越来越近了，很快他就会确保我(让我)怀孕的...";
            }
            else
            if (Content == "天啊！对对嗯...这么好... Don‘t pull out... I‘m几乎there...嗯..嗯..啊...")
            {
                Content = "天啊！对对嗯...这么好... 嗯嘛请不要拔出来... 我几乎这里很...嗯..嗯..啊...";
            }
            else
            if (Content == "我的（919）（929）每次撞击都会震动, my whole身体glistening with sweat as I 精液.")
            {
                Content = "我的（919）（929）每次撞击都会震动, 我的整个身体在我高潮时汗流浃背";
            }
            else
            if (Content == "(919)Oh Gods! Yes! Yes! Uhhh... So good... Don‘t pull out... I‘m几乎there... Uh... uh... ahhh.")
            {
                Content = "(919)哦天啊! 好! 好! 啊啊啊... 非常棒... 别！不要拔出来... 我几乎这里... 啊... 啊... 要去了.";
            }
            else
            if (Content == "((919))I‘ll let you精液anywhere you want. In my(929)... Even in my(939)...")
            {
                Content = "((919))我会让你的精液到达任何你想要的地方。 在我的（929）...甚至在我的（939）...";
            }
            else
            if (Content == "(919)feels巨大的amounts of(929)enter her,一些水从她身上滴落（939）,顺着她的大腿.")
            {
                Content = "(919)感受巨大的,长长的(929)进入她的体内,一些爱液从她身上滴落(939),顺着她的大腿.";
            }
            else
            if (Content == "(919)thrusts his knot进入(929), instantly bringing her to climax.")
            {
                Content = "(919) 将他的肉棒插入 (929)，确保立刻将她带到高潮。";
            }

            if (Content.Contains("一个小块降低"))
            {
                Content = Content.Replace("一个小块降低", "降低一个肉洞");
            }

            return Content;
        }

        public string CurrentLine = "";
        public string ProcessWords(ref List<EngineProcessItem> EngineProcessItems, string Content, BDLanguage From, BDLanguage To)
        {
            if (Content.Trim().Length == 0) return string.Empty;
            Content = Content.Replace("-", " - ");
            //‘s 的 //in 在里面
            Content = Content.Replace("\u200b", "").Replace("\u200B", "").Replace("他妈的", "该死的").Replace("去精液", "射精液").Replace("辣味", "热乎乎的").Replace("负载", "正在").Replace("不想精液", "不想让精液").Replace("快点精液", "快点射精液").Replace("上帝", "天啊").Replace("肉棒in", "肉棒在").Replace("precum","先走汁").Replace("还不精液","还不射精液").Replace("烟蒂","臀部").Replace("厚的","浓稠的").Replace("轴","阳物");

            if (Content.Contains("u200b"))
            {
                Content = Content.Replace(@"\u200b", "");
            }

            if (CurrentLine != Content)
            {
                CurrentLine = Content;
            }
            else
            {
                CurrentLine = Content;
            }
     
            if (HasChinese(Content))
            {
                return Content;
            }

            string TempTextA = Content;

            FormatText(ref Content);

            WordProcess.SendTranslateMsg("Step0字符串修正", TempTextA, Content);

            Content = ProcessWordGroups(ref EngineProcessItems,Content,From,To);

            string RichText = "";

            if (DeFine.PhraseEngineUsing)
            {
                var Contents = Content.Split(' ');

                foreach (var Get in Contents)
                {
                    if (Get.Trim().Length > 0)
                    {
                        string GetWord = Get;

                        GetWord = GetTranslate(ref EngineProcessItems, GetWord, From, To) + " ";

                        RichText += GetWord;
                    }
                }

                RichText = RichText.Trim();
            }
            else
            {
                RichText = Content;
            }

            TranslateCache CreatTranslate = new TranslateCache(ref EngineProcessItems,RichText);

            WordProcess.SendTranslateMsg("Step1本地翻译引擎(污)", Content, CreatTranslate.Content);

            if (From == BDLanguage.EN && To == BDLanguage.CN)
            {
                if (CheckTranslate(CreatTranslate.Content))
                {
                    string TempValue = CreatTranslate.Content;

                    if (CreatTranslate.GetRemainingText(CreatTranslate.Content).Trim().Length > 0)
                    {
                        string TempText = CreatTranslate.Content;
                        CreatTranslate.Content = new LanguageHelper().EnglishToCN(CreatTranslate.Content);

                        EngineProcessItems.Add(new EngineProcessItem("YunEngine",TempText, CreatTranslate.Content,0));
                    }
                    else
                    { 
                    
                    }
                }
            }

            CreatTranslate.UsingDBWord();

            if (DeFine.DivCacheEngineUsing)
            {
                string TempStr = CreatTranslate.Content;

                DivTranslateEngine.UsingCacheEngine(ref CreatTranslate.Content);

                WordProcess.SendTranslateMsg("自定义引擎", TempStr, CreatTranslate.Content);
            }
            
            WordProcess.SendTranslateMsg("最终结果", "混合翻译", CreatTranslate.Content);

            return CreatTranslate.Content;
        }



    }

    public class EngineProcessItem
    {
        public string EngineName = "";
        public string Text = "";
        public string Result = "";
        public int State = 0;

        public EngineProcessItem(string EngineName,string Text,string Result,int State)
        {
            this.EngineName = EngineName;
            this.Text = Text;
            this.Result = Result;
            this.State = State;
        }
    }

    public enum BDLanguage
    { 
     NULL=-1,EN=0, CN=1
    }

    public class CardItem
    {
        public string Str = "";
        public string EN = "";
        public CardItem(string Str, string EN)
        {
            this.Str = Str;
            this.EN = EN;
        }
    }

    public class TranslateCache
    {
        public string Content = "";
        public List<CardItem> CardItems = new List<CardItem>();
        public CodeProcess CodeParsing =new CodeProcess();
        public ConjunctionHelper Conjunction = new ConjunctionHelper();

        public TranslateCache(ref List<EngineProcessItem> EngineMsgs,string Msg)
        {
            if (DeFine.CodeParsingEngineUsing)
            {
                this.Content = this.CodeParsing.ProcessCode(Msg);
            }
            else
            {
                this.Content = Msg;
            }

            if (DeFine.ConjunctionEngineUsing)
            {
                this.Content = this.Conjunction.ProcessStr(ref EngineMsgs,this.Content);
            }

            if (DeFine.PhraseEngineUsing)
            {
                this.Content = DetachDBWord(this.Content, ref CardItems);
            }
        }

        public string DetachDBWord(string Content,ref List<CardItem>Cards)
        {
            bool IsSign = false;

            string SignText = "";
            string RichText = "";

            int AutoAdd = 0;

            for (int i = 0; i < Content.Length; i++)
            {
                string Char = Content.Substring(i, 1);

                if (Char == "）")
                {
                    if (SignText.Trim().Length > 0)
                    {
                        AutoAdd++;

                        string NewName = "30" + AutoAdd.ToString() + "03";

                        CardItem OneCard = new CardItem(SignText, NewName);

                        Cards.Add(OneCard);

                        RichText += "（" + OneCard.EN + "）";

                        SignText = String.Empty;
                    }

                    IsSign = false;
                }
         
                if (IsSign)
                {
                    SignText += Char;
                }

                if (Char == "（")
                {
                    IsSign = true;
                }

                if (Char != "（" && Char != "）" && IsSign == false)
                {
                    RichText += Char;
                }
            }


            return RichText;
        }

        public void UsingDBWord()
        {
            string GetText = this.Content;

            if (DeFine.CodeParsingEngineUsing)
            {
                GetText = CodeParsing.UsingCode(GetText);
            }

            if (DeFine.ConjunctionEngineUsing)
            {
                GetText = Conjunction.UsingStr(GetText);
            }

            if (DeFine.PhraseEngineUsing)
            {
                foreach (var Get in this.CardItems)
                {
                    GetText = GetText.Replace(" （" + Get.EN.ToString() + "） ", Get.Str);
                    GetText = GetText.Replace("（" + Get.EN.ToString() + "）", Get.Str);
                    GetText = GetText.Replace(" (" + Get.EN.ToString() + ") ", Get.Str);
                    GetText = GetText.Replace("(" + Get.EN.ToString() + ")", Get.Str);
                    GetText = GetText.Replace(Get.EN.ToString(), Get.Str);
                }

                GetText = GetText.Replace("  ", " ").Replace(" ( ", "(").Replace(" ) ", ")").Replace(" )", ")").Replace("( ", "(").Replace(" (", "(").Replace(") ", ")").Replace(" ‘ ", "‘").Replace("  ‘  ", "‘").Replace(" ‘", "‘").Replace("‘ ", "‘").Trim();
            }

            this.Content = GetText;
        }

        public string GetRemainingText(string Text)
        {
            string GetText = Text;

            if (DeFine.CodeParsingEngineUsing)
            {
                GetText = this.CodeParsing.GetRemainingText(GetText);
            }

            if (DeFine.ConjunctionEngineUsing)
            {
                GetText = this.Conjunction.GetRemainingText(GetText);
            }

            if (DeFine.PhraseEngineUsing)
            {
                foreach (var Get in this.CardItems)
                {
                    GetText = GetText.Replace("（" + Get.EN.ToString() + "）", string.Empty);
                    GetText = GetText.Replace("(" + Get.EN.ToString() + ")", string.Empty);
                    GetText = GetText.Replace(Get.EN.ToString(), string.Empty);
                }
            }
          
            new WordProcess().FormatText(ref GetText);

            GetText = GetText.Replace(".", "").Replace(",", "").Replace("-", "").Replace(":", "").Replace("'", "").Replace("\"","");
            GetText = GetText.Replace(">", "").Replace("<", "");
            GetText = GetText.Replace(" ", "");

            return GetText;
        }
    }
}

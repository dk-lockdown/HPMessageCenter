using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.IO;
using System.Linq;
using System.Web;

namespace MessageCenter.Portal
{
    public class ValidationCodeHelper
    {
        public static char[] ValidateCodeChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        //// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="length">指定验证码的长度</param>
        /// <returns></returns>
        public static string CreateValidateCode(int length)
        {
            int rand;
            string randomcode = String.Empty;

            //生成一定长度的验证码
            System.Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                rand = random.Next() % 34;
                randomcode += ValidateCodeChars[rand].ToString();
            }
            return randomcode;
        }

        //// <summary>
        /// 生成验证码(纯数字)
        /// </summary>
        /// <param name="length">指定验证码的长度</param>
        /// <returns></returns>
        public static string CreateSimpleValidateCode(int length)
        {
            int[] randMembers = new int[length];
            int[] validateNums = new int[length];
            string validateNumberStr = "";
            //生成起始序列值
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random seekRand = new Random(seekSeek);
            int beginSeek = (int)seekRand.Next(0, Int32.MaxValue - length * 10000);
            int[] seeks = new int[length];
            for (int i = 0; i < length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成随机数字
            for (int i = 0; i < length; i++)
            {
                Random rand = new Random(seeks[i]);
                int pownum = 1 * (int)Math.Pow(10, length);
                randMembers[i] = rand.Next(pownum, Int32.MaxValue);
            }
            //抽取随机数字
            for (int i = 0; i < length; i++)
            {
                string numStr = randMembers[i].ToString();
                int numLength = numStr.Length;
                Random rand = new Random();
                int numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
            }
            //生成验证码
            for (int i = 0; i < length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }
            return validateNumberStr;
        }

        /// <summary>
        /// 创建验证码的图片
        /// </summary>
        /// <param name="containsPage">要输出到的page对象</param>
        /// <param name="validateNum">验证码</param>
        public static byte[] CreateValidateGraphic(string validateCode, int height)
        {
            return CreateValidateGraphic(validateCode, height, 0, 0);
        }

        /// <summary>
        /// 创建验证码的图片
        /// </summary>
        /// <param name="validateCode">验证码</param>
        /// <param name="height">图片高度</param>
        /// <param name="rightOffsetWidth">文字的右边偏移量宽度</param>
        /// <returns></returns>
        public static byte[] CreateValidateGraphic(string validateCode, int height, int leftOffsetWidth, int rightOffsetWidth)
        {
            int randAngle = 45; //随机转动角度
            int mapwidth = (int)(validateCode.Length * 24) + leftOffsetWidth + rightOffsetWidth;
            Bitmap image = new Bitmap(mapwidth, height);//创建图片背景
            Graphics graph = Graphics.FromImage(image);
            try
            {

                graph.Clear(Color.AliceBlue);//清除画面，填充背景
                Random rand = new Random();



                //验证码旋转，防止机器识别
                char[] chars = validateCode.ToCharArray();//拆散字符串成单字符数组

                //文字距中
                StringFormat format = new StringFormat(StringFormatFlags.NoClip);
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                //定义颜色
                Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
                //定义字体
                string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };
                int cindex = rand.Next(7);

                for (int i = 0; i < chars.Length; i++)
                {
                    int findex = rand.Next(5);

                    Font f = new System.DrawingCore.Font(font[findex], 20, System.DrawingCore.FontStyle.Bold);//字体样式(参数2为字体大小)
                    Brush b = new System.DrawingCore.SolidBrush(c[cindex]);

                    Point dot = new Point(20, 12);
                    //graph.DrawString(dot.X.ToString(),fontstyle,new SolidBrush(Color.Black),10,150);//测试X坐标显示间距的
                    float angle = rand.Next(-randAngle, randAngle);//转动的度数

                    graph.TranslateTransform(dot.X, dot.Y);//移动光标到指定位置
                    graph.RotateTransform(angle);
                    graph.DrawString(chars[i].ToString(), f, b, 0, 5, format);
                    //graph.DrawString(chars[i].ToString(),fontstyle,new SolidBrush(Color.Blue),1,1,format);
                    graph.RotateTransform(-angle);//转回去
                    graph.TranslateTransform(-2, -dot.Y);//移动光标到指定位置，每个字符紧凑显示，避免被软件识别
                }
                Brush lb = new System.DrawingCore.SolidBrush(c[cindex]);
                graph.TranslateTransform(-graph.Transform.OffsetX, -graph.Transform.OffsetY);
                // graph.DrawLine(new Pen(Color.DarkGray, 3), 0, image.Height / 2, image.Width, image.Height / 2);
                int x1 = 0;
                int x2 = 0;
                int y1 = 0;
                int y2 = 0;
                int lt = image.Width / 20;
                int dy = 2;
                //背景噪点生成
                for (int i = 0; i < 30; i++)
                {
                    x1 = i == 0 ? lt * i : x2;
                    y1 = i == 0 ? (image.Height / 2 + rand.Next(-dy, dy)) : y2;
                    x2 = x1 + lt;
                    y2 = image.Height / 2 + rand.Next(-dy, dy);
                    graph.DrawLine(new Pen(Color.DarkGray, 2), x1, y1, x2, y2);
                }

                //for (int i = 0; i < 0; i++)
                //{

                //    int x11 = rand.Next(image.Width);
                //    int x12 = rand.Next(image.Width);
                //    int y11 = rand.Next(image.Height);
                //    int y12 = rand.Next(image.Height);
                //    graph.DrawLine(new Pen(Color.DarkGray), x11, y11, x12, y12);
                //}

                //生成图片
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    image.Save(ms, System.DrawingCore.Imaging.ImageFormat.Gif);
                    return ms.ToArray();
                }
            }
            finally
            {
                graph.Dispose();
                image.Dispose();
            }
        }
    }
}
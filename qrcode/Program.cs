using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Text;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace qrcode
{
    class Creation
    {
        public void Show(string arg)//实现在控制台屏幕输出二维码
        {
            if (IsNumber(arg) == false || arg.Length > 100)//合法内容的要求（自定义）：不含数字且长度不大于100
            {
                Console.WriteLine("Sorry, your content is illegal. ");//提示内容不合要求
            }
            else
            {
                QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);//实例化新的QrEncoder
                QrCode qrCode = qrEncoder.Encode(arg);//为参数的字符串信息编码
                for (int j = 0; j < qrCode.Matrix.Width; j++)//遍历每一行
                {
                    for (int i = 0; i < qrCode.Matrix.Width; i++)//遍历每一列
                    {
                        string charToPrint = qrCode.Matrix[i, j] ? "  " : "█";//根据0/1信息选择白或黑色的方块，由于控制台屏幕背景是黑色，调换顺序以后才能识别成功
                        Console.Write(charToPrint);//将方块打印到屏幕
                    }
                    Console.WriteLine();//换行
                }
            }
        }

        public void Save(string arg)//实现二维码保存到文件
        {
            if (arg.IndexOf("txt") > 0)//判断是否属于文本文件
            {
                if (File.Exists(arg)) {//判断文件是否存在
                    StreamReader sr = new StreamReader(arg, Encoding.Default);//若存在，读入文件内容
                    String line;
                    for (int i = 1; (line = sr.ReadLine()) != null; i++)//对每一行内容生成二维码图片，调用ToSave函数
                    {
                        ToSave(line, i);
                    } }
                else {
                    Console.WriteLine("文件不存在！");//若不存在，提示
                }
            }
            else
            {
                Console.WriteLine("尚未支持其他文件格式！");//其他文件提示未支持
            }
        }

        public bool IsNumber(string arg)//判断命令行输入参数是否包含数字
        {
            bool result = false;
            //循环判断每一个字符是否为数字
            for (int i = 0; i < arg.Length; i++)
            {
                if (Char.IsNumber(arg, i))
                {
                    return result;//匹配到数字后返回false
                }
                else
                {
                    result = true;//否则修改结果为true并保持
                }
            }
            return result;//确保必定有返回值
        }

        public string SetName(string line, int i)//按要求设定保存二维码的文件名
        {
            string fileName = "D:\\nayukiconversation\\temp\\";
            //分行数讨论
            if (i < 10)//如果行序数只有一位
            {
                fileName += "00" + i + line.Substring(0, 4);//该行信息的前四个字符
            }
            else if (i >= 10 && i < 100)//如果行序数有两位
            {
                fileName += "0" + i + line.Substring(0, 4);
            }
            else if (i >= 100)//如果行序数有三位
            {
                fileName += i + line.Substring(0, 4);
            }
            fileName += ".png";//保存为png格式

            return fileName;
        }

        public void ToSave(string line, int i)
        {
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);//实例化新的QrEcoder
            QrCode qrCode = new QrCode();//实例化新的QrCode
            qrEncoder.TryEncode(line, out qrCode);//为输入的一行内容编码
            //生成显示二维码的渲染器，并设定二维码的尺寸和两种逻辑的表示颜色
            GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(9, QuietZoneModules.Two), Brushes.Black, Brushes.White);

            string fileName = SetName(line, i);//调用SetName函数确定生成图片的文件名
            DrawingSize dSize = renderer.SizeCalculator.GetSize(qrCode.Matrix.Width);//设定尺寸
            Bitmap map = new Bitmap(dSize.CodeWidth, dSize.CodeWidth);//使用待生成二维码的尺寸创建位图
            Graphics g = Graphics.FromImage(map);//由从map继承的对象生成Graphics，准备将二维码画入
            renderer.Draw(g, qrCode.Matrix);//画出二维码
            if (File.Exists(fileName))
            {
                File.Delete(fileN);
            }
            map.Save(fileName);//保存该修改后的位图
        }

        static void Main(string[] args)//主程序
        {
            Creation test = new Creation();//实例化新的生成二维码的类

            if (args.Length > 1 && args[0] == "-f")//当参数大于一个且第一个参数为-f时，将生成的二维码以图片形式保存
            {
                test.Save(args[1]);
            }
            else if (args.Length >= 1 && args[0]!="-f")//当用户输入参数但无-f命令时，直接在控制台打出二维码
                {
                    string arg ="";
                    for(int i = 0; i < args.Length; i++)
                    {
                        arg = arg + args[i] + " ";
                    }
                    test.Show(arg);
                }
            else//其他情况，希望再次检查输入
            {
                Console.WriteLine("Please check your input!");
            }
            Console.WriteLine("Press any key to quit.");//按任意键退出程序
            Console.ReadKey();//逐行读取
        }
    }
}
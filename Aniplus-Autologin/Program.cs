using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
namespace Aniplus_Autologin
{
    class Program
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
                                                        int size, string filePath);
        const int Size = 256;
        static void Main(string[] args)
        {
            ini ini = new ini();
            string Check = ini.GetIni("Info", "Lastlogin-Datetime");
            System.IO.File.AppendAllText(System.Windows.Forms.Application.StartupPath+@"\Log.txt", "Start:" + DateTime.Now.ToString() + Environment.NewLine);
            if(Check == DateTime.Now.ToShortDateString())
            {
                Environment.Exit(0);
            }
            string ID = ini.GetIni("Login", "ID");
            string PW = ini.GetIni("Login", "PW");
            if (ID == "" || PW == "")
            {
                Console.Write("ID:");
                ID = Console.ReadLine();
                Console.Write("PW:");
                PW = Console.ReadLine();
            }
            IWebDriver driver = new ChromeDriver();
            driver.Url = "https://www.aniplustv.com/login/login.asp?reURL=%23/default.asp%3FgCode%3DMA%26sCode%3D000"; //로그인
            bool Login = false;
            while (!Login)
            {
                IWebElement context = driver.FindElement(By.Name("userid"));
                context.Clear();
                context.SendKeys(ID); //아이디 입력
                context = driver.FindElement(By.Name("password"));
                context.Clear();
                context.SendKeys(PW); //비밀번호 입력

                driver.FindElement(By.Id("btnLogin")).Click();
                System.Threading.Thread.Sleep(2000); //로그인 결과 기다림
                try
                {
                    IAlert Alert = driver.SwitchTo().Alert();

                    Console.Clear();
                    Console.WriteLine("로그인 실패:" + Alert.Text);
                    Alert.Accept();
                    Console.Write("ID:");
                    ID = Console.ReadLine();
                    Console.Write("PW:");
                    PW = Console.ReadLine();
                }
                catch (Exception) { //알림창 없음
                    Login = true;
                    ini.SetIni("Login", "ID", ID);
                    ini.SetIni("Login", "PW", PW);
                    ini.SetIni("Info", "Lastlogin-Datetime",DateTime.Now.ToShortDateString());
                }
            }
            driver.Quit();
            driver.Dispose();
            Environment.Exit(0);
        }
    }
}

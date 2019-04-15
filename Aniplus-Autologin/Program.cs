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
            Log("Start:" + DateTime.Now.ToString());
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

                driver.FindElement(By.Id("btnLogin")).Click(); //로그인 버튼 클릭
                while (true)
                {
                    IAlert Alert = SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent().Invoke(driver);
                    if (Alert != null) //팝업창이 있는지
                    {
                        Console.Clear();
                        Console.WriteLine("로그인 실패:" + Alert.Text);
                        Alert.Accept();
                        Console.Write("ID:");
                        ID = Console.ReadLine();
                        Console.Write("PW:");
                        PW = Console.ReadLine();
                        break;
                    }
                    IWebElement Name = SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("id")).Invoke(driver);
                    if(Name != null) //로그인 성공해서 닉네임이 존재하는지
                    {
                        ini.SetIni("Login", "ID", ID);
                        ini.SetIni("Login", "PW", PW);
                        ini.SetIni("Info", "Lastlogin-Datetime", DateTime.Now.ToShortDateString());
                        Log(Name.Text + "-로그인 성공");
                        Login = true;
                        break;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }
            driver.Quit();
            driver.Dispose();
            Log("Success");
            Environment.Exit(0);
        }
        static void Log(string str)
        {
            System.IO.File.AppendAllText(System.Windows.Forms.Application.StartupPath + @"\Log.txt", str + Environment.NewLine);
        }
    }
}

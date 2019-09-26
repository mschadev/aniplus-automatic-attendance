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
        static void Main(string[] args)
        {
            ini ini = new ini();
            Log("Start:" + DateTime.Now.ToString());
#if DEBUG


#else
            string Check = ini.GetIni("Info", "Lastlogin-Datetime");
            if (Check == DateTime.Now.ToShortDateString())
            {
                Environment.Exit(0);
            }

#endif
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
            driver.Url = "https://www.aniplustv.com/login/login.asp"; //Login url
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            IWebElement context = driver.FindElement(By.Name("userid"));
            context.Clear();
            context.SendKeys(ID); //아이디 입력
            context = driver.FindElement(By.Name("password"));
            context.Clear();
            context.SendKeys(PW); //비밀번호 입력
            driver.FindElement(By.Id("btnLogin")).Click(); //로그인 버튼 클릭
            
            IAlert alert = null;
            bool login = false;
            while ((alert = SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent().Invoke(driver)) == null &&
                !(login = IsElementPresent(driver,By.CssSelector("#open_user > a > span.id"))))
            {
                System.Threading.Thread.Sleep(10);
            }

            if (alert != null) //팝업창이 있는지
            {
                Console.Clear();
                Console.WriteLine("로그인 실패:" + alert.Text);
                Console.WriteLine("아이디 비번 입력후 수동으로 재실행 해주세요.");
                alert.Accept();
                Console.Write("ID:");
                ID = Console.ReadLine();
                Console.Write("PW:");
                PW = Console.ReadLine();
                ini.SetIni("Login", "ID", ID);
                ini.SetIni("Login", "PW", PW);
            }
            
            if (login) //로그인 성공해서 닉네임이 존재하는지
            {
                IWebElement Name = driver.FindElement(By.CssSelector("#open_user > a > span.id"));
                ini.SetIni("Login", "ID", ID);
                ini.SetIni("Login", "PW", PW);
                ini.SetIni("Info", "Lastlogin-Datetime", DateTime.Now.ToShortDateString());
                Log(Name.Text + "-로그인 성공");
            }
            else if(ini.GetIni("Debug","debug") == "1")
            {
                Log(driver.PageSource);
            }
            System.Threading.Thread.Sleep(1000);
            driver.Quit();
            driver.Dispose();
            Log("Success");
            Environment.Exit(0);
        }
        static bool IsElementPresent(IWebDriver driver, By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        static void Log(string str)
        {
            System.IO.File.AppendAllText(System.Windows.Forms.Application.StartupPath + @"\Log.txt", str + Environment.NewLine);
        }
    }
}

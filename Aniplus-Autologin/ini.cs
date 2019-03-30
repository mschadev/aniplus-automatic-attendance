using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace Aniplus_Autologin
{
    class ini
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string FilePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string FilePath);

        public void SetIni(string _Section, string _Key, string _Value)
        {
            WritePrivateProfileString(_Section, _Key, _Value, System.Windows.Forms.Application.StartupPath + "\\config.ini");
        }

        public string GetIni(string _Section, string _Key)
        {
            StringBuilder STBD = new StringBuilder(1000);
            GetPrivateProfileString(_Section, _Key, null, STBD, 5000, System.Windows.Forms.Application.StartupPath + "\\config.ini");

            return STBD.ToString().Trim();
        }

    }
}

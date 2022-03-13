using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AccountManager
{
    public static class DatabaseManager
    {
        #region Methods

        public static IEnumerable<Account> ParseDatabase(string path)
        {
            List<Account> data = new();


            XDocument database = XDocument.Parse(path);




            return data;
        }

        public static void UpdateDatabase(string path)
        {
        }

        #endregion Methods
    }
}
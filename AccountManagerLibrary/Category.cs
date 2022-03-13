using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager
{
    public class Category
    {
        #region Properties

        public string Name { get; set; }
        public List<Category> RequiredCategories { get; } = new List<Category>();

        #endregion Properties
    }
}
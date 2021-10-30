using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorna_ver_3
{
    public class Cell
    {
        public string exp = "";
        public string value = "NULL";
        public bool used = false;
        public bool used2= false;
        public List<Pair> dependentsFrom = new List<Pair>();
        public List<Pair> dependentsTo = new List<Pair>();

        
       
    }
}
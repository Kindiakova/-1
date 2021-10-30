using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboratorna_ver_3
{
    class TableVoid
    {
        public Dictionary<string, int> LettersToInt = new Dictionary<string, int>();
        public Dictionary<int, string> IntToLetters = new Dictionary<int, string>();
        public Cell[,] table;
        const int st = 100;
        public int Columns = 0;
        public int Rows = 0;
        public bool Circle = false;
        int k = 0;
        public TableVoid(ref Cell[,] TABLE, int COLUMNS, int ROWS)
        {
            table = TABLE;
            string Letter = "";
            for (int i = 1; i <= st; ++i)
            {
                int it = -1;
                for (int j = Letter.Length - 1; j >= 0; --j)
                    if (Letter[j] != 'Z') it = j;
                if (it == -1) Letter = new String('A', Letter.Length + 1);
                else Letter = Letter.Substring(0, it) + (char)((int)Letter[it] + 1) + new String('A', Letter.Length - it - 1);

                LettersToInt.Add(Letter, i);
                IntToLetters.Add(i, Letter);
            }
            Columns = COLUMNS;
            Rows = ROWS;
        }

        public bool FindCircle(Pair cell) {

            k++;
            if (table[cell.first, cell.second].used) Circle = true;
            
            if (Circle) return true;
            table[cell.first, cell.second].used = true;
            for (int i = 0; i < table[cell.first, cell.second].dependentsFrom.Count; ++i)
            {
                Circle = FindCircle(table[cell.first, cell.second].dependentsFrom[i]);
                if (Circle) break;
            }

            table[cell.first, cell.second].used = false;
         
            return Circle;
        }
 
        public void Update(MyParser myParser, Pair cell)
        {
            if (table[cell.first, cell.second].used2) return;
            table[cell.first, cell.second].used2 = true;
             table[cell.first, cell.second].value = myParser.getExpValue(table[cell.first, cell.second].exp, cell);
            

            for (int i = 0; i < table[cell.first, cell.second].dependentsTo.Count; ++i)            
                Update(myParser, table[cell.first, cell.second].dependentsTo[i]);
            
            table[cell.first, cell.second].used2 = false;
        }
        public Pair getCell(string letter, string num)
        {
            if (LettersToInt.ContainsKey(letter) == false) throw new FormatException("Неправильне посилання на клітинку: " + letter + num);
            int _second, _first = Convert.ToInt32(num) - 1;
            LettersToInt.TryGetValue(letter, out _second);
            _second--;
            return new Pair(_first, _second);
        }
        public string GetCellValue(string cellName, Pair cell)
        {
            if (cell.first >= Rows || cell.second >= Columns) throw new FormatException("Неіснуюча клітинка: " + cellName);
            if (cell.first < 0 || cell.second < 0) throw new FormatException("Неіснуюча клітинка: " + cellName);

            if (table[cell.first, cell.second].value == "True") return "True";
            if (table[cell.first, cell.second].value == "False") return "False";
            return "n";
        }

        public void DependsUpdate(List<Pair> depends, Pair cell)
        {
            for (int i = 0; i < table[cell.first, cell.second].dependentsFrom.Count; ++i)
            {
                Pair to = table[cell.first, cell.second].dependentsFrom[i];
                table[to.first, to.second].dependentsTo.Remove(cell);
            }
            table[cell.first, cell.second].dependentsFrom = depends;
            for (int i = 0; i < depends.Count; ++i)
            {
                Pair to = depends[i];
                table[to.first, to.second].dependentsTo.Add(cell);
            }
        }

    }
}

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
    class MyParser
    {
        private Dictionary<string, int> Letter;
        const double exp = 1e-9;
        const int INF = 1000000009;
        
        TableVoid tableVoid;
        public MyParser(TableVoid TABLE)
        {
            tableVoid = TABLE;
        }

        private string IsOperator(string line)
        {
            if (line == "=") return "=";
            if (line == ">") return ">";
            if (line == "<") return "<";
            if (line == "+" || line == "Inc" || line == "inc") return "+";
            if (line == "-" || line == "Dec" || line == "dec") return "-";

            if (line == "!" || line == "Not" || line == "not") return "!";
            if (line == "&" || line == "And" || line == "and") return "&";
            if (line == "|" || line == "Or" || line == "or") return "|";
            if (line == "~" || line == "Eqv" || line == "eqv") return "~";
            return "0";
        }

        private void FormatLine(ref List<string> elements, string line)
        {
            int i = 0, countOpen = 0;
            while (i < line.Length)
            {
                if (countOpen > 0)
                {
                    if (line[i] == '(') countOpen++;
                    if (line[i] == ')') countOpen--;
                    if (line[i] == ')' && countOpen == 0)
                    {
                        string parsedBracket = Parse(elements[elements.Count - 1]);
                        elements[elements.Count - 1] = parsedBracket;
                    }
                    else elements[elements.Count - 1] = elements[elements.Count - 1] + line[i];
                    i++;
                }
                else
                {
                    if (line[i] == '(')
                    {
                        countOpen++;
                        elements.Add("");
                        i++;
                    }
                    else if (line[i] == ')') throw new FormatException("Перевірте дужки");
                    else if (IsOperator("" + line[i]) != "0")
                    {
                        elements.Add("" + line[i]);
                        i++;
                    }
                    else if (Char.IsLetter(line[i]))
                    {
                        string s = "";
                        while (i < line.Length && Char.IsLetter(line[i]))
                        {
                            s += line[i];
                            i++;
                        }
                        if (IsOperator(s) != "0") elements.Add(IsOperator(s));
                        else if (s == "True" || s == "true") elements.Add("t");
                        else if (s == "False" || s == "false") elements.Add("f");
                        else throw new FormatException("Невідома операція: " + s);
                    }
                    else if (Char.IsDigit(line[i]))
                    {
                        bool comma = false;
                        string num = "";
                        while (i < line.Length && (Char.IsDigit(line[i]) || (line[i] == ',' && comma == false)))
                        {
                            if (line[i] == ',') comma = true;
                            num += line[i];
                            i++;
                        }
                        elements.Add(num);
                    }
                    else if (line[i] == ' ') i++;
                    else throw new FormatException("Невідомий символ: " + line[i]);
                }

            }
            if (countOpen != 0) throw new FormatException("Перевірте дужки");
            return;
        }

        private string FindCells(string line, ref List<Pair> cells)
        {
            string res = "";
            int i = 0;
            bool nullValue = false;
            while (i < line.Length)
            {
                string letter = "";
                string num = "";
                while (i < line.Length && (int)line[i] >= 'A' && (int)line[i] <= 'Z')
                {
                    letter += line[i];
                    i++;
                }
                while (i < line.Length && Char.IsDigit(line[i]))
                {
                    num += line[i];
                    i++;
                }

                if (letter.Length == 0 && num.Length == 0) { res += line[i]; i++; }
                else if (letter.Length == 0 || num.Length == 0) res += letter + num;
                else
                {
                    cells.Add(tableVoid.getCell(letter, num));
                    string cellValue = tableVoid.GetCellValue(letter + num, cells[cells.Count - 1]);
                    res += cellValue;
                    if (cellValue == "n") nullValue = true; 
                    
                }
            }
            if (nullValue) return "n";
            return res;
        }

        public string getExpValue(string text, Pair cell)
        {
            if (text.Length == 0) return "NULL";
            List<Pair> cells = new List<Pair>();
            text = FindCells(text, ref cells);

            tableVoid.DependsUpdate(cells, cell);            
            if (tableVoid.FindCircle(cell)) return "NULL";
            if (text == "n") return "NULL";


            string res = Parse(text);
            if (res == "t") return "True";
            if (res == "f") return "False";
            return "NULL";
        }

        public string Parse(string text)
        {
                if (text.Length == 0) throw new FormatException();

            List<string> elements = new List<string>();
            FormatLine(ref elements, text);

            

            return Calc(ref elements, 0, elements.Count - 1);

        }

        private int FindIndex(ref List<string> elements, int start, int end, string key)
        {
            for (int i = start; i <= end; ++i)
                if (elements[i] == key) return i;
            return INF;
        }
        private string Calc(ref List<string> elements, int start, int end)
        {
            if (end - start < 0) throw new FormatException();
            if (end - start == 0)
            {
                if (IsOperator(elements[start]) == "0") return elements[start];
                throw new FormatException();
            }
            int i = INF;
            // = Eqv
            i = FindIndex(ref elements, start, end, "~");
            if (i != INF)
            {
                string first = Calc(ref elements, start, i - 1),
                       second = Calc(ref elements, i + 1, end);
                if (Char.IsDigit(first[0]) || Char.IsDigit(second[0])) throw new InvalidCastException();
                if (first == second) return "t";
                else return "f";
            }
            //  OR
            i = FindIndex(ref elements, start, end, "|");
            if (i != INF)
            {
                string first = Calc(ref elements, start, i - 1),
                       second = Calc(ref elements, i + 1, end);
                if (Char.IsDigit(first[0]) || Char.IsDigit(second[0])) throw new InvalidCastException();
                if (first == "t" || second == "t") return "t";
                else return "f";
            }
            // AND
            i = FindIndex(ref elements, start, end, "&");
            if (i != INF)
            {
                string first = Calc(ref elements, start, i - 1),
                        second = Calc(ref elements, i + 1, end);
                if (Char.IsDigit(first[0]) || Char.IsDigit(second[0])) throw new InvalidCastException();
                if (first == "t" && second == "t") return "t";
                else return "f";
            }

            // > < =
            i = Math.Min(FindIndex(ref elements, start, end, "="),
                Math.Min(FindIndex(ref elements, start, end, ">"), FindIndex(ref elements, start, end, "<")));
            if (i != INF)
            {
                string first = Calc(ref elements, start, i - 1),
                        second = Calc(ref elements, i + 1, end);
                if (!Char.IsDigit(first[0]) || !Char.IsDigit(second[0])) throw new InvalidCastException();
                double x, y;
                Double.TryParse(first, out x);
                Double.TryParse(second, out y);
                if (elements[i] == "=")
                {
                    if (Math.Abs(x - y) < exp) return "t";
                    return "f";
                }
                if (elements[i] == ">")
                {
                    if (x > y) return "t";
                    return "f";
                }
                if (elements[i] == "<")
                {
                    if (x < y) return "t";
                    return "f";
                }

            }

            // not
            i = FindIndex(ref elements, start, end, "!");
            if (i != INF)
            {
                if (i != start) throw new FormatException();
                string first = Calc(ref elements, start + 1, end);
                if (Char.IsDigit(first[0])) throw new InvalidCastException();
                if (first == "t") return "f";
                return "t";
            }

            // inc dec
            i = Math.Min(FindIndex(ref elements, start, end, "+"), FindIndex(ref elements, start, end, "-"));
            if (i != INF)
            {
                if (i != start) throw new FormatException();
                string first = Calc(ref elements, start + 1, end);
                if (!Char.IsDigit(first[0])) throw new InvalidCastException();
                double x;
                Double.TryParse(first, out x);
                if (elements[i] == "+") x++;
                else x--;
                return x.ToString();
            }

            throw new FormatException();
        }
    }
}

/*intcrement ++, decrement --
 = > <
 ! - not
 || && eqv or and eqv*/



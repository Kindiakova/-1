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
    public partial class Form1 : Form
    {
        const int st = 100;
        
        int COLUMNS = 5;
        int ROWS = 10;
        TableVoid tableVoid;
        Cell[,] table = new Cell[st, st];
        MyParser myParser;
        bool LoadEnds = false;
        bool Clicked = false;
        bool Value = false;
        string Help = "Варіант 77\n" +
         "Дозволені операції:\n" +
        "Логічне і(And, and, &)\n" +
        "Логічне або(Or, or, |)\n" +
        "Еквівалентність(Eqv, eqv, ~)\n" +
        "Логічне заперечення(Not, not, !)\n" +
        "Порівняння чисел(=, >, <)\n" +
        "(пре)Інкремент(Inc, inc, +)\n" +
        "(пре)Декремент(Dec, dec, -)\n";

        public Form1()
        {
            InitializeComponent();
            tableVoid = new TableVoid(ref table, COLUMNS, ROWS);
            openFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            saveFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
        }

        /// проверь 0 в парсере table
        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < COLUMNS; ++i)
            {
                string Name = Convert.ToString(i + 1);
                string Header = "";
                tableVoid.IntToLetters.TryGetValue(i+1, out Header);
                dataGridView1.Columns.Add(Name, Header);
            }
            for (int i = 0; i < ROWS; ++i)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].HeaderCell.Value = String.Format("{0}", i + 1);
               
            } 
            for (int i = 0; i < ROWS; ++i)
            {
                for (int j = 0; j < COLUMNS; ++j)
                {
                    table[i, j] = new Cell();
                    dataGridView1[j, i].Value = "";
                }
            }
            LoadEnds = true;
            myParser = new MyParser(tableVoid);
            
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (LoadEnds && !Clicked & !Value)
            {
                int i = e.RowIndex, j = e.ColumnIndex;
                table[i, j].exp = dataGridView1[j, i].Value.ToString();
                try
                {
                    tableVoid.Update(myParser, new Pair(i, j));
                    if (tableVoid.Circle)
                        MessageBox.Show("Цикл", "Знайдено цикл");
                    tableVoid.Circle = false;
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("Помилка формату виразу: " + ex.Message);
                    tableVoid.DependsUpdate(new List<Pair>(), new Pair(i, j));
                }
                catch (InvalidCastException ex)
                {
                    MessageBox.Show("Помилка у приведенні типів:" + ex.Message);
                    tableVoid.DependsUpdate(new List<Pair>(), new Pair(i, j));
                }
            }
        }     

 
        private void button2_Click(object sender, EventArgs e)
        {
            Clicked = true;
            for (int i = 0; i < ROWS; ++i)
                for (int j = 0; j < COLUMNS; ++j)
                    if (!Value) dataGridView1[j, i].Value = table[i, j].value;
                    else dataGridView1[j, i].Value = table[i, j].exp;

            Value = !Value;
            Clicked = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Help, "Допомога");
        }

        private void відкритиToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку
            string fileText = System.IO.File.ReadAllText(filename);
            textBox1.Text = fileText;
            MessageBox.Show("Файл відкрито");
        }

        private void зберегтиToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = saveFileDialog1.FileName;
            // сохраняем текст в файл
            System.IO.File.WriteAllText(filename, textBox1.Text);
            MessageBox.Show("Файл збережено");
        }

        private void додатиРядокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clicked = true;
            if (ROWS == st) MessageBox.Show("Кількість рядків завелика");
            else
            {
                
                dataGridView1.Rows.Add();
                dataGridView1.Rows[ROWS].HeaderCell.Value = String.Format("{0}", ROWS+1);
                ROWS++;
                tableVoid.Rows++;
            }
            Clicked = false;
        }

        private void додатиКолонкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clicked = true;
            if (COLUMNS == st) MessageBox.Show("Кількість рядків завелика");
            else
            {
                string ColumnHeader;
                tableVoid.IntToLetters.TryGetValue(COLUMNS + 1, out ColumnHeader);
                dataGridView1.Columns.Add(COLUMNS.ToString(), ColumnHeader);
                COLUMNS++;
                tableVoid.Columns++;
            }
            Clicked = false;
        }


    }
}

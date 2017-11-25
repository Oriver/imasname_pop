using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace imasname_pop
{
    public partial class Form1 : Form
    {
#if DEBUG
        private const String CsvName = "imasname_pop.name_test.csv";
#else
        private const String CsvName = "imasname_pop.name.csv";
#endif
        private const String SaveFile = "data.dat";
        private const String LogFile = "log.txt";

        private Queue<int> queue = new Queue<int>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(SaveFile)) load();
            else reset();
        }

        private void reset()
        {
            try
            {
                var lst = new List<int>();
                var r = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(CsvName));
                int count = 0;
                while (r.ReadLine() != null) lst.Add(count++);

                r.Close();

                var rnd = new Random();

                lst = lst.OrderBy((i) => rnd.Next()).ToList();
                queue.Clear();

                foreach (int i in lst) queue.Enqueue(i);

                var w = new StreamWriter(new FileStream(LogFile, FileMode.Append));
                w.WriteLine("=== RESET ===");
                w.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private String pop()
        {
            if(queue.Count == 0)
            {
                reset();
                return "出し切ったのでリセットします";
            }
            int no = queue.Dequeue();

            try
            {
                var r = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(CsvName), Encoding.GetEncoding("shift_jis"));
                for (int i = no; i > 0; --i) r.ReadLine();
                var str = r.ReadLine().Split(',');
                var ret = str[0] + "/" + str[1];
                r.Close();

                var w = new StreamWriter(new FileStream(LogFile, FileMode.Append), Encoding.GetEncoding("shift_jis"));
                w.WriteLine(ret);
                w.Close();

                return ret;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return "";
        }

        private void save()
        {
            try
            {
                var w = new BinaryWriter(new FileStream(SaveFile, FileMode.Create));
                w.Write(queue.Count);
                while (queue.Count != 0) w.Write(queue.Dequeue());
                w.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private void load()
        {
            try
            {
                var r = new BinaryReader(new FileStream(SaveFile, FileMode.Open));
                int count = r.ReadInt32();
                byte[] buf = new byte[count * 4];
                r.Read(buf, 0, count * 4);
                queue = new Queue<int>();
                for (int i = 0; i < count; ++i) queue.Enqueue(BitConverter.ToInt32(buf, i * 4));
                r.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = pop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            save();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        class test {
            static public bool wok = false;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var filePath = openFileDialog1.FileName;
                    using (Stream str = openFileDialog1.OpenFile())
                    {
                        textBox1.Text = filePath.ToString();
                    }
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(readFile));

            if (thread.ThreadState.ToString() == "Unstarted") thread.Start();

            if (thread.ThreadState.ToString() == "Running")
            {
                test.wok = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = folderBrowserDialog2.SelectedPath;
            }
        }

        private void readFile(object _obj)
        {            
            string[] fileBadWorns = File.ReadAllLines(openFileDialog1.FileName, System.Text.Encoding.GetEncoding(1251));
            Dictionary<string, int> statBadWords = new Dictionary<string, int>();

            var fileInDic = Directory.EnumerateFiles(folderBrowserDialog1.SelectedPath, "*.txt", SearchOption.AllDirectories);
            int statBar = (100 / fileInDic.Count()) - (1 * fileInDic.Count() / 100);

            foreach (var findedFile in fileInDic)
            {
                while (!test.wok)
                {
                    Thread.Sleep(500);
                }

                FileInfo fileInf = new FileInfo(findedFile);
                bool flagToReplace = false;
                Dictionary<string, int> findBadWords = new Dictionary<string, int>();

                string fileText = File.ReadAllText(findedFile.ToString(), System.Text.Encoding.GetEncoding(1251));

                listBox1.Invoke(new Action(() => listBox1.Items.Add(findedFile.ToString() + " \t" + fileInf.Length + " байт")));

                foreach (var carrentWord in fileText.Split(' ', '.', ',', '!', '?', '\n', '\r'))
                {
                    foreach (var badWord in fileBadWorns)
                    {
                        if (carrentWord == badWord)
                        {
                            flagToReplace = true;
                            if (findBadWords.ContainsKey(badWord))
                            {
                                int tempVounter = findBadWords[badWord];
                                tempVounter += 1;
                                findBadWords.Remove(badWord);
                                findBadWords.Add(badWord, tempVounter);
                            } else
                            {
                                findBadWords.Add(badWord, 1);
                            }
                        }
                    }
                }

                foreach (var item in findBadWords)
                {
                    if (statBadWords.ContainsKey(item.Key))
                    {
                        int tempVounter = statBadWords[item.Key];
                        tempVounter += item.Value;
                        statBadWords.Remove(item.Key);
                        statBadWords.Add(item.Key, tempVounter);
                    } else
                    {
                        statBadWords.Add(item.Key, item.Value);
                    }
                }

                if (flagToReplace)
                {
                    foreach (var item in fileBadWorns)
                    {
                        fileText = fileText.Replace(item, "*******");
                    }
                    File.WriteAllText(folderBrowserDialog2.SelectedPath + "\\" + Path.GetFileName(findedFile), fileText);
                }

                progressBar1.Invoke(new Action(() => progressBar1.Value += statBar));

                foreach (var item in findBadWords)
                {
                    listBox1.Invoke(new Action(()=>listBox1.Items.Add("   " + item.Key + " - " + item.Value)));
                }
                Thread.Sleep(1000);
            }

            listBox1.Invoke(new Action(() => listBox1.Items.Add("")));
            listBox1.Invoke(new Action(() => listBox1.Items.Add("- - - - - - топ 10 слов - - - - - -")));
            int counter = 1;
            foreach (var pair in statBadWords.OrderByDescending(pair => pair.Value))
            {
                if (counter > 10)
                {
                    break;
                }
                listBox1.Invoke(new Action(() => listBox1.Items.Add(counter + ". " + pair.Key + " - " + pair.Value)));
                counter++;
            }
            progressBar1.Invoke(new Action(() => progressBar1.Value = 100));
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (test.wok)
            {
                test.wok = false;
            }
        }
    }
}

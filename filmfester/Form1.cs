using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace filmfester
{
    public partial class Form1 : Form
    {
        const string FILMS_FILE = "films.json";

        Timer timer;
        DateTime startTime;
        int totalTime;

        public Form1()
        {
            InitializeComponent();
            
            //Load the json file.
            LoadJSON();

            startTime = DateTime.Now;
            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);

            textBox_upnext.TextChanged += new EventHandler(textBox_upnext_TextChanged);
            textBox_justwatched.TextChanged += new EventHandler(textBox_justwatched_TextChanged);


            dataGridView1.MouseDown += new MouseEventHandler(gridDataBoundGrid1_MouseDown);
        }

        void gridDataBoundGrid1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (dataGridView1.SelectedRows.Count <= 0)
                    return;

                if (dataGridView1.SelectedRows[0].Cells[0].Value == null)
                    return;

                if (dataGridView1.SelectedRows[0].Cells[0].Style.BackColor == Color.GreenYellow)
                {
                    dataGridView1.SelectedRows[0].Cells[0].Style.BackColor = Color.White;
                }
                else
                {
                    dataGridView1.SelectedRows[0].Cells[0].Style.BackColor = Color.GreenYellow;
                }

                dataGridView1.ClearSelection();
                dataGridView1.Refresh();
            }
        }



        private void LoadJSON()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            List<filmentries> entries = new List<filmentries>();

            if (!File.Exists(FILMS_FILE))
            {
                MessageBox.Show(string.Format("Couldn't find {0}\n\nMaking a placeholder file now.", FILMS_FILE));
                return;
            }

            string fileContents = GetFileContents(FILMS_FILE);

            try
            {
                entries = JsonConvert.DeserializeObject<List<filmentries>>(fileContents);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Error parsing {0}.\n\n{1}", FILMS_FILE, e.Message));
                return;
            }

            if (entries.Count <= 0)
            {
                MessageBox.Show(string.Format("{0} has no entries.", FILMS_FILE));
                return;
            }

            for (int i = 0; i < entries[0].entries.Length; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);  // this line was missing
                row.Cells[0].Value = entries[0].entries[i];
                
                row.HeaderCell.Value = String.Format("{0}", i + 1);
                row.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Rows.Add(row);
            }
        }

        


        private void textBox_upnext_TextChanged(object sender, EventArgs e)
        {
            string theText = string.Empty;
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                theText = textBox.Text;
            }

            WriteToTextfile("upnext.txt", theText);

            
        }

        private void textBox_justwatched_TextChanged(object sender, EventArgs e)
        {
            string theText = string.Empty;
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                theText = textBox.Text;
            }

            WriteToTextfile("justwatched.txt", theText);

            
        }


        private string GetFileContents(string filepath)
        {
            string output = "";

            try
            {
                using (FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        //dump file contents into a string.
                        output = reader.ReadToEnd();
                    }
                }
            }
            catch(Exception e)
            {
                return string.Empty;
            }

            return output;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //the arrow button.
            if (dataGridView1.SelectedRows.Count <= 0)
                return;

            if (dataGridView1.SelectedRows[0].Cells[0].Value == null)
                return;

            //Move text to the just watched box.
            string lastText = textBox_upnext.Text;
            textBox_justwatched.Text = lastText;

            dataGridView1.SelectedRows[0].Cells[0].Style.BackColor = Color.GreenYellow;
            //dataGridView1.SelectedRows[0].HeaderCell.Style.Font = new Font("Microsoft Sans Serif", 10.75F, FontStyle.Bold);

            string text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            text = text.Replace("\n", Environment.NewLine);

            textBox_upnext.Text = text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox_upnext.Text = string.Empty;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SetTimer(180);
        }

        private void SetTimer(int maxTime)
        {
            totalTime = maxTime;
            timer.Start();
            startTime = DateTime.Now;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            int elapsedSeconds = (int)(DateTime.Now - startTime).TotalSeconds;
            int remainingSeconds = totalTime - elapsedSeconds;

            if (remainingSeconds < 0)
            {
                label_timer.Text = "0:00";
                this.Text = "0:00";
                timer.Stop();
                return;
            }

            TimeSpan t = TimeSpan.FromSeconds(remainingSeconds);
            string timeStr = string.Format("{0}:{1:D2}", t.Minutes, t.Seconds);
            label_timer.Text = timeStr;
            this.Text = timeStr;

            WriteToTextfile("countdown.txt", timeStr);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SetTimer(120);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SetTimer(60);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SetTimer(0);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            totalTime += 30;
            timer.Start();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            totalTime -= 30;

            if (totalTime < 0)
                totalTime = 0;
        }

        private void WriteToTextfile(string filePath, string text)
        {
            if (!File.Exists(filePath))
            {
                using (FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, System.IO.FileAccess.Write, FileShare.Read))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.WriteLine(string.Empty);
                    }
                }
            }

            using (FileStream fileStream = File.Open(filePath, FileMode.Truncate, System.IO.FileAccess.Write, FileShare.Read))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(text);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openThisFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Environment.CurrentDirectory);
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("Failed to open folder.\n\n{0}", err.Message));
            }
        }

        private void copyFolderPathToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(Environment.CurrentDirectory);
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("Failed to copy folder path.\n\n{0}", err.Message));
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Film Fester\nby Brendon Chung\nMay 2020\n\n", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void reloadJSONFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadJSON();
        }

        private void editJSONFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(FILMS_FILE);
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("Failed to open JSON file.\n\n{0}", err.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



    }
}

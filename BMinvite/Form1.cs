using MoreLinq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace BMinvite
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string profile_path = "";
        public static List<String> Id_Profile = new List<String>();
        public static List<String> ListComment = new List<String>();
        public static List<String> Success = new List<String>();
        public static List<String> Fail = new List<String>();
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        int _camxuc;
        string[] list_camxuc = { "Like", "Love", "Care", "Haha", "Wow", "Sad", "Angry" };
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
        private void load_View(String _Stt, String _Profile)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView1);  // this line was missing
            row.Cells[0].Value = _Stt;
            row.Cells[1].Value = _Profile;
            row.Cells[2].Value = DateTime.Now.ToString("h:mm:ss")+" Đang khởi tạo trình duyệt";

            Invoke(new Action(() =>
            {
                dataGridView1.Rows.Add(row);
            }));

        }
        private void update_View(String _Status, int _Index)
        {
            Invoke(new Action(() =>
            {
                dataGridView1.Rows[_Index].Cells[2].Value = _Status;
                
            }));
        }
        private void update_Status(String _Status, int _Index)
        {
            Invoke(new Action(() =>
            {
                dataGridView1.Rows[_Index].Cells[3].Value = _Status;
            }));
        }

        private void update_Success()
        {
            Invoke(new Action(() =>
            {
                label10.Text = Success.Count().ToString();
            }));
        }
        private void update_Fail()
        {
            Invoke(new Action(() =>
            {
                label12.Text = Fail.Count().ToString();
            }));
        }
        
        private async void run_bm()
        {
            
            Invoke(new Action(() =>
            {
                dataGridView1.Rows.Clear();
            }));
            for (int x = 0; x < textBox1.Lines.Length; x++)
            {
                string[] temp = textBox1.Lines[x].Split('|');

                Id_Profile.Add(temp[0]); // add profile id
                                         // => Trùng index với nhau
            }
            for (int x = 0; x < textBox3.Lines.Length; x++)
            {

                ListComment.Add(textBox3.Lines[x]); // add profile id
                                         // => Trùng index với nhau
            }
            
            for (int i = 0; i < textBox1.Lines.Length; i++)
            {

                load_View((i+1).ToString(), textBox1.Lines[i]);


            }
            int number_of_threads = Convert.ToInt32(numericUpDown3.Value); /* số luồng */
            int max_like = Convert.ToInt32(numericUpDown1.Value); /* số luồng */

            List<Task> tasks = new List<Task>();
            int dem = 0;
            for (int i = 0; i < max_like/ number_of_threads; i++)
            {
                var temp1 = i;
                for (int x = 0; x < number_of_threads; x++)
                {
                    var temp = x + number_of_threads * temp1;
                    var temp2 = x;
                    var temp3 = dem;
                    Task t1 = new Task(() =>
                    {
                        random_camxuc();
                        bool is_done = automation_linkbm(temp3, textBox2.Text, Id_Profile[temp], _camxuc);
                        if (!is_done)
                        {
                            update_View(DateTime.Now.ToString("h:mm:ss") + " Lỗi!", temp3);
                        }
                        
                    });
                    t1.Start();
                    tasks.Add(t1);

                    dem++;
                }
                
                Task task = Task.WhenAll(tasks);

                try
                {
                    task.Wait();
                    //MessageBox.Show("Thành công");

                }
                catch (Exception)
                {

                    throw;
                }
                tasks.Clear();


            }
        }
        public bool automation_linkbm(int _i, String fb_id, String profile_id, int _camxuc)
        {
            var options = new ChromeOptions();
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            if (checkBox2.Checked) /* ẩn chrome */
            {
                service.HideCommandPromptWindow = true;
                options.AddArgument("--window-position=-32000,-32000");
            }
            options.AddArgument(@"user-data-dir="+profile_path+"\\"+profile_id);
            ChromeDriver chromeDriver = new ChromeDriver(service, options);
            chromeDriver.Url = "https://mbasic.facebook.com/reactions/picker/?is_permalink=1&ft_id="+fb_id;

            update_View(DateTime.Now.ToString("h:mm:ss") + " Đang vào link bài viết", _i);
            chromeDriver.Navigate();
            Thread.Sleep(2000);
            string myTitle = chromeDriver.Title;
            /* Check xem có vào được link không*/
            /* Điền tên vào input bằng class*/
            try
            {   
                var element = chromeDriver.FindElements(By.ClassName("x"));
                element[_camxuc].Click();
                update_View(DateTime.Now.ToString("h:mm:ss") +" "+ list_camxuc[_camxuc] + " thành công", _i);
                update_Status(" Success", _i);

            }
            catch (Exception)
            {
                chromeDriver.Quit();
                update_Fail();

                return false;
            }
            Thread.Sleep(2000);
            try
            {
                var element = chromeDriver.FindElements(By.ClassName("cmt_like_def"));
                if(element != null)
                {
                    Random rnd = new Random();
                    int comment = rnd.Next(0, element.Count());
                    var like_comment = element[comment].FindElements(By.TagName("a"))[1];
                    like_comment.Click();
                    Thread.Sleep(1000);
                    var element2 = chromeDriver.FindElements(By.ClassName("x"));
                    element2[_camxuc].Click();
                    update_View(DateTime.Now.ToString("h:mm:ss") + " " + list_camxuc[_camxuc] + "  comment thành công", _i);

                }

            }
            catch (Exception)
            {
                chromeDriver.Quit();
                update_Fail();

                return false;
            }
            Thread.Sleep(1000);
            try
            {
                Random rnd = new Random();
                int r = rnd.Next(ListComment.Count);
                var element = chromeDriver.FindElement(By.Id("composerInput"));
                element.SendKeys((string)ListComment[r]);
                chromeDriver.FindElement(By.CssSelector("input[type='submit']")).Click();

                update_View(DateTime.Now.ToString("h:mm:ss") + " Comment \"" + ListComment[r] + "\" thành công ", _i);
                update_Status(" Success", _i);

            }
            catch (Exception)
            {
                chromeDriver.Quit();
                update_Fail();

                return false;
            }
            chromeDriver.Quit();
            return true;
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Id_Profile.Clear();
            //if (true)
            //{   
            //    ThreadStart ts = new ThreadStart(run_bm);
            //    Thread thrd = new Thread(ts);
            //    thrd.IsBackground = false;
            //    thrd.Start();
            //}else
            //{
            //    MessageBox.Show("Vui lòng nhập đường dẫn profile Maxcare");

            //}
            var proxy = new HelperXproxy();
            var data = proxy.test();
            MessageBox.Show(data);
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }


        private void label6_Click(object sender, EventArgs e)
        {

        }


        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                label8.Text = "Profile: " + folderBrowserDialog1.SelectedPath;
                profile_path = folderBrowserDialog1.SelectedPath;
            }
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {
            
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged_1(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            _camxuc = 0;
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            _camxuc = 1;

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            _camxuc = 3;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            _camxuc = 2;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            _camxuc = 4;
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            _camxuc = 5;
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            _camxuc = 6;
        }
        void random_camxuc()
        {
            Random rnd = new Random();
            _camxuc = rnd.Next(0, 7);
        }
        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            random_camxuc();
        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}

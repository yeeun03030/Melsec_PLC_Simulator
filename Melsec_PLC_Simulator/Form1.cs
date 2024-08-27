using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACTMULTILIB_K;
using MySql.Data.MySqlClient;

namespace Melsec_PLC_Simulator
{
    public partial class Form1 : Form
    {
        ActEasyIF control = new ActEasyIF();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(control.Open() == 0)
            {
                MessageBox.Show("연결되었습니다.");
                timer1.Enabled = true;

                List<History> ret = new List<History>();
                MySqlDataReader reader = null; // 읽음

                using (MySqlConnection connection = new MySqlConnection("Server=aikopo.net;Port=3306;Database=aikopo;Uid=aikopo;Pwd=poly1234"))
                { // MySQL 서버 url 적용
                    try // 예외 처리
                    {
                        connection.Open();
                        // ExecuteReader를 이용하여
                        // 연결 모드로 데이터 가져오기
                        MySqlCommand cmd;

                        cmd = new MySqlCommand("SELECT * FROM tb_sensor WHERE name = 'Job2'", connection);

                        reader = cmd.ExecuteReader(); // reader가 다 저장하고 있음

                        while (reader.Read())
                        {
                            History tmp = new History(); // List로 하나하나씩 쌓았음
                            tmp.idx = int.Parse(reader["idx"].ToString());
                            tmp.name = reader["name"].ToString();
                            tmp.sensor = int.Parse(reader["sensor"].ToString());
                            ret.Add(tmp);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("실패");
                        Console.WriteLine(ex.ToString());
                    }
                }

                reader.Close();

                dataGridView1.DataSource = ret;
            }
            else
            {
                MessageBox.Show("연결을 실패하였습니다.");
            }
        }

        // Y 디바이스는 출력용
        private void button2_Click(object sender, EventArgs e)
        {
            // 전진
            short value = 0x01 << 1;
            control.WriteDeviceBlock2("Y0", 1, ref value);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 후진
            short value = 0x01 << 2;
            control.WriteDeviceBlock2("Y0", 1, ref value);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            short sensors = 0; // short는 16bit 정수
            control.ReadDeviceBlock2("X0", 1, out sensors);

            if ((sensors & (0x01 << 2)) != 0)
            {
                label1.Text = "전진";
                pictureBox1.ImageLocation = "./cylinderon.png";
            }
            else if ((sensors & (0x01 << 3)) != 0)
            {
                label1.Text = "후진";
                pictureBox1.ImageLocation = "./cylinderoff.png";
            }
            if ((sensors & (0x01 << 4)) != 0)
            {
                label2.Text = "후진";
                pictureBox2.ImageLocation = "./cylinderoff.png";
            }
            else if ((sensors & (0x01 << 5)) != 0)
            {
                label2.Text = "전진";
                pictureBox2.ImageLocation = "./cylinderon.png";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 전진
            short value = 0x01 << 3;
            control.WriteDeviceBlock2("Y0", 1, ref value);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // 후진
            short value = 0x01 << 4;
            control.WriteDeviceBlock2("Y0", 1, ref value);
        }


        private void button7_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection("Server=aikopo.net;Port=3306;Database=aikopo;Uid=aikopo;Pwd=poly1234"))
            { // MySQL 서버 url 적용
                try // 예외 처리
                {
                    connection.Open();
                    // ExecuteReader를 이용하여
                    // 연결 모드로 데이터 가져오기
                    MySqlCommand cmd;

                    cmd = new MySqlCommand("INSERT INTO tb_sensor(name, sensor) values ('Job2', 123)", connection);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}

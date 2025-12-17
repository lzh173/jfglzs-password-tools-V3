using jfglzs_password_tool_v3.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace jfglzs_password_tools_V3
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
            this.Text = PasswordGenerator.GenerateTitle();
            textBox1.Text = PasswordGenerator.GetComputerLastCharAscii();
            label1.Visible = false;
            label3.Visible = false;
            label3.ForeColor = Color.Blue;
        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            int choose = 1;
            if (radioButton1.Checked == true)
            {
                choose = 1;
                label1.Text = PasswordGenerator.GeneratePassword(choose, dateTimePicker1.Value, textBox1.Text);
            }
            if (radioButton2.Checked == true)
            {
                choose = 2;
                label1.Text = PasswordGenerator.GeneratePassword(choose, dateTimePicker1.Value, textBox1.Text);
            }
            if (radioButton3.Checked == true)
            {
                choose = 3;
                label1.Text = PasswordGenerator.GeneratePassword(choose, dateTimePicker1.Value, textBox1.Text);
            }
            if (radioButton4.Checked == true)
            {
                choose = 4;
                label1.Text = PasswordGenerator.GeneratePassword(choose, dateTimePicker1.Value, textBox1.Text);
            }
            if (radioButton5.Checked == true)
            {
                choose = 5;
                label1.Text = PasswordGenerator.GeneratePassword(choose, dateTimePicker1.Value, textBox1.Text);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(radioButton6.Checked == false && radioButton7.Checked == false)
            {
                string ps = RegistryHelper.GetRegistryValue("n");
                if (ps != null)
                {
                    if (Tools.HasSpecialCharacters(ps))
                    {
                        radioButton6.Checked = true;
                        label3.Visible = true;
                        label3.Text = "自动选择版本-9.98+";

                    }
                    else
                    {
                        radioButton7.Checked = true;
                        label3.Visible = true;
                        label3.Text = "自动选择版本-10.2+";
                    }
                }
                else
                {
                    MessageBox.Show("未检测到密码，未安装助手？", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if(radioButton6.Checked == true)
            {
                if (!Tools.HasSpecialCharacters(textBox2.Text))
                {
                    radioButton7.Checked = true;
                    label3.Visible = true;
                    label3.Text = "自动选择版本-10.2+";
                    textBox3.Text = "此版本密码无法读取";
                    return;
                }
                textBox2.Text = RegistryHelper.GetRegistryValue("n");
                textBox3.Text = OptimizedTruncatedDecryptor.DecryptTruncatedOptimized(textBox2.Text);
            }
            if (radioButton7.Checked == true)
            {
                if (Tools.HasSpecialCharacters(textBox2.Text)){
                    radioButton6.Checked = true;
                    label3.Visible = true;
                    label3.Text = "自动选择版本-9.98+";
                    textBox2.Text = RegistryHelper.GetRegistryValue("n");
                    textBox3.Text = OptimizedTruncatedDecryptor.DecryptTruncatedOptimized(textBox2.Text);
                    return;
                }
                textBox2.Text = RegistryHelper.GetRegistryValue("n");
                textBox3.Text = "此版本密码无法读取";

            }
           
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}

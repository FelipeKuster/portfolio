using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace Interface
{
    public partial class Form1 : Form
    {
        bool formIsBlocked = true;
        int[] saves = { 0, 0, 0, 0 };
        public Form1()
        {

            InitializeComponent();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Port.Write(new byte[] { 0, Convert.ToByte(trackBar1.Value) }, 0, 2);
            textBox1.Text = "" + Convert.ToByte(trackBar1.Value);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Port.Open();
                Port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            }
            catch
            {
                MessageBox.Show("Cabo desconectado", "Erro");
                Close();
            }
        }
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            char teste = Convert.ToChar(Port.ReadChar());
            Port.ReadExisting();
            if (teste == '4')
            {
               
                MessageBox.Show("Objeto Encontrado", "AVISO",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            else
                return;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (Port.IsOpen)
                {
                    Port.Close();
                }
            }
            catch
            { }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (Port.IsOpen)
                {
                    Port.Close();
                }
            }
            catch
            { }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            Port.Write(new byte[] { 1, Convert.ToByte(trackBar2.Value) }, 0, 2);
            textBox2.Text = "" + Convert.ToByte(trackBar2.Value);
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            Port.Write(new byte[] { 3, Convert.ToByte(trackBar4.Value) }, 0, 2);
            textBox4.Text = "" + Convert.ToByte(trackBar4.Value);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            Port.Write(new byte[] { 2, Convert.ToByte(trackBar2.Value) }, 0, 2);
            textBox3.Text = "" + Convert.ToByte(trackBar2.Value);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void trackBar2_Scroll_1(object sender, EventArgs e)
        {
            Port.Write(new byte[] { 1, Convert.ToByte(trackBar2.Value) }, 0, 2);
            textBox2.Text = "" + Convert.ToByte(trackBar2.Value);
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void trackBar3_Scroll_1(object sender, EventArgs e)
        {
            Port.Write(new byte[] { 2, Convert.ToByte(trackBar3.Value) }, 0, 2);
            textBox3.Text = "" + Convert.ToByte(trackBar3.Value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Port.Write(new byte[] { 4, Convert.ToByte(255) }, 0, 2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Port.Write(new byte[] { 4, Convert.ToByte(0) }, 0, 2);
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form2 
        }

    }
}

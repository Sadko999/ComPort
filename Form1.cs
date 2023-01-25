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

namespace ComPort
{
    public partial class Form1 : Form
    {
        // Create global variable
        string dataOut;
        string sendWith;
        string dataIn;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            cBoxCOMPORT.Items.AddRange(ports);

            btnOpen.Enabled = true;
            btnClose.Enabled = false;

            chBoxDtrEnable.Checked = false;
            serialPort1.DtrEnable = false;
            chBoxRtsEnable.Checked = false;
            serialPort1.RtsEnable = false;
            btnSendData.Enabled = false;
            chBoxWriteLine.Checked = false;
            chBoxWrite.Checked = true;
            sendWith = "Write";

            chBoxAlwaysUpdate.Checked = false;
            chBoxAddToOLdData.Checked = true;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                // Настройки COM порта
                serialPort1.PortName = cBoxCOMPORT.Text;
                serialPort1.BaudRate = Convert.ToInt32(cBoxBaudeRate.Text);
                serialPort1.DataBits = Convert.ToInt32(cBoxDataBits.Text);
                serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cBoxStopBits.Text);
                serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), cBoxParityBits.Text);

                serialPort1.Open();
                progressBar1.Value = 100;
                btnOpen.Enabled = false;
                btnClose.Enabled = true;
                lblStatusCom.Text = "ON";
            }
            catch (Exception err)
            { 
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnOpen.Enabled = true;
                btnClose.Enabled = false;
                lblStatusCom.Text = "OFF";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if(serialPort1.IsOpen)
            {
                serialPort1.Close();
                progressBar1.Value = 0;
                btnOpen.Enabled = true;
                btnClose.Enabled = false;
                lblStatusCom.Text = "OFF";
            }
        }

        private void btnSendData_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                dataOut = tBoxDataOut.Text;
                if (sendWith == "WriteLine")
                {
                    serialPort1.WriteLine(dataOut);
                }
                else if (sendWith == "Write")
                {
                    serialPort1.Write(dataOut);
                }
            }
        }

        private void chBoxDtrEnable_CheckedChanged(object sender, EventArgs e)
        {
            if(chBoxDtrEnable.Checked) { serialPort1.DtrEnable = true; }
            else { serialPort1.DtrEnable = false; }
        }

        private void chBoxRTSEnable_CheckedChanged(object sender, EventArgs e)
        {
            if(chBoxRtsEnable.Checked) { serialPort1.RtsEnable = true; }
            else { serialPort1.RtsEnable = false; };
        }

        private void btnClearDataOut_Click(object sender, EventArgs e)
        {
            if (tBoxDataOut.Text != "") { tBoxDataOut.Text = ""; }
        }

        private void chBoxUsingButton_CheckedChanged(object sender, EventArgs e)
        {
            if(chBoxUsingButton.Checked)
                btnSendData.Enabled = true;
            else
                btnSendData.Enabled = false;
        }

        private void chBoxUsingEnter_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chBoxWriteLine_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxWriteLine.Checked)
            {
                sendWith = "WriteLine";
                chBoxWrite.Checked = false;
                chBoxWriteLine.Checked = true;
            }
        }

        private void chBoxWrite_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxWrite.Checked)
            {
                sendWith = "Write";
                chBoxWrite.Checked = true;
                chBoxWriteLine.Checked = false;
            }
        }

        private void tBoxDataOut_TextChanged(object sender, EventArgs e)
        {
            int dataOutLenght = tBoxDataOut.Text.Length;
            lblDataOutLenght.Text = dataOutLenght.ToString();

            if (chBoxUsingEnter.Checked)
            {
                // This Is For Prevent Enter Key to Create New Line
                tBoxDataOut.Text = tBoxDataOut.Text.Replace(Environment.NewLine, "");
            }
        }

        private void tBoxDataOut_KeyDown(object sender, KeyEventArgs e)
        {
            if (chBoxUsingEnter.Checked)
            {
                if(e.KeyCode == Keys.Enter)
                {
                    // Send Serial Data
                    if (serialPort1.IsOpen)
                    {
                        dataOut = tBoxDataOut.Text;
                        if (sendWith == "WriteLine")
                        {
                            serialPort1.WriteLine(dataOut);
                        }
                        else if (sendWith == "Write")
                        {
                            serialPort1.Write(dataOut);
                        }
                    }
                }
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            dataIn = serialPort1.ReadExisting();
            this.Invoke(new EventHandler(ShowData));    // This Methode Show The data Serial into TextBox.
                                                        // You Can not Show The Data to The TextBox Directly Without Using This Methode
        }

        private void ShowData(object sender, EventArgs e)
        {
            int dataInLenght = dataIn.Length;
            lblDataInLeght.Text = string.Format("{0:00}", dataInLenght);
            // This is for Determine the TextBox Always Show The Update Data or Add to Old Data
            if (chBoxAlwaysUpdate.Checked)
            {
                tBoxDataIn.Text = dataIn;
            }
            else if (chBoxAddToOLdData.Checked)
            {
            }
            tBoxDataIn.Text += dataIn;
        }

        private void chBoxAlwaysUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if(chBoxAlwaysUpdate.Checked)
            {
                //chBoxAlwaysUpdate.Checked = true;      // This Is For toggle Condition
                chBoxAddToOLdData.Checked = false;
            }
            //else { chBoxAddToOLdData.Checked = true; }
        }

        private void chBoxAddToOLdData_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxAddToOLdData.Checked)
            {
                //chBoxAddToOLdData.Checked = true;
                chBoxAlwaysUpdate.Checked = false;      // This Is For toggle Condition
            }
            //else { chBoxAlwaysUpdate.Checked = true; }
        }

        private void btnClearDataIN_Click(object sender, EventArgs e)
        {
            if(tBoxDataIn.Text != "")
            {
                tBoxDataIn.Text = "";
            }
        }

        private void btnUpdatePorts_Click(object sender, EventArgs e)
        {
            string[] portList = SerialPort.GetPortNames();
            cBoxCOMPORT.Text = "";
            cBoxCOMPORT.Items.Clear();

            if (portList.Length != 0) 
            { 
                cBoxCOMPORT.Items.AddRange(portList);
                cBoxCOMPORT.SelectedIndex= 0;
            }

        }
    }
}

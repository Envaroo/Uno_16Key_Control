using System;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.IO.Ports;  //시리얼통신을 위해 추가해줘야 함
using System.Security.Principal;
using System.Windows.Forms;

namespace Serial_Communication
{
    public partial class Form1 : Form
    {
        public static MemoryMappedFile sharedBuffer;
        public static MemoryMappedViewAccessor sharedBufferAccessor;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)  //폼이 로드되면
        {
            comboBox_port.DataSource = SerialPort.GetPortNames(); //연결 가능한 시리얼포트 이름을 콤보박스에 가져오기 
            MemoryMappedFileSecurity CustomSecurity = new MemoryMappedFileSecurity();
            SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            var acct = sid.Translate(typeof(NTAccount)) as NTAccount;
            CustomSecurity.AddAccessRule(new System.Security.AccessControl.AccessRule<MemoryMappedFileRights>(acct.ToString(), MemoryMappedFileRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));
            sharedBuffer = MemoryMappedFile.CreateOrOpen("Local\\BROKENITHM_SHARED_BUFFER", 1024, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, CustomSecurity, System.IO.HandleInheritability.Inheritable);
            sharedBufferAccessor = sharedBuffer.CreateViewAccessor();
        }

        private void Button_connect_Click(object sender, EventArgs e)  //통신 연결하기 버튼
        {
            if (!serialPort1.IsOpen)  //시리얼포트가 열려 있지 않으면
            {
                serialPort1.PortName = comboBox_port.Text;  //콤보박스의 선택된 COM포트명을 시리얼포트명으로 지정
                serialPort1.BaudRate = 115200;  //보레이트 변경이 필요하면 숫자 변경하기
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Parity = Parity.None;
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived); //이것이 꼭 필요하다
                try
                {
                    serialPort1.Open();  //시리얼포트 열기
                }
                catch (Exception ex)
                {
                    richTextBox_received.Text = ex.ToString();
                }
                label_status.Text = "Connected";
                comboBox_port.Enabled = false;  //COM포트설정 콤보박스 비활성화
            }
            else  //시리얼포트가 열려 있으면
            {
                label_status.Text = "Already Connected";
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)  //수신 이벤트가 발생하면 이 부분이 실행된다.
        {
            try
            {
                this.Invoke(new EventHandler(MySerialReceived));  //메인 쓰레드와 수신 쓰레드의 충돌 방지를 위해 Invoke 사용. MySerialReceived로 이동하여 추가 작업 실행.
            }
            catch
            {

            }
        }

        private void MySerialReceived(object s, EventArgs e)  //여기에서 수신 데이타를 사용자의 용도에 따라 처리한다.
        {
            string rawData = serialPort1.ReadTo("3");
            string trimmedData = rawData.Substring(rawData.LastIndexOf("2") + 1, 16);
            byte[] sharedBuff = new byte[32];
            for (int i = 0; i < trimmedData.Length; i++)
            {
                if (trimmedData[i] == '1')
                {
                    DrawSquare(23 * i, 20, true);
                    sharedBuff[31 - i * 2] = 128;
                    sharedBuff[31 - i * 2 - 1] = 128;
                }
                else
                {
                    DrawSquare(23 * i, 20, false);
                    sharedBuff[31 - i * 2] = 0;
                    sharedBuff[31 - i * 2 - 1] = 0;
                }
            }
            sharedBufferAccessor.WriteArray<byte>(6, sharedBuff, 0, 32);
            richTextBox_received.Text = trimmedData.Length.ToString();
            /*
        int RecSize = serialPort1.BytesToRead;
        string inp = string.Empty;
        richTextBox_received.Text = RecSize.ToString() + '\n';
        if (serialPort1.ReadByte() == '7')
        {
            byte[] buff = new byte[RecSize];
            byte[] sharedBuff = new byte[32];
            serialPort1.Read(buff, 0, RecSize);
            for (int i = 0; i < RecSize - 1; i++)
            {
                if (buff[i + 1] == '1')
                {
                    DrawSquare(23 * i, 20, true);
                    sharedBuff[31 - i * 2] = 128;
                    sharedBuff[31 - i * 2 - 1] = 128;
                }
                else
                {
                    DrawSquare(23 * i, 20, false);
                    sharedBuff[31 - i * 2] = 0;
                    sharedBuff[31 - i * 2 - 1] = 0;
                }

            }


            sharedBufferAccessor.WriteArray<byte>(6, sharedBuff, 0, 32);
            for (int i = 0; i < RecSize; i++)
            {
                inp += " " + Convert.ToChar(buff[i]);
            }

        }
*/
            //시리얼 버터에 수신된 데이타를 ReceiveData 읽어오기
            ///richTextBox_received.Text += inp;  //int 형식을 string형식으로 변환하여 출력
            //string.Format("{0:X2}", ReceiveData);
        }

        public void DrawSquare(int x, int width, bool isFull)
        {
            Graphics g = panel1.CreateGraphics();
            //g.Clear(this.BackColor);
            Rectangle rect = new Rectangle(x, 0, width, 100);
            if (isFull == true)
            {
                g.FillRectangle(Brushes.Yellow, rect);
            }
            else
            {
                g.FillRectangle(Brushes.Red, rect);
            }
            g.Dispose();
        }

        private void Button_send_Click(object sender, EventArgs e)  //보내기 버튼을 클릭하면
        {
            serialPort1.Write(textBox_send.Text);  //텍스트박스의 텍스트를 시리얼통신으로 송신
        }

        private void Button_disconnect_Click(object sender, EventArgs e)  //통신 연결끊기 버튼
        {
            if (serialPort1.IsOpen)  //시리얼포트가 열려 있으면
            {
                serialPort1.Close();  //시리얼포트 닫기

                label_status.Text = "Closed";
                comboBox_port.Enabled = true;  //COM포트설정 콤보박스 활성화
            }
            else  //시리얼포트가 닫혀 있으면
            {
                label_status.Text = "Already Closed";
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox_received.Clear();
        }

        private void Recalibrate_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
                serialPort1.Write("a");
        }
    }
}

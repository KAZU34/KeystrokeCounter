using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using DxLibDLL;

namespace KeystrokeCounter
{
    public partial class Form1 : Form
    {
        private readonly int[] keyMap = { DX.PAD_INPUT_1, DX.PAD_INPUT_2, DX.PAD_INPUT_3, DX.PAD_INPUT_4, DX.PAD_INPUT_5, DX.PAD_INPUT_6, DX.PAD_INPUT_7 };
        private readonly bool[] onKey = new bool[7];
        private readonly int[] count = new int[9]; // スクラッチ分の +2
        private int countTotal = 0;
        private int scratchStatus = 0;　// 停止中0、右回転(引き皿)1、左回転(押し皿)-1
        private int buffX;
        private long time = DateTime.Now.Ticks;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DX.SetUserWindow(this.Handle);
            if (DX.DxLib_Init() != 0 ||
                DX.SetDrawScreen(DX.DX_SCREEN_BACK) != 0 ||
                DX.SetJoypadDeadZone(DX.DX_INPUT_PAD1, 0.0) != 0
                ) Environment.Exit(-1);
            Thread.Sleep(1);
            DX.GetJoypadAnalogInput(out buffX, out _, DX.DX_INPUT_PAD1);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            DX.DxLib_End();
        }

        private void radioButton1INF_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = true;
        }

        private void radioButton2LR2_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = false;
        }

        public void MainLoop()
        {
            var joypadState = DX.GetJoypadInputState(DX.DX_INPUT_PAD1);

            for (int i = 0; i < keyMap.Length; i++)
            {
                if ((joypadState & keyMap[i]) != 0)
                {
                    if (!onKey[i])
                    { // 前判定まで未入力であれば入力の瞬間とする
                        onKey[i] = true;
                        count[i]++;
                        countTotal++;
                    }
                }
                else
                { // そもそも現判定が未入力であれば入力済みフラグをリセット
                    onKey[i] = false;
                }
            }

            if (onKey[0]) labelCount1.BackColor = Color.Aqua; else labelCount1.BackColor = SystemColors.Control;
            if (onKey[1]) labelCount2.BackColor = Color.Aqua; else labelCount2.BackColor = SystemColors.Control;
            if (onKey[2]) labelCount3.BackColor = Color.Aqua; else labelCount3.BackColor = SystemColors.Control;
            if (onKey[3]) labelCount4.BackColor = Color.Aqua; else labelCount4.BackColor = SystemColors.Control;
            if (onKey[4]) labelCount5.BackColor = Color.Aqua; else labelCount5.BackColor = SystemColors.Control;
            if (onKey[5]) labelCount6.BackColor = Color.Aqua; else labelCount6.BackColor = SystemColors.Control;
            if (onKey[6]) labelCount7.BackColor = Color.Aqua; else labelCount7.BackColor = SystemColors.Control;

            if (radioButton1INF.Checked)
            {
                InfModeScratch();
            }
            else
            {
                Lr2ModeScratch(joypadState);
            }

            labelCountTotal.Text = "total\r\n" + countTotal.ToString();
            labelCount1.Text = "1key\r\n" + count[0].ToString();
            labelCount2.Text = "2key\r\n" + count[1].ToString();
            labelCount3.Text = "3key\r\n" + count[2].ToString();
            labelCount4.Text = "4key\r\n" + count[3].ToString();
            labelCount5.Text = "5key\r\n" + count[4].ToString();
            labelCount6.Text = "6key\r\n" + count[5].ToString();
            labelCount7.Text = "7key\r\n" + count[6].ToString();
            labelCount8Pull.Text = "pull\r\n" + count[7].ToString();
            labelCount9Push.Text = "push\r\n" + count[8].ToString();
        }

        private void Lr2ModeScratch(int joypadState)
        {
            if ((joypadState & DX.PAD_INPUT_LEFT) != 0)
            { // LEFTだけどDAOコン(LR2mode)だと右回転(引き皿・時計回り)
                if (scratchStatus != 1)
                { // まだ右回転してなければ
                    scratchStatus = 1;
                    count[7]++;
                    countTotal++;
                }
                labelCount8Pull.BackColor = Color.Aqua;
                labelCount9Push.BackColor = SystemColors.Control;
            }
            else if ((joypadState & DX.PAD_INPUT_RIGHT) != 0)
            { // RIGHTだけどDAOコン(LR2mode)だと左回転(押し皿・反時計回り)
                if (scratchStatus != -1)
                { // まだ左回転してなければ
                    scratchStatus = -1;
                    count[8]++;
                    countTotal++;
                }
                labelCount8Pull.BackColor = SystemColors.Control;
                labelCount9Push.BackColor = Color.Aqua;
            }
            else
            { // 回転停止
                scratchStatus = 0;
                labelCount8Pull.BackColor = SystemColors.Control;
                labelCount9Push.BackColor = SystemColors.Control;
            }
        }

        private void InfModeScratch()
        {
            DX.GetJoypadAnalogInput(out int inputX, out _, DX.DX_INPUT_PAD1);

            if (buffX != inputX && Math.Abs(buffX - inputX) < 2000)
            {
                if (buffX > inputX)
                { // DAOコン(INFmode)の右回転(引き皿・時計回り)
                    if (scratchStatus != 1)
                    { // まだ右回転してなければ
                        scratchStatus = 1;
                        count[7]++;
                        countTotal++;
                    }
                    labelCount8Pull.BackColor = Color.Aqua;
                    labelCount9Push.BackColor = SystemColors.Control;
                }
                else
                { // DAOコン(INFmode)の左回転(押し皿・反時計回り)
                    if (scratchStatus != -1)
                    { // まだ左回転してなければ
                        scratchStatus = -1;
                        count[8]++;
                        countTotal++;
                    }
                    labelCount8Pull.BackColor = SystemColors.Control;
                    labelCount9Push.BackColor = Color.Aqua;
                }
                time = DateTime.Now.Ticks;
            }
            else if((DateTime.Now.Ticks - time) > 100000 * numericUpDown1.Value)
            { // 回転停止
                scratchStatus = 0;
                labelCount8Pull.BackColor = SystemColors.Control;
                labelCount9Push.BackColor = SystemColors.Control;

            }

            buffX = inputX;
        }
    }
}

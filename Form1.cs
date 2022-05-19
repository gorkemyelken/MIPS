using MetroSet_UI.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MIPS.Sim;
using System.Threading;
using System.Diagnostics;

namespace MIPS
{
    public partial class Form1 : MetroSetForm
    {
        public bool mode;
        public MIPSSimulator simulator;
        public SimStateMachine state = SimStateMachine.Ready;

        public Form1()
        {
            InitializeComponent();
            UpdateButtons();
            simulator = new MIPSSimulator(richTextBox1.Lines, richTextBox2.Lines, this);
            InitDGVbindings();
        }
        public void SendLog(string msg)
        {
            metroSetListBox1.AddItem(msg);
        }
        public void ReportError(string err)
        {
            metroSetListBox1.AddItem(err);
            metroSetListBox1.AddItem("Halted!");
            state = SimStateMachine.Stopped;
            RunFlag = false;
            backgroundWorker1.CancelAsync();
            simulator.isHalted = true;
        }
        public void updateState()
        {
            //metroSetTabControl2.SelectedTab.Controls[0].Refresh();
        }
        private void InitDGVbindings()
        {
            Registers.DataSource = simulator.Registers;
            var test = new BindingSource();
            test.DataSource = simulator.Registers.Where(i => i.Label.StartsWith("$s") && !i.Label.EndsWith("sp")).ToList();
            dataGridView2.DataSource = test;
            dataGridView2.Update();

            test = new BindingSource();
            test.DataSource = simulator.Registers.Where(i => i.Label.StartsWith("$v")).ToList();
            dataGridView3.DataSource = test;
            dataGridView3.Update();

            test = new BindingSource();
            test.DataSource = simulator.Registers.Where(i => i.Label.StartsWith("$a") && !i.Label.EndsWith("at")).ToList();
            dataGridView4.DataSource = test;
            dataGridView4.Update();

            test = new BindingSource();
            test.DataSource = simulator.Registers.Where(i => i.Label.StartsWith("$t"));
            dataGridView5.DataSource = test;
            dataGridView5.Update();

            test = new BindingSource();
            test.DataSource = simulator.Stack;
            dataGridView1.DataSource = test;
            dataGridView1.Update();

            test = new BindingSource();
            test.DataSource = simulator.Registers.Skip(32);
            dataGridView7.DataSource = test;
            dataGridView7.Update();
        }
        private int index = 0;

        private bool RunFlag = false;
        private void Enable(Button button) => button.Enabled = true;
        private void Disable(Button button) => button.Enabled = false;

        private void EnableEditing()
        {
            richTextBox1.ReadOnly = false;
            richTextBox2.ReadOnly = false;
            richTextBox1.Refresh();
            richTextBox2.Refresh();
        }
        private void DisableEditing()
        {
            richTextBox1.ReadOnly = true;
            richTextBox2.ReadOnly = true;
            richTextBox1.Refresh();
            richTextBox2.Refresh();
        }
        private void UpdateButtons()
        {
            switch (state)
            {
                case SimStateMachine.Ready:

                    Enable(RunOneButton);

                    Enable(ResetButton);
                    EnableEditing();
                    break;
                case SimStateMachine.Running:
                    Disable(RunOneButton);
                    Disable(ResetButton);
                    DisableEditing();
                    break;
                case SimStateMachine.Paused:

                    Enable(RunOneButton);

                    Enable(ResetButton);
                    DisableEditing();
                    break;
                case SimStateMachine.Stopped:

                    Enable(RunOneButton);

                    Enable(ResetButton);
                    EnableEditing();
                    break;
                case SimStateMachine.Finished:

                    Disable(RunOneButton);

                    Enable(ResetButton);
                    EnableEditing();
                    break;
                default:
                    break;
            }
        }
        private void UpdateSim()
        {
            simulator.PreLoad(richTextBox1.Lines, richTextBox2.Lines);
        }
        private void Run(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
                return;
            if (index == richTextBox1.Lines.Length)
            {
                index = 0;
            }

            if (state == SimStateMachine.Ready || state == SimStateMachine.Finished)
                UpdateSim();
            RunFlag = true;
            state = SimStateMachine.Running;
            backgroundWorker1.RunWorkerAsync(new Tuple<bool, int, string[]>(RunFlag, index, richTextBox1.Lines));
            UpdateButtons();
        }
        private void RunOneStep(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
                return;

            if (state == SimStateMachine.Ready || state == SimStateMachine.Finished)
                UpdateSim();
            RunFlag = false;
            state = SimStateMachine.Running;
            backgroundWorker1.RunWorkerAsync(new Tuple<bool, int, string[]>(RunFlag, index, richTextBox1.Lines));
            UpdateButtons();
        }
        private void Pause(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            RunFlag = false;
            state = SimStateMachine.Paused;
            UpdateButtons();
        }
        private void Stop(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            state = SimStateMachine.Stopped;
            RunFlag = false;
            index = 0;
            simulator.Stop(richTextBox1.Lines, richTextBox2.Lines);
            UpdateButtons();
        }

        private void Simulator_DoWork(object sender, DoWorkEventArgs e)
        {
            //BackgroundWorker args
            Tuple<bool, int, string[]> args = e.Argument as Tuple<bool, int, string[]>;//run flag,line number,input
            bool runFlag = args.Item1;
            int LineIndex = args.Item2;
            string[] Input = args.Item3;

            if (runFlag)
            {
                while (!simulator.DoneFlag)
                {
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    //Thread.Sleep(new TimeSpan(0, 0, 2)); //test
                    LineIndex = simulator.Execute();

                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    backgroundWorker1.ReportProgress(simulator.LastLine);
                }
            }
            else
            {
                //Thread.Sleep(new TimeSpan(0, 0, 2)); //test
                LineIndex = simulator.Execute();
                
                backgroundWorker1.ReportProgress(simulator.LastLine);
            }

            e.Result = new Tuple<int,bool>(LineIndex,simulator.DoneFlag);
        }
        private void EvaluateSelection(int index)
        {
            if (index >= richTextBox1.Lines.Length)
                return;

            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = Color.White;

            int indexPos = 0;

            for(int i = 0; i< index; i++)
            {
                indexPos += (richTextBox1.Lines[i].Length + 1);

            }

            richTextBox1.Select(indexPos, richTextBox1.Lines[index].Length);
            richTextBox1.SelectionBackColor = Color.FromArgb(95, 207, 255);
        }
        private void Simulator_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            index = e.ProgressPercentage;

            var test = new BindingSource();
            test.DataSource = simulator.MemoryTable;
            dataGridView6.DataSource = test;
            dataGridView6.Update();
            EvaluateSelection(index);
            index++;
            MetroSetTabControl2.SelectedTab.Controls[0].Refresh();
            AutoSwitch();
            UpdateButtons();
        }

        private void AutoSwitch()
        {
            if (false)
            {
                switch (simulator.args[0])
                {
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                        MetroSetTabControl2.SelectTab(0);
                        break;
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 24:
                    case 25:
                        MetroSetTabControl2.SelectTab(1);
                        break;
                    case 2:
                    case 3:
                        MetroSetTabControl2.SelectTab(2);
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        MetroSetTabControl2.SelectTab(3);
                        break;
                    case 29:
                        MetroSetTabControl2.SelectTab(4);
                        break;
                    default:
                        MetroSetTabControl2.SelectTab(6);
                        break;
                }
            }
        }

        private void Simulator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RunFlag = false;
            state = SimStateMachine.Paused;
            if (e.Cancelled)
            {
                SendLog("Cancelled");
                return;
            }
            var res = (e.Result as Tuple<int, bool>);

            index = res.Item1;
            if (res.Item2)
            {
                state = SimStateMachine.Finished;
            }
            if (simulator.isHalted)
            {
                Stop(null, null);
                return;
            }
            UpdateButtons();

        }

        private void InputChanged(object sender, EventArgs e)
        {
            if (!richTextBox1.Lines.SequenceEqual(simulator.InputText) || !richTextBox2.Lines.SequenceEqual(simulator.InputData))
                Stop(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double temp;
            double.TryParse("31.31", out temp);
            SendLog(temp.ToString());
        }


        private void Reset(object sender, EventArgs e)
        {
            simulator.Reset(richTextBox1.Lines, richTextBox2.Lines);
            MetroSetTabControl2.SelectedTab.Controls[0].Refresh();
        }

    }
}

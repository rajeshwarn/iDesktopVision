using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Controller.Handlers;

namespace Controller
{
    public partial class ExplorerForm : Form
    {
        private readonly Explorer _explorer;

        public ExplorerForm(Explorer explorer)
        {
            InitializeComponent();

            _explorer = explorer;
            _explorer.PathContentReceived += PathContentReceived;
            _explorer.FileReceived += FileReceived;
            _explorer.RequestPathContent("C:\\");
        }

        private void FileReceived(object sender, FileEventArgs e)
        {
            
        }

        private void PathContentReceived(object sender, PathContentEventArgs eventArgs)
        {
            Invoke((MethodInvoker) (() =>
            {
                textRemotePath.Text = eventArgs.Path;
                listRemoteFiles.Items.Clear();

                listRemoteFiles.Items.Add(new ListViewItem(new[]
                {
                    "..",
                    "-"
                }));
                
                foreach (var item in eventArgs.PathListing.OrderBy(i => i.Value != -1).ThenBy(i => i.Key))
                {
                    listRemoteFiles.Items.Add(new ListViewItem(new[]
                    {
                        item.Key,
                        item.Value == -1 ? "-" : Util.GetSIPrefix(item.Value)
                    }));
                }
            }));
        }

        private void textBoxPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            _explorer.RequestPathContent(textRemotePath.Text);
        }

        private void listViewItems_DoubleClick(object sender, EventArgs e)
        {
            var selected = listRemoteFiles.SelectedItems[0].Text;
            
            var isFile = listRemoteFiles.SelectedItems[0].SubItems[1].Text != "-";
            if (!isFile)
            {
                if (selected != "..")
                {
                    _explorer.RequestPathContent(Path.Combine(_explorer.CurrentDictionary, selected));
                }
                else
                {
                    var i = _explorer.CurrentDictionary.LastIndexOf('\\');
                    if (_explorer.CurrentDictionary.Length - 1 == i)
                        i = _explorer.CurrentDictionary.LastIndexOf('\\', i - 1);

                    var path = _explorer.CurrentDictionary.Substring(0, i + 1);
                    if (path == "")
                        path = _explorer.CurrentDictionary;

                    _explorer.RequestPathContent(path);
                }

                return;
            }

            var filePath = Path.Combine(_explorer.CurrentDictionary, selected);
            _explorer.RequestFile(filePath);
        }

        private void ExplorerForm_Load(object sender, EventArgs e)
        {
            ListLocalFiles("C:\\");
        }

        private void textLocalPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            ListLocalFiles(textLocalPath.Text);
        }

        private void ListLocalFiles(string directory)
        {
            textLocalPath.Text = directory;

            listLocalFiles.Items.Clear();
            listLocalFiles.Items.Add(new ListViewItem(new[]
            {
                "..",
                "-"
            }));

            try
            {

                var list = Directory.GetDirectories(directory).ToDictionary(Path.GetFileName, k => "-")
                    .Union(Directory.GetFiles(directory)
                        .ToDictionary(Path.GetFileName, k => Util.GetSIPrefix(new FileInfo(k).Length)));


                foreach (var item in list)
                {
                    listLocalFiles.Items.Add(new ListViewItem(new[]
                    {
                        item.Key,
                        item.Value
                    }));
                }
            }
            catch
            {
            }
        }

        private void listLocalFiles_DoubleClick(object sender, EventArgs e)
        {
            var selected = listLocalFiles.SelectedItems[0].Text;
            if (listLocalFiles.SelectedItems[0].SubItems[1].Text != "-") return;

            if (selected != "..")
            {
                ListLocalFiles(selected);
            }
            else
            {
                var parent = (listLocalFiles.Items.Count > 1)
                    ? Directory.GetParent(listLocalFiles.Items[1].Text).Name
                    : "C:\\";
                ListLocalFiles(parent);
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {

        }
    }
}

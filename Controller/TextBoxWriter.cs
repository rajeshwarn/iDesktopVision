using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Controller
{
    class TextBoxWriter : TextWriter
    {
        readonly TextBox _output;

        public TextBoxWriter(TextBox output)
        {
            _output = output;
        }

        public override void Write(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
            _output.Invoke((MethodInvoker)(() => _output.AppendText(new string(buffer, index, count))));
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}

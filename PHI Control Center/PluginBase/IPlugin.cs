using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PluginBase
{
    public interface IPlugin
    {
        Dictionary<string, FunctionBox> GetFunctions();
        bool Selftest();
        string GetPluginName();
        void Init(IGHP_Implementation serial);
    }

    public interface IGHP_Implementation
    {
        void Send_Message(byte[] bytes);
    }

    public interface IDialogForm
    {
        List<object> go(List<object> start);
    }

    public class FunctionBox
    {
        public delegate void Function(byte chan, int val, params object[] par);
        public Function function;
        public byte functionType;
        public IDialogForm dialogForm;

        public FunctionBox(Function f, byte fType, IDialogForm df)
        {
            function = f;
            functionType = fType;
            dialogForm = df;
        }

        public List<object> aditionalArgs(List <object> start)
        {
            return dialogForm.go(start);
        }
    }
}

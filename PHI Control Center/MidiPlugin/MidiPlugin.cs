using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginBase;
using TobiasErichsen.teVirtualMIDI;

namespace MidiPlugin
{
    public class MidiPlugin : IPlugin
    {
        public const byte CONTROL_CHANGE = 0xB0;
        public const byte SET_LED = (byte)'L';

        public Dictionary<String, FunctionBox> Functions;
        TeVirtualMIDI MidiPort;
        bool init = false;
        IGHP_Implementation serial;
        dialogForm df;

        public Dictionary<string, FunctionBox> GetFunctions()
        {
            return Functions;
        }

        public MidiPlugin()
        {
            df = new dialogForm();
            Functions = new Dictionary<string, FunctionBox>();
            Functions.Add("MidiCCAnalog", new FunctionBox(MidiCCAnalog, 0, df));
            Functions.Add("MidiCCButton", new FunctionBox(MidiCCAnalog, 1, df));
            Functions.Add("MidiCCRGBButton", new FunctionBox(MidiCCAnalog, 2, df));
        }

        public void MidiCCAnalog(byte chan, int val, params object[] par)
        {
            if (init)
            {
                MidiPort.sendCommand(new byte[] { (byte)(CONTROL_CHANGE + (byte)par[1]), (byte)par[0], (byte)(val / 8) });
            }
        }

        public void MidiCCButton(byte chan, int val, params object[] par)
        {
            if (init)
            {
                MidiPort.sendCommand(new byte[] { (byte)(CONTROL_CHANGE + (byte)par[1]), (byte)par[0], (byte)(val * 127) });
            }
        }

        public void MidiCCRGBButton(byte chan, int val, params object[] par)
        {
            if (init)
            {
                MidiPort.sendCommand(new byte[] { (byte)(CONTROL_CHANGE+(byte)par[1]), (byte)par[0], (byte)(val * 127) });
                serial.Send_Message(new byte[] { SET_LED, chan, (byte)(val * 255), 0, 0 });
            }
        }

        public bool Selftest()
        {
            try
            {
                TeVirtualMIDI vm = new TeVirtualMIDI("PHI CC");
                vm.shutdown();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetPluginName()
        {
            return "MidiPlugin";
        }

        public void Init(IGHP_Implementation GHP)
        {
            MidiPort = new TeVirtualMIDI("PHI CC");
            serial = GHP;
            init = true;
        }
    }
}

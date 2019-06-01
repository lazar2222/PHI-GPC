using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PluginBase;
using TobiasErichsen.teVirtualMIDI;

namespace MidiPlugin
{
    public class MidiPlugin : IPlugin
    {
        public const byte CONTROL_CHANGE = 0xB0;
        public const byte CONTINUE = 0xFB;
        public const byte PLAY = 0xFA;
        public const byte STOP = 0xFC;
        public const byte SET_LED = (byte)'L';

        public Dictionary<String, FunctionBox> Functions;
        TeVirtualMIDI MidiPort;
        bool init = false;
        dialogForm df;
        Dictionary<byte, bool> states;
        Dictionary<byte, bool> ledstates;
        Dictionary<byte, int> encval;
        bool ofs = false;
        bool encshift = false;
        string[] lcd;

        public Dictionary<string, FunctionBox> GetFunctions()
        {
            return Functions;
        }

        public MidiPlugin()
        {
            df = new dialogForm();
            Functions = new Dictionary<string, FunctionBox>
            {
                { "MidiCCAnalog", new FunctionBox(MidiCCAnalog, CType.Analog, df,false) },
                { "MidiCCMomentaryButton", new FunctionBox(MidiCCMomentaryButton, CType.Button, df,false) },
                { "MidiCCToggleButton", new FunctionBox(MidiCCToggleButton, CType.Button, df,false) },
                { "MidiCCEncoder", new FunctionBox(MidiCCEncoder, CType.Encoder, df,false) },
                { "MidiCCLED", new FunctionBox(MidiCCLED, CType.RGBLED, df,true) },
                { "MidiCCLCD", new FunctionBox(MidiCCLCD, CType.LCD, df,true) },
                { "MidiPLAY", new FunctionBox(MidiPLAY, CType.Button, df,false) },
                { "MidiCONT", new FunctionBox(MidiCONT, CType.Button, df,false) },
                { "MidiSTOP", new FunctionBox(MidiSTOP, CType.Button, df,false) },
                { "MidiENCSHIFT", new FunctionBox(MidiENCSHIFT, CType.Button, df,false) }
            };
        }

        private void MidiCCLCD(byte chan, int val, List<object> par, IGHP ghp)
        {
            lcd[0] = "PLAY CONT STOP ";
            lcd[0] += encshift ? "SHIFT" : "NORM ";
            lcd[1] = "                    ";
            if (encshift)
            {
                lcd[2] = "CC68 CC69 CC70 CC71 ";
                lcd[3] = encval[68].ToString().PadLeft(4) + " " + encval[69].ToString().PadLeft(4) + " " + encval[70].ToString().PadLeft(4) + " " + encval[71].ToString().PadLeft(4) + " ";
            }
            else
            {
                lcd[2] = "CC64 CC65 CC66 CC67 ";
                lcd[3] = encval[64].ToString().PadLeft(4) + " " + encval[65].ToString().PadLeft(4) + " " + encval[66].ToString().PadLeft(4) + " " + encval[67].ToString().PadLeft(4) + " ";
            }
            for (int i = 0; i < 4; i++)
            {
                if (lcd[i] != lcd[i + 4])
                {
                    Console.WriteLine(lcd[i].Length);
                    ghp.SendMessage(108,(byte)i);
                    for (int j = 0; j < 20; j++)
                    {
                        ghp.SendMessage((byte)lcd[i][j]);
                    }
                    Thread.Sleep(50);
                    lcd[i + 4] = lcd[i];
                }
            }

        }

        private void MidiENCSHIFT(byte chan, int val, List<object> par, IGHP ghp)
        {
            if (val == 1)
            {
                encshift = !encshift;
            }
        }

        private void MidiSTOP(byte chan, int val, List<object> par, IGHP ghp)
        {
            if (init && val==1)
            {
                MidiPort.sendCommand(new byte[] { STOP });
            }
        }

        private void MidiCONT(byte chan, int val, List<object> par, IGHP ghp)
        {
            if (init && val == 1)
            {
                MidiPort.sendCommand(new byte[] { CONTINUE });
            }
        }

        private void MidiPLAY(byte chan, int val, List<object> par, IGHP ghp)
        {
            if (init && val == 1)
            {
                MidiPort.sendCommand(new byte[] { PLAY });
            }
        }

        public void MidiCCAnalog(byte chan, int val, List<object> par, IGHP gHP)
        {
            if (init)
            {
                MidiPort.sendCommand(new byte[] { (byte)(CONTROL_CHANGE + Convert.ToByte(par[1])), Convert.ToByte(par[0]), (byte)(val / 8) });
            }
        }

        public void MidiCCMomentaryButton(byte chan, int val, List<object> par, IGHP gHP)
        {
            if (init)
            {
                MidiPort.sendCommand(new byte[] { (byte)(CONTROL_CHANGE + Convert.ToByte(par[1])), Convert.ToByte(par[0]), (byte)(val * 127) });
            }
        }

        public void MidiCCToggleButton(byte chan, int val, List<object> par, IGHP gHP)
        {
            if (val == 1)
            {
                if (!states.ContainsKey(chan)) { states.Add(chan, false); }
                states[chan] = !states[chan];
                if (init)
                {
                    MidiPort.sendCommand(new byte[] { (byte)(CONTROL_CHANGE + Convert.ToByte(par[1])), Convert.ToByte(par[0]), (byte)(states[chan] ? 127 : 0) });
                }
            }
        }

        public void MidiCCEncoder(byte chan, int val, List<object> par, IGHP gHP)
        {
            byte cc = Convert.ToByte(par[0]);
            if (chan < 4 && encshift) { cc += 4; }
            if (val == 1)
            {
                if (!encval.ContainsKey(cc)) { encval.Add(cc, 0); }
                encval[cc]++;
            }
            else
            {
                if (!encval.ContainsKey(cc)) { encval.Add(cc, 0); }
                encval[cc]--;
            }
            encval[cc] = Math.Min(Math.Max(encval[cc], 0), 127);
            if (init)
            {
                 MidiPort.sendCommand(new byte[] { (byte)(CONTROL_CHANGE + Convert.ToByte(par[1])), Convert.ToByte(par[0]), (byte)encval[cc] });
            }
        }

        public void MidiCCLED(byte chan, int val, List<object> par, IGHP gHP)
        {
            if (states.ContainsKey(chan))
            {
                if (!ledstates.ContainsKey(chan)) { ledstates.Add(chan, false); }
                if (states[chan] != ledstates[chan])
                {
                    ledstates[chan] = states[chan];
                    gHP.SendMessage(76, chan, (byte)(states[chan] ? 255 : 0), 0, 0);
                }
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

        public void Init()
        {
            MidiPort = new TeVirtualMIDI("PHI CC");
            states = new Dictionary<byte, bool>();
            ledstates = new Dictionary<byte, bool>();
            encval = new Dictionary<byte, int>();
            lcd = new string[8];
            for (int i = 64; i < 72; i++)
            {
                encval.Add((byte)i, 0);
            }
            lcd[4] = "";
            lcd[5] = "";
            lcd[6] = "";
            lcd[7] = "";
            init = true;
        }

        public void Close()
        {
            MidiPort.shutdown();
        }
    }
}

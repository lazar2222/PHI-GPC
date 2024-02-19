using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace MagicConsole
{
    class Program
    {
        static InputSimulator IS = new InputSimulator();
        static void Main(string[] args)
        {
            UdpClient listener = new UdpClient(53729);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 53729);
            while (true)
            {
                byte[] receive_byte_array = listener.Receive(ref groupEP);
                if (receive_byte_array.Length == 1)
                {
                    if (receive_byte_array[0] == 'D')
                    {
                        IS.Keyboard.KeyPress(VirtualKeyCode.DOWN);
                    }
                    else if (receive_byte_array[0] == 'U')
                    {
                        IS.Keyboard.KeyPress(VirtualKeyCode.UP);
                    }
                }
            }
        }
    }
}

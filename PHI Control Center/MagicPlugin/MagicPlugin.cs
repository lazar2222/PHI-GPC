using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using PluginBase;

namespace MagicPlugin
{
    public class MagicPlugin : IPlugin
    {
        Socket sending_socket;
        public Dictionary<String, FunctionBox> Functions;
        bool init = false;
        dialogForm df;

        public MagicPlugin()
        {
            df = new dialogForm();
            Functions = new Dictionary<string, FunctionBox>
            {
                { "MagicUP", new FunctionBox(MagicUP, CType.Button, df,false) },
                { "MagicDOWN", new FunctionBox(MagicDOWN, CType.Button, df,false) }
            };
        }

        private void MagicDOWN(byte chan, int val, List<object> par, IGHP ghp)
        {
            if (val == 1)
            {
                if (init)
                {
                    IPAddress send_to_address = IPAddress.Parse(Convert.ToString(par[0]));
                    IPEndPoint sending_end_point = new IPEndPoint(send_to_address, 53729);
                    try
                    {
                        sending_socket.SendTo(new byte[] { (byte)'D' }, sending_end_point);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
        }

        private void MagicUP(byte chan, int val, List<object> par, IGHP ghp)
        {
            if (val == 1)
            {
                if (init)
                {
                    IPAddress send_to_address = IPAddress.Parse(Convert.ToString(par[0]));
                    IPEndPoint sending_end_point = new IPEndPoint(send_to_address, 53729);
                    try
                    {
                        sending_socket.SendTo(new byte[] { (byte)'U' }, sending_end_point);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
        }

        public void Close()
        {
            sending_socket.Close();
        }

        public Dictionary<string, FunctionBox> GetFunctions()
        {
            return Functions;
        }

        public string GetPluginName()
        {
            return "Magic";
        }

        public void Init()
        {
            sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
            init = true;
        }

        public bool Selftest()
        {
            try
            {
                Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sending_socket.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHI_Control_Center.Classes
{
    class Device:IComparable<Device>
    {
        public String Name;
        public byte DeviceId;
        public bool IsAddon;
        public byte AddonAddress;
        public List<Bank> Banks;
        public List<PersistentVariable> PersistentVariables;

        public Device()
        {
            Banks = new List<Bank>();
            PersistentVariables = new List<PersistentVariable>();
        }

        public int CompareTo(Device b)
        {
            int compinta =DeviceId + (IsAddon ? 1 + AddonAddress : 0);
            int compintb =b.DeviceId+(b.IsAddon?1+b.AddonAddress:0);
            return compinta.CompareTo(compintb);
        }

        public override string ToString()
        {
            if (IsAddon) { return "    " + Name; }
            return Name;
        }

    }

    class Bank
    {
        public String Name;
        public List<Control> Controls;

        public Bank()
        {
            Controls = new List<Control>();
        }
    }

    class Control
    {
        public enum TType
        {
            Analog = 0,
            Button = 1,
            RGBButton = 2,
            Encoder = 3,
            LCD = 4
        };

        public TType Type;
        public byte Id;
        public byte Lin;
        public byte Char;

        public int X;
        public int Y;
        public int W;
        public int H;

    }

    class PersistentVariable
    {
        public string Name;
        public byte Id;
    }
}

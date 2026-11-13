using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Neptunia_Modding.Structures.Database.Character.Effects
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SpecialEffekt

    {
        public unsafe ushort Effekt;
        public unsafe short Value;
    }
}

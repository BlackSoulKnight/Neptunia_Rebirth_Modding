using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Neptunia_DataStructs.Structures.Character.Attacks
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ComboRow
    {
        public unsafe ComboAttack Rush;
        public unsafe ComboAttack Break;
        public unsafe ComboAttack Power;
    }
}

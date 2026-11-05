using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Neptunia_Modding.Structures.Character.Attacks
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ComboAttack
    {
        public unsafe short CharacterID;
        public unsafe short AttackID;
    }
}

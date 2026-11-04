using System.Runtime.InteropServices;

namespace Neptunia_DataStructs.Structures.Character.Effects
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PassiveEffekt
    {
        public unsafe int Passive;
        public unsafe int IsActive;
    }
}

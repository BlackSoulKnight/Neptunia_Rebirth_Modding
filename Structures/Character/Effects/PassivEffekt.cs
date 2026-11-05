using System.Runtime.InteropServices;

namespace Neptunia_Modding.Structures.Character.Effects
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PassiveEffekt
    {
        public unsafe int Passive;
        public unsafe int IsActive;
    }
}

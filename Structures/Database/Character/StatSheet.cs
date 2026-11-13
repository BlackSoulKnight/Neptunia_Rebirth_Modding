using System.Runtime.InteropServices;

namespace Neptunia_Modding.Structures.Database.Character
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CurrentStats
    {
        public unsafe int CurrentHP; 
        public unsafe int UnkownField; 
        public unsafe int CurrentSP; 
        public unsafe int CurrentGuard;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct StatSheet
    {
        public unsafe int MaxHP; //[0x10]
        public unsafe int MaxCP; //[0x14]
        public unsafe int MaxSP; //[0x18]
        public unsafe int STR; //[0x1C]
        public unsafe int VIT; //[0x20]
        public unsafe int INT; //[0x24]
        public unsafe int MEN; //[0x28]
        public unsafe int AGI; //[0x2C]
        public unsafe int TEC; //[0x30]
        public unsafe int AVD; //[0x34]
        public unsafe int LUK; //[0x38]
        public unsafe int MOV; //[0x3C]
        public unsafe int PhysRes; //[0x40]
        public unsafe int FireRes; //[0x44]
        public unsafe int IceRes; //[0x48]
        public unsafe int WindRes; //[0x4C]
        public unsafe int LightRes; //[0x50]
        public unsafe int SliceRes; //[0x54]
        public unsafe int BluntRes; //[0x58]
        public unsafe int PierceRes; //[0x5C]
        public unsafe int BulletRes; //[0x60]
        public unsafe int BeamRes; //[0x64]
    }
}

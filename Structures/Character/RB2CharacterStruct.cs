using Neptunia_DataStructs.Structures.Character.Attacks;
using Neptunia_DataStructs.Structures.Character.Effects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neptunia_DataStructs.Structures.Character
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct RB2CharacterStruct
    {
        [FieldOffset(0x0)] public unsafe short Form; //0x8120 for Fake purple heart
        [FieldOffset(0x2)] public unsafe short Form2;
        [FieldOffset(0x4)] public unsafe short CharacterID;
        [FieldOffset(0x6)] public unsafe short Type; //1byte
        [FieldOffset(0x8)] public fixed byte Name[32];
        [FieldOffset(0x28)] public unsafe short CurrentEXP;
        [FieldOffset(0x2C)] public unsafe byte LevelProgressionID; //1byte
        [FieldOffset(0x2D)] public unsafe byte ShareAlignment; //1byte
        [FieldOffset(0x2E)] public unsafe short Level;
        [FieldOffset(0x30)] public unsafe short MenuIcon;
        [FieldOffset(0x32)] public unsafe short ModelID;
        [FieldOffset(0x34)] public unsafe short VariantID;
        [FieldOffset(0x36)] public unsafe short ViralChangeIntoIDLily1;
        [FieldOffset(0x36)] public unsafe short Lily1;
        [FieldOffset(0x38)] public unsafe short Lily2;
        //[FieldOffset(0x3A)] public unsafe short;
        [FieldOffset(0x3C)] public unsafe short PartnerID; //NEVER be same as one of the other party members
        [FieldOffset(0x3E)] public unsafe short ViralChance; //1byte Help; 2byte Self
        [FieldOffset(0x40)] public unsafe StatSheet CurrentStats; //only used in battle
        [FieldOffset(0x50)] public unsafe StatSheet Stats;
        //[FieldOffset(0xA8)] public unsafe int;
        [FieldOffset(0xAC)] public unsafe int WeaponID;
        [FieldOffset(0xB0)] public unsafe int ArmorID;
        [FieldOffset(0xB4)] public unsafe int OrnamentID;
        [FieldOffset(0xB8)] public unsafe int ClothingBodyID;
        [FieldOffset(0xBC)] public unsafe int ClothingHeadID;
        [FieldOffset(0xC0)] public unsafe int CpuCoreID;
        [FieldOffset(0xC4)] public unsafe int CpuHeadID;
        [FieldOffset(0xC8)] public unsafe int CpuBodyID;
        [FieldOffset(0xCC)] public unsafe int CpuShoulderID;
        [FieldOffset(0xD0)] public unsafe int CpuWingID;
        [FieldOffset(0xD4)] public unsafe int CpuLegID;
        [FieldOffset(0xD8)] public unsafe StatusEffekts StatusEffekt;
        [FieldOffset(0x170)] public unsafe SpecialEffekts SpecialEffekts;
        [FieldOffset(0x184)] public unsafe PassiveEffekts PassiveEffekts;
        [FieldOffset(0x1C0)] public unsafe AttackList Attacks;
        [FieldOffset(0x490)] public unsafe ComboRow ComboRow1;
        [FieldOffset(0x49C)] public unsafe ComboRow ComboRow2;
        [FieldOffset(0x4A8)] public unsafe ComboRow ComboRow3;
        [FieldOffset(0x4B4)] public unsafe ComboAttack ComboFinisher1;
        [FieldOffset(0x4B8)] public unsafe ComboAttack ComboFinisher2;
        [FieldOffset(0x4BC)] public unsafe ComboAttack ComboFinisher3;
        [FieldOffset(0x4C0)] public unsafe ComboAttack ComboFinisher4;
        [FieldOffset(0x4C4)] public unsafe float UnkownSpeed; //3byte
        [FieldOffset(0x4C8)] public unsafe float MovementSpeedModifier; //3byte
        //[FieldOffset(0x4CC)] public unsafe int;  100-50% HP
        //[FieldOffset(0x4D0)] public unsafe int;  50-25%  HP
        //[FieldOffset(0x4D4)] public unsafe int;  25-0%   HP
        [FieldOffset(0x4D8)] public unsafe uint BattleVoice;
        [FieldOffset(0x4DC)] public unsafe int GuardPoints;
        [FieldOffset(0x4E0)] public unsafe int ExpReward;
        [FieldOffset(0x4E4)] public unsafe int MoneyReward;
        [FieldOffset(0x4E8)] public unsafe int OverallDropRate;
        [FieldOffset(0x4EC)] public unsafe int DropRate1;
        [FieldOffset(0x4F0)] public unsafe int DropRate2;
        [FieldOffset(0x4F4)] public unsafe int DropRate3;
        [FieldOffset(0x4F8)] public unsafe int EnemyDrop1;
        [FieldOffset(0x4FC)] public unsafe int EnemyDrop2;
        [FieldOffset(0x500)] public unsafe int EnemyDrop3;
        [FieldOffset(0x504)] public unsafe byte* DescriptionPointer;
    }

    [InlineArray(19)]
    public unsafe struct StatusEffekts
    {
        StatusEffekt _element;
    }
    [InlineArray(5)]
    public unsafe struct SpecialEffekts
    {
        SpecialEffekt _element;
    }
    [InlineArray(5)]
    public unsafe struct PassiveEffekts
    {
        PassiveEffekt _element;
    }
    [InlineArray(90)]
    public unsafe struct AttackList
    {
        Attack _element;
    }
}

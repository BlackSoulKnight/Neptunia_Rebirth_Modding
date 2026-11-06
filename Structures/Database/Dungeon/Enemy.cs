using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Neptunia_Modding.Structures.Database.Dungeon
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct EnemyOverworldAI
    {
        public float DetectionRange;
        public float DetectionCone;
        public uint MinWaitTime;
        public uint MaxWaitTime;
        public uint MinWalkDistance;
        public uint MaxWalkDistance;
    }
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct EnemyEncounterInfo
    {
        public ushort EnemyID;
        public ushort MinQuantity;
        public ushort MaxQuantity;
    }


    [StructLayout(LayoutKind.Explicit, Size = 0x1d90)]
    public unsafe struct DungeonEnemy
    {
        //[FieldOffset(0x0)] short ;
        //[FieldOffset(0x2)] short ;
        [FieldOffset(0x4)] EnemyOverworldAI Unaware;
        [FieldOffset(0x1C)] EnemyOverworldAI Alerted;
        //[FieldOffset(0x34)] float ;
        [FieldOffset(0x38)] ushort RespawnTime;
        //[FieldOffset(0x3A)] ushort ;
        //[FieldOffset(0x3C)] ushort ;
        [FieldOffset(0x3E)] DungeonEnemiesEnocunter EncounterInfo;
        //[FieldOffset(0x66)] short ;
        //[FieldOffset(0x68)] short ;
        //[FieldOffset(0x6A)] short ;

    }
    [InlineArray(15)]
    public struct DungeonEnemies
    {
        private DungeonEnemy _firstElement;
    }
    [InlineArray(5)]
    public struct DungeonEnemiesEnocunter
    {
        private EnemyEncounterInfo _firstElement;
    }
}

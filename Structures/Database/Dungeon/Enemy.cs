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
        public float DetectionRange { get; set; }
        public float DetectionCone { get; set; }
        public uint MinWaitTime { get; set; }
        public uint MaxWaitTime { get; set; }
        public uint MinWalkDistance { get; set; }
        public uint MaxWalkDistance { get; set; }
    }
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct EnemyEncounterInfo
    {
        public ushort EnemyID { get; set; }
        public ushort MinQuantity { get; set; }
        public ushort MaxQuantity { get; set; }
        public ushort u4 { get; set; } //always 0
    }


    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DungeonEnemy
    {
        public short unkown1 { get; set; } //always 0x1 even when no mob exist
        public short unkown2 { get; set; } //always 0xf when mob exist
        public EnemyOverworldAI Searching { get; set; }
        public EnemyOverworldAI Chasing { get; set; }
        public float AlertCheckRate { get; set; } // Value divided roughly by 57   -> smaller == faster
        public ushort RespawnTime { get; set; }
        public ushort unkown3 { get; set; } //always 3
        public ushort unkown4 { get; set; } //always 0
        public DungeonEnemiesEncounter Encounter { get; set; }
        public short unkown5 { get; set; } //always 0
        public short unkown6 { get; set; } //always 0
        public short unkown7 { get; set; } //always 0
    }
    [InlineArray(15)]
    public struct DungeonEnemies
    {
        public DungeonEnemy _firstElement;
    }
    [InlineArray(5)]
    public struct DungeonEnemiesEncounter
    {
        public EnemyEncounterInfo _firstElement;
    }
}

using Neptunia_Modding.Modding.SaveFile;
using Neptunia_Modding.Structures.Database.Dungeon;
using Neptunia_Modding.Structures.SaveFile;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace Neptunia_Modding.Modding.CustomContent
{
    public unsafe class CustomDungeon : AdditionalContent
    {
        string Path;

        const string DungeonInfo = "/Dungeon.json";
        const string OverworldInfo = "/OverWorld.json";
        const string NormalEnemies = "/Enemies/NormalEnemies.json";
        const string ChangeEnemies = "/Enemies/ChangeEnemies.json";
        const string ChangeDungeonEnemies = "/Enemies/ChangeDungeonEnemies.json";
        const string NormalGather = "/Gather/NormalGather.json";
        const string ChangeGather = "/Gather/ChangeGather.json";
        const string ChangeDungeonGather = "/Gather/ChangeDungeonGather.json";
        const int StringSize = 32;


        public CustomDungeon(string modPath)
        {
            Path = modPath;
        }

        GatherSpots LoadGatherSpots(string path)
        {
            GatherSpots gatherSpots = new();

            using (StreamReader file = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                var o2 = (JArray)JToken.ReadFrom(reader);
                for (int i = 0; i < 15 && i < o2.Count; i++)
                {
                    DB_Dungeon_GatherSpot newSpot = ((JObject)o2[i]).ToObject<DB_Dungeon_GatherSpot>();
                    gatherSpots[i] = newSpot;
                }
            }
            return gatherSpots;
        }
        DungeonEnemies LoadEnemies(string path)
        {
            DungeonEnemies enemies = new();
            using (StreamReader file = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                var o2 = (JArray)JToken.ReadFrom(reader);
                for (int i = 0; i < 15 || i < o2.Count; i++)
                {
                    DungeonEnemy newEnemy = ((JObject)o2[i]).ToObject<DungeonEnemy>();
                    var obj = (JObject)o2[i];
                    if (obj.ContainsKey("EncounterInfo"))
                    {
                        DungeonEnemiesEncounter encounterInfo = new();
                        var encounter = (JArray)obj["EncounterInfo"]!;
                        for (int x = 0; x < 5 && x < encounter.Count; x++)
                        {
                            encounterInfo[x] = ((JObject)encounter[x]).ToObject<EnemyEncounterInfo>();
                        }
                        newEnemy.Encounter = encounterInfo;
                    }

                    newEnemy.unkown1 = 1;
                    if (newEnemy.Encounter[0].EnemyID != 0)
                    {
                        newEnemy.unkown2 = 0xf;
                        newEnemy.unkown3 = 0x3;
                        newEnemy.unkown4 = 0x0;
                        newEnemy.unkown5 = 0x0;
                        newEnemy.unkown6 = 0x0;
                        newEnemy.unkown7 = 0x0;
                    }

                    enemies[i] = newEnemy;
                }
            }
            return enemies;
        }
        unsafe bool InitDungeon(Dungeon* newDungeon, string path)
        {
            using (StreamReader file = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                var dungeonMeta = (JObject)JToken.ReadFrom(reader);
                if (!dungeonMeta.ContainsKey("Dungeon"))
                    return false;
                if (!dungeonMeta.ContainsKey("DungeonName"))
                    return false;

                var dungeon = (JObject)dungeonMeta["Dungeon"]!;

                if (dungeon.ContainsKey("DungeonID"))
                    newDungeon->DungeonID = (ushort)dungeon["DungeonID"]!;
                if (dungeon.ContainsKey("BGM"))
                    newDungeon->BGM = (ushort)dungeon["BGM"]!;
                if (dungeon.ContainsKey("DungeonMap"))
                    newDungeon->DungeonMap = (ushort)dungeon["DungeonMap"]!;
                if (dungeon.ContainsKey("DungeonMapVariant"))
                    newDungeon->DungeonMapVariant = (ushort)dungeon["DungeonMapVariant"]!;
                if (dungeon.ContainsKey("ScoutDays"))
                    newDungeon->ScoutDays = (ushort)dungeon["ScoutDays"]!;
                if (dungeon.ContainsKey("ScoutDuration"))
                    newDungeon->ScoutDuration = (ushort)dungeon["ScoutDuration"]!;
                if (dungeon.ContainsKey("ScoutTimeout"))
                    newDungeon->ScoutTimeout = (ushort)dungeon["ScoutTimeout"]!;
                if (dungeon.ContainsKey("TreasureID"))
                {
                    var treasureIdArray = (JArray)dungeon["TreasureID"]!;
                    for (int i = 0; i < 10 && i < treasureIdArray.Count; i++)
                        treasureIdArray[i] = (ushort)treasureIdArray[i];
                }
                if (dungeon.ContainsKey("Connections"))
                {
                    var connectionsArray = (JArray)dungeon["Connections"]!;
                    for (int i = 0; i < 5 && i < connectionsArray.Count; i++)
                        connectionsArray[i] = (ushort)connectionsArray[i];
                }
                if (dungeon.ContainsKey("MapInfo"))
                {
                    var mapInfo = (JObject)dungeon["MapInfo"]!;
                    DungeonMapInfo map = new();
                    newDungeon->MapInfo = mapInfo.ToObject<DungeonMapInfo>();
                }

                fixed (byte* utf8String = Encoding.UTF8.GetBytes(dungeonMeta["DungeonName"]!.ToString())) {
                    byte* target = null;
                    if (Dungeons.NameList.ContainsKey(newDungeon->DungeonID))
                        target = Dungeons.NameList[newDungeon->DungeonID];
                    else
                        target = (byte*)Marshal.AllocHGlobal(sizeof(byte) * StringSize);
                    Buffer.MemoryCopy(utf8String,target,StringSize,StringSize);
                    newDungeon->NamePointer = (nint)target;
                    Dungeons.NameList[newDungeon->DungeonID] = target;
                }
            }
            return true;
        }
        override protected void LoadMod()
        {
            //most important files

            if (!File.Exists(Path + DungeonInfo))
                return;
            Dungeon* newDungeon = (Dungeon*)Marshal.AllocHGlobal(sizeof(Dungeon));
            //Init everything with 0
            *newDungeon = new Dungeon();
            if (!InitDungeon(newDungeon, Path + DungeonInfo))
                return;

            if (File.Exists(Path + NormalEnemies))
                newDungeon->NormalEnemies = LoadEnemies(Path + NormalEnemies);
            if (File.Exists(Path + ChangeEnemies))
                newDungeon->ChangeEnemy = LoadEnemies(Path + ChangeEnemies);
            if (File.Exists(Path + ChangeDungeonEnemies))
                newDungeon->ChangeDungeonEnemy = LoadEnemies(Path + ChangeDungeonEnemies);

            if (File.Exists(Path + NormalGather))
                newDungeon->NormalGather = LoadGatherSpots(Path + NormalGather);
            if (File.Exists(Path + ChangeGather))
                newDungeon->ChangeGather = LoadGatherSpots(Path + ChangeGather);
            if (File.Exists(Path + ChangeDungeonGather))
                newDungeon->ChangeDungeonGather = LoadGatherSpots(Path + ChangeDungeonGather);


            Dungeons.CreateDungeon(newDungeon);
        }

    }
}

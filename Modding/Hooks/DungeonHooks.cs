using Neptunia_Modding.Modding.SaveFile;
using Neptunia_Modding.Structures.Database.Dungeon;
using Neptunia_Modding.Structures.SaveFile;
using Neptunia_Modding.Test;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Neptunia_Modding.Modding.Hooks
{
    public unsafe class DungeonHooks
    {
        public static void _sprintfHelper(string source,byte* dest,int size)
        {
            var data = Encoding.UTF8.GetBytes(source);
            int i = 0;
            for (; i < size && i < data.Length; i++)
                dest[i] = data[i];
            for (; i < size; i++)
                dest[i] = 0;
        }

        public static List<IAsmHook> _asmHooks = new();
        public static IReverseWrapper<OnResetEntities> _onResetEntities;
        public static IReverseWrapper<OnValidateDungeon> _onValidateDungeon;
        public static IReverseWrapper<OnGetDungeonOrCreateNew> _onGetDungeonOrCreateNew;
        public static IReverseWrapper<OnAddDungeon> _onAddDungeon;
        public static IReverseWrapper<OnCountDungeonOnMap> _onCountDungeonOnMap;
        public static IReverseWrapper<OnPlaceDungeonOnMap> _onPlaceDungeonOnMap;
        public static IReverseWrapper<OnReduceScoutDays> _onReduceScoutDays;
        public static IReverseWrapper<OnMemDataCoreSetupFinish> _onDungeonDBInit;

        public static IHook<OnGetMapTexturePath> _onGetMapTexturePath;   
        public static IHook<OnGetMapTexturePath> _onGetMapEffektPath;
        public static List<IHook> _iHooks = new();
        public static IFunction<DunnoButImportant> _smth;

        [Function(CallingConventions.Stdcall)]
        public delegate int DunnoButImportant(Dungeon* dungeon);

        [Function(CallingConventions.Stdcall)]
        public delegate void OnResetEntities();
        //A1 ?? ?? ?? ?? 8D 88 ?? ?? ?? ?? 05 30 03 01 00 3B C1 73 ?? 56
        public static void ResetEntities()
        {
            DungeonSaveManager.ResetDungeonEntities();
        }

        [Function(CallingConventions.Stdcall)]
        public delegate bool OnValidateDungeon(int dungeonId, int subworld);
        //3B F2 0F 83
        public static bool ValidateDungeon(int dungeonId,int subworld)
        {
            return DungeonSaveManager.ValidateDungeon(dungeonId, subworld);
        }

        [Function(CallingConventions.Stdcall)]
        public delegate DungeonSave* OnGetDungeonOrCreateNew(ushort dungeonID);
        //55 8B EC 51 8B 15 ?? ?? ?? ?? 53 66 8B 5D
        public static DungeonSave* GetDungeonOrCreateNew(ushort dungeonID)
        {
            return DungeonSaveManager.GetDungeonSaveOrCreateNew(dungeonID);
        }

        [Function(CallingConventions.Stdcall)]
        public delegate bool OnAddDungeon(Dungeon* dungeon, bool enable);
        //55 8B EC 53 8B 5D ?? 85 DB 75 ?? 32 C0 5B 5D C3 A1
        public static bool AddDungeon(Dungeon* dungeon, bool enable)
        {

            DungeonSaveManager.AddDungeon(dungeon, enable);
            //Stella Dungeon setup

            return true;
        }

        [Function(CallingConventions.Stdcall)]
        public delegate int OnCountDungeonOnMap(int subworld);
        //55 8B EC 8B 0D ?? ?? ?? ?? 33 D2 57 69 81 ?? ?? ?? ?? 3C 20 00 00
        public static int CountDungeonOnMap(int subworld) {
            return DungeonSaveManager.GetDungeonCountOnMap(subworld);
        }



        [Function(CallingConventions.Stdcall)]
        public delegate int OnPlaceDungeonOnMap(uint subworld, nuint par2, uint maxCount);
        //55 8B EC 83 EC 08 53 8B 1D ?? ?? ?? ?? 56 69 83 ?? ?? ?? ?? 3C 20 00 00
        public static int PlaceDungeonOnMap(uint subworld,nuint par2,uint maxCount)
        {
            return DungeonSaveManager.PlaceDungeonOnMap(subworld,par2, maxCount);
        }

        //55 8B EC 8B 0D ?? ?? ?? ?? 56 69 81 ?? ?? ?? ?? 3C 20 00 00
        //Unused
        [Function(CallingConventions.Stdcall)]
        public delegate byte OnReduceScoutDays(uint Days);
        public static byte ReduceScoutDays(uint Days)
        {
            return 1;
        }

        //C7 81 ?? ?? ?? ?? 00 00 00 00 E8 ?? ?? ?? ?? 8B 4D (+BFEB3)
        [Function(CallingConventions.Stdcall)]
        public delegate void OnMemDataCoreSetupFinish();
        public static void AfterMemDataCoreSetup()
        {
            Dungeons.DungeonInit();
            return;
        }

        //A1 ?? ?? ?? ?? 68 B8 4B 06 00 Reset Dungeon Saves
        // /MODEL/DUNGEON/MAP/0200/TEXTURE/m02_groundA_c.tid
        

        //55 8B EC 8B 45 ?? FF 75 ?? 85 C0 
        [Function(CallingConventions.Cdecl)]
        public delegate bool OnGetMapTexturePath(Dungeon* dungeon,nint fileName,int* someValue, byte* stringDest,int size);
        public static bool GetMapTexturePath(Dungeon* dungeon,nint fileName, int* someValue, byte* stringDest, int size)
        {
            string path = $"/MODEL/DUNGEON/MAP/{dungeon->DungeonMap:D4}/TEXTURE/{Marshal.PtrToStringUTF8(fileName)}";
            if (dungeon->HERB[0] != 702 && dungeon->HERB[0] != 0)
            {
                string newPath = $"/MODS/{dungeon->HERB[0]}/MODEL/DUNGEON/{dungeon->DungeonMap:D4}/TEXTURE/{Marshal.PtrToStringUTF8(fileName)}";
                if (File.Exists((NeptuniaModder.LooseFiles + "/game2" + newPath).ToLower()))
                    path = newPath;
            }
            _sprintfHelper(path, stringDest, size);
            *someValue = 2;
            return true;
        }
        //55 8B EC 8B 4D ?? 0F B7 41 
        [Function(CallingConventions.Cdecl)]
        public delegate bool OnGetMapPath(Dungeon* dungeon,int* someValue, byte* stringDest,int size);
        public static bool GetMapPath(Dungeon* dungeon, int* someValue, byte* stringDest, int size)
        {
            string path = $"/MODEL/DUNGEON/MAP/{dungeon->DungeonMap:D4}/{dungeon->DungeonMapVariant:D3}";
            if (dungeon->HERB[0] != 702 && dungeon->HERB[0] != 0)
            {
                string newPath = $"/MODS/{dungeon->HERB[0]}/MODEL/DUNGEON/{dungeon->DungeonMap:D4}/{dungeon->DungeonMapVariant:D3}";
                if (Directory.Exists((NeptuniaModder.LooseFiles + "/game2" + newPath).ToLower()))
                    path = newPath;
            }
            _sprintfHelper(path, stringDest, size);
            *someValue = 2;
            return true;
        }

        //55 8B EC 8B 45 ?? FF 75 ?? 0F B7 40 ?? 50 68 ?? ?? ?? ?? FF 75 ?? FF 75 ?? FF 15 ?? ?? ?? ?? 8B 45 ?? 83 C4 14 C7 00 02 00 00 00 B0 01 5D C3 ?? 
        [Function(CallingConventions.Cdecl)]
        public delegate bool OnGetMapEffektPath(Dungeon* dungeon,nint fileName,int* someValue, byte* stringDest,int size);
        public static bool GetMapEffektPath(Dungeon* dungeon,nint fileName, int* someValue, byte* stringDest, int size)
        {

            string path = $"/MODEL/DUNGEON/MAP/{dungeon->DungeonMap:D4}/EFFECT/{Marshal.PtrToStringUTF8(fileName)}";
            if (dungeon->HERB[0] != 702 && dungeon->HERB[0] != 0)
            {
                string newPath = $"/MODS/{dungeon->HERB[0]}/MODEL/DUNGEON/{dungeon->DungeonMap:D4}/EFFECT/{Marshal.PtrToStringUTF8(fileName)}";
                if (File.Exists((NeptuniaModder.LooseFiles + "/game2" + newPath).ToLower()))
                    path = newPath;
            }
            _sprintfHelper(path, stringDest, size);
            *someValue = 2;
            return true;
        }





        public static void SetupHooks(IReloadedHooks hooks)
        {
            nuint offset = 0; 
            nuint[] offsets;
            if (FunctionScanner.FindFunction("Not Sure", "55 8B EC 81 EC D0 00 00 00 A1 ?? ?? ?? ?? 33 C5 89 45 ?? 53", out offset))
                _smth = hooks.CreateFunction<DunnoButImportant>((int)(NeptuniaModder.ModuleBase + offset));
            if (FunctionScanner.FindFunction("Reduce Scout Days", "55 8B EC 8B 0D ?? ?? ?? ?? 56 69 81 ?? ?? ?? ?? 3C 20 00 00", out offset))
            {
                string[] asm = {
                "use32",
                "ret",
                };
                _asmHooks.Add(hooks.CreateAsmHook(asm, (long)(NeptuniaModder.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
            }
            if (FunctionScanner.FindFunction("Add Dungeon", "55 8B EC 53 8B 5D ?? 85 DB 75 ?? 32 C0 5B 5D C3 A1", out offset))
            {
                string[] asm = {
                "use32",
                "push ebp",
                "mov ebp,esp",
                "sub esp, 0x4",
                "pushad",
                "pushfd",
                "push dword[ebp+0xC]",
                "push dword[ebp+0x8]",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(AddDungeon, out _onAddDungeon)}",
                "mov [ebp-0x4],eax",
                "popfd",
                "popad",
                "mov eax,[ebp-0x4]",
                "mov esp,ebp",
                "pop ebp",
                "ret",
                };
                _asmHooks.Add(hooks.CreateAsmHook(asm, (int)(NeptuniaModder.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
            }
            if (FunctionScanner.FindFunction("Count Dungeons On Map", "55 8B EC 8B 0D ?? ?? ?? ?? 33 D2 57 69 81 ?? ?? ?? ?? 3C 20 00 00", out offset))
            {
                string[] asm = {
                "use32",
                "push ebp",
                "mov ebp,esp",
                "sub esp, 0x4",
                "pushad",
                "pushfd",
                "push dword[ebp+0x8]",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(CountDungeonOnMap, out _onCountDungeonOnMap)}",
                "mov [ebp-0x4],eax",
                "popfd",
                "popad",
                "mov eax,[ebp-0x4]",
                "mov esp,ebp",
                "pop ebp",
                "ret",
                };
                _asmHooks.Add(hooks.CreateAsmHook(asm, (int)(NeptuniaModder.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
            }
            if (FunctionScanner.FindFunction("Get Dungeon Or Create New", "55 8B EC 51 8B 15 ?? ?? ?? ?? 53 66 8B 5D", out offset))
            {
                string[] asm = {
                "use32",
                "push ebp",
                "mov ebp,esp",
                "sub esp, 0x4",
                "pushad",
                "pushfd",
                "push dword[ebp+0x8]",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(GetDungeonOrCreateNew, out _onGetDungeonOrCreateNew)}",
                "mov [ebp-0x4],eax",
                "popfd",
                "popad",
                "mov eax,[ebp-0x4]",
                "mov esp,ebp",
                "pop ebp",
                "ret",
                };
                _asmHooks.Add(hooks.CreateAsmHook(asm, (int)(NeptuniaModder.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
            }
            if (FunctionScanner.FindFunction("Place Dungeon On Map", "55 8B EC 83 EC 08 53 8B 1D ?? ?? ?? ?? 56 69 83 ?? ?? ?? ?? 3C 20 00 00", out offset))
            {
                string[] asm = {
                "use32",
                "push ebp",
                "mov ebp,esp",
                "sub esp, 0x4",
                "pushad",
                "pushfd",
                "push dword[ebp+0x10]",
                "push dword[ebp+0xC]",
                "push dword[ebp+0x8]",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(PlaceDungeonOnMap, out _onPlaceDungeonOnMap)}",
                "mov [ebp-0x4],eax",
                "popfd",
                "popad",
                "mov eax,[ebp-0x4]",
                "mov esp,ebp",
                "pop ebp",
                "ret",
                };
                _asmHooks.Add(hooks.CreateAsmHook(asm, (int)(NeptuniaModder.ModuleBase + offset), AsmHookBehaviour.ExecuteFirst).Activate());
            }
            if (FunctionScanner.FindFunction("Reset Entities", "A1 ?? ?? ?? ?? 8D 88 ?? ?? ?? ?? 05 30 03 01 00 3B C1 73 ?? 56", out offset))
            {
                string[] asm = {
                "use32",
                "push ebp",
                "mov ebp,esp",
                "sub esp, 0x4",
                "pushad",
                "pushfd",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(ResetEntities, out _onResetEntities)}",
                "popfd",
                "popad",
                "mov esp,ebp",
                "pop ebp",
                "ret",
                };
                //_asmHooks.Add(hooks.CreateAsmHook(asm, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
            }
            if (FunctionScanner.FindFunction("Validate Dungeon", "3B F2 0F 83", out offset))
            {
                string[] asm = {
                "use32",
                "push ebp",
                "mov ebp,esp",
                "sub esp, 0x4",
                "pushad",
                "pushfd",
                "push dword[ebp+0xC]",
                "push dword[ebp+0x8]",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(ValidateDungeon, out _onValidateDungeon)}",
                "mov [ebp-0x4],eax",
                "popfd",
                "popad",
                "mov eax,[ebp-0x4]",
                "mov esp,ebp",
                "pop ebp",
                "ret",
                };
                _asmHooks.Add(hooks.CreateAsmHook(asm, (int)(NeptuniaModder.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
            }
            if (FunctionScanner.FindFunction("Dungeon Array Init", "C7 81 ?? ?? ?? ?? 00 00 00 00 E8 ?? ?? ?? ?? 8B 4D", out offset))
            {
                string[] asm = {
                "use32",
                "pushad",
                "pushfd",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(AfterMemDataCoreSetup, out _onDungeonDBInit)}",
                "popfd",
                "popad",
                };
                _asmHooks.Add(hooks.CreateAsmHook(asm, (int)(NeptuniaModder.ModuleBase + offset), AsmHookBehaviour.ExecuteAfter).Activate());
            }
            if (FunctionScanner.FindFunction("Get Map Path", "55 8B EC 8B 4D ?? 0F B7 41", out offset))
            {
                _iHooks.Add(hooks.CreateHook<OnGetMapPath>(GetMapPath, (long)(NeptuniaModder.ModuleBase + offset)).Activate());
            }
            if (FunctionScanner.FindFunction("Get Map Texture Path", "55 8B EC 8B 45 ?? FF 75 ?? 85 C0", out offset))
            {
                _iHooks.Add(hooks.CreateHook<OnGetMapTexturePath>(GetMapTexturePath, (long)(NeptuniaModder.ModuleBase + offset)).Activate());
            }
            if (FunctionScanner.FindFunctions("Get Map Effekts Path", "55 8B EC 8B 45 ?? FF 75 ?? 0F B7 40 ?? 50 68 ?? ?? ?? ?? FF 75 ?? FF 75 ?? FF 15 ?? ?? ?? ?? 8B 45 ?? 83 C4 14 C7 00 02 00 00 00 B0 01 5D C3 ??", out offsets))
            {
                foreach (var off in offsets)
                {
                    //overrides "/MODEL/DUNGEON/MAP/%04u/OBJECT/mobj%02u.ism2" pretty sure this is unused
                    _iHooks.Add(hooks.CreateHook<OnGetMapEffektPath>(GetMapEffektPath, (long)(NeptuniaModder.ModuleBase + off)).Activate());
                }
            }

        }

    }
}

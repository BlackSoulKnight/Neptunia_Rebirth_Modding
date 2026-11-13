using Neptunia_Modding.Modding.SaveFile;
using Neptunia_Modding.Structures.Database.Dungeon;
using Neptunia_Modding.Structures.SaveFile;
using Neptunia_Modding.Test;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Neptunia_Modding.Modding.Hooks
{
    public unsafe class DungeonHooks
    {
        public static List<IAsmHook> _asmHooks = new();

        public static IReverseWrapper<OnResetEntities> _onResetEntities;
        public static IReverseWrapper<OnValidateDungeon> _onValidateDungeon;
        public static IReverseWrapper<OnGetDungeonOrCreateNew> _onGetDungeonOrCreateNew;
        public static IReverseWrapper<OnAddDungeon> _onAddDungeon;
        public static IReverseWrapper<OnCountDungeonOnMap> _onCountDungeonOnMap;
        public static IReverseWrapper<OnPlaceDungeonOnMap> _onPlaceDungeonOnMap;
        public static IReverseWrapper<OnReduceScoutDays> _onReduceScoutDays;
        public static IReverseWrapper<OnDungeonDBInit> _onDungeonDBInit;


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

        [Function(CallingConventions.Stdcall)]
        public delegate byte OnReduceScoutDays(uint Days);
        //55 8B EC 8B 0D ?? ?? ?? ?? 56 69 81 ?? ?? ?? ?? 3C 20 00 00
        //Unused
        public static byte ReduceScoutDays(uint Days)
        {
            return 1;
        }

        [Function(CallingConventions.Stdcall)]
        public delegate void OnDungeonDBInit();
        //8B 85 ?? ?? ?? ?? 39 47 ?? 76
        public static void DungeonDBInit()
        {
            Dungeons.DungeonInit();
            return;
        }




        public static void SetupHooks(IReloadedHooks hooks)
        {
            nuint offset = 0;
            if (FunctionScanner.FindFunction("Not Sure", "55 8B EC 81 EC D0 00 00 00 A1 ?? ?? ?? ?? 33 C5 89 45 ?? 53", out offset))
                _smth = hooks.CreateFunction<DunnoButImportant>((int)(Mod.ModuleBase + offset));
            if (FunctionScanner.FindFunction("Reduce Scout Days", "55 8B EC 8B 0D ?? ?? ?? ?? 56 69 81 ?? ?? ?? ?? 3C 20 00 00", out offset))
            {
                string[] asm = {
                "use32",
                "ret",
                };
                _asmHooks.Add(hooks.CreateAsmHook(asm, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
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
                _asmHooks.Add(hooks.CreateAsmHook(asm, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
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
                _asmHooks.Add(hooks.CreateAsmHook(asm, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
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
                _asmHooks.Add(hooks.CreateAsmHook(asm, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
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
                _asmHooks.Add(hooks.CreateAsmHook(asm, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.ExecuteFirst).Activate());
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
                _asmHooks.Add(hooks.CreateAsmHook(asm, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
            }
            if (FunctionScanner.FindFunction("Dungeon Array Init", "01 47 ?? 8B 85", out offset))
            {
                string[] asm = {
                "use32",
                "pushad",
                "pushfd",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(DungeonDBInit, out _onDungeonDBInit)}",
                "popfd",
                "popad",
                };
                _asmHooks.Add(hooks.CreateAsmHook(asm, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.ExecuteAfter).Activate());
            }
        }

    }
}

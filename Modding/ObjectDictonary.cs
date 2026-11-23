using Neptunia_Modding.Structures.SaveFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Neptunia_Modding.Modding
{
    internal unsafe class ObjectDictonary<KeyIndexer, ObjectType> where KeyIndexer : struct where ObjectType : struct
    {
        readonly Dictionary<KeyIndexer, IntPtr> list = new();

        static readonly ObjectType DefaultValue = new ObjectType();

        static readonly int ObjectSize = sizeof(ObjectType);
        public ObjectType*[] Values => _values();

        public ObjectType* GetValue(KeyIndexer key)
        {
            return list.ContainsKey(key) ? (ObjectType*)list[key] : null;
        }
        public ObjectType* this[KeyIndexer key]
        {
            get => GetValue(key);
            set => AddValue(key, value);
        }
        public void AddValue(KeyIndexer key, ObjectType* value)
        {
            if (list.ContainsKey(key) && list[key] != (IntPtr)value)
            {
                Marshal.FreeHGlobal(list[key]);
            }
            list[key] = (IntPtr)value;
        }
        public bool ContainsKey(KeyIndexer key) => list.ContainsKey(key);
        public bool ContainsValue(ObjectType* value) => list.ContainsValue((IntPtr)value);
        public void Clear()
        {
            foreach (var values in list.Values)
                Marshal.FreeHGlobal(values);
            list.Clear();
        }
        public ObjectType* CreateDefault(KeyIndexer key)
        {
            var result = (ObjectType*)Marshal.AllocHGlobal(ObjectSize);
            fixed (ObjectType* de = &DefaultValue)
                Buffer.MemoryCopy(de, result, ObjectSize, ObjectSize);
            list[key] = (IntPtr)result;
            return result;
        }

        ObjectType*[] _values()
        {
            var result = new ObjectType*[list.Count];
            var array = list.Values.ToArray();
            for (int i = 0; i < array.Length; i++)
                result[i] = (ObjectType*)array[i];
            return result;
        }
    }

    internal unsafe class ObjectList<ObjectType> where ObjectType : struct
    {
        readonly List<IntPtr> list = new();

        static readonly ObjectType DefaultValue = new ObjectType();

        static readonly int ObjectSize = sizeof(ObjectType);
        public ObjectType*[] Values => _values();

        public ObjectType* GetValue(int index)
        {
            return index < list.Count ? (ObjectType*)list[index] : null;
        }
        public ObjectType* this[int index]
        {
            get => GetValue(index);
            set => AddValue(value);
        }
        public void AddValue(ObjectType* value)
        {
            list.Add((IntPtr)value);
        }
        public void Clear()
        {
            foreach (var values in list)
                Marshal.FreeHGlobal(values);
            list.Clear();
        }

        ObjectType*[] _values()
        {
            var result = new ObjectType*[list.Count];
            for (int i = 0; i < list.Count; i++)
                result[i] = (ObjectType*)list[i];
            return result;
        }
    }
}

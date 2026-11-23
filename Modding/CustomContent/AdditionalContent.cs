using Neptunia_Modding.Modding.SaveFile;
using Neptunia_Modding.Structures.Database.Dungeon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Neptunia_Modding.Modding.CustomContent
{
    public enum AdditionalContentTypes
    {
        Dungeon = 1
    }

    public partial class AdditionalContent
    {
        static string Manifest = "info.json";
        static List<AdditionalContent> Content = new();
        public delegate void LoadFiles();

        public static bool CreateAdditionalContent(string path,out AdditionalContent content)
        {
            content = null;
            if (path[(path.Length-Manifest.Length)..] != Manifest)
                return false;
            string type = "";
            using (StreamReader file = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                var o2 = (JObject)JToken.ReadFrom(reader);
                if (o2.ContainsKey("Type"))
                    type = o2["Type"]!.ToString();
            }
            if (!Enum.IsDefined(typeof(AdditionalContentTypes), type))
                return false;

            var defType = Enum.Parse<AdditionalContentTypes>(type);
            switch (defType)
            {
                case AdditionalContentTypes.Dungeon:
                    content = new CustomDungeon(path[..(path.Length - Manifest.Length)]);
                    Dungeons.OnLoadDungeons += content.LoadMod;
                    break;
            }

            return true;
        }

        public static void FindMods()
        {
            string[] files = Directory.GetFiles(Mod.ModFolder + "/CustomContent",Manifest,SearchOption.AllDirectories);
            foreach (string file in files)
            {
                Console.WriteLine($"Found new content at: {file}");
                if (CreateAdditionalContent(file, out AdditionalContent content))
                    Content.Add(content);
            }
        }

        protected AdditionalContent() { }

        protected virtual void LoadMod() { }
    }
}

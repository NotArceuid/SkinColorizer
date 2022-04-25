using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace SkinColorizer
{
    public class Colorizer
    {
        private readonly InputService inputService;

        public Colorizer(InputService inputService)
        {
            this.inputService = inputService;

            var skin = inputService.HandleUserInput();

            var skinElements = GetSkinElements(skin.Path);
            Colorize(skin.HueDegrees, FilterSkinElements(skinElements), skin.OutputDirectory);
        }

        private void Colorize(float degrees, List<string> skinElements, string outputDirectory)
        {
            foreach (string skinElementPath in skinElements)
            {
                using (var image = Image.Load(skinElementPath))
                {
                    image.Mutate(x => x.Hue(degrees));
                    image.SaveAsPng(outputDirectory + "/" + Path.GetFileName(skinElementPath));
                }
            }
        }

        private List<string> GetSkinElements(string path)
        {
            var skinElements = Directory.EnumerateFiles(path);
            return skinElements.ToList();
        }

        private List<string> FilterSkinElements(List<string> skinElements)
        {
            var json = JsonConvert.DeserializeObject<SkinElements>(File.ReadAllText("./SkinElements.json"));
            skinElements.RemoveAll(x =>
            {
                return !x.EndsWith(".png");
            });

            var filteredSkinElements = new List<string>();
            foreach (var jsonElements in json.Elements)
            {
                var filteredElements = skinElements.Where(x => x.Contains(jsonElements));
                filteredSkinElements.AddRange(filteredElements);
            }

            return filteredSkinElements;
        }

    }

    public class SkinElements
    {
        [JsonProperty("SkinElements")] public IEnumerable<string> Elements { get; set; }
    }
}
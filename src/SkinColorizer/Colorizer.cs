using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SkinColorizer.Models;

namespace SkinColorizer
{
    public class Colorizer
    {
        private readonly InputService inputService;

        public Colorizer(InputService inputService)
        {
            this.inputService = inputService;
        }

        public void Run()
        {
            var skin = inputService.HandleUserInput();

            var skinElementPaths = GetSkinElements(skin.Path);
            var elements = FilterSkinElements(skinElementPaths);
            Colorize(skin.HueDegrees, elements, skin.OutputDirectory);
        }

        private List<string> GetSkinElements(string skinPath)
        {
            var skinElements = Directory.EnumerateFiles(skinPath);
            return skinElements.ToList();
        }

        private void Colorize(float degrees, List<string> skinElementPaths, string outputDirectory)
        {
            System.Console.WriteLine(outputDirectory);
            foreach (string skinElementPath in skinElementPaths)
            {
                System.Console.WriteLine(skinElementPath);
                using (var image = Image.Load(skinElementPath))
                {
                    image.Mutate(x => x.Hue(degrees));
                    image.SaveAsPng(outputDirectory);
                }
            }
        }

        private List<string> FilterSkinElements(List<string> currentSkinElementNames)
        {
            var json = JsonConvert.DeserializeObject<SkinElements>(File.ReadAllText("./SkinElements.json"));       
            currentSkinElementNames.RemoveAll(x => { 
                return !x.EndsWith(".png"); 
            });

            List<string> filteredSkinElements = new();
            foreach (var _skinElements in json.Elements)
            {
                var filteredElements = currentSkinElementNames.Where(x => x.Contains(_skinElements));
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
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using Newtonsoft.Json;

namespace SkinColorizer
{
    public class Colorizer
    {
        private readonly InputService inputService;

        public Colorizer(InputService inputService)
        {
            this.inputService = inputService;

            var skin = inputService.UserInputHandler();
            if (skin is not null)
            {
                var skinElements = GetSkinElements(skin.Path);
                Colorize(skin.HueDegrees, FilterSkinElements(skinElements), skin.OutputDirectory);

                SaveSkinElements(Directory.GetFiles(skin.Path).ToList(), skin.OutputDirectory);
            }

            return;
        }

        private void Colorize(int degrees, List<string> skinElements, string outputPath)
        {
            foreach (string element in skinElements)
            {
                byte[] photoBytes = File.ReadAllBytes(element);
                using (var inStream = new MemoryStream(photoBytes))
                {
                    using (var fs = new FileStream(Path.Combine(outputPath, Path.GetFileName(element)), FileMode.Create, FileAccess.ReadWrite))
                    {
                        using var imageFactory = new ImageFactory(preserveExifData: true);

                        imageFactory.Load(inStream)
                                    .Format(new PngFormat())
                                    .Hue(degrees, false)
                                    .Save(fs);
                    }
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

        private void SaveSkinElements(List<string> skinElements, string outputDir)
        {
            var json = JsonConvert.DeserializeObject<SkinElements>(File.ReadAllText("./SkinElements.json"));
            skinElements.RemoveAll(x => json.Elements.Any(e => x.Contains(e)));
            foreach (var elements in skinElements)
            {
                File.Copy(elements, Path.Combine(outputDir, Path.GetFileName(elements)), true);
            }
        }

        public class SkinElements
        {
            [JsonProperty("SkinElements")] public IEnumerable<string> Elements { get; set; }
        }
    }
}

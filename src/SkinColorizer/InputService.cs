using System.Text.RegularExpressions;
using SkinColorizer.Models;
using SkinColorizer.Models.Enums;

namespace SkinColorizer
{
    public class InputService
    {

        private readonly Func<string> inputProvider;
        private readonly Action<string> outputProvider;

        public InputService(Action<string> outputProvider, Func<string> inputProvider)
        {
            this.outputProvider = outputProvider;
            this.inputProvider = inputProvider;
        }

        public ColorizerOptions UserInputHandler()
        {
            var options = new ColorizerOptions();

            string path = GetPath();
            string outputPath = GetOutputPath(path);

            float hue = GetHueDegrees();

            options.Path = path;
            options.OutputDirectory = outputPath;
            options.HueDegrees = hue;

            return options;
        }

        private string GetPath()
        {
            outputProvider("Please enter your osu skin directory");
            var directory = inputProvider();

            bool doesSkinExist = File.Exists(directory + @"\skin.ini");
            if (VerifyDirectory(directory) && File.Exists(directory + @"\skin.ini"))
            {
                return directory;
            }

            outputProvider("Invalid Path");
            return string.Empty;
        }

        private string GetOutputPath(string skinDirPath)
        {
            outputProvider("Please enter your output directory name");
            var name = inputProvider();

            if (string.IsNullOrWhiteSpace(name))
            {
                outputProvider("Invalid Name");
                return string.Empty;
            }

            string skinDir = Path.GetDirectoryName(skinDirPath);
            string dir = skinDir + @"\" + name;
            Directory.CreateDirectory(dir);
            return dir;
        }

        private bool VerifyDirectory(string directory) => Regex.IsMatch(directory, @"((?:[^/]*/)*)(.*)") &&
                                                                Directory.Exists(directory);

        private float GetHueDegrees()
        {
            outputProvider("Please enter the color you want: [1] Red, [2] Green, [3] Blue, [4] Purple, [5] Custom Color");
            var isSuccessful = int.TryParse(inputProvider(), out int value);

            if (isSuccessful)
            {
                float result = value switch
                {
                    1 => (float)HueDegrees.Red,
                    2 => (float)HueDegrees.Green,
                    3 => (float)HueDegrees.Blue,
                    4 => (float)HueDegrees.Purple,
                    5 => CustomColorHandler(),
                    _ => InvalidChoiceHandler()
                };

                return result;
            }

            return InvalidChoiceHandler();

        }

        private float CustomColorHandler()
        {
            outputProvider("Please enter the custom color you want in hue degrees");
            var isSuccessful = float.TryParse(inputProvider(), out float value);

            if (isSuccessful)
            {
                if (value < 0 || value > 360)
                {
                    return InvalidChoiceHandler();
                }
                return value;
            }

            return InvalidChoiceHandler();
        }

        private float InvalidChoiceHandler()
        {
            outputProvider($"Your Choice is invalid");
            return 0;
        }

    }
}
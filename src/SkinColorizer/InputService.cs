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


        public ColorizerOptions HandleUserInput()
        {
            var options = new ColorizerOptions();

            var path = GetPath();
            if (string.IsNullOrWhiteSpace(path))
            {
                System.Console.WriteLine("rqgewrg");
                return null;
            }

            var outputDirectory = GetOutputDirectory();
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                System.Console.WriteLine("wrwqwq");
                return null;
            }

            var hue = GetHueDegrees();

            options.Path = path;
            options.OutputDirectory = outputDirectory;
            System.Console.WriteLine(options.Path);
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

        private string GetOutputDirectory()
        {
            outputProvider("Please enter your destination path");
            var directory = inputProvider();

            if (!VerifyDirectory(directory))
            {
                outputProvider("Directory does not exist, Would you like to create a new directory? y/n");
                var choice = inputProvider();

                if (choice.ToLower() == "y")
                {
                    Directory.CreateDirectory(directory);
                    outputProvider("Directory created successfully");

                    return directory;
                }
                else 
                {
                    return string.Empty;
                }
            }
            return directory;
        }

        private bool VerifyDirectory(string directory) => Regex.IsMatch(directory, @"((?:[^/]*/)*)(.*)") && 
                                                                Directory.Exists(directory);

        private float GetHueDegrees()
        {
            outputProvider("Please enter the color you want: [1] Red, [2] Green, [3] Yellow, [4] Blue, [5] Purple, [6] Custom Color");
            var isSuccessful = int.TryParse(inputProvider(), out int value);

            if (isSuccessful)
            {
                float result = value switch
                {
                    1 => (float)Colors.Red,
                    2 => (float)Colors.Green,
                    3 => (float)Colors.Yellow,
                    4 => (float)Colors.Blue,
                    5 => (float)Colors.Purple,
                    6 => CustomColorHandler(),
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
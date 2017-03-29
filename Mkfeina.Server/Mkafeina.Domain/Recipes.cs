using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Mkfeina.Domain
{
    public class Recipes
    {
        public const string RECIPES = "recipes",
                            INGREDIENTS = "ingredients",
                            APP_SETTINGS = "appSettings";
        private static Dictionary<string, Dictionary<char, int>> __recipes;

        public static Recipes _singleton = new Recipes();

        public static Recipes Singleton { get => _singleton; }

        private Recipes()
        {
        }

        public IEnumerable<string> LoadRecipesAsync()
        {
#warning make it safe for things not found
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            var section = (AppSettingsSection)configuration.GetSection(APP_SETTINGS);

            var recipesNames = section.Settings[RECIPES].Value
                                                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                                        .Select(r => r.Trim())
                                                        .ToArray();

            Task.Factory.StartNew(() => {
                var ingredients = section.Settings[INGREDIENTS].Value
                                                               .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                                               .Select(r => r.Trim().ToCharArray()[0])
                                                               .ToArray();

                // gambiarra
                if (__recipes == null)
                    __recipes = new Dictionary<string, Dictionary<char, int>>();

                lock (__recipes) {
                    __recipes = null;

                    __recipes = new Dictionary<string, Dictionary<char, int>>();

                    foreach (var name in recipesNames) {
                        var proportions = new Dictionary<char, int>();
                        foreach (var ingredient in ingredients) {
                            var quantity = int.Parse(section.Settings[$"{name}.{ingredient}"].Value);
                            proportions.Add(ingredient, quantity);
                        }
                        __recipes.Add(name, proportions);
                    }
                }
            });

            return recipesNames;
        }

        public Dictionary<char, int> this[string recipe] {
            get => __recipes[recipe];
        }
    }
}
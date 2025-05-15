using System.Xml;
using System.Runtime.CompilerServices;

using RoR2;

using R2API;
using BepInEx;
using R2API.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;

namespace GetSurivorStats
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    public class GetSurvivorStats : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "IncogniDeveloper";
        public const string PluginName = "Survivor Stats Complete";
        public const string PluginVersion = "1.1.0";


        public void Awake()
        { 
            InitConfigs();
        }

        [RoR2.SystemInitializer(new Type[] { typeof(IL.RoR2.SurvivorCatalog) })]
        private static void InitConfigs()
        {
            IEnumerable<SurvivorDef> survivors = SurvivorCatalog.allSurvivorDefs;
            foreach (CharacterBody body in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                foreach (SurvivorDef survivor in survivors)
                {
                    if (body.name == survivor.cachedName)
                    {
                        string text = ((UnityEngine.Object)body).name.Replace("Body", "");
                        string section = text + " Stats";
                        string text2 = text + ": ";
                        ConfigFile config = new ConfigFile(Paths.ConfigPath + "\\ConfigurableStats\\" + text + ".cfg", true);

                        body.baseMaxHealth = (float)NewConfigEntry<float>(config, body.baseMaxHealth, section, text2 + "Base Max Health");
                    }
                }

            }
        }

        public static object NewConfigEntry<T>(ConfigFile config, object arg, string section, string key)
        {
            ConfigEntry<T> val = config.Bind<T>(new ConfigDefinition(section, key), (T)arg, new ConfigDescription("", (AcceptableValueBase)null, Array.Empty<object>()));
            return val.Value;
        }

    }
}

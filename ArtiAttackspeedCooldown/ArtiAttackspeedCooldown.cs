using System.Xml;
using System.Runtime.CompilerServices;

using RoR2;

using R2API;
using BepInEx;
using R2API.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BepInEx.Configuration;
using ArtiAttackspeedCooldown;

namespace CustomSurvivorStats

{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI), nameof(RecalculateStatsAPI))]
    public class ArtiAttackspeedCooldown : BaseUnityPlugin
    {
        public const string PluginGUID = PluginName;
        public const string PluginName = "ArtiAttackspeedCooldown";
        public const string PluginVersion = "1.1.0";

        private static ConfigEntry<int> reductionPercent { get; set; }

        private static float fReductionPct { get; set; }

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            //Init our logging class so that we can properly log for debugging
            Log.Init(Logger);

            //set config
            reductionPercent = Config.Bind(
                "Artificer Primary Cooldown",
                "Attack speed % used",
                50,
                "Cooldown Attack speed is multiplied by a coefficient of 2, "
            );

            //Check that value is positive and between 0 and 100;
            if (reductionPercent.Value < 0)
            {
                reductionPercent.Value = 0;
            }
            else if (reductionPercent.Value > 100)
            {
                reductionPercent.Value = 100;
            }

            Log.LogInfo("Reduction Config amount: " + reductionPercent.Value);

            fReductionPct = reductionPercent.Value > 0 ? (float)reductionPercent.Value / 100 : 0f;

            Log.LogInfo("Reduction amount: " + fReductionPct.ToString("0.00"));

            On.RoR2.CharacterBody.RecalculateStats += delegate (On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody body)
            {
                orig.Invoke(body);
                ModifyArtiCharges(body);
                ModifyArtiCooldown(body);
            };


            // This line of log will appear in the bepinex console when the Awake method is done.
            Log.LogInfo(nameof(Awake) + " done.");
        }

        private static void ModifyArtiCooldown(CharacterBody body)
        {
            var artiPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageBody.prefab").WaitForCompletion();
            var artiBody = artiPrefab.GetComponent<CharacterBody>();

            if (artiBody.bodyIndex == body.bodyIndex)
            {
                if (body.skillLocator.primary)
                {
                    body.skillLocator.primary.cooldownScale /= body.attackSpeed * 2 * fReductionPct;
                }
            }
        }

        private static void ModifyArtiCharges(CharacterBody body)
        {
            var artiPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageBody.prefab").WaitForCompletion();
            var artiBody = artiPrefab.GetComponent<CharacterBody>();

            if (artiBody.bodyIndex == body.bodyIndex)
            {
                if (body.skillLocator.primary)
                {
                    body.skillLocator.primary.baseSkill.baseMaxStock = 5;
                }

            }
        }
    }
}

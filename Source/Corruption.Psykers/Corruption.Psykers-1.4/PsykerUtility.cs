using Corruption.Core.Gods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Psykers
{
    [StaticConstructorOnStartup]
    public static class PsykerUtility
    {
        public static Color AbilityUnlockedBorder = TexUI.HighlightLineResearchColor;
        public static Color AbilityUnlockedBG = new ColorInt(26, 38, 46).ToColor;
        public static Color AbilityLockedBorder = TexUI.DefaultBorderResearchColor;
        public static Color AbilityLockedBG = new ColorInt(42, 42, 42).ToColor;
        public static Color AbilitySelectedBG = new ColorInt(72, 72, 72).ToColor;
        public static Color AbilityPerequisiteBorder = new Color(1f, 0f, 0f);

        public static Dictionary<AbilityCastType, float> CastTypeWeights = new Dictionary<AbilityCastType, float>()
        {
            { AbilityCastType.SelfHeal, 90f },
            { AbilityCastType.Attack, 60f },
            { AbilityCastType.Heal, 40f },
            { AbilityCastType.Defend, 35f },
            { AbilityCastType.Flee, 30f },
            { AbilityCastType.Buff, 20f }
        };

        public static readonly Texture2D PsykerIcon = ContentFinder<Texture2D>.Get("UI/Abilities/PsykerLearning", true);
        public static readonly Texture2D PowerLevelKappa = ContentFinder<Texture2D>.Get("UI/PsykerLevels/PsykerPowerLevelKappa", true);
        public static readonly Texture2D PowerLevelIota = ContentFinder<Texture2D>.Get("UI/PsykerLevels/PsykerPowerLevelIota", true);
        public static readonly Texture2D PowerLevelZeta = ContentFinder<Texture2D>.Get("UI/PsykerLevels/PsykerPowerLevelZeta", true);
        public static readonly Texture2D PowerLevelEpsilon = ContentFinder<Texture2D>.Get("UI/PsykerLevels/PsykerPowerLevelEpsilon", true);
        public static readonly Texture2D PowerLevelDelta = ContentFinder<Texture2D>.Get("UI/PsykerLevels/PsykerPowerLevelDelta", true);
        public static readonly Texture2D PowerLevelBeta = ContentFinder<Texture2D>.Get("UI/PsykerLevels/PsykerPowerLevelBeta", true);

        public static readonly Texture2D PowerCooldownBarTex = SolidColorMaterials.NewSolidColorTexture(new Color32(9, 203, 4, 64));

        public static List<HediffDef> DemonicAttentionHediffs => DefDatabase<HediffDef>.AllDefsListForReading.FindAll(x => x.defName.StartsWith("DemonicAttention"));
        public static List<HediffDef> DemonicPossessionHediffs => DefDatabase<HediffDef>.AllDefsListForReading.FindAll(x => x.defName.StartsWith("DemonicPossession"));

        public static readonly Dictionary<int, int> PsykerDegreeXPCost = new Dictionary<int, int>{
            {1, 50 },
            {2, 150 },
            {3, 250 },
            {4, 400 },
            };

        public static Dictionary<int, int> PsykerAbilitieSlots = new Dictionary<int, int>()
        {
            {1, 2 },
            {2, 4 },
            {3, 3 },
            {4, 2 },
            {5, 1 }
        };

        public static List<AbilityDef> GetPowerDefsFor(int level = 0, GodDef god = null)
        {
            if (god == null)
            {
                return DefDatabase<AbilityDef>.AllDefsListForReading.Where(x => x.level == level).ToList();
            }
            return god.psykerPowers.FindAll(x => x.level == level);
        }

        public static Dictionary<int, string> PowerLevelLetters = new Dictionary<int, string>()
        {
            {10, "Kappa" },
            {20, "Iota" },
            {30, "Zeta" },
            {40, "Epsilon" },
            {50, "Delta" },
            {60, "Beta" }
        };

        public static SimpleCurve PowerLevelCostCurve = new SimpleCurve(){
            new CurvePoint(10f,100f),
            new CurvePoint(20f,1000f),
            new CurvePoint(30f,2000f),
            new CurvePoint(40f,5000f),
            new CurvePoint(50f,9000f),
            new CurvePoint(60f,50000f)
        };

        public static bool TryGetAbilityOpportunity(Pawn pawn, out AbilityOpportunity opportunity)
        {
            if (!pawn.CompPsyker()?.ShouldAutoCast ?? false)
            {
                opportunity = default(AbilityOpportunity);
                return false;
            }

            List<AbilityOpportunity> opportunities = GetAbilityOpportunities(pawn);

            if (opportunities.Count == 0)
            {
                opportunity = default(AbilityOpportunity);
                return false;
            }
            else
            {
                opportunity = opportunities.RandomElementByWeight(x => PsykerUtility.CastTypeWeights[x.castType]);
                return true;
            }            
        }

        public static List<AbilityOpportunity> GetAbilityOpportunities(Pawn pawn)
        {
            List<AbilityOpportunity> opportunities = new List<AbilityOpportunity>();
            foreach (var ability in pawn.abilities.abilities)
            {
                AbilityComp_AICast aiCast = ability.CompOfType<AbilityComp_AICast>();
                if (aiCast != null)
                {
                    LocalTargetInfo target = LocalTargetInfo.Invalid;
                    if (aiCast.TryGetTarget(out target))
                    {
                        opportunities.Add(new AbilityOpportunity(ability, aiCast.Props.abilityCastType, target));
                    }
                }
            }
            return opportunities;
        }
    }

    [DefOf]
    public static class PsykerTraitDefOf
    {
        public static TraitDef Psyker;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Corruption.Core.Soul;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Corruption.Psykers
{
    [StaticConstructorOnStartup]
    public class HarmonyPatches
    {

        static HarmonyPatches()
        {
            Log.Message("Generating Corruption Psykers Patches");
            Harmony harmony = new Harmony("rimworld.ohu.corruption.psykers");
            harmony.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), "Notify_EquipmentAdded", null), null, new HarmonyMethod(typeof(HarmonyPatches), "Notify_EquipmentAddedPostfix", null));
            harmony.Patch(AccessTools.Method(typeof(Pawn_ApparelTracker), "Wear", null), null, new HarmonyMethod(typeof(HarmonyPatches), "WearPostfix", null));
            harmony.Patch(AccessTools.Method(typeof(Pawn), "PreApplyDamage", null), new HarmonyMethod(typeof(HarmonyPatches), "PreApplyDamagePrefix", null));

            harmony.Patch(AccessTools.Method(typeof(Pawn_PsychicEntropyTracker), "NeedToShowGizmo", null), new HarmonyMethod(typeof(HarmonyPatches), "NeedToShowGizmoPrefix"), null, null);
            harmony.Patch(AccessTools.Method(typeof(Toils_Combat), "FollowAndMeleeAttack", new Type[] { typeof(TargetIndex), typeof(TargetIndex), typeof(Action) }, null), null, new HarmonyMethod(typeof(HarmonyPatches), "FollowAndMeleeAttackPostfix"), null, null);

        }

        private static void Notify_EquipmentAddedPostfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            Pawn pawn = __instance.pawn;
            CompImbuedAbility compImbued = eq.GetComp<CompImbuedAbility>();
            if (compImbued != null)
            {
                foreach (var ability in compImbued.ImbuedAbilities)
                {
                    pawn.abilities.GainAbility(ability);
                }
            }
        }

        private static void WearPostfix(Apparel newApparel, bool dropReplacedApparel = true, bool locked = false)
        {
            Pawn pawn = newApparel.Wearer;
            if (pawn != null)
            {
                CompImbuedAbility compImbued = newApparel.GetComp<CompImbuedAbility>();
                if (compImbued != null)
                {
                    foreach (var ability in compImbued.ImbuedAbilities)
                    {
                        pawn.abilities.GainAbility(ability);
                    }
                }
            }
        }

        private static bool PreApplyDamagePrefix(ref bool absorbed, ref DamageInfo dinfo, Pawn __instance)
        {
            var shieldHediffs = __instance.health.hediffSet.GetAllComps().Where(x => x is HediffComp_Shield);
            foreach (var hediff in shieldHediffs)
            {
                ((HediffComp_Shield)hediff).TryAbsorbDamage(ref dinfo, out absorbed);
                if (absorbed)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool NeedToShowGizmoPrefix(Pawn_PsychicEntropyTracker __instance, ref bool __result)
        {
            CompPsyker psyker = __instance.Pawn.CompPsyker();
            if (__result && psyker != null && psyker.PsykerPowerTrait != null)
            {
                __result = false;
                return false;
            }
            return true;
        }

        private static void FollowAndMeleeAttackPostfix(TargetIndex targetInd, Action hitAction, Toil __result)
        {
            __result.AddPreTickAction(delegate
            {
                Pawn pawn = __result.actor;
                if ((pawn.story?.traits?.HasTrait(PsykerTraitDefOf.Psyker) ?? false))
                {
                    AbilityOpportunity opportunity;
                    if (PsykerUtility.TryGetAbilityOpportunity(pawn, out opportunity))
                    {
                        LocalTargetInfo target = pawn.CurJob.GetTarget(targetInd);
                        opportunity.ability.verb.TryStartCastOn(target);
                    }
                }
            });
        }

    }
}

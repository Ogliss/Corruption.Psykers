using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Corruption.Psykers
{
    public static class PsykerAIExtension
    {
        private static Dictionary<AbilityCastType, float> weightedAbility = new Dictionary<AbilityCastType, float>()
        {
            { AbilityCastType.SelfHeal, 90f },
            { AbilityCastType.Attack, 60f },
            { AbilityCastType.Heal, 40f },
            { AbilityCastType.Buff, 20f },
            { AbilityCastType.Flee, 5f }
        };

        public static bool TryCastAbility(this Pawn pawn)
        {
            if (pawn.abilities.abilities.Count > 0)
            {
                var ability = TryGetAbility(pawn);
                if (ability != null)
                {
                    TargetScanFlags targetScanFlags = TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedLOSToNonPawns | TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable;
                    if (ability.verb.IsIncendiary())
                    {
                        targetScanFlags |= TargetScanFlags.NeedNonBurning;
                    }
                    Thing thing = GetTarget(pawn, targetScanFlags);
                    if (thing != null)
                    {
                        ability.verb.TryStartCastOn(thing);
                        return true;
                    }
                }
            }
            return false;
        }

        private static Ability TryGetAbility(Pawn pawn)
        {
            var abilities = pawn.abilities.abilities.Where(x => AICastable(x));

            if (abilities == null) { return null; }
            return abilities.RandomElementByWeight(x => x.def.level + 1);
        }

        private static Thing GetTarget(Pawn pawn, TargetScanFlags targetScanFlags)
        {
            return (Thing)AttackTargetFinder.BestShootTargetFromCurrentPosition(pawn, targetScanFlags);
        }

        private static bool AICastable(Ability a)
        {
            var aiComp = a.CompOfType<AbilityComp_AICast>();
            var castType = weightedAbility.RandomElementByWeight(x => x.Value).Key;
            return aiComp != null; //&& a.CanCast && aiComp.Props.abilityCastType == CastType;
        }

        private static bool IsOpportune(Ability a, AbilityCastType castType)
        {
            return true;
        }
    }
}

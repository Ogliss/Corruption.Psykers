using Corruption.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions.Must;
using Verse;
using Verse.AI;

namespace Corruption.Psykers
{
    public class AbilityComp_AICast : AbilityComp
    {
        public AbilityCompProperties_AICast Props => this.props as AbilityCompProperties_AICast;

        public virtual bool TryGetTarget(out LocalTargetInfo target)
        {
            target = GetTarget();
            bool recklessCast = this.Recklessness();
            return target != null && !parent.pawn.Downed && !parent.pawn.Dead && recklessCast && IsOpportune(parent, Props.abilityCastType, target);
        }

        private bool Recklessness()
        {
            if (parent is PsykerCast psykerCast)
            {
                List< HediffDemonicAttention > list = new List< HediffDemonicAttention >();
                parent.pawn.health.hediffSet.GetHediffs<HediffDemonicAttention>(ref list);
                if (!list.NullOrEmpty() && list.Any(x => x.def.maxSeverity <= x.Severity + this.parent.def.EntropyGain))
                {
                    return false;
                }
            }
            if (parent.pawn.psychicEntropy.MaxEntropy <= parent.def.EntropyGain + parent.pawn.psychicEntropy.EntropyValue)
            {
                return false;
            }
            return true;
        }

        protected virtual LocalTargetInfo GetTarget()
        {
            TargetScanFlags targetScanFlags = TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedLOSToNonPawns | TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable;
            if (parent.verb.IsIncendiary_Ranged() || parent.verb.IsIncendiary_Melee())
            {
                targetScanFlags |= TargetScanFlags.NeedNonBurning;
            }
            return (Thing)AttackTargetFinder.BestShootTargetFromCurrentPosition(parent.pawn, targetScanFlags);
        }

        protected virtual bool IsOpportune(Ability a, AbilityCastType castType, LocalTargetInfo target)
        {
            return !a.pawn.WorkTagIsDisabled(WorkTags.Violent) && a.pawn.IsFighting() || a.pawn.CurJob?.def == JobDefOf.UseVerbOnThing;
        }
    }

    public class AbilityComp_AICastHeal : AbilityComp_AICast
    {
        protected override LocalTargetInfo GetTarget()
        {
            Pawn pawn = parent.pawn;
            Predicate<Thing> validator = delegate (Thing t)
            {
                Pawn pawn3 = (Pawn)t;
                return (pawn3.health.summaryHealth.SummaryHealthPercent < 1f && parent.CanApplyOn((LocalTargetInfo)pawn3) && parent.verb.targetParams.CanTarget(pawn3)) ? true : false;
            };
            Pawn pawn2 = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.None, TraverseParms.For(pawn), parent.verb.verbProps.range, validator);
            return pawn2;
        }

        protected override bool IsOpportune(Ability a, AbilityCastType castType, LocalTargetInfo target)
        {
            return true;
        }
    }

    public class AbilityComp_AICastTeleport : AbilityComp_AICast
    {
        protected override LocalTargetInfo GetTarget()
        {
            Thing target = parent.pawn.mindState.enemyTarget;
            if (target != null)
            {
                var mainVerb = parent.pawn.TryGetAttackVerb(target);
                if (mainVerb != null)
                {
                    if (mainVerb.IsMeleeAttack)
                    {
                        return target;
                    }

                    CastPositionRequest newReq = default(CastPositionRequest);
                    newReq.caster = parent.pawn;
                    newReq.target = target;
                    newReq.verb = mainVerb;
                    newReq.maxRangeFromTarget = mainVerb.verbProps.range;
                    newReq.wantCoverFromTarget = (mainVerb.verbProps.range > 5f);
                    IntVec3 dest;
                    if (CastPositionFinder.TryFindCastPosition(newReq, out dest))
                    {
                        return dest;
                    }
                }
            }
            return null;
        }

        protected override bool IsOpportune(Ability a, AbilityCastType castType, LocalTargetInfo target)
        {
            return true;
        }
    }

    public class AbilityComp_AICastBuff : AbilityComp_AICast
    {
        protected override LocalTargetInfo GetTarget()
        {
            Pawn pawn = parent.pawn;
            Predicate<Thing> validator = delegate (Thing t)
            {
                Pawn pawn3 = (Pawn)t;
                return (parent.CanApplyOn((LocalTargetInfo)pawn3) && parent.verb.targetParams.CanTarget(pawn3)) ? true : false;
            };
            Pawn pawn2 = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.None, TraverseParms.For(pawn), parent.verb.verbProps.range, validator);
            return pawn2;
        }

        protected override bool IsOpportune(Ability a, AbilityCastType castType, LocalTargetInfo target)
        {
            return base.IsOpportune(a, castType, target);
        }
    }

    public class AbilityComp_AICastBuffSelf : AbilityComp_AICast
    {
        protected override LocalTargetInfo GetTarget()
        {
            return parent.pawn;
        }
    }

    public class AbilityComp_AICastHealSelf : AbilityComp_AICast
    {
        protected override LocalTargetInfo GetTarget()
        {
            return parent.pawn;
        }

        protected override bool IsOpportune(Ability a, AbilityCastType castType, LocalTargetInfo target)
        {
            return parent.pawn.health.summaryHealth.SummaryHealthPercent < 1f;
        }
    }

    public class AbilityCompProperties_AICast : AbilityCompProperties
    {
        public AbilityCastType abilityCastType;

        public bool friendlyFireAI = true;

        public AbilityCompProperties_AICast()
        {
            this.compClass = typeof(AbilityComp_AICast);
        }
    }

    public enum AbilityCastType
    {
        Attack,
        Defend,
        Buff,
        Heal,
        SelfHeal,
        Flee
    }
}

using RimWorld;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Corruption.Psykers
{
    public class PsykerCast : Ability
    {
        private Mote moteCast;

        private Sustainer soundCast;

        private static float MoteCastFadeTime = 0.4f;

        private static float MoteCastScale = 1f;

        private static Vector3 MoteCastOffset = new Vector3(0f, 0f, 0.48f);

        public override bool CanCast
        {
            get
            {
                if (!base.CanCast)
                {
                    return false;
                }
                if (def.EntropyGain > float.Epsilon)
                {
                    return !pawn.psychicEntropy.WouldOverflowEntropy(def.EntropyGain);
                }
                return true;
            }
        }

        public PsykerCast(Pawn pawn, AbilityDef def) : base(pawn, def)
        {
        }

        public PsykerCast(Pawn pawn) : base(pawn)
        {
        }

        public override IEnumerable<Command> GetGizmos()
        {
            if (this.gizmo == null)
            {
                this.gizmo = new Command_PsyCastCorruption(this);
            }
            yield return this.gizmo;
        }

        public override bool Activate(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (def.EntropyGain > float.Epsilon && !pawn.psychicEntropy.TryAddEntropy(def.EntropyGain))
            {
                return false;
            }
            if (base.Activate(target, dest))
            {
                var psyComp = this.pawn.GetComp<CompPsyker>();
                psyComp.AddXP(this.def.level);
                TryGainAttention();
                return true;
            }
            return false;
        }

        public override void ApplyEffects(IEnumerable<CompAbilityEffect> effects, LocalTargetInfo target, LocalTargetInfo dest)
        {
            Thing thing = target.Thing;
            if (thing != null && thing.Faction == this.pawn.Faction && !(this.CompOfType<AbilityComp_AICast>()?.Props.friendlyFireAI ?? true))
            {
                return;
            }
            using (IEnumerator<CompAbilityEffect> enumerator = effects.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Apply(target, dest);

                }
            }

            if (HasCooldown)
            {
                StartCooldown(def.cooldownTicksRange.RandomInRange);
            }
        }

        public override bool CanApplyOn(LocalTargetInfo target)
        {
            foreach (CompAbilityEffect effectComp in this.EffectComps)
            {
                if (!effectComp.CanApplyOn(target, null))
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanApplyPsycastTo(LocalTargetInfo target)
        {
            Pawn pawn = target.Pawn;
            if (pawn != null)
            {
                if (pawn.GetStatValue(StatDefOf.PsychicSensitivity) < float.Epsilon && base.EffectComps.Any((CompAbilityEffect e) => e.Props.psychic))
                {
                    return false;
                }
                if (pawn.Faction == Faction.OfMechanoids && base.EffectComps.Any((CompAbilityEffect e) => !e.Props.applicableToMechs))
                {
                    return false;
                }
            }
            return true;
        }

        public override void QueueCastingJob(LocalTargetInfo target, LocalTargetInfo destination)
        {
            base.QueueCastingJob(target, destination);
            if (moteCast == null || moteCast.Destroyed)
            {
                moteCast = MoteMaker.MakeAttachedOverlay(pawn, ThingDefOf.Mote_CastPsycast, MoteCastOffset, MoteCastScale, base.verb.verbProps.warmupTime - MoteCastFadeTime);
            }
        }

        public override void AbilityTick()
        {
            base.AbilityTick();
            if (moteCast != null && !moteCast.Destroyed && base.verb.WarmingUp)
            {
                moteCast.Maintain();
            }
            if (base.verb.WarmingUp)
            {
                if (soundCast == null || soundCast.Ended)
                {
                    soundCast = SoundDefOf.PsycastCastLoop.TrySpawnSustainer(SoundInfo.InMap(new TargetInfo(pawn.Position, pawn.Map), MaintenanceType.PerTick));
                }
                else
                {
                    soundCast.Maintain();
                }
            }
        }

        protected virtual void TryGainAttention()
        {
            if (this.pawn.health.hediffSet.hediffs.Any(x => PsykerUtility.DemonicPossessionHediffs.Contains(x.def)))
            {
                return;
            }
            if (this.pawn.health.hediffSet.hediffs.Any(x => PsykerUtility.DemonicAttentionHediffs.Contains(x.def)))
            {
                return;
            }
            this.pawn.health.AddHediff(PsykerUtility.DemonicAttentionHediffs.RandomElement());
        }
    }
}

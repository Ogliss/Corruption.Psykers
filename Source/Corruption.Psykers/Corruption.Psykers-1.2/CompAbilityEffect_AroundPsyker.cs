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
    public class CompAbilityEffect_AroundPsyker : CompAbilityEffect
    {
        public new CompProperties_AroundPsyker Props => this.props as CompProperties_AroundPsyker;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (this.Props.mote != null)
            {
                MoteMaker.MakeAttachedOverlay(this.parent.pawn, this.Props.mote, Vector3.zero);
            }
            List<LocalTargetInfo> targets = new List<LocalTargetInfo>();
            foreach (var thing in GenRadial.RadialDistinctThingsAround(target.Cell, this.parent.pawn.Map, Math.Min(150f, this.parent.def.EffectRadius), true))
            {
                if (this.parent.def.verbProperties.targetParams.CanTarget(thing))
                {
                    if (!this.Props.applyToSelf && thing == this.parent.pawn)
                    {
                        continue;
                    }
                    if (this.Props.friendliesOnly && thing.Faction.HostileTo(Faction.OfPlayer))
                    {
                        continue;
                    }
                    if (this.Props.hostilesOnly && !thing.Faction.HostileTo(Faction.OfPlayer))
                    {
                        continue;
                    }
                    if (!targets.Contains(thing))
                    {
                        targets.Add(thing);
                    }
                }
            }

            int hitTargets = 0;
            foreach (var targetedThing in targets.OrderBy(x => x.Cell.DistanceTo(this.parent.pawn.Position)))
            {
                foreach (var effect in this.parent.EffectComps.Where(x => x is CompAbilityEffect_AroundPsyker == false))
                {
                    effect.Apply(targetedThing, dest);
                }
                hitTargets++;
                if (hitTargets > this.Props.maxTargets)
                {
                    break;
                }
            }
        }
    }

    public class CompProperties_AroundPsyker : CompProperties_AbilityEffect
    {
        public bool applyToSelf = false;

        public bool hostilesOnly = false;

        public bool friendliesOnly = false;

        public int maxTargets = int.MaxValue;

        public ThingDef mote;

        public CompProperties_AroundPsyker()
        {
            this.compClass = typeof(CompAbilityEffect_AroundPsyker);
        }
    }

}

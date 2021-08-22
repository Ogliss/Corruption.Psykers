using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Verse;

namespace Corruption.Psykers
{
    public class CompAbilityEffect_DirectDamage : CompAbilityEffect
    {
        public new CompProperties_DirectDamage Props => this.props as CompProperties_DirectDamage;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Map map = this.parent.pawn.Map;
            Thing hitThing = target.Thing;
            if (hitThing != null)
            {
                DamageInfo dinfo = new DamageInfo(this.Props.damageDef, this.Props.damage, this.Props.armorPenetration, -1f, this.parent.pawn, null, null, DamageInfo.SourceCategory.ThingOrUnknown, target.Thing);
                hitThing.TakeDamage(dinfo);

                Pawn pawn = hitThing as Pawn;
                if (pawn != null && !pawn.Dead)
                {
                    pawn.stances.StaggerFor(this.Props.staggerTicks);
                }
                if (this.Props.moteDef != null)
                {
                    MoteMaker.MakeAttachedOverlay(this.parent.pawn, this.Props.moteDef, Vector3.zero);
                }
            }
        }
    }

    public class CompProperties_DirectDamage : CompProperties_AbilityEffect
    {
        public DamageDef damageDef;
        public int damage = 1;
        public float armorPenetration = 0f;
        public int staggerTicks = 95;
        public ThingDef moteDef;

        public override IEnumerable<string> ConfigErrors(AbilityDef parentDef)
        {
            if (this.moteDef == null) this.moteDef = ThingDefOf.Mote_ExplosionFlash;
            return base.ConfigErrors(parentDef);
        }
    }
}

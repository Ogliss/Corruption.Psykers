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
    public class HediffComp_Heal : HediffComp
    {
        private int ticksToEffect = 1;

        public HediffCompProperties_Heal Props => this.props as HediffCompProperties_Heal;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.ticksToEffect--;
            if (ticksToEffect <= 0)
            {
                float healingAmount = this.Props.healingPower * this.Pawn.HealthScale * 0.1f;
                foreach (var hediff in this.Pawn.health.hediffSet.hediffs.Where(x => x is Hediff_Injury || x.def.makesSickThought == true))
                {
                    hediff.Heal(healingAmount);
                }

                ticksToEffect = this.Props.ticksToEffect;
            }
            if (Pawn.IsHashIntervalTick(100) && Pawn.Map != null && !Pawn.Position.Fogged(Pawn.Map))
            {
                MoteMaker.ThrowMetaIcon(this.Pawn.Position, Pawn.Map, ThingDefOf.Mote_HealingCross);
            }
        }
    }

    public class HediffCompProperties_Heal : HediffCompProperties
    {
        public float healingPower = 8f;

        public int ticksToEffect = 120;

        public HediffDef incompatibleWith;

    }
}

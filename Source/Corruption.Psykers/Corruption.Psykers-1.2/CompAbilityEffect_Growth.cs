using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class CompAbilityEffect_Growth : CompAbilityEffect
    {
        private ComProperties_Growth growthProperties
        {
            get
            {
                return this.Props as ComProperties_Growth;
            }
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Plant plant = target.Thing as Plant;
            if (plant != null)
            {
                plant.Growth += growthProperties.growthPower / plant.def.plant.growDays;
                MoteMaker.ThrowAirPuffUp(plant.DrawPos, plant.Map);
            }
        }
    }

    public class ComProperties_Growth : CompProperties_AbilityEffect
    {
        public float growthPower = 1f;
    }
}

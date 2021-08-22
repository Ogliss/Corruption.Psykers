using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class CompAbilityEffect_SpawnBeam : CompAbilityEffect
    {
        public new CompProperties_SpawnBeam Props => this.props as CompProperties_SpawnBeam;

        private static ThingDef infernoDef = DefDatabase<ThingDef>.GetNamed("PyroInfernoBeam");

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            InfernoBeam beam = new InfernoBeam(this.Props.radius, this.Props.damageRange);
            beam.instigator = this.parent.pawn;
            beam.duration = (int)(parent.def.statBases.GetStatValueFromList(StatDefOf.Ability_Duration, 5f) * 60);

            GenSpawn.Spawn(beam, target.Cell, this.parent.pawn.Map);
            beam.StartStrike();
        }
    }

    public class CompProperties_SpawnBeam : CompProperties_AbilityEffect
    {
        public IntRange damageRange = IntRange.one;
        public float radius = 5f;
    }
}

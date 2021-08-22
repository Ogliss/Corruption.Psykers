using Corruption.Core;
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
    public class CompAbilityEffect_SpawnVortex : CompAbilityEffect
    {
        public new CompProperties_SpawnVortex Props => this.props as CompProperties_SpawnVortex;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Vortex vortex = (Vortex)ThingMaker.MakeThing(this.Props.vortexToSpawn);
            vortex.Instigator = this.parent.pawn;
            vortex.Radius = this.Props.radius; //this.parent.def.statBases.GetStatValueFromList(StatDefOf.Ability_EffectRadius, 1);
            vortex.LifeTimeSecs = (int)this.parent.def.statBases.GetStatValueFromList(StatDefOf.Ability_Duration, 1);
            GenSpawn.Spawn(vortex, target.Cell, this.parent.pawn.Map, WipeMode.Vanish);            
        }

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            GenDraw.DrawFieldEdges(GenRadial.RadialCellsAround(target.Cell, this.Props.radius, true).ToList(), Valid(target) ? Color.white : Color.red);
        }
    }

    public class CompProperties_SpawnVortex : CompProperties_AbilityEffect
    {
        public ThingDef vortexToSpawn;

        public float radius;

        public CompProperties_SpawnVortex()
        {
            this.compClass = typeof(CompAbilityEffect_SpawnVortex);
        }
    }
}

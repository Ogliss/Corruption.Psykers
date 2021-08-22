using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Verse;
using Verse.Noise;

namespace Corruption.Psykers
{
    public class CompAbilityEffect_SpawnPattern : CompAbilityEffect
    {
        public new ComProperties_SpawnThing Props => this.props as ComProperties_SpawnThing;

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            GenDraw.DrawFieldEdges(AffectedCells(target, parent.pawn.Map).ToList(), Valid(target) ? Color.white : Color.red);
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Map map = this.parent.pawn.Map;
            IntVec3 casterPos = this.parent.pawn.Position;

            List<Thing> list = new List<Thing>();
            list.AddRange(AffectedCells(target, map).SelectMany((IntVec3 c) => from t in c.GetThingList(map)
                                                                               where t.def.category == ThingCategory.Item
                                                                               select t));
            foreach (Thing item in list)
            {
                item.DeSpawn();
            }

            foreach (var cell in AffectedCells(target, this.parent.pawn.Map))
            {
                if (!cell.Filled(this.parent.pawn.Map) && cell.Standable(parent.pawn.Map))
                {
                    ThingWithComps thing = ThingMaker.MakeThing(this.Props.thingToSpawn) as ThingWithComps; // = GenSpawn.Spawn(this.Props.thingToSpawn, cell, this.parent.pawn.Map) as ThingWithComps;           
                    if (thing != null)
                    {
                        CompLifespan compLifespan = thing.TryGetComp<CompLifespan>();
                        if (compLifespan != null)
                        {
                            compLifespan.age = (int)(compLifespan.Props.lifespanTicks - (this.parent.def.statBases.GetStatValueFromList(StatDefOf.Ability_Duration, compLifespan.Props.lifespanTicks) * GenTicks.TicksPerRealSecond));
                        }
                        CompDamageOnSpawn compDamage = thing.TryGetComp<CompDamageOnSpawn>();
                        if (compDamage != null)
                        {
                            compDamage.Caster = this.parent.pawn;
                        }
                        GenSpawn.Spawn(thing, cell, this.parent.pawn.Map);
                    }
                }
            }
        }

        protected IEnumerable<IntVec3> AffectedCells(LocalTargetInfo target, Map map)
        {
            var sourceVector = this.parent.pawn.Position.ToVector3();
            var targetVector = target.Cell.ToVector3();
            var angleRef = (target.Cell.ToVector3() - sourceVector).AngleFlat();
            foreach (IntVec2 item in Props.pattern)
            {
                var patternTarget = (target.Cell + new IntVec3(item.x, 0, item.z)).ToVector3();
                var fromCenter = patternTarget - targetVector;
                var rotated = fromCenter.RotatedBy(angleRef);

                yield return (targetVector + rotated).ToIntVec3();
            } 
        }
    }

    public class ComProperties_SpawnThing : CompProperties_AbilityEffect
    {
        public ThingDef thingToSpawn;

        public ThingDef Stuff;

        public List<IntVec2> pattern;

        public List<ThingCategory> thingCategoriesToDespawn = new List<ThingCategory>();

        public Color moteColor = new Color(0.55f, 0.55f, 0.55f, 4f);

        public override IEnumerable<string> ConfigErrors(AbilityDef parentDef)
        {
            foreach (var error in base.ConfigErrors(parentDef))
            {
                yield return error;
            }

            if (this.Stuff != null && this.Stuff.IsStuff == false)
            {
                yield return $"{this.Stuff.defName} is no stuff in Properties {this.ToString()} ";
            }
        }
    }
}

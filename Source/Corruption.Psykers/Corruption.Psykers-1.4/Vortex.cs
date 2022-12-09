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
    public class Vortex : ThingWithComps
    {
        private int spawnTick;
        public float Radius;
        public Pawn Instigator;
        private bool shouldDisappear;

        public float AgeSecs
        {
            get
            {
                return (float)(Find.TickManager.TicksGame - spawnTick) / 60f;
            }
        }

        public int LifeTimeSecs;

        private int ticksToEffect;

        private CompVortex _vortexComp;

        public CompVortex VortexComp
        {
            get
            {
                if (_vortexComp == null)
                {
                    _vortexComp = this.GetComp<CompVortex>();
                }
                return _vortexComp;
            }
        }

        public override void PostMake()
        {
            base.PostMake();
            this.Radius = this.VortexComp.Props.effectRadius;
            this.LifeTimeSecs = this.VortexComp.Props.avgLifetime.RandomInRange;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.spawnTick = GenTicks.TicksGame;
            MoteMaker.ThrowLightningGlow(this.DrawPos, this.Map, this.Radius);
        }

        public float exactRotation = 0f;

        public override void Draw()
        {
            Graphics.DrawMesh(MeshPool.GridPlane(Graphic.drawSize), DrawPos, Quaternion.AngleAxis(exactRotation, Vector3.up), Graphic.MatSingle, 0);
            Comps_PostDraw();
        }

        public override void Tick()
        {
            base.Tick();
            ticksToEffect--;
            this.exactRotation++;
            if (ticksToEffect <= 0)
            {
                ticksToEffect = this.VortexComp.Props.ticksPerEffectCycle;
                var targets = GetAffectedTargets();
                ApplyEffects(targets);
            }

            if (this.AgeSecs > this.LifeTimeSecs + this.def.mote.fadeOutTime)
            {
                this.Destroy();
            }
        }

        public void ApplyEffects(IEnumerable<LocalTargetInfo> targets)
        {
            foreach (var target in targets)
            {
                Thing hitThing = target.Thing;
                if (hitThing != null && this.VortexComp.Props.damageDef != null)
                {
                    DamageInfo dinfo = new DamageInfo(this.VortexComp.Props.damageDef, this.VortexComp.Props.damageAmount, this.VortexComp.Props.armorPenetration, -1f, this.Instigator, null, null, DamageInfo.SourceCategory.ThingOrUnknown, hitThing);
                    hitThing.TakeDamage(dinfo);
                }
                if (hitThing != null && this.VortexComp.Props.hediffToGive != null)
                {
                    TryGiveHediff(hitThing);
                }
            }
        }

        private void TryGiveHediff(Thing hitThing)
        {
            var pawn = hitThing as Pawn;
            if (pawn != null)
            {
                if (this.VortexComp.Props.hediffSeverityToAdd < 1)
                {
                    pawn.health.AddHediff(this.VortexComp.Props.hediffToGive, null, null);
                }
                else
                {
                    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(this.VortexComp.Props.hediffToGive);
                    if (hediff == null)
                    {
                        hediff = pawn.health.AddHediff(this.VortexComp.Props.hediffToGive);
                    }
                    hediff.Severity += this.VortexComp.Props.hediffSeverityToAdd;
                }
            }
        }

        public IEnumerable<LocalTargetInfo> GetAffectedTargets()
        {
            foreach (LocalTargetInfo item in from t in GenRadial.RadialDistinctThingsAround(this.Position, this.Map, this.VortexComp.Props.effectRadius, useCenter: true)
                                             where this.VortexComp.Props.verb.targetParams.CanTarget(t)
                                             select new LocalTargetInfo(t))
            {
                yield return item;
            }
        }

        public virtual float Alpha
        {
            get
            {
                float ageSecs = AgeSecs;
                if (ageSecs <= def.mote.fadeInTime)
                {
                    if (def.mote.fadeInTime > 0f)
                    {
                        return ageSecs / def.mote.fadeInTime;
                    }
                    return 1f;
                }
                if (ageSecs <= def.mote.fadeInTime + LifeTimeSecs)
                {
                    return 1f;
                }
                if (def.mote.fadeOutTime > 0f)
                {
                    return 1f - Mathf.InverseLerp(def.mote.fadeInTime + LifeTimeSecs, def.mote.fadeInTime + LifeTimeSecs + def.mote.fadeOutTime, ageSecs);
                }
                return 1f;
            }
        }
    }

    public class CompVortex : ThingComp
    {
        public CompProperties_Vortex Props => this.props as CompProperties_Vortex;

        public override void PostDraw()
        {
            base.PostDraw();
        }
    }

    public class CompProperties_Vortex : CompProperties
    {
        public float effectRadius;
        public int ticksPerEffectCycle = 60;
        public DamageDef damageDef;
        public int damageAmount;
        public float armorPenetration;
        public HediffDef hediffToGive;
        public float hediffSeverityToAdd = -1;
        public VerbProperties verb = new VerbProperties();
        public IntRange avgLifetime = IntRange.one;
        public ThingDef moteOnEffect = ThingDefOf.Mote_ExplosionFlash;

        public CompProperties_Vortex()
        {
            this.compClass = typeof(CompVortex);
        }

    }
}

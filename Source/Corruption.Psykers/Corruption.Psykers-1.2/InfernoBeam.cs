using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Verse;

namespace Corruption.Psykers
{
    public class InfernoBeam : OrbitalStrike
    {
        private const int FiresStartedPerTick = 4;

        private IntRange FlameDamageAmountRange = new IntRange(65, 100);

        private static readonly IntRange CorpseFlameDamageAmountRange = new IntRange(5, 10);

        private static List<Thing> tmpThings = new List<Thing>();

        private float radius;

        public InfernoBeam(float radius, IntRange damageAmount)
        {
            this.radius = radius;
            this.FlameDamageAmountRange = damageAmount;
        }

        public override void StartStrike()
        {
            base.StartStrike();
            MoteMaker.MakePowerBeamMote(base.Position, base.Map);
        }

        public override void Tick()
        {
            base.Tick();
            if (!base.Destroyed)
            {
                for (int i = 0; i < 4; i++)
                {
                    StartRandomFireAndDoFlameDamage();
                }
            }
        }


        private void StartRandomFireAndDoFlameDamage()
        {
            IntVec3 c = (from x in GenRadial.RadialCellsAround(base.Position, 15f, useCenter: true)
                         where x.InBounds(base.Map)
                         select x).RandomElementByWeight((IntVec3 x) => 1f - Mathf.Min(x.DistanceTo(base.Position) / 15f, 1f) + 0.05f);
            FireUtility.TryStartFireIn(c, base.Map, Rand.Range(0.1f, 0.925f));
            tmpThings.Clear();
            tmpThings.AddRange(c.GetThingList(base.Map));
            for (int i = 0; i < tmpThings.Count; i++)
            {
                int num = (tmpThings[i] is Corpse) ? CorpseFlameDamageAmountRange.RandomInRange : FlameDamageAmountRange.RandomInRange;
                Pawn pawn = tmpThings[i] as Pawn;
                BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = null;
                if (pawn != null)
                {
                    battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(pawn, RulePackDefOf.DamageEvent_PowerBeam, instigator as Pawn);
                    Find.BattleLog.Add(battleLogEntry_DamageTaken);
                }
                tmpThings[i].TakeDamage(new DamageInfo(DamageDefOf.Flame, num, 0f, -1f, instigator, null, weaponDef)).AssociateWithLog(battleLogEntry_DamageTaken);
            }
            tmpThings.Clear();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.radius, "radius");
            Scribe_Values.Look<IntRange>(ref this.FlameDamageAmountRange, "flameDamageAmountRange");
        }
    }
}

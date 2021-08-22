using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Corruption.Psykers
{
    public class CompAbilityEffect_PsyProjectile : CompAbilityEffect
    {
        private VerbProperties verbProps
        {
            get
            {
                return this.parent.verb.verbProps;
            }
        }

        
        private Thing Caster
        {
            get
            {
                return this.parent?.pawn;
            }
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            for (int i = 0; i < this.parent.verb.verbProps.burstShotCount; i++)
            {
                if (target.HasThing && target.Thing.Map != this.Caster.Map)
                {
                    return;
                }
                ThingDef projectile = this.parent.verb.verbProps.defaultProjectile;
                if (projectile == null)
                {
                    return;
                }
                bool flag = this.parent.verb.CanHitTargetFrom(this.Caster.Position, target);
                if (this.verbProps.stopBurstWithoutLos && !flag)
                {
                    return;
                }
                ShootLine shootLine = new ShootLine(this.Caster.Position, target.Cell);
                Thing launcher = this.Caster;
                Vector3 drawPos = this.Caster.DrawPos;
                Projectile projectile2 = (Projectile)GenSpawn.Spawn(projectile, shootLine.Source, this.Caster.Map, WipeMode.Vanish);
                if (this.verbProps.forcedMissRadius > 0.5f)
                {
                    float num = VerbUtility.CalculateAdjustedForcedMiss(this.verbProps.forcedMissRadius, dest.Cell - this.Caster.Position);
                    if (num > 0.5f)
                    {
                        int max = GenRadial.NumCellsInRadius(num);
                        int num2 = Rand.Range(0, max);
                        if (num2 > 0)
                        {
                            IntVec3 c = dest.Cell + GenRadial.RadialPattern[num2];
                            ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
                            if (Rand.Chance(0.5f))
                            {
                                projectileHitFlags = ProjectileHitFlags.All;
                            }
                            projectileHitFlags &= ~ProjectileHitFlags.NonTargetPawns;

                            projectile2.Launch(launcher, drawPos, c, target, projectileHitFlags, projectile2, null);
                            return;
                        }
                    }
                }
                ShotReport shotReport = ShotReport.HitReportFor(this.Caster, this.parent.verb, target);
                Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
                ThingDef targetCoverDef = (randomCoverToMissInto != null) ? randomCoverToMissInto.def : null;
                if (!Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
                {
                    shootLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget);
                    ProjectileHitFlags projectileHitFlags2 = ProjectileHitFlags.NonTargetWorld;
                    if (Rand.Chance(0.5f))
                    {
                        projectileHitFlags2 |= ProjectileHitFlags.NonTargetPawns;
                    }
                    projectile2.Launch(launcher, drawPos, shootLine.Dest, target, projectileHitFlags2, null, targetCoverDef);
                    return;
                }
                if (target.Thing != null && target.Thing.def.category == ThingCategory.Pawn && !Rand.Chance(shotReport.PassCoverChance))
                {
                    ProjectileHitFlags projectileHitFlags3 = ProjectileHitFlags.NonTargetWorld;
                    projectileHitFlags3 |= ProjectileHitFlags.NonTargetPawns;

                    projectile2.Launch(launcher, drawPos, randomCoverToMissInto, target, projectileHitFlags3, null, targetCoverDef);
                    return;
                }
                ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetPawns;

                if (!target.HasThing || target.Thing.def.Fillage == FillCategory.Full)
                {
                    projectileHitFlags4 |= ProjectileHitFlags.NonTargetWorld;
                }
                if (target.Thing != null)
                {
                    projectile2.Launch(launcher, drawPos, target, target, projectileHitFlags4, null, targetCoverDef);
                }
                else
                {
                    projectile2.Launch(launcher, drawPos, shootLine.Dest, target, projectileHitFlags4, null, targetCoverDef);
                }
            }

        }
    }
}

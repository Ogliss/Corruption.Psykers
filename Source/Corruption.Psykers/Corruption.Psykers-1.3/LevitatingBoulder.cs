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
    public class LevitatingBoulder : Projectile_Explosive
    {
        private ThingDef chunkDef;

        private Graphic cachedGraphic;

        private float exactRotation = 0f;

        private Material drawMat;

        public Material DrawMaterial
        {
            get
            {
                if(drawMat == null)
                {
                    InitGraphic();
                }
                return drawMat;
            }
        }


        public override void Tick()
        {
            base.Tick();
            this.exactRotation += 2f;
        }

        public override void Draw()
        {
            Graphics.DrawMesh(MeshPool.GridPlane(Graphic.drawSize), DrawPos, Quaternion.AngleAxis(exactRotation, Vector3.up), this.DrawMaterial, 0);
            Comps_PostDraw();
        }

        private Vector2 drawSize
        {
            get
            {
                var exRadius = this.def.projectile.explosionRadius;
                return new Vector2(exRadius, exRadius);
            }
        }

        private void InitGraphic()
        {
            if (this.chunkDef.graphicData.graphicClass == null)
            {
                cachedGraphic = null;
                return;
            }
            ShaderTypeDef cutout = this.chunkDef.graphicData.shaderType;
            if (cutout == null)
            {
                cutout = ShaderTypeDefOf.Cutout;
            }
            Shader shader = cutout.Shader;
            cachedGraphic = GraphicDatabase.Get(this.chunkDef.graphicData.graphicClass, this.chunkDef.graphicData.texPath, shader, this.def.graphicData.drawSize, this.chunkDef.graphicData.color, this.chunkDef.graphicData.colorTwo, this.chunkDef.graphicData, this.chunkDef.graphicData.shaderParameters);
            if (this.chunkDef.graphicData.onGroundRandomRotateAngle > 0.01f)
            {
                cachedGraphic = new Graphic_RandomRotated(cachedGraphic, this.chunkDef.graphicData.onGroundRandomRotateAngle);
            }
            this.drawMat = cachedGraphic.MatSingle;

        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                var rockTypes = Find.World.NaturalRockTypesIn(map.Tile);
                this.chunkDef = rockTypes.RandomElement().building.mineableThing;
            }
        }

        public override void Explode()
        {
            Map map = base.Map;
            Destroy();
            if (base.def.projectile.explosionEffect != null)
            {
                Effecter effecter = base.def.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(base.Position, map), new TargetInfo(base.Position, map));
                effecter.Cleanup();
            }
            GenExplosion.DoExplosion(base.Position, map, base.def.projectile.explosionRadius, base.def.projectile.damageDef, base.launcher, base.DamageAmount, base.ArmorPenetration, base.def.projectile.soundExplode, base.equipmentDef, base.def, intendedTarget.Thing, this.chunkDef, base.def.projectile.postExplosionSpawnChance, base.def.projectile.postExplosionSpawnThingCount, preExplosionSpawnThingDef: base.def.projectile.preExplosionSpawnThingDef, preExplosionSpawnChance: base.def.projectile.preExplosionSpawnChance, preExplosionSpawnThingCount: base.def.projectile.preExplosionSpawnThingCount, applyDamageToExplosionCellsNeighbors: base.def.projectile.applyDamageToExplosionCellsNeighbors, chanceToStartFire: base.def.projectile.explosionChanceToStartFire, damageFalloff: base.def.projectile.explosionDamageFalloff, direction: origin.AngleToFlat(destination));
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }

    }
}

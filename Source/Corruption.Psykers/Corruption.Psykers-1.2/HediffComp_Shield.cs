using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Corruption.Psykers
{
    public class HediffComp_Shield : HediffComp
    {
        public float MaxShieldCapacity;
        public float ShieldCapacity = 100f;
        private float RechargeRate = 0f;
        private bool shouldRemove;
        private ShieldState ShieldState = ShieldState.Active;

        public override void CompPostMake()
        {
            base.CompPostMake();
            this.MaxShieldCapacity = this.parent.CurStage.statOffsets.GetStatValueFromList(StatDefOf.EnergyShieldEnergyMax, 100f);
            this.ShieldCapacity = this.MaxShieldCapacity;
            this.RechargeRate = this.parent.CurStage.statOffsets.GetStatValueFromList(StatDefOf.EnergyShieldRechargeRate, 0f);
        }

        public override bool CompShouldRemove => this.shouldRemove;

        public HediffCompProperties_Shield Props => this.props as HediffCompProperties_Shield;

        public bool ShouldDisplay => !this.Pawn.Downed && !this.Pawn.Dead;

        public void TryAbsorbDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            AbsorbedDamage(dinfo);
            float remainingCapacity = this.ShieldCapacity - dinfo.Amount;
            if (remainingCapacity < 0)
            {
                dinfo.SetAmount(Math.Abs(remainingCapacity));
                this.ShieldCapacity = 0;
                absorbed = false;
                if (this.Props.removeOnSpent)
                {
                    this.shouldRemove = true;
                }
            }
            else
            {
                this.ShieldCapacity = remainingCapacity;
                absorbed = true;
            }
        }

        private void AbsorbedDamage(DamageInfo dinfo)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(this.Pawn.Position, this.Pawn.Map));
            var impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 loc = this.Pawn.TrueCenter() + impactAngleVect.RotatedBy(180f) * 0.5f;
            float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
            MoteMaker.MakeStaticMote(loc, this.Pawn.Map, ThingDefOf.Mote_ExplosionFlash, num);
            int num2 = (int)num;
            for (int i = 0; i < num2; i++)
            {
                MoteMaker.ThrowDustPuff(loc, this.Pawn.Map, Rand.Range(0.8f, 1.2f));
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.ShieldCapacity += this.RechargeRate;
            this.DrawShield();
        }

        public void DrawShield()
        {
            if (ShieldState == ShieldState.Active && ShouldDisplay)
            {
                float num = Mathf.Lerp(1.5f, 2.55f, ShieldCapacity);
                Vector3 drawPos = this.Pawn.Drawer.DrawPos;
                drawPos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                float angle = 0f;
                Vector3 s = new Vector3(num, 1f, num);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(drawPos, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, this.Props.drawMat, 0);

            }
        }
    }

    public class HediffCompProperties_Shield : HediffCompProperties
    {
        public bool removeOnSpent;
        public string materialPath = "Other/ShieldBubble";
        public GraphicData graphicData;
        public Material drawMat;

        public override void PostLoad()
        {
            base.PostLoad();
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                if (graphicData != null)
                {
                    this.drawMat = this.graphicData.Graphic.MatSingle;
                }
                else
                {
                    this.drawMat = MaterialPool.MatFrom(materialPath, ShaderDatabase.Transparent);
                }
            });

        }
    }

}

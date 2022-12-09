using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class CompDamageOnSpawn : ThingComp
    {
        public CompProperties_DamageOnSpawn Props => this.props as CompProperties_DamageOnSpawn;

        public Pawn Caster;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!respawningAfterLoad && this.Props.damageDef != null)
            {
                var things = parent.Map.thingGrid.ThingsListAt(parent.Position);
                for (int i = 0; i < things.Count; i++)
                {
                    things[i].TakeDamage(new DamageInfo(this.Props.damageDef, this.Props.amount, this.Props.armorPenetration, -1, this.Caster));
                }
            }
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look<Pawn>(ref Caster, "caster");
        }
    }

    public class CompProperties_DamageOnSpawn : CompProperties
    {
        public DamageDef damageDef;

        public float amount;

        public float armorPenetration;

        public CompProperties_DamageOnSpawn()
        {
            this.compClass = typeof(CompDamageOnSpawn);
        }
    }
}

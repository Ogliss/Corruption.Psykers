using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class PsykerProjectile : Projectile
    {
        public override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
        }
    }

    public class PsykerProjectile_Growth : PsykerProjectile
    {
        public override void Impact(Thing hitThing)
        {
            Plant plant = hitThing as Plant;
            if (plant != null)
            {
                plant.Growth += 1f / plant.def.plant.growDays * this.DamageAmount;
            }
        }
    }
}

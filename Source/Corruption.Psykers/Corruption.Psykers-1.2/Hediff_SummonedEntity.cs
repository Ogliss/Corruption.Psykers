using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class HediffComp_TemporalEntity : HediffComp_Disappears
    {
        public new HediffCompProperties_TemporalEntity Props => this.props as HediffCompProperties_TemporalEntity;

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            this.Pawn.Destroy();
        }

        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            if (this.Pawn.Corpse != null)
            {
                this.Pawn.Corpse.Destroy();
            }
            else if (!this.Pawn.Destroyed)
            {
                this.Pawn.Destroy();
            }
        }

        private void SpawnMote()
        {
            if (this.Pawn.Spawned && this.Props.despawnMote != null)
            {
                MoteMaker.MakeStaticMote(this.Pawn.Position, this.Pawn.Map, this.Props.despawnMote, this.Pawn.Graphic.drawSize.x);
            }
        }
    }

    public class HediffCompProperties_TemporalEntity : HediffCompProperties_Disappears
    {
        public ThingDef despawnMote;

        public HediffCompProperties_TemporalEntity()
        {
            this.compClass = typeof(HediffComp_TemporalEntity);
        }
    }
}

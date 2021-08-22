using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class CompAbilityEffect_SatisfyNeed : CompAbilityEffect
    {
        public new CompProperties_SatisfyNeed Props => this.props as CompProperties_SatisfyNeed;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn pawn = target.Pawn;
            if (pawn != null)
            {
                var need = pawn.needs.TryGetNeed(this.Props.need);
                if (need != null)
                {
                    need.CurLevel += this.Props.needOffset;
                }
            }
        }
    }

    public class CompProperties_SatisfyNeed : CompProperties_AbilityEffect
    {
        public NeedDef need;

        public float needOffset = 0.2f;

        public CompProperties_SatisfyNeed()
        {
            this.compClass = typeof(CompAbilityEffect_SatisfyNeed);
        }
    }
}

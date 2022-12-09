using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Corruption.Psykers
{
    public class JobDriver_MeditateXP : JobDriver_Meditate
    {
        private static IntRange XpIntervalRange = new IntRange(90, 140);

        public override IEnumerable<Toil> MakeNewToils()
        {
            Toil lastToil = new Toil();
            IEnumerator<Toil> enumerator = base.MakeNewToils().GetEnumerator();
            while (enumerator.MoveNext())
            {
                lastToil = enumerator.Current;
                yield return enumerator.Current;
            }
            var psyComp = this.pawn.CompPsyker();
            if (psyComp != null && psyComp.PsykerPowerTrait != null)
            {
                lastToil.AddPreTickAction(delegate
                {
                    if (this.pawn.IsHashIntervalTick(XpIntervalRange.RandomInRange))
                    {
                        psyComp.AddXP(1);
                    }
                });
            }
        }
    }
}

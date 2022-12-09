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
    public class JobDriver_WaitCastPower : JobDriver_Wait
    {
        public JobDriver_WaitCastPower() : base()
        {

        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            if (this.pawn.TryCastAbility())
            {
                collideWithPawns = true;
            }
            
            return base.MakeNewToils();
        }

    }
}

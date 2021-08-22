using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public static class CompPsykerExtension
    {
        public static CompPsyker CompPsyker(this Pawn pawn)
        {
            return pawn.GetComp<CompPsyker>();
        }
    }
}

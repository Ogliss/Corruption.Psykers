using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class Comp_Weirdboy : ThingComp
    {
        public Pawn Pawn => this.parent as Pawn;

        public Trait PsykerPowerTrait
        {
            get
            {
                Trait psykerTrait = Pawn?.story?.traits.GetTrait(PsykerTraitDefOf.Psyker);
                if (psykerTrait != null)
                {
                    return psykerTrait;
                }
                return null;
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            return base.CompGetGizmosExtra();
        }
    }
}

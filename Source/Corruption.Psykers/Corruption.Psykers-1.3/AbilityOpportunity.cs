using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public struct AbilityOpportunity
    {
        public Ability ability;
        public AbilityCastType castType;
        public LocalTargetInfo target;

        public AbilityOpportunity(Ability ability, AbilityCastType castType, LocalTargetInfo target) : this()
        {
            this.ability = ability;
            this.castType = castType;
            this.target = target;
        }
    }
}

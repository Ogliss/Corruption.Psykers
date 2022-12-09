using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class PsykerCastAoE : PsykerCast
    {
        public PsykerCastAoE(Pawn pawn, AbilityDef def) : base(pawn, def)
        {
        }

        public PsykerCastAoE(Pawn pawn) : base(pawn)
        {
        }

        public override void ApplyEffects(IEnumerable<CompAbilityEffect> effects, LocalTargetInfo target, LocalTargetInfo dest)
        {
            var effectAoE = effects.FirstOrDefault(x => x is CompAbilityEffect_AroundPsyker);
            if (effectAoE != null && target == this.pawn)
            {
                effectAoE.Apply(target, dest);
            }

            if (HasCooldown)
            {
                StartCooldown(def.cooldownTicksRange.RandomInRange);
            }
        }
    }
}

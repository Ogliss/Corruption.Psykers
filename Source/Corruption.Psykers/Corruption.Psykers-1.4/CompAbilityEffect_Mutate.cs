using Corruption.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class CompAbilityEffect_Mutate : CompAbilityEffect
    {
        public new CompProperties_AbilityMutate Props => this.props as CompProperties_AbilityMutate;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            ApplyMutation(target.Pawn);
        }

        protected void ApplyMutation(Pawn pawn)
        {
            if (pawn == null)
            {
                return;
            }
            MutationUtility.ApplyMutation(pawn, this.Props.mutationHediffs, this.parent.def.EntropyGain / 100f);

        }
    }

    public class CompAbilityEffect_MutateSelf : CompAbilityEffect_Mutate
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            this.ApplyMutation(this.parent.pawn);
        }
    }

    public class CompProperties_AbilityMutate : CompProperties_AbilityEffect
    {
        public List<HediffDef> mutationHediffs = new List<HediffDef>();

        public float severityFactorFromEntropy = 1f;

        public CompProperties_AbilityMutate()
        {
            this.compClass = typeof(CompAbilityEffect_Mutate);
        }
    }

}

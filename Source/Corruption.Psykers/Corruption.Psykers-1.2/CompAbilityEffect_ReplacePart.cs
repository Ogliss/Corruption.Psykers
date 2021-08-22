using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class CompAbilityEffect_ReplacePart : CompAbilityEffect
    {
        public new CompProperties_ReplacePart Props => this.props as CompProperties_ReplacePart;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            ApplyInner(target.Pawn);
        }

		protected void ApplyInner(Pawn target)
		{
			if (target == null)
			{
				return;
			}

			var parts = target.def.race.body.AllParts.Where(x => x.def == this.Props.partToReplace);

			if (parts.Count() == 0)
			{
				return;
			}

			var fittingParts = parts.Where(x => target.health.hediffSet.PartIsMissing(x) == false);
			if (fittingParts.Count() == 0)
			{
				return;
			}

			var selectedPart = fittingParts.RandomElement();

			target.health.AddHediff(this.Props.hediff, selectedPart);
		}
	}

	public class CompAbilityEffect_ReplacePartSelf : CompAbilityEffect_ReplacePart
	{
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			this.ApplyInner(this.parent.pawn);
		}
	}


	public class CompProperties_ReplacePart : CompProperties_AbilityEffect
    {
        public HediffDef hediff;

        public BodyPartDef partToReplace;

		public float addedSeverityIfExists;

		public override IEnumerable<string> ConfigErrors(AbilityDef parentDef)
		{
			foreach (var error in base.ConfigErrors(parentDef))
			{
				yield return error;
			}
			if (this.hediff.hediffClass != typeof(Hediff_AddedPart))
			{
				yield return $"HediffDef {this.hediff.defName} has CompProperty_ReplacePart but is not of Type Hediff_AddedPart.";
			}

			if (this.partToReplace == null)
			{
				yield return $"HediffDef {this.hediff.defName} has CompProperty_ReplacePart but is missing a BodyPartDef";
			}
		}
	}

}

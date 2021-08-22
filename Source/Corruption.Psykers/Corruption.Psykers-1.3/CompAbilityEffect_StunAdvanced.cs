using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class CompAbilityEffect_StunAdvanced : CompAbilityEffect_WithDuration
    {
		public new CompProperties_AbilityEffectWithDurationAdvanced Props => this.props as CompProperties_AbilityEffectWithDurationAdvanced;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			if (target.HasThing)
			{
				base.Apply(target, dest);
				Pawn pawn = target.Thing as Pawn;
				if (pawn != null && this.parent.def.verbProperties.targetParams.CanTarget(pawn) && this.canEffect(pawn))
				pawn?.stances.stunner.StunFor(GetDurationSeconds(pawn).SecondsToTicks(), parent.pawn, addBattleLog: false);
			}
		}

		private bool canEffect(Pawn pawn)
		{
			if (pawn == this.parent.pawn && !this.Props.affectsSelf)
			{
				return false;
			}

			if (pawn.Faction == this.parent.pawn.Faction && !this.Props.friendlyFire)
			{
				return false;
			}
			return true;
		}
	}
}

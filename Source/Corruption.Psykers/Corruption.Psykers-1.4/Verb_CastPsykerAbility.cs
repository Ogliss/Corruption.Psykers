using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Psykers
{
    public class Verb_CastPsykerAbility : Verb_CastAbility
    {
		public PsykerCast PsykerCast => ability as PsykerCast;

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
		{
			if (!base.ValidateTarget(target, showMessages))
			{
				return false;
			}
			if (CasterPawn.psychicEntropy.WouldOverflowEntropy(PsykerCast.def.EntropyGain + PsycastUtility.TotalEntropyFromQueuedPsycasts(CasterPawn)))
			{
				Messages.Message("CommandPsycastWouldExceedEntropy".Translate(), caster, MessageTypeDefOf.RejectInput);
				return false;
			}
			return true;
		}

		public override void OrderForceTarget(LocalTargetInfo target)
		{
			if (IsApplicableTo(target))
			{
				base.OrderForceTarget(target);
			}
		}

		public override void DrawHighlight(LocalTargetInfo target)
		{
			AbilityDef def = ability.def;
			if (verbProps.range > 0f)
			{
				DrawRadiusCapped();
			}
			if (CanHitTarget(target) && IsApplicableTo(target))
			{
				if (def.HasAreaOfEffect)
				{
					if (target.IsValid)
					{
						GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.MetaOverlays);
						GenDraw.DrawRadiusRing(target.Cell, def.EffectRadius, RadiusHighlightColor);
					}
				}
				else
				{
					GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.MetaOverlays);
				}
			}
			if (target.IsValid)
			{
				ability.DrawEffectPreviews(target);
			}
		}

		public override void OnGUI(LocalTargetInfo target)
		{
			bool flag = ability.EffectComps.Any((CompAbilityEffect e) => e.Props.psychic);
			Texture2D texture2D = UIIcon;
			if (!PsykerCast.CanApplyPsycastTo(target))
			{
				texture2D = TexCommand.CannotShoot;
				DrawIneffectiveWarning(target);
			}
			if (target.IsValid && CanHitTarget(target) && IsApplicableTo(target))
			{
				if (flag)
				{
					foreach (LocalTargetInfo affectedTarget in ability.GetAffectedTargets(target))
					{
						if (PsykerCast.CanApplyPsycastTo(affectedTarget))
						{
							DrawSensitivityStat(affectedTarget);
						}
						else
						{
							DrawIneffectiveWarning(affectedTarget);
						}
					}
				}
				if (ability.EffectComps.Any((CompAbilityEffect e) => !e.Valid(target)))
				{
					texture2D = TexCommand.CannotShoot;
				}
			}
			else
			{
				texture2D = TexCommand.CannotShoot;
			}
			{
				GenUI.DrawMouseAttachment(texture2D);
			}
		}

		private void DrawIneffectiveWarning(LocalTargetInfo target)
		{
			if (target.Pawn != null)
			{
				Vector3 drawPos = target.Pawn.DrawPos;
				drawPos.z += 1f;
				GenMapUI.DrawText(new Vector2(drawPos.x, drawPos.z), "Ineffective".Translate(), Color.red);
			}
		}

		private void DrawSensitivityStat(LocalTargetInfo target)
		{
			if (target.Pawn != null)
			{
				Pawn pawn = target.Pawn;
				float statValue = pawn.GetStatValue(StatDefOf.PsychicSensitivity);
				Vector3 drawPos = pawn.DrawPos;
				drawPos.z += 1f;
				GenMapUI.DrawText(new Vector2(drawPos.x, drawPos.z), (string)(StatDefOf.PsychicSensitivity.LabelCap + ": ") + statValue, (statValue > float.Epsilon) ? Color.white : Color.red);
			}
		}


		public void DrawRadiusCapped()
		{
			if (ability.pawn.Spawned && verbProps.range <55f)
			{
				GenDraw.DrawRadiusRing(ability.pawn.Position, Math.Min(54f, verbProps.range));
			}
		}

	}
}

using Corruption.Core.Soul;
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
    [StaticConstructorOnStartup]
    public class Command_PsyCastCorruption : Command_Ability
    {
        private CompSoul soul;

        public Command_PsyCastCorruption(Ability ability) : base(ability)
        {
            this.soul = ability.pawn.Soul();
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            AbilityDef def = this.ability.def;
            Pawn pawn = this.ability.pawn;
            GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth);
            if (def.EntropyGain > float.Epsilon)
            {
                Text.Font = GameFont.Tiny;
                string text = def.EntropyGain.ToString();
                float x = Text.CalcSize(text).x;
                Widgets.Label(new Rect(topLeft.x + GetWidth(maxWidth) - x - 5f, topLeft.y + 5f, x, 18f), text);
            }
            this.disabled = false;
            base.disabled = false;
            if (def.EntropyGain > 1.401298E-45f)
            {
                Trait psykerTrait = pawn.story.traits.GetTrait(PsykerTraitDefOf.Psyker);
                if (psykerTrait == null)
                {
                    base.DisableWithReason("CommandPsykerCastForNonPsyker".Translate(pawn.Name.ToStringShort));
                }
                else if (psykerTrait.Degree < def.level)
                {
                    base.DisableWithReason("CommandPsykerCastLevelTooHigh".Translate(def.level - psykerTrait.Degree));
                }
                else if (pawn.psychicEntropy.WouldOverflowEntropy(def.EntropyGain + PsycastUtility.TotalEntropyFromQueuedPsycasts(pawn)))
                {
                    base.DisableWithReason("CommandPsycastWouldExceedEntropy".Translate(def.label));
                }
            }
            return result;
        }

        public override void GizmoUpdateOnMouseover()
        {
            Verb_CastPsykerAbility verb_CastAbility;
            if ((verb_CastAbility = (ability.verb as Verb_CastPsykerAbility)) != null || this.ability.CompsOfType<CompAbilityEffect_AroundPsyker>().Count() > 0)
            {
                verb_CastAbility.DrawRadiusCapped();
            }
        }
    }
}

using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class CompAbilityEffect_GiveThought : CompAbilityEffect
    {
        public new CompProperties_GiveThought Props => this.props as CompProperties_GiveThought;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn pawn = target.Pawn;
            if (pawn != null)
            {
                Thought_MemoryPsionic thought = (Thought_MemoryPsionic)ThoughtMaker.MakeThought(this.Props.thought);
                thought.expirationTicks = (int)this.parent.def.statBases.GetStatValueFromList(StatDefOf.Ability_Duration, 0f);
                thought.SetForcedStage(this.Props.stage);
                pawn.needs.mood.thoughts.memories.TryGainMemory(thought);
            }
        }
    }

    public class CompProperties_GiveThought : CompProperties_AbilityEffect
    {
        public ThoughtDef thought;
        public int stage;
    }
}

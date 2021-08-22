using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class HediffComp_AffectSkill : HediffComp
    {
        private int previousLevel;

        private float previousXp;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            var skillRecord = this.Pawn.skills.GetSkill(this.Props.skill);
            this.previousLevel = skillRecord.Level;
            this.previousXp = skillRecord.XpTotalEarned;

            skillRecord.Level = skillRecord.Level + this.Props.minimumOffset + (int)(this.parent.CurStage.minSeverity * this.Props.severityOffsetFactor);
            skillRecord.xpSinceLastLevel = 0f;
        }

        public override void Notify_PawnDied()
        {
            this.ResetSkill();
            base.Notify_PawnDied();
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            this.ResetSkill();
        }

        private void ResetSkill()
        {
            var skillRecord = this.Pawn.skills.GetSkill(this.Props.skill);
            var learntXP = skillRecord.xpSinceLastLevel;
            skillRecord.Level = this.previousLevel;
            skillRecord.xpSinceLastLevel = this.previousXp + learntXP;
        }

        public HediffCompProperties_AffectSkill Props => this.props as HediffCompProperties_AffectSkill;

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.previousLevel, "previousLevel");
            Scribe_Values.Look<float>(ref this.previousXp, "previousXp");
        }
    }

    public class HediffCompProperties_AffectSkill : HediffCompProperties
    {
        public SkillDef skill;

        public int minimumOffset = 1;

        public int severityOffsetFactor;

        public HediffCompProperties_AffectSkill()
        {
            compClass = typeof(HediffComp_AffectSkill);
        }
    }
}

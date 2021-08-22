using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class HediffComp_TemporaryRecruit : HediffComp_Disappears
    {
        public Faction PawnFactionOri;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            this.PawnFactionOri = this.Pawn.Faction;
            Faction setFaction = this.PawnFactionOri == Faction.OfPlayer ? Find.FactionManager.FirstFactionOfDef(Corruption.Core.FactionsDefOf.IoM_NPC) : Faction.OfPlayer;

            this.Pawn.SetFaction(setFaction);

            if (this.Pawn.Faction == Faction.OfPlayer)
            {
                this.Pawn.drafter.Drafted = true;
            }

            Find.ColonistBar.MarkColonistsDirty();
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_References.Look<Faction>(ref this.PawnFactionOri, "PawnFactionOri");
        }

        public override bool CompShouldRemove
        {
            get
            {
                if (base.CompShouldRemove)
                {
                    if (this.Pawn.Faction == Faction.OfPlayer)
                    {
                        this.Pawn.drafter.Drafted = false;
                    }
                    this.Pawn.SetFactionDirect(PawnFactionOri);
                    this.Pawn.jobs.EndCurrentJob(Verse.AI.JobCondition.InterruptForced);
                    Find.ColonistBar.MarkColonistsDirty();
                    return true;
                }
                return false;

            }
        }
    }
}

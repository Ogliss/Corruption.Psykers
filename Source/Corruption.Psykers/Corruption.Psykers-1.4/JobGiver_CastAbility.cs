using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Corruption.Psykers
{
    public class JobGiver_CastAbility : ThinkNode_Conditional
    {
        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            Job job = GetCastJob(pawn);
            if (job == null)
            {
                return ThinkResult.NoJob;
            }
            return new ThinkResult(job, this);
        }

        private Job GetCastJob(Pawn pawn)
        {
            AbilityOpportunity opportunity;
            if(PsykerUtility.TryGetAbilityOpportunity(pawn, out opportunity))
            {Job job = JobMaker.MakeJob(JobDefOf.UseVerbOnThing, opportunity.target);
                job.verbToUse = opportunity.ability.verb;
                return job;
            }
            return null;
        }

        public override bool Satisfied(Pawn pawn)
        {
            return pawn.abilities.abilities.Count > 0;
        }
    }
}

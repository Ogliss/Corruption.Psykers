using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace Corruption.Psykers
{
    public class CompAbilityEffect_SpawnPawn : CompAbilityEffect_WithDuration
    {
        public new CompProperties_SpawnPawn Props => this.props as CompProperties_SpawnPawn;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            for (int i = 0; i < this.Props.spawnCount; i++)
            {
                Pawn newPawn = PawnGenerator.GeneratePawn(Props.kindDef, this.Props.isPsykerControlled ? this.parent.pawn.Faction : Find.FactionManager.FirstFactionOfDef(Props.kindDef.defaultFactionType)); // Find.FactionManager.FirstFactionOfDef(Corruption.Core.FactionsDefOf.IoM_NPC));
                GenSpawn.Spawn(newPawn, target.Cell, this.parent.pawn.Map);
                if (newPawn.Faction != null && newPawn.Faction != Faction.OfPlayer)
                {
                    Lord lord = null;
                    if (newPawn.Map.mapPawns.SpawnedPawnsInFaction(newPawn.Faction).Any((Pawn p) => p != newPawn))
                    {
                        lord = ((Pawn)GenClosest.ClosestThing_Global(newPawn.Position, newPawn.Map.mapPawns.SpawnedPawnsInFaction(newPawn.Faction), 99999f, (Thing p) => p != newPawn && ((Pawn)p).GetLord() != null)).GetLord();
                    }
                    if (lord == null)
                    {
                        LordJob_DefendPoint lordJob = new LordJob_DefendPoint(newPawn.Position);
                        lord = LordMaker.MakeNewLord(newPawn.Faction, lordJob, Find.CurrentMap);
                    }
                    lord.AddPawn(newPawn);
                }

                if (this.Props.spawningMentalState != null)
                {
                    newPawn.mindState.mentalStateHandler.TryStartMentalState(this.Props.spawningMentalState);
                }
                foreach (var hediffDef in this.Props.spawningHediffs)
                {
                    Hediff hediff = HediffMaker.MakeHediff(hediffDef, newPawn);
                    HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
                    if (hediffComp_Disappears != null)
                    {
                        hediffComp_Disappears.ticksToDisappear = GetDurationSeconds(newPawn).SecondsToTicks();
                    }
                    newPawn.health.AddHediff(hediff);
                }

            }
        }
    }

    public class CompProperties_SpawnPawn : CompProperties_AbilityEffectWithDuration
    {
        public PawnKindDef kindDef;

        public int spawnCount = 1;

        public bool isPsykerControlled = false;

        public List<HediffDef> spawningHediffs = new List<HediffDef>();

        public MentalStateDef spawningMentalState;

        public CompProperties_SpawnPawn()
        {
            this.compClass = typeof(CompAbilityEffect_SpawnPawn);
        }
    }
}

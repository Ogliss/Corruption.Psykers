using Corruption.Core;
using Corruption.Core.Abilities;
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
    public class CompPsyker : ThingComp, IAbilityLearner
    {
        public List<AbilityDef> LearnedAbilities = new List<AbilityDef>();

        public PsykerDisciplineDef MainDiscipline = PsykerDisciplineDefOf.Initiate;

        public List<PsykerDisciplineDef> minorDisciplines = new List<PsykerDisciplineDef>();

        public Pawn Pawn => this.parent as Pawn;

        public PsykerClassDef psykerClassDef;

        private bool psykerInitialized;

        public bool ShouldAutoCast = true;

        public Trait PsykerPowerTrait
        {
            get
            {
                Trait psykerTrait = Pawn?.story?.traits.GetTrait(PsykerTraitDefOf.Psyker);
                if (psykerTrait != null)
                {
                    return psykerTrait;
                }
                return null;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!this.psykerInitialized)
            {
                InitializePsyker();
            }

            if (this.PsykerPowerTrait != null)
            {
                if (this.Pawn.psychicEntropy.CurrentPsyfocus < 0)
                {
                    this.Pawn.psychicEntropy.OffsetPsyfocusDirectly(this.Pawn.psychicEntropy.TargetPsyfocus);
                }
            }
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void CompTick()
        {
            base.CompTick();
        }

        protected virtual void InitializePsyker()
        {
            CorruptionPawnKindDef psykerKind = this.Pawn.kindDef as CorruptionPawnKindDef;
            if (psykerKind != null && psykerKind.affliction != null)
            {

                PsykerClassDef psykerClass = GetPsykerClass(psykerKind);
                if (psykerClass != null)
                {
                    this.psykerClassDef = psykerClass;
                    if (this.PsykerPowerTrait != null) this.Pawn.story.traits.allTraits.Remove(this.PsykerPowerTrait);
                    int degree = psykerClass.MinPsykerLevel;
                    this.Pawn.story.traits.GainTrait(new Trait(PsykerTraitDefOf.Psyker, degree));


                    foreach (var power in psykerClass.forcedPowers)
                    {
                        this.TryLearnAbility(power);
                    }

                    this.MainDiscipline = psykerClass.mainDiscipline;
                    if (this.MainDiscipline.practicionerTrait != null)
                        this.Pawn.story.traits.GainTrait(new Trait(psykerClass.mainDiscipline.practicionerTrait));

                    if (psykerClass.minorDisciplines.Count > 0)
                    {
                        this.minorDisciplines.Add(psykerClass.minorDisciplines[0]);
                    }

                    float availableXP = psykerClass.startingXP;

                    var availablePowers = MainDiscipline.abilities;
                    foreach (var discipline in this.minorDisciplines)
                    {
                        availablePowers.AddRange(discipline.abilities);
                    }
                    bool canSpend = true;
                    while (canSpend)
                    {
                        var learnables = availablePowers.Where(x => this.LearningRequirementsMet(x) && !this.HasLearnedAbility(x.ability) && x.cost <= availableXP);
                        if (learnables.Count() == 0)
                        {
                            canSpend = false;
                        }
                        else
                        {
                            var learnable = learnables.RandomElement();
                            if (this.TryLearnAbility(learnable))
                            {
                                availableXP -= learnable.cost;
                            }
                            else
                            {
                                canSpend = false;
                            }
                        }
                    }
                }
            }
        }

        protected static PsykerClassDef GetPsykerClass(CorruptionPawnKindDef psykerKind)
        {
            var availableClasses = DefDatabase<PsykerClassDef>.AllDefsListForReading.Where(x => x.tags.Intersect(psykerKind.additionalTags).Count() > 0);

            if (availableClasses.Count() > 0)
            {
                return availableClasses.RandomElementByWeight(x => x.commonality);
            }
            return null;
        }

        public float PsykerXP;

        public void AddXP(float amount)
        {
            int adjustedXP = (int)(amount);
            this.PsykerXP += Math.Abs(adjustedXP);
        }

        public bool HasLearnedAbility(AbilityDef def)
        {
            return this.LearnedAbilities.Contains(def);
        }

        public bool LearningRequirementsMet(LearnableAbility selectedPower)
        {
            return selectedPower.perequesiteAbility == null || this.LearnedAbilities.Contains(selectedPower.perequesiteAbility);
        }

        public bool TryLearnAbility(AbilityDef def)
        {
            if (this.LearnedAbilities.Contains(def))
            {
                return false;
            }
            this.LearnedAbilities.Add(def);
            this.Pawn.abilities.GainAbility(def);

            return true;
        }

        public bool TryLearnAbility(LearnableAbility learnablePower)
        {
            float previousXP = this.PsykerXP;
            this.PsykerXP = this.PsykerXP - learnablePower.cost;
            if (this.PsykerXP < 0)
            {
                this.PsykerXP = previousXP;
                if (this.Pawn.IsColonistPlayerControlled)
                {
                    Messages.Message("PsykerLearnXPShortage".Translate(), null, MessageTypeDefOf.RejectInput);
                }
                return false;
            }

            if (learnablePower.replacesPerequisite)
            {
                this.Pawn.abilities.RemoveAbility(learnablePower.perequesiteAbility);
            }

            return this.TryLearnAbility(learnablePower.ability);

        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if (this.PsykerPowerTrait != null)
            {
                Command_Action command = new Command_Action();
                command.defaultLabel = "PsykerPowerManagement".Translate();
                command.defaultDesc = "PsykerPowerManagementDescr".Translate();
                command.icon = PsykerUtility.PsykerIcon;
                command.action = delegate
                {
                    Find.WindowStack.Add(new Window_Psyker(this));
                };
                yield return command;
                                
                Command_Toggle autoCast = new Command_Toggle();
                autoCast.defaultLabel = "PsykerAutoCast".Translate();
                autoCast.defaultLabel = "PsykerAutoCastDesc".Translate();
                autoCast.icon = ContentFinder<Texture2D>.Get("UI/Commands/HoldFire");
                autoCast.toggleAction = delegate
                {
                    this.ShouldAutoCast = !this.ShouldAutoCast;
                };
                autoCast.isActive = (() => this.ShouldAutoCast);
                yield return autoCast;
            }

        }

        public override string CompInspectStringExtra()
        {
            var builder = new StringBuilder();
            if (this.psykerClassDef != null)
            {
                builder.AppendLine("PsykerClass".Translate(new NamedArgument(this.psykerClassDef.label, "NAME")));
            }
            if (this.PsykerPowerTrait != null)
            {
                string powerLevelLetter = PsykerUtility.PowerLevelLetters[PsykerPowerTrait.Degree];
                builder.AppendLine("PsykerLevelInfo".Translate(powerLevelLetter));
            }
            return builder.ToString().TrimEndNewlines();
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look<float>(ref this.PsykerXP, "psykerPX");
            Scribe_Collections.Look<AbilityDef>(ref this.LearnedAbilities, "learnedAbilities", LookMode.Def);
            Scribe_Collections.Look<PsykerDisciplineDef>(ref this.minorDisciplines, "minorDisciplines", LookMode.Def);
            Scribe_Defs.Look<PsykerDisciplineDef>(ref this.MainDiscipline, "chosenDiscipline");
            Scribe_Defs.Look<PsykerClassDef>(ref psykerClassDef, "psykerClassDef");
            Scribe_Values.Look<bool>(ref this.psykerInitialized, "psykerInitialized");
            Scribe_Values.Look<bool>(ref this.ShouldAutoCast, "ShouldAutoCast");
        }

    }
}

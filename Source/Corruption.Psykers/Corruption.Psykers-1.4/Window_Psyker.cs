using Corruption.Core;
using Corruption.Core.Abilities;
using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace Corruption.Psykers
{
    [StaticConstructorOnStartup]
    public class Window_Psyker : Window
    {
        private const float MINOR_DISCIPLINE_COST = 1000f;

        private PsykerDisciplineDef selectedDiscipline = PsykerDisciplineDefOf.Initiate;
        private LearnableAbility selectedPower;

        private CompPsyker comp;
        private int psykerPowerLevel;

        private static Vector2 descrScrollPos;

        private static Color PowerLimitColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);

        private static Texture2D LockedTex = SolidColorMaterials.NewSolidColorTexture(TexUI.LockedResearchColor);

        public Window_Psyker(CompPsyker comp)
        {
            this.comp = comp;
            UpdatePowers();
            this.closeOnClickedOutside = true;
            this.forcePause = true;
        }

        public void UpdatePowers()
        {
            this.psykerPowerLevel = comp.PsykerPowerTrait.Degree;
            this.selectedDiscipline = comp.MainDiscipline;
        }

        public override Vector2 InitialSize => new Vector2(1000f, 700f);

        public override void DoWindowContents(Rect inRect)
        {
            inRect = inRect.ContractedBy(17f);
            GUI.BeginGroup(inRect);
            Rect disciplineRect = new Rect(0f, 0f, 162f, inRect.height);
            Widgets.DrawMenuSection(disciplineRect);
            disciplineRect = disciplineRect.ContractedBy(17f);
            DrawDisciplineRect(disciplineRect);

            Rect infoRect = new Rect(168f, 0f, 520f, 148f);
            Widgets.DrawMenuSection(infoRect);
            infoRect = infoRect.ContractedBy(17f);
            DrawInfoRect(infoRect);

            Rect powersRect = new Rect(168f, 154f, 520f, disciplineRect.yMax - infoRect.yMax - 6f);

            Widgets.DrawBox(powersRect);
            powersRect = powersRect.ContractedBy(17f);
            DrawPowersRect(powersRect);

            Rect selectionRect = new Rect(powersRect.xMax + 23f, 0f, 220f, inRect.height);

            Widgets.DrawBox(selectionRect);
            Widgets.DrawMenuSection(selectionRect);
            selectionRect = selectionRect.ContractedBy(17f);
            Corruption.Core.Abilities.AbilityUI.DrawSelection(selectionRect, this.selectedPower);

            GUI.EndGroup();
            if (Widgets.CloseButtonFor(inRect.ExpandedBy(17f).AtZero()))
            {
                this.Close();
            }
        }

        //private void DrawSelection(Rect selectionRect)
        //{
        //    GUI.BeginGroup(selectionRect);
        //    Rect labelRect = new Rect(0f, 0f, selectionRect.width, 32f);
        //    Text.Font = GameFont.Medium;
        //    Widgets.Label(labelRect, "SelectedPower".Translate());
        //    Text.Font = GameFont.Small;

        //    Rect iconRect = new Rect(selectionRect.width / 2f - 37f, labelRect.yMax + 4f, 74f, 74f);

        //    if (this.selectedPower == null)
        //    {
        //        GUI.DrawTexture(iconRect, Corruption.Core.Abilities.AbilityUI.BGTex);
        //        GUI.DrawTexture(iconRect, PsykerDisciplineDefOf.Initiate.Icon);
        //    }
        //    else
        //    {
        //        GUI.DrawTexture(iconRect, Corruption.Core.Abilities.AbilityUI.BGTex);
        //        GUI.DrawTexture(iconRect, selectedPower.ability.uiIcon);

        //        Rect powerLabelRect = new Rect(0f, iconRect.yMax + 4f, selectionRect.width, 48f);
        //        Text.Anchor = TextAnchor.UpperCenter;
        //        Widgets.Label(powerLabelRect, selectedPower.ability.LabelCap);
        //        Text.Anchor = TextAnchor.UpperLeft;

        //        string description = selectedPower.ability.description;
        //        Rect descriptionRect = new Rect(0f, powerLabelRect.yMax + 8f, selectionRect.width, Text.CalcHeight(description, selectionRect.width)); ;

        //        Widgets.Label(descriptionRect, description);
        //        float curY = descriptionRect.yMax + 4f;
        //        Widgets.ListSeparator(ref curY, selectionRect.width, "TabStats".Translate());
        //        Rect factRect = new Rect(0f, curY, selectionRect.width, selectionRect.height - curY);

        //        string toolTip = selectedPower.ability.StatSummary.ToLineList();

        //        if (this.selectedPower.ability.targetRequired)
        //        {
        //            toolTip += "\n" + "Range".Translate() + ": " + this.selectedPower.ability.verbProperties.range.ToString("F0");
        //        }

        //        foreach (var directDamageComp in selectedPower.ability.comps.Where(x => x is CompProperties_DirectDamage).Cast<CompProperties_DirectDamage>())
        //        {
        //            toolTip += "\n" + "DirectDamageDescr".Translate(new NamedArgument(directDamageComp.damage, "AMOUNT"), new NamedArgument(directDamageComp.damageDef.label, "DAMAGEDEF"));
        //        }

        //        foreach (var severityComp in selectedPower.ability.comps.Where(x => x is CompProperties_AbilityGiveHediffSeverity).Cast<CompProperties_AbilityGiveHediffSeverity>())
        //        {
        //            var stage = severityComp.hediffDef.stages[0];
        //            if (severityComp is CompProperties_AbilityGiveHediffSeverity compSeverity)
        //            {
        //                stage = compSeverity.hediffDef.stages.FirstOrDefault(x => x.minSeverity >= compSeverity.severity);
        //            }
        //            foreach (var stat in stage.SpecialDisplayStats())
        //            {
        //                toolTip += "\n" + stat.LabelCap + ": " + stat.ValueString;
        //            }
        //            foreach (var cap in stage.capMods)
        //            {
        //                toolTip += "\n" + cap.capacity.LabelCap + ":" + cap.offset;
        //            }
        //        }

        //        foreach (var conflictingPower in this.selectedPower.conflictsWith)
        //        {
        //            toolTip += "\n" + "LearntExcludingPower".Translate(new NamedArgument(conflictingPower.label, "ABILITY"));
        //        }

        //        if (selectedPower.ability.comps.Any(x => x.compClass == typeof(CompAbilityEffect_PsyProjectile)))
        //        {
        //            var projectile = selectedPower.ability.verbProperties.defaultProjectile.projectile;
        //            int maxDamage = projectile.GetDamageAmount(1f) * selectedPower.ability.verbProperties.burstShotCount;
        //            toolTip += "\n" + "ProjectileDamageDescr".Translate(new NamedArgument(maxDamage, "AMOUNT"), new NamedArgument(projectile.damageDef.label, "DAMAGEDEF"));
        //            if (projectile.explosionRadius > 0f) toolTip += "\n" + "ProjectileAoERadius".Translate(new NamedArgument(projectile.explosionRadius, "RADIUS"));
        //        }

        //        Widgets.TextAreaScrollable(factRect, toolTip, ref descrScrollPos, true);

        //        toolTip += "\n\n" + "LearnPowerCost".Translate(new NamedArgument((int)(this.selectedPower.cost), "COST"));
        //    }

        //    GUI.EndGroup();
        //}

        private void DrawPowersRect(Rect powersRect)
        {
            GUI.BeginGroup(powersRect);

            Widgets.DrawLineVertical(16f, 16f, 50f * PsykerTraitDefOf.Psyker.degreeDatas.Count);

            DrawPowerLevels(powersRect);


            foreach (var learnablePower in this.selectedDiscipline.abilities)
            {
                float x = 64f + 64 * (learnablePower.position.x - 1);
                float y = powerLevelYLookup[learnablePower.ability.level] - 16f;
                Rect abilityRect = new Rect(x, y, 32f, 32f);
                Color borderColor = PsykerUtility.AbilityLockedBorder;
                Color bgColor = PsykerUtility.AbilityLockedBG;
                int borderSize = 1;
                bool hasLearnedAbility = comp.HasLearnedAbility(learnablePower.ability);
                if (this.selectedPower == learnablePower)
                {
                    bgColor = PsykerUtility.AbilitySelectedBG;
                    borderColor = PsykerUtility.AbilityUnlockedBorder;
                    borderSize = 2;
                }
                else if (hasLearnedAbility)
                {
                    borderColor = PsykerUtility.AbilityUnlockedBorder;
                    bgColor = PsykerUtility.AbilityUnlockedBG;
                    borderSize = 2;
                }
                else if (!hasLearnedAbility && learnablePower.ability == this.selectedPower?.perequesiteAbility)
                {
                    borderColor = PsykerUtility.AbilityPerequisiteBorder;
                }
                if (Widgets.CustomButtonText(ref abilityRect, "", bgColor, Color.white, borderColor, false, borderSize))
                {
                    this.selectedPower = learnablePower;
                }
                Material material = learnablePower.ability.level > psykerPowerLevel ? TexUI.GrayscaleGUI : null;
                GenUI.DrawTextureWithMaterial(abilityRect, learnablePower.ability.uiIcon, material);
                Widgets.DrawHighlightIfMouseover(abilityRect);

                TooltipHandler.TipRegion(abilityRect, new TipSignal(learnablePower.ability.LabelCap, learnablePower.ability.GetHashCode()));

                if (learnablePower.perequesiteAbility != null)
                {
                    var perequisite = this.selectedDiscipline.abilities.FirstOrDefault(a => a.ability == learnablePower.perequesiteAbility);
                    if (perequisite != null)
                    {
                        float xPer = 64f + 64 * (perequisite.position.x - 1);
                        float yPer = powerLevelYLookup[perequisite.ability.level];
                        var start = new Vector2(xPer + 16f, yPer - 16f);

                        var end = new Vector2(x + 16f, y + 32f);
                        Widgets.DrawLine(start, end, Color.white, 1f);
                    }
                }
            }

            if (this.selectedPower != null)
            {
                DrawSelectedPower(powersRect);
            }
            GUI.EndGroup();
        }

        private void DrawSelectedPower(Rect powersRect)
        {
            Rect costRect = new Rect(64f, 318f, powersRect.width - 64f, Text.LineHeight);
            Widgets.Label(costRect, "LearnPowerCost".Translate(new NamedArgument((int)(this.selectedPower.cost), "COST")));
            Rect buttonRect = new Rect(64f, costRect.yMax + 4f, 196f, 32f);

            if (this.selectedPower.ability.level > this.comp.PsykerPowerTrait.Degree)
            {
                Widgets.CustomButtonText(ref buttonRect, "LearnPower".Translate(), TexUI.LockedResearchColor, TexUI.HighlightBorderResearchColor, TexUI.DefaultLineResearchColor);
                TooltipHandler.TipRegion(buttonRect, new TipSignal("PsykerLearnAboveLevel".Translate()));
            }
            else if (this.selectedPower.cost > this.comp.PsykerXP)
            {
                Widgets.CustomButtonText(ref buttonRect, "LearnPower".Translate(), TexUI.LockedResearchColor, TexUI.HighlightBorderResearchColor, TexUI.DefaultLineResearchColor);
                TooltipHandler.TipRegion(buttonRect, new TipSignal("PsykerLearnXPShortage".Translate()));
            }
            else if (this.comp.HasLearnedAbility(selectedPower.ability))
            {
                Widgets.CustomButtonText(ref buttonRect, "LearntPower".Translate(), TexUI.LockedResearchColor, TexUI.HighlightBorderResearchColor, TexUI.HighlightBorderResearchColor);
            }
            else if (this.selectedPower.conflictsWith.Any(x => this.comp.HasLearnedAbility(x)))
            {
                Widgets.CustomButtonText(ref buttonRect, "LearntExcludingPower".Translate(new NamedArgument(this.selectedPower.conflictsWith.FirstOrDefault(x => this.comp.HasLearnedAbility(x)).label, "ABILITY")), TexUI.LockedResearchColor, TexUI.HighlightBorderResearchColor, TexUI.HighlightBorderResearchColor);
            }
            else if (this.comp.Pawn.WorkTagIsDisabled(WorkTags.Violent) && this.selectedPower.ability.comps.Any(x => x.compClass == typeof(CompAbilityEffect_PsyProjectile) || x.compClass == typeof(CompAbilityEffect_DirectDamage)))
            {
                Widgets.CustomButtonText(ref buttonRect, "CannotLearnViolentPower".Translate(), TexUI.LockedResearchColor, TexUI.HighlightBorderResearchColor, TexUI.HighlightBorderResearchColor);
            }
            else if (!this.comp.LearningRequirementsMet(selectedPower))
            {
                Widgets.CustomButtonText(ref buttonRect, "PowerPerequisitesMissing".Translate(), TexUI.LockedResearchColor, TexUI.HighlightBorderResearchColor, TexUI.HighlightBorderResearchColor);
            }
            else
            {
                if (Widgets.ButtonText(buttonRect, "LearnPower".Translate()))
                {
                    if (this.comp.TryLearnAbility(this.selectedPower))
                    {
                        CoreSoundDefOf.Corruption_UnlockMinor.PlayOneShotOnCamera();
                    }
                }
            }

            Rect debugRect = new Rect(buttonRect);
            debugRect.x = buttonRect.xMax + 8f;
            if (Prefs.DevMode)
            {
                if (Widgets.ButtonText(debugRect, "Debug: +100XP"))
                {
                    this.comp.AddXP(100f);
                }
            }
        }

        private void DrawPowerLevels(Rect inRect)
        {
            float offset = 50f;
            float lineY = 25f;

            Rect powerLimitRect = new Rect(0f, 0f, inRect.width - 4f, (PsykerTraitDefOf.Psyker.degreeDatas.Max(x => x.degree) - this.comp.PsykerPowerTrait.Degree) / 10 * offset);
            Widgets.DrawBoxSolid(powerLimitRect, PowerLimitColor);

            Rect barRect = new Rect(0f, 0f, 48f, offset * PsykerTraitDefOf.Psyker.degreeDatas.Count);
            float curDegree = comp.PsykerPowerTrait.Degree;
            float maxDegree = PsykerTraitDefOf.Psyker.degreeDatas.Max(x => x.degree);
            WidgetExtensions.FillableVerticalBar(barRect, curDegree / maxDegree, selectedDiscipline.MainTex, LockedTex, true);

            var levelRect = new Rect(8f, 9f, 32f, 32f);
            Widgets.DrawLineHorizontal(12f, lineY, inRect.width - 16f);
            GUI.DrawTexture(levelRect, PsykerUtility.PowerLevelBeta);
            TooltipHandler.TipRegion(levelRect, new TipSignal("Beta"));

            levelRect.y += offset;
            lineY += offset;
            Widgets.DrawLineHorizontal(12f, lineY, inRect.width - 16f);
            GUI.DrawTexture(levelRect, PsykerUtility.PowerLevelDelta);
            TooltipHandler.TipRegion(levelRect, new TipSignal("Delta"));

            levelRect.y += offset;
            lineY += offset;
            Widgets.DrawLineHorizontal(12f, lineY, inRect.width - 16f);
            GUI.DrawTexture(levelRect, PsykerUtility.PowerLevelEpsilon);
            TooltipHandler.TipRegion(levelRect, new TipSignal("Epsilon"));

            levelRect.y += offset;
            lineY += offset;
            Widgets.DrawLineHorizontal(12f, lineY, inRect.width - 16f);
            GUI.DrawTexture(levelRect, PsykerUtility.PowerLevelZeta);
            TooltipHandler.TipRegion(levelRect, new TipSignal("Zeta"));

            levelRect.y += offset;
            lineY += offset;
            Widgets.DrawLineHorizontal(12f, lineY, inRect.width - 16f);
            GUI.DrawTexture(levelRect, PsykerUtility.PowerLevelIota);
            TooltipHandler.TipRegion(levelRect, new TipSignal("Iota"));

            levelRect.y += offset;
            lineY += offset;
            Widgets.DrawLineHorizontal(12f, lineY, inRect.width - 16f);
            GUI.DrawTexture(levelRect, PsykerUtility.PowerLevelKappa);
            TooltipHandler.TipRegion(levelRect, new TipSignal("Kappa"));

            Rect upgradePsykerLevelRect = new Rect(0f, levelRect.yMax + 1f, 48f, 48f);

            float upgradeCost = PsykerUtility.PowerLevelCostCurve.Evaluate(curDegree + 10);
            if (comp.PsykerXP < upgradeCost)
            {
                if (Widgets.ButtonTextSubtle(upgradePsykerLevelRect, "+", comp.PsykerXP / upgradeCost, upgradePsykerLevelRect.width / 2f - 2f))
                {
                    Messages.Message("PsykerLearnXPShortage".Translate(), MessageTypeDefOf.RejectInput, false);
                }
            }
            else if (Widgets.ButtonText(upgradePsykerLevelRect, "+", true, true, comp.PsykerXP >= PsykerUtility.PowerLevelCostCurve.Evaluate(curDegree + 10) && curDegree < maxDegree))
            {
                int prevDegree = comp.PsykerPowerTrait.Degree;
                comp.Pawn.story.traits.allTraits.Remove(comp.PsykerPowerTrait);
                comp.Pawn.story.traits.GainTrait(new Trait(PsykerTraitDefOf.Psyker, prevDegree + 10, true));
                comp.PsykerXP -= upgradeCost;
                CoreSoundDefOf.Corruption_UnlockMajor.PlayOneShotOnCamera();
            }

            TooltipHandler.TipRegion(upgradePsykerLevelRect, new TipSignal("UpgradePsykerLevel".Translate(upgradeCost)));



        }

        private Dictionary<int, float> powerLevelYLookup = new Dictionary<int, float>()
        {
            {60, 25f },
            {50, 75f },
            {40, 125f },
            {30, 175f },
            {20, 225f },
            {10, 275f }
        };

        private void DrawInfoRect(Rect infoRect)
        {
            GUI.BeginGroup(infoRect);

            GUI.color = comp.MainDiscipline.color;
            Text.Font = GameFont.Medium;
            Rect disciplineRect = new Rect(0f, 0f, infoRect.width, Text.LineHeight);
            Widgets.Label(disciplineRect, comp.MainDiscipline.practitionerTitle.CapitalizeFirst());
            GUI.color = Color.white;
            Text.Font = GameFont.Small;

            if (comp.minorDisciplines != null && comp.minorDisciplines.Any(x => x != PsykerDisciplineDefOf.Initiate))
            {
                Rect minorTitleRect = new Rect(0f, disciplineRect.yMax + 4f, infoRect.width, Text.LineHeight);
                Widgets.Label(minorTitleRect, "MinorDisciplineInfo".Translate(comp.minorDisciplines[0].practitionerTitle.CapitalizeFirst()));
            }

            Rect psykerLevelRect = new Rect(0f, disciplineRect.yMax + 24f, infoRect.width, Text.LineHeight);

            string powerLevelLetter = PsykerUtility.PowerLevelLetters[comp.PsykerPowerTrait.Degree];
            Widgets.Label(psykerLevelRect, "PsykerLevelInfo".Translate(powerLevelLetter));

            Rect psykerXPRect = new Rect(psykerLevelRect);
            psykerXPRect.y = psykerXPRect.yMax + 4f;

            Widgets.Label(psykerXPRect, "PsykerXPInfo".Translate(new NamedArgument((int)this.comp.PsykerXP, "XP")));

            GUI.EndGroup();
        }

        private void DrawDisciplineRect(Rect disciplineRect)
        {
            GUI.BeginGroup(disciplineRect);
            Rect labelRect = new Rect(0f, 0f, disciplineRect.width, 32f);
            Text.Font = GameFont.Medium;
            Widgets.Label(labelRect, "PsykerDisciplines".Translate());
            Text.Font = GameFont.Small;

            Rect disciplineIconRect = new Rect(disciplineRect.width / 2f - 64f, labelRect.yMax + 4f, 128f, 128f);
            GUI.DrawTexture(disciplineIconRect, AbilityUI.BGTex);
            GUI.DrawTexture(disciplineIconRect, comp.MainDiscipline.Icon);

            Widgets.DrawHighlightIfMouseover(disciplineIconRect);


            if (comp.MainDiscipline == PsykerDisciplineDefOf.Initiate)
            {
                if (Widgets.ButtonInvisible(disciplineIconRect))
                {
                    Find.WindowStack.Add(new Window_PsykerDiscipline(this.comp, this));
                }
            }
            else
            {
                if (Widgets.ButtonInvisible(disciplineIconRect))
                {
                    this.selectedDiscipline = comp.MainDiscipline;
                    this.selectedPower = null;
                }
                TooltipHandler.TipRegion(disciplineIconRect, new TipSignal(comp.MainDiscipline.description));
            }

            Rect rect2 = new Rect(0f, disciplineIconRect.yMax + 8f, disciplineRect.width, Text.LineHeight);
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(rect2, "MinorDisciplines".Translate());
            Text.Anchor = TextAnchor.UpperLeft;


            Rect minorPowerRect = new Rect(39f, rect2.yMax + 4f, 64f, 64f);
            if (comp.minorDisciplines.NullOrEmpty())
            {
                TooltipHandler.TipRegion(minorPowerRect, new TipSignal("UnlockMinorDisciplineSlot".Translate(new NamedArgument(MINOR_DISCIPLINE_COST, "COST"))));
                if (Widgets.ButtonText(minorPowerRect, "+"))
                {
                    if (comp.PsykerXP < MINOR_DISCIPLINE_COST)
                    {
                        Messages.Message("PsykerLearnXPShortage".Translate(), null, MessageTypeDefOf.RejectInput);
                    }
                    else
                    {
                        this.comp.PsykerXP -= MINOR_DISCIPLINE_COST;
                        this.comp.minorDisciplines.Add(PsykerDisciplineDefOf.Initiate);
                    }
                }
            }
            else
            {

                GUI.DrawTexture(minorPowerRect, AbilityUI.BGTex);
                GUI.DrawTexture(minorPowerRect, comp.minorDisciplines[0].Icon);
                if (this.selectedDiscipline == comp.minorDisciplines[0])
                {
                    Widgets.DrawHighlight(minorPowerRect);
                }
                else
                {
                    Widgets.DrawHighlightIfMouseover(minorPowerRect);
                }

                if (Widgets.ButtonInvisible(minorPowerRect))
                {
                    if (comp.MainDiscipline == PsykerDisciplineDefOf.Initiate)
                    {
                        Messages.Message("MinorDisciplineWithoutMajorDiscipline".Translate(), null, MessageTypeDefOf.RejectInput);
                    }
                    else if (comp.minorDisciplines[0] == PsykerDisciplineDefOf.Initiate)
                    {
                        Find.WindowStack.Add(new Window_PsykerDisciplineMinor(this.comp, this));
                    }
                    else
                    {
                        this.selectedDiscipline = comp.minorDisciplines[0];
                        this.selectedPower = null;
                        TooltipHandler.TipRegion(disciplineIconRect, new TipSignal(comp.minorDisciplines[0].description));
                    }
                }
            }


            //Rect eqipmentTitleRect = new Rect(0f, minorPowerRect.yMax + 16f, disciplineRect.width, Text.LineHeight);
            //Widgets.Label(eqipmentTitleRect, "PsykerEquipment".Translate());

            GUI.EndGroup();
        }

        public override void PostClose()
        {
            base.PostClose();
        }
    }
}

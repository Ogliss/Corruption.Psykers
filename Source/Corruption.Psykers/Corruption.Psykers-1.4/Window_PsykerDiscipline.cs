using Corruption.Core.Soul;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;
using RimWorld.IO;
using System.Linq.Expressions;
using RimWorld.QuestGen;
using Corruption.Core.Abilities;

namespace Corruption.Psykers
{
    public class Window_PsykerDiscipline : Window
    {
        protected PsykerDisciplineDef selectedDef;

        protected List<PsykerDisciplineDef> psykerDisciplines = new List<PsykerDisciplineDef>();

        protected CompPsyker comp;

        protected Window_Psyker parentWindow;

        protected virtual string Title => "MainDiscipline".Translate();

        protected virtual PsykerDisciplineCategory Category => PsykerDisciplineCategory.Main;

        private static Vector2 descrScrollPos = Vector2.zero;

        public override Vector2 InitialSize => new Vector2(800f, 700f);

        public Window_PsykerDiscipline(CompPsyker comp, Window_Psyker windowPsyker)
        {
            this.closeOnClickedOutside = true;

            this.comp = comp;
            this.parentWindow = windowPsyker;


            this.psykerDisciplines = this.LoadDisciplines();
            this.selectedDef = psykerDisciplines.Contains(comp.MainDiscipline) ? comp.MainDiscipline : psykerDisciplines.FirstOrDefault();
        }

        protected List<PsykerDisciplineDef> LoadDisciplines()
        {
            var pawnPantheon = comp.Pawn.Soul().ChosenPantheon;
            var disciplines = DefDatabase<Corruption.Psykers.PsykerDisciplineDef>.AllDefsListForReading.FindAll(x => x.associatedPantheons.NullOrEmpty() == false && x != PsykerDisciplineDefOf.Initiate);
            return disciplines.FindAll(x => x.associatedPantheons.Any(p => pawnPantheon == p) && x.category == this.Category);
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect disciplinesRect = inRect.ContractedBy(17f);
            GUI.BeginGroup(inRect);
            disciplinesRect.height = inRect.height - 128f;

            GUI.BeginGroup(disciplinesRect);

            var vector = new Vector2(disciplinesRect.width / 2f, disciplinesRect.height / 2f);

            if (psykerDisciplines.Count > 1)
            {
                Vector2 initialVector = new Vector2(0f, disciplinesRect.height / 2f - 64f);

                float radialOffset = 360f / psykerDisciplines.Count;

                for (int i = 0; i < psykerDisciplines.Count; i++)
                {
                    var outerPosition = initialVector.RotatedBy(radialOffset * i) + vector;
                    DrawDiscipline(psykerDisciplines[i], outerPosition);
                }
            }

            if (selectedDef != null)
            {
                Rect selectedRect = new Rect(disciplinesRect.width / 2f - 64f, disciplinesRect.height / 2f - 64f, 128f, 128f);
                DrawSelectedDiscipline(selectedRect);
            }
            GUI.EndGroup();


            Rect confirmRect = new Rect(inRect.width / 2f - 132f, disciplinesRect.yMax + 32f, 128f, 56f);
            if (this.selectedDef.abilities.Min(x => x.ability.level > this.comp.PsykerPowerTrait?.Degree))
            {
                Widgets.CustomButtonText(ref confirmRect, "DisciplineLevelTooHigh".Translate(), TexUI.LockedResearchColor, TexUI.HighlightBorderResearchColor, TexUI.HighlightBorderResearchColor);
                
            }
            else if (Widgets.ButtonText(confirmRect, "ChooseDiscipline".Translate(), true, true, this.comp.MainDiscipline != this.selectedDef))
            {
                this.ChoosePower();
            }
            Rect cancelRect = new Rect(confirmRect.xMax + 8f, disciplinesRect.yMax + 32f, 128f, 56f);
            if (Widgets.ButtonText(cancelRect, "Cancel".Translate()))
            {
                this.Close();
            }
            GUI.EndGroup();


            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperCenter;
            Rect titleRect = new Rect(0f, 8f, inRect.width, Text.LineHeight);
            Widgets.Label(titleRect, this.Title);
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
        }

        private void DrawSelectedDiscipline(Rect inRect)
        {
            Rect iconRect = new Rect(inRect.x, inRect.y, 128f, 128f);
            GUI.DrawTexture(iconRect, AbilityUI.BGTex);
            GUI.DrawTexture(iconRect, this.selectedDef.Icon);
            Rect descriptionRect = new Rect(iconRect.x - 38f, iconRect.yMax + 8f, iconRect.width + 76f, Text.LineHeight * 4f);
            Widgets.DrawBox(descriptionRect);
            descriptionRect = descriptionRect.ContractedBy(2f);
            Widgets.TextAreaScrollable(descriptionRect, selectedDef.description, ref descrScrollPos, true);
        }

        private void DrawDiscipline(PsykerDisciplineDef def, Vector2 vector)
        {
            Rect rect = new Rect(vector.x - 32f, vector.y - 32f, 64f, 64f);
            Color bgColor = this.selectedDef == def ? PsykerUtility.AbilitySelectedBG : PsykerUtility.AbilityLockedBG;
            Color fColor = this.selectedDef == def ? PsykerUtility.AbilityUnlockedBorder : PsykerUtility.AbilityLockedBorder;
            if (Widgets.CustomButtonText(ref rect, "", bgColor, Color.white, fColor))
            {
                this.selectedDef = def;
            }
            GUI.DrawTexture(rect, def.Icon);
            Widgets.DrawHighlightIfMouseover(rect);
            TooltipHandler.TipRegion(rect, new TipSignal(def.label));

            Rect titleRect = new Rect(rect.x - 32f, rect.yMax + 4f, rect.width + 64f, Text.LineHeight * 2f);
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(titleRect, def.LabelCap);
            Text.Anchor = TextAnchor.UpperLeft;
        }

        protected virtual void ChoosePower()
        {
            comp.MainDiscipline = this.selectedDef;
            if (this.selectedDef.practitionerHediff != null)
            {
                comp.Pawn.health.AddHediff(this.selectedDef.practitionerHediff);
            }
            if (this.selectedDef.practicionerTrait != null)
            {
                var trait = new Trait(this.selectedDef.practicionerTrait);
                comp.Pawn.story.traits.GainTrait(trait);
            }
            this.comp.AddXP(this.selectedDef.initialXP);
            this.parentWindow.UpdatePowers();
            this.Close();
        }
    }

    public class Window_PsykerDisciplineMinor : Window_PsykerDiscipline
    {

        protected override string Title => "MinorDiscipline".Translate();

        protected override PsykerDisciplineCategory Category => PsykerDisciplineCategory.Minor;

        public Window_PsykerDisciplineMinor(CompPsyker comp, Window_Psyker windowPsyker) : base(comp, windowPsyker)
        {
        }

        protected override void ChoosePower()
        {
            this.comp.minorDisciplines[0] = this.selectedDef;
            this.parentWindow.UpdatePowers();
            this.Close();
        }
    }
}

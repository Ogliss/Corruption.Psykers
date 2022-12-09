using Corruption.Core.Abilities;
using Corruption.Core.Gods;
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
    public class PsykerDisciplineDef : Def
    {
        public List<PantheonDef> associatedPantheons = new List<PantheonDef>();

        public List<LearnableAbility> abilities;

        public TraitDef practicionerTrait;

        public HediffDef practitionerHediff;

        public PsykerDisciplineCategory category;

        public float initialXP = 200f;

        public string practitionerTitle;

        public Color color = Color.gray;

        public string iconPath;

        public Texture2D Icon = BaseContent.BadTex;

        public Texture2D MainTex { get; private set; }

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                this.Icon = ContentFinder<Texture2D>.Get(this.iconPath);
                this.MainTex = SolidColorMaterials.NewSolidColorTexture(this.color);
            });
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }
            if (this.initialXP < this.abilities.Min(x => x.cost))
            {
                this.initialXP = this.abilities.Min(x => x.cost);
                yield return $"PsykerDiscipline {this.defName} has an initialXP below its cheapest ability.";
            }
        }
    }

    public class PsykerLearnablePower
    {
        public AbilityDef ability;

        public AbilityDef perequesiteAbility;

        public bool replacesPerequisite = false;

        public List<AbilityDef> conflictsWith = new List<AbilityDef>();

        public float cost;

        public Vector2 position = Vector2.zero;

    }

}

using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class CompImbuedAbility : ThingComp
    {
        public List<AbilityDef> ImbuedAbilities = new List<AbilityDef>();

        public CompProperties_ImbuedAbility Props
        {
            get
            {
                return this.props as CompProperties_ImbuedAbility;
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.ImbuedAbilities.AddRange(this.Props.initialAbilities);
        }

        public void Imbue(AbilityDef def)
        {
            if (this.Props.maximumSlots > this.ImbuedAbilities.Count && this.ImbuedAbilities.Contains(def) ==false)
            {
                this.ImbuedAbilities.Add(def);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look<AbilityDef>(ref this.ImbuedAbilities, "imbuedAbilities", LookMode.Def);
        }
    }


    public class CompProperties_ImbuedAbility : CompProperties
    {
        public int maximumSlots = 1;
        public List<AbilityDef> initialAbilities = new List<AbilityDef>();
    }
}

using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class PsykerClassDef : Def
    {
        public PsykerDisciplineDef mainDiscipline;

        public List<PsykerDisciplineDef> minorDisciplines = new List<PsykerDisciplineDef>();

        public List<AbilityDef> forcedPowers = new List<AbilityDef>();

        public List<string> tags = new List<string>();

        public float startingXP = 600f;

        private int minPsykerLevel = -1;

        public int MinPsykerLevel
        {
            get
            {
                if (this.minPsykerLevel > 0)
                {
                    return this.minPsykerLevel;
                }
                return forcedPowers.Max(x => x.level);
            }
        }

        public float commonality = 1f;

        public override IEnumerable<string> ConfigErrors()
        {
            foreach(string error in base.ConfigErrors())
            {
                yield return error;
            }
            if (this.minPsykerLevel == -1 && this.forcedPowers.Count == 0)
            {
                this.minPsykerLevel = 10;
                yield return $"PsykerClassDef {this.defName} has no forcedPowers and no minPsykerLevel. Defaulting.";
            }
        }

        public override void PostLoad()
        {
            base.PostLoad();
            if (this.mainDiscipline == null)
            {
                LongEventHandler.ExecuteWhenFinished(delegate
                {
                    this.mainDiscipline = PsykerDisciplineDefOf.Initiate;
                });
            }
        }
    }
}

using Corruption.Core;
using Corruption.Core.Gods;
using Corruption.Core.Soul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class PsykerStoryTrackerComponent : CorruptionStoryTrackerComponent
    {
        public override void Initialize()
        {
            var settings = LoadedModManager.GetMod<CorruptionMod>().GetSettings<ModSettings_Corruption>();
            foreach (var def in DefDatabase<ThingDef>.AllDefs.Where(x => x.race != null && x.race.Humanlike))
            {
                if(def.comps.Any(x => x.compClass == typeof(CompSoul)) && !def.comps.Any(x => x.compClass == typeof(CompPsyker)))
                {
                    var psyComp = new CompProperties_Psyker();
                    def.comps.Add(psyComp);
                    def.ResolveReferences();
                }
            }
        }
    }
}

using Corruption.Core.Soul;
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
    public class ScenPart_PsykerStart : ScenPart
    {
        protected int minPsykerCount;

        private string countBuf;

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            //base.DoEditInterface(listing);
            Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 1f);
            Rect rect3 = new Rect(scenPartRect.x, scenPartRect.y, scenPartRect.width, scenPartRect.height);
            Widgets.TextFieldNumeric(rect3, ref minPsykerCount, ref countBuf, 0f);
        }

        public override void PostGameStart()
        {
            base.PostGameStart();
            for (int i = 0; i < this.minPsykerCount; i++)
            {
                Pawn pawn = Find.ColonistBar.GetColonistsInOrder().Where( x=> !x.story.traits.HasTrait(PsykerTraitDefOf.Psyker)).RandomElement();
                if (pawn != null && !pawn.story.traits.HasTrait(PsykerTraitDefOf.Psyker))
                {
                    pawn.story.traits.GainTrait(new Trait(PsykerTraitDefOf.Psyker, PsykerTraitDefOf.Psyker.degreeDatas.RandomElementByWeight(x => 60f - x.degree).degree, true));
                }
            }
        }
    }
}

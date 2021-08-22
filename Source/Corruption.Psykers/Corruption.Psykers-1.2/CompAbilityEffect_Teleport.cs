using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Corruption.Psykers
{
    public class CompAbilityEffect_Teleport : CompAbilityEffect
    {

        public new CompProperties_Teleport Props => this.props as CompProperties_Teleport;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Map map = this.parent.pawn.Map;
            Pawn pawn = this.parent.pawn;
            CreatePortalFor(pawn);
            pawn.DeSpawn();
            GenSpawn.Spawn(pawn, target.Cell, map);
            this.parent.pawn.Position = target.Cell;
            CreatePortalFor(pawn);
            if (pawn.IsColonist)
            {
                pawn.drafter.Drafted = true;
            }
        }

        private void CreatePortalFor(Pawn pawn)
        {
            if (pawn != null && pawn.Spawned)
            {
                Map map = pawn.Map;
                var loc = pawn.DrawPos;
                if (map != null)
                {
                    MoteMaker.ThrowSmoke(loc, map, 1.5f);
                    MoteMaker.ThrowMicroSparks(loc, map);
                    MoteMaker.ThrowLightningGlow(loc, map, 1.0f);
                    var mote = MoteMaker.MakeStaticMote(loc, map, this.Props.mote) as MoteThrown;
                    if (mote != null)
                    {
                        mote.rotationRate = 180f;
                        SoundInfo info = SoundInfo.InMap(new TargetInfo(loc.ToIntVec3(), map));

                        if (this.Props.sound != null)
                        {
                            this.Props.sound.PlayOneShot(info);
                        }
                    }
                }
            }

        }
    }

    public class CompProperties_Teleport : CompProperties_AbilityEffect
    {
        public CompProperties_Teleport()
        {
            this.compClass = typeof(CompAbilityEffect_Teleport);
        }

        public ThingDef mote;

        public SoundDef sound;
    }
}

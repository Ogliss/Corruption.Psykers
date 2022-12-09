using Corruption.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Psykers
{
    public class Graphic_Vortex : Graphic_Collection
    {
		private const int BaseTicksPerFrameChange = 15;

		private const int ExtraTicksPerFrameChange = 10;

		private const float MaxOffset = 0.05f;

		public override Material MatSingle => subGraphics[Rand.Range(0, subGraphics.Length)].MatSingle;

		public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
		{
			if (thingDef == null)
			{
				Log.ErrorOnce("Vortex DrawWorker with null thingDef: " + loc, 3427324);
				return;
			}
			if (subGraphics == null)
			{
				Log.ErrorOnce("Graphic_Flicker has no subgraphics " + thingDef, 358773632);
				return;
			}
			int num = Find.TickManager.TicksGame;
			if (thing != null)
			{
				num += Mathf.Abs(thing.thingIDNumber ^ 0x80FD52);
			}
			int num2 = num / 15;
			int num3 = Mathf.Abs(num2 ^ ((thing?.thingIDNumber ?? 0) * 391)) % subGraphics.Length;
			float num4 = 1f;
			Vortex vortex = thing as Vortex;
			if (vortex != null)
			{
				num4 = vortex.Radius * 2f;
			}
			if (num3 < 0 || num3 >= subGraphics.Length)
			{
				Log.ErrorOnce("Vortex drawing out of range: " + num3, 7453435);
				num3 = 0;
			}
			Graphic graphic = subGraphics[num3];	
			Vector3 pos = thing.DrawPos;
			Vector3 s = new Vector3(num4, 1f, num4);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(pos, Quaternion.identity, s);
			Graphics.DrawMesh(MeshPool.plane10, matrix, graphic.MatSingle, 0);
		}
	}
}

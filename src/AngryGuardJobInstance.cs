using Pipliz;
using Jobs;
using UnityEngine;

namespace AngryGuards {

	public class AngryGuardJobInstance: BlockJobInstance
	{
		public Players.Player target;
		public Vector3 eyePosition;

		public AngryGuardJobInstance(IBlockJobSettings settings, Pipliz.Vector3Int position, ItemTypes.ItemType type, ByteReader reader): base(settings, position, type, reader)
		{
			this.eyePosition = position.Vector;
			this.eyePosition[1] += 1;
		}

		public AngryGuardJobInstance(IBlockJobSettings settings, Pipliz.Vector3Int position, ItemTypes.ItemType type, Colony colony): base (settings, position, type, colony)
		{
			this.eyePosition = position.Vector;
			this.eyePosition[1] += 1;
		}
	}

}


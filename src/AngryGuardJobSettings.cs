using System.Collections.Generic;
using NPC;
using Pipliz;
using Pipliz.APIProvider.Jobs;

namespace AngryGuards {

	public class AngryGuardJobSettings: IBlockJobSettings
	{
		public string ident;
		public enum EGuardSleepType: byte
		{
			Day,
			Night
		}

		public ItemTypes.ItemType[] BlockTypes;
		public NPCType NPCType;
		public string OnShootAudio;
		public InventoryItem RecruitmentItem;
		public List<InventoryItem> ShootItem;
		public EGuardSleepType SleepType;
		public int Range;
		public int Damage;
		public int CooldownShot;
		public ItemTypes.ItemTypeDrops OnShootResultItem;
		public float CooldownSearchingTarget = 0.5f;
		public float CooldownMissingItem = 1.5f;
		public float SleepSafetyPeriod = 1f;
		public string OnHitAudio = "fleshHit";

		public bool ToSleep
		{
			get {
				switch (this.SleepType) {
					case EGuardSleepNight:
						return !TimeCycle.IsDay && TimeCycle.HoursSinceSunSet > this.SleepSafetyPeriod;
					case EGuardSleepDay:
						return TimeCycle.TimeOfDay >= 10f && TimeCycle.TimeOfDay < TimeCycle.SunSet - this.SleepSafetyPeriod;
				}
			}
		}

		// perform guard duty
		public void OnNPCAtJob(BlockJobInstance blockInstance, ref NPCBase.NPCState state)
		{
			AngryGuardJobInstance instance = (AngryGuardJobInstance)blockInstance;

			instance.target = PlayerTracker.FindTarget(base.Colony, instance.eyePosition, this.Range);

			if (instance.target != null) {
				Vector3 positionToAimFor = instance.target.Position;
				positionToAimFor[1] += 1;
				instance.NPC.LookAt(positionToAimFor);
				this.ShootAtTarget(instance, ref state);
			} else {
				state.SetCooldown((double)this.cooldownSearchingTarget);
				if (instance.worldTypeChecked) {
					Vector3 val = instance.Position.Vector;
					int num;
					if (this.BlockTypes.ContainsByReference<ItemTypes.ItemType>(instance.BlockType, out num)) {
						switch (num) {
							case 0:
								val += Vector3.right;
								break;
							case 1:
								val += Vector3.left;
								break;
							case 2:
								val += Vector3.forward;
								break;
							case 3:
								val += Vector3.back;
								break;
						}
						instance.NPC.LookAt(val);
					}
				}
			}
			return;
		}

		// Do shoot 
		public void ShootAtTarget(AngryGuardJobInstance instance, ref NPCBase.NPCState state)
		{
			if (!instance.Owner.Stockpile.TryRemove(this.ShootItem) {
				state.SetIndicator(new IndicatorState(this.CooldownMissingItem, this.ShootItem[0].Type, true, false), true);
				return;
			}

			if (this.OnShootAudio != null) {
				ServerManager.SendAudio(instance.eyePosition, this.OnShootAudio);
			}
			if (this.OnHitAudio != null) {
				ServerManager.SendAudio(instance.target.Position, this.OnHitAudio);
			}
			Vector3 positionToAimFor = instance.target.Position;
			positionToAimFor[1] += 1;
			Vector3 normalized = Vector3.Normalize(positionToAimFor - instance.eyePosition);
			ServerManager.SendParticleTrail(instance.eyePosition + normalized * 0.15f, positionToAimFor - normalized * 0.15f, Pipliz.Random.NextFloat(1.5f, 2.5f));

			instance.target.OnHit(this.shootDamage, instance.NPC, ModLoader.OnHitData.EHitSourceType.NPC);
			state.SetIndicator(new IndicatorState(this.CooldownShot, this.ShootItem[0].Type, false, true), true);
			if (this.OnShootResultItem != null && this.OnShootResultItem.item.Type > 0 && Random.NextDouble(0.0, 1.0) <= this.OnShootResultItem.chance) {
				instance.Owner.Stockpile.Add(this.OnShootResultItem.item);
			}
		}

		// unused for guards
		public void OnNPCAtStockpile(BlockJobInstance blockInstance, ref NPCBase.NPCState state)
		{
		}

		// unused for guards
		public void OnGoalChanged(BlockJobInstance blockInstance, NPCBase.NPCGoal goalOld, NPCBase.NPCGoal goalNew)
		{
		}

		// get location for the NPC to walk to
		public Vector3Int GetJobLocation(BlockJobInstance blockInstance)
		{
			return blockInstance.Position;
		}

	} // class

} // namespace


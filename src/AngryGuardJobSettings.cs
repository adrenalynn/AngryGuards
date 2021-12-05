using System.Collections.Generic;
using NPC;
using Pipliz;
using Jobs;
using Shared;
using UnityEngine;

namespace AngryGuards {

	public class AngryGuardJobSettings: BlockJobSettingsSpawnableDefault<AngryGuardJobInstance>
	{
		public string ident;
		public enum EGuardSleepType: byte
		{
			Day,
			Night
		}

		// public ItemTypes.ItemType[] BlockTypes { get; set; }
		// public NPCType NPCType { get; set; }
		//public InventoryItem RecruitmentItem { get; set; }
		public string OnShootAudio;
		public List<InventoryItem> ShootItem;
		public EGuardSleepType SleepType;
		public int Range;
		public int Damage;
		public int CooldownShot;
		public ItemTypes.ItemTypeDrops OnShootResultItem;
		public float CooldownSearchingTarget;
		public float CooldownMissingItem;
		public float SleepSafetyPeriod;
		public string OnHitAudio;

/*
		public float NPCShopGameHourMinimum
		{
			get
			{
				ServerManager.ServerSettingsData.TimeSettings settings = TimeCycle.Settings;
				if (this.SleepType == EGuardSleepType.Day || this.SleepType != EGuardSleepType.Night)
				{
					return settings.GuardShiftNightEnd - 0.1f;
				}
				return settings.GuardShiftDayEnd - 0.1f;
			}
		}

		public float NPCShopGameHourMaximum
		{
			get
			{
				ServerManager.ServerSettingsData.TimeSettings settings = TimeCycle.Settings;
				if (this.SleepType == EGuardSleepType.Day || this.SleepType != EGuardSleepType.Night)
				{
					return settings.GuardShiftNightEnd + 0.1f;
				}
				return settings.GuardShiftDayEnd + 0.1f;
			}
		}
*/

/*
		public bool ToSleep
		{
			get
			{
				ServerManager.ServerSettingsData.TimeSettings settings = TimeCycle.Settings;
				float timeOfDayHours = TimeCycle.TimeOfDayHours;
				if (this.SleepType == EGuardSleepType.Day || this.SleepType != EGuardSleepType.Night) {
					if (timeOfDayHours <= settings.GuardShiftNightStart) {
						return timeOfDayHours > settings.GuardShiftNightEnd;
					}
					return false;
				}
				if (!(timeOfDayHours >= settings.GuardShiftDayEnd)) {
					return timeOfDayHours < settings.GuardShiftDayStart;
				}
				return true;
			}
		}
*/

		public AngryGuardJobSettings(string blockTypeKey, string npcTypeKey, EGuardSleepType sleepType, int damage, int range, int cooldownShot, string shootAudio, InventoryItem shootItem, InventoryItem recruitmentItem)
		{
			this.BlockTypes = new ItemTypes.ItemType[5] {
				ItemTypes.GetType(blockTypeKey),
				ItemTypes.GetType(blockTypeKey + "x+"),
				ItemTypes.GetType(blockTypeKey + "x-"),
				ItemTypes.GetType(blockTypeKey + "z+"),
				ItemTypes.GetType(blockTypeKey + "z-")
			};
			this.OnShootAudio = shootAudio;
			this.ShootItem = new List<InventoryItem> { shootItem };
			this.NPCType = NPCType.GetByKeyNameOrDefault(npcTypeKey);
			this.SleepType = sleepType;
			this.Damage = damage;
			this.Range = range;
			//this.RecruitmentItem = recruitmentItem;
			this.CooldownShot = cooldownShot;

			// those are static for all
			this.CooldownSearchingTarget = 0.5f;
			this.CooldownMissingItem = 1.5f;
			this.SleepSafetyPeriod = 1f;
			this.OnHitAudio = "fleshHit";
		}

		// perform guard duty
		public override void OnNPCAtJob(BlockJobInstance blockInstance, ref NPCBase.NPCState state)
		{
			AngryGuardJobInstance instance = (AngryGuardJobInstance)blockInstance;

			instance.target = PlayerTracker.FindTarget(instance.Owner, instance.Position.Add(0, 1, 0).Vector, this.Range);

			if (instance.target != null) {
				Vector3 positionToAimFor = instance.target.Position;
				positionToAimFor[1] += 1;
				instance.NPC.LookAt(positionToAimFor);
				this.ShootAtTarget(instance, ref state);
			} else {
				state.SetCooldown((double)this.CooldownSearchingTarget);
				Vector3 val = instance.Position.Vector;
				int num;
				if (this.BlockTypes.ContainsByReference<ItemTypes.ItemType>(instance.BlockType, out num)) {
					switch (num) {
						case 1:
							val += Vector3.right;
							break;
						case 2:
							val += Vector3.left;
							break;
						case 3:
							val += Vector3.forward;
							break;
						case 4:
							val += Vector3.back;
							break;
					}
					instance.NPC.LookAt(val);
				}
			}
			return;
		}

		// Do shoot 
		public void ShootAtTarget(AngryGuardJobInstance instance, ref NPCBase.NPCState state)
		{
			if (!instance.Owner.Stockpile.TryRemove(this.ShootItem)) {
				state.SetIndicator(IndicatorState.NewMissingItemIndicator(this.CooldownMissingItem, this.ShootItem[0].Type));
				return;
			}

			Vector3 eyePosition = instance.Position.Add(0, 1, 0).Vector;
			if (this.OnShootAudio != null) {
				AudioManager.SendAudio(eyePosition, this.OnShootAudio);
			}
			if (this.OnHitAudio != null) {
				AudioManager.SendAudio(instance.target.Position, this.OnHitAudio);
			}
			Vector3 positionToAimFor = instance.target.Position;
			positionToAimFor.y += 1;
			Vector3 normalized = Vector3.Normalize(positionToAimFor - eyePosition);
			ServerManager.SendParticleTrail(eyePosition + normalized * 0.15f, positionToAimFor - normalized * 0.15f, Pipliz.Random.NextFloat(1.5f, 2.5f));

			Players.TakeHit(instance.target, (float)this.Damage, instance.NPC, ModLoader.OnHitData.EHitSourceType.NPC);
			state.SetIndicator(IndicatorState.NewItemIndicator(this.CooldownShot, this.ShootItem[0].Type));
			if (this.OnShootResultItem.item.Type > 0 && Pipliz.Random.NextDouble(0.0, 1.0) <= this.OnShootResultItem.chance) {
				instance.Owner.Stockpile.Add(this.OnShootResultItem.item);
			}
		}

		// unused for guards
		public override void OnNPCAtStockpile(BlockJobInstance blockInstance, ref NPCBase.NPCState state)
		{
		}

/*
		// unused for guards
		public void OnGoalChanged(BlockJobInstance blockInstance, INPCGoal goalOld, INPCGoal goalNew)
		{
		}
*/

		// get location for the NPC to walk to
		public override Pipliz.Vector3Int GetJobLocation(BlockJobInstance blockInstance)
		{
			return blockInstance.Position;
		}

	} // class

} // namespace


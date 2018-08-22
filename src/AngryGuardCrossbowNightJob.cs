using System.Collections.Generic;
using Pipliz.Mods.APIProvider.Jobs;
using Server.NPCs;
using BlockTypes.Builtin;
using UnityEngine;

namespace AngryGuards {

	public class AngryGuardCrossbowNightJob : AngryGuardsBaseJob, INPCTypeDefiner
	{
		public static GuardSettings CachedSettings;

		public override string NPCTypeKey {
			get {
				return "angryguards.guardcrossbownight";
			}
		}

		public static GuardSettings GetGuardSettings()
		{
			if (AngryGuardCrossbowNightJob.CachedSettings == null) {
				GuardSettings guardSettings = new GuardSettings();
				guardSettings.cooldownMissingItem = 1.5f;
				guardSettings.cooldownSearchingTarget = 0.5f;
				guardSettings.cooldownShot = 8f;
				guardSettings.range = 25;
				guardSettings.recruitmentItem = new InventoryItem(BuiltinBlocks.Crossbow, 1);
				guardSettings.shootItem = new List<InventoryItem> {
					new InventoryItem(BuiltinBlocks.CrossbowBolt, 1)
				};
				guardSettings.shootDamage = 65f;
				guardSettings.sleepSafetyPeriod = 1f;
				guardSettings.sleepType = EGuardSleepType.Day;
				guardSettings.typeXN = AngryGuards.Blocks.GuardCrossbowJobNightXN.ItemIndex;
				guardSettings.typeXP = AngryGuards.Blocks.GuardCrossbowJobNightXP.ItemIndex;
				guardSettings.typeZN = AngryGuards.Blocks.GuardCrossbowJobNightZN.ItemIndex;
				guardSettings.typeZP = AngryGuards.Blocks.GuardCrossbowJobNightZP.ItemIndex;
				guardSettings.OnShootAudio = "bowShoot";
				guardSettings.OnHitAudio = "fleshHit";
				AngryGuardCrossbowNightJob.CachedSettings = guardSettings;
			}
			return AngryGuardCrossbowNightJob.CachedSettings;
		}

		public override GuardSettings SetupSettings()
		{
			return AngryGuardCrossbowNightJob.GetGuardSettings();
		}

		NPCTypeStandardSettings INPCTypeDefiner.GetNPCTypeDefinition()
		{
			NPCTypeStandardSettings NpcSettings = new NPCTypeStandardSettings();
			NpcSettings.keyName = this.NPCTypeKey;
			NpcSettings.printName = "AngryGuard Crossbow Night";
			NpcSettings.maskColor1 = new Color32(52, 52, 52, 255);
			NpcSettings.type = NPCTypeID.GetNextID();
			return NpcSettings;
		}

	} // class

} // namespace


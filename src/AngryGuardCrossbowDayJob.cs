using System.Collections.Generic;
using Pipliz.Mods.APIProvider.Jobs;
using Server.NPCs;
using BlockTypes.Builtin;
using UnityEngine;

namespace AngryGuards {

	public class AngryGuardCrossbowDayJob : AngryGuardsBaseJob, INPCTypeDefiner
	{
		public static GuardSettings CachedSettings;

		public override string NPCTypeKey {
			get {
				return "angryguards.guardcrossbowday";
			}
		}

		public static GuardSettings GetGuardSettings()
		{
			if (AngryGuardCrossbowDayJob.CachedSettings == null) {
				GuardSettings guardSettings = new GuardSettings();
				guardSettings.cooldownMissingItem = 1.5f;
				guardSettings.cooldownSearchingTarget = 0.5f;
				guardSettings.cooldownShot = 8f;
				guardSettings.range = 25;
				guardSettings.recruitmentItem = new InventoryItem(BuiltinBlocks.Crossbow, 1);
				guardSettings.shootItem = new List<InventoryItem> {
					new InventoryItem(BuiltinBlocks.CrossbowBolt, 1)
				};
				guardSettings.shootDamage = 90f;
				guardSettings.sleepSafetyPeriod = 1f;
				guardSettings.sleepType = EGuardSleepType.Night;
				guardSettings.typeXN = AngryGuards.Blocks.GuardCrossbowJobDayXN.ItemIndex;
				guardSettings.typeXP = AngryGuards.Blocks.GuardCrossbowJobDayXP.ItemIndex;
				guardSettings.typeZN = AngryGuards.Blocks.GuardCrossbowJobDayZN.ItemIndex;
				guardSettings.typeZP = AngryGuards.Blocks.GuardCrossbowJobDayZP.ItemIndex;
				guardSettings.OnShootAudio = "bowShoot";
				guardSettings.OnHitAudio = "fleshHit";
				AngryGuardCrossbowDayJob.CachedSettings = guardSettings;
			}
			return AngryGuardCrossbowDayJob.CachedSettings;
		}

		public override GuardSettings SetupSettings()
		{
			return AngryGuardCrossbowDayJob.GetGuardSettings();
		}

		NPCTypeStandardSettings INPCTypeDefiner.GetNPCTypeDefinition()
		{
			NPCTypeStandardSettings NpcSettings = new NPCTypeStandardSettings();
			NpcSettings.keyName = this.NPCTypeKey;
			NpcSettings.printName = "AngryGuard Crossbow Day";
			NpcSettings.maskColor1 = new Color32(52, 52, 52, 255);
			NpcSettings.type = NPCTypeID.GetNextID();
			return NpcSettings;
		}

	} // class

} // namespace


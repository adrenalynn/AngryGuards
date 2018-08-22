using System.Collections.Generic;
using Pipliz.Mods.APIProvider.Jobs;
using Server.NPCs;
using BlockTypes.Builtin;
using UnityEngine;

namespace AngryGuards {

	public class AngryGuardBowNightJob : AngryGuardsBaseJob, INPCTypeDefiner
	{
		public static GuardSettings CachedSettings;

		public override string NPCTypeKey {
			get {
				return "angryguards.guardbownight";
			}
		}

		public static GuardSettings GetGuardSettings()
		{
			if (AngryGuardBowNightJob.CachedSettings == null) {
				GuardSettings guardSettings = new GuardSettings();
				guardSettings.cooldownMissingItem = 1.5f;
				guardSettings.cooldownSearchingTarget = 0.5f;
				guardSettings.cooldownShot = 5f;
				guardSettings.range = 20;
				guardSettings.recruitmentItem = new InventoryItem(BuiltinBlocks.Bow, 1);
				guardSettings.shootItem = new List<InventoryItem> {
					new InventoryItem(BuiltinBlocks.BronzeArrow, 1)
				};
				guardSettings.shootDamage = 45f;
				guardSettings.sleepSafetyPeriod = 1f;
				guardSettings.sleepType = EGuardSleepType.Day;
				guardSettings.typeXN = AngryGuards.Blocks.GuardBowJobNightXN.ItemIndex;
				guardSettings.typeXP = AngryGuards.Blocks.GuardBowJobNightXP.ItemIndex;
				guardSettings.typeZN = AngryGuards.Blocks.GuardBowJobNightZN.ItemIndex;
				guardSettings.typeZP = AngryGuards.Blocks.GuardBowJobNightZP.ItemIndex;
				guardSettings.OnShootAudio = "bowShoot";
				guardSettings.OnHitAudio = "fleshHit";
				AngryGuardBowNightJob.CachedSettings = guardSettings;
			}
			return AngryGuardBowNightJob.CachedSettings;
		}

		public override GuardSettings SetupSettings()
		{
			return AngryGuardBowNightJob.GetGuardSettings();
		}

		NPCTypeStandardSettings INPCTypeDefiner.GetNPCTypeDefinition()
		{
			NPCTypeStandardSettings NpcSettings = new NPCTypeStandardSettings();
			NpcSettings.keyName = this.NPCTypeKey;
			NpcSettings.printName = "AngryGuard Bow Night";
			NpcSettings.maskColor1 = new Color32(160, 107, 50, 255);
			NpcSettings.type = NPCTypeID.GetNextID();
			return NpcSettings;
		}

	} // class

} // namespace


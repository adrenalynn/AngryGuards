using System.Collections.Generic;
using Pipliz.Mods.APIProvider.Jobs;
using Server.NPCs;
using BlockTypes.Builtin;
using UnityEngine;

namespace AngryGuards {

	public class AngryGuardBowDayJob : AngryGuardsBaseJob, INPCTypeDefiner
	{
		public static GuardSettings CachedSettings;

		public override string NPCTypeKey {
			get {
				return "angryguards.guardbowday";
			}
		}

		public static GuardSettings GetGuardSettings()
		{
			if (AngryGuardBowDayJob.CachedSettings == null) {
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
				guardSettings.sleepType = EGuardSleepType.Night;
				guardSettings.typeXN = AngryGuards.Blocks.GuardBowJobDayXN.ItemIndex;
				guardSettings.typeXP = AngryGuards.Blocks.GuardBowJobDayXP.ItemIndex;
				guardSettings.typeZN = AngryGuards.Blocks.GuardBowJobDayZN.ItemIndex;
				guardSettings.typeZP = AngryGuards.Blocks.GuardBowJobDayZP.ItemIndex;
				guardSettings.OnShootAudio = "bowShoot";
				guardSettings.OnHitAudio = "fleshHit";
				AngryGuardBowDayJob.CachedSettings = guardSettings;
			}
			return AngryGuardBowDayJob.CachedSettings;
		}

		public override GuardSettings SetupSettings()
		{
			return AngryGuardBowDayJob.GetGuardSettings();
		}

		NPCTypeStandardSettings INPCTypeDefiner.GetNPCTypeDefinition()
		{
			NPCTypeStandardSettings NpcSettings = new NPCTypeStandardSettings();
			NpcSettings.keyName = this.NPCTypeKey;
			NpcSettings.printName = "AngryGuard Bow Day";
			NpcSettings.maskColor1 = new Color32(160, 107, 50, 255);
			NpcSettings.type = NPCTypeID.GetNextID();
			return NpcSettings;
		}

	} // class

} // namespace


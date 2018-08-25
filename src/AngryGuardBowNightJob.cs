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
				guardSettings.shootDamage = (float)AngryGuards.Bow.Damage;
				guardSettings.cooldownShot = (float)AngryGuards.Bow.Reload;
				guardSettings.range = AngryGuards.Bow.Range;
				guardSettings.recruitmentItem = new InventoryItem(BuiltinBlocks.Bow, 1);
				guardSettings.shootItem = new List<InventoryItem> {
					new InventoryItem(BuiltinBlocks.BronzeArrow, 1)
				};
				guardSettings.sleepSafetyPeriod = 1f;
				guardSettings.sleepType = EGuardSleepType.Day;
				guardSettings.typeXN = ItemTypes.IndexLookup.GetIndex("angryguards.guardbownightx-");
				guardSettings.typeXP = ItemTypes.IndexLookup.GetIndex("angryguards.guardbownightx+");
				guardSettings.typeZN = ItemTypes.IndexLookup.GetIndex("angryguards.guardbownightz-");
				guardSettings.typeZP = ItemTypes.IndexLookup.GetIndex("angryguards.guardbownightz+");
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


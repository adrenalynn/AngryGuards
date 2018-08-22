using System.Collections.Generic;
using Pipliz.Mods.APIProvider.Jobs;
using Server.NPCs;
using BlockTypes.Builtin;
using UnityEngine;

namespace AngryGuards {

	public class AngryGuardMatchlockGunNightJob : AngryGuardsBaseJob, INPCTypeDefiner
	{
		public static GuardSettings CachedSettings;

		public override string NPCTypeKey {
			get {
				return "angryguards.guardmatchlockgunnight";
			}
		}

		public static GuardSettings GetGuardSettings()
		{
			if (AngryGuardMatchlockGunNightJob.CachedSettings == null) {
				GuardSettings guardSettings = new GuardSettings();
				guardSettings.cooldownMissingItem = 1.5f;
				guardSettings.cooldownSearchingTarget = 0.5f;
				guardSettings.cooldownShot = 12f;
				guardSettings.range = 30;
				guardSettings.recruitmentItem = new InventoryItem(BuiltinBlocks.MatchlockGun, 1);
				guardSettings.shootItem = new List<InventoryItem> {
					new InventoryItem(BuiltinBlocks.LeadBullet, 1),
					new InventoryItem(BuiltinBlocks.GunpowderPouch, 1)
				};
				guardSettings.shootDamage = 500f;
				guardSettings.sleepSafetyPeriod = 1f;
				guardSettings.sleepType = EGuardSleepType.Day;
				guardSettings.typeXN = AngryGuards.Blocks.GuardMatchlockGunJobNightXN.ItemIndex;
				guardSettings.typeXP = AngryGuards.Blocks.GuardMatchlockGunJobNightXP.ItemIndex;
				guardSettings.typeZN = AngryGuards.Blocks.GuardMatchlockGunJobNightZN.ItemIndex;
				guardSettings.typeZP = AngryGuards.Blocks.GuardMatchlockGunJobNightZP.ItemIndex;
				guardSettings.OnShootAudio = "matchlock";
				guardSettings.OnHitAudio = "fleshHit";
				AngryGuardMatchlockGunNightJob.CachedSettings = guardSettings;
			}
			return AngryGuardMatchlockGunNightJob.CachedSettings;
		}

		public override GuardSettings SetupSettings()
		{
			return AngryGuardMatchlockGunNightJob.GetGuardSettings();
		}

		public override void OnShoot()
		{
			if (Pipliz.Random.NextFloat(0f, 1f) < 0.9f) {
				Stockpile.GetStockPile(base.owner).Add(BuiltinBlocks.LinenPouch, 1);
			}
			base.OnShoot();
		}

		NPCTypeStandardSettings INPCTypeDefiner.GetNPCTypeDefinition()
		{
			NPCTypeStandardSettings NpcSettings = new NPCTypeStandardSettings();
			NpcSettings.keyName = this.NPCTypeKey;
			NpcSettings.printName = "AngryGuard Matchlock Night";
			NpcSettings.maskColor1 = new Color32(205, 207, 141, 255);
			NpcSettings.type = NPCTypeID.GetNextID();
			return NpcSettings;
		}

	} // class

} // namespace


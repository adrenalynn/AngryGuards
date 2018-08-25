using System.Collections.Generic;
using Pipliz.Mods.APIProvider.Jobs;
using Server.NPCs;
using BlockTypes.Builtin;
using UnityEngine;

namespace AngryGuards {

	public class AngryGuardMatchlockGunDayJob : AngryGuardsBaseJob, INPCTypeDefiner
	{
		public static GuardSettings CachedSettings;

		public override string NPCTypeKey {
			get {
				return "angryguards.guardmatchlockgunday";
			}
		}

		public static GuardSettings GetGuardSettings()
		{
			if (AngryGuardMatchlockGunDayJob.CachedSettings == null) {
				GuardSettings guardSettings = new GuardSettings();
				guardSettings.cooldownMissingItem = 1.5f;
				guardSettings.cooldownSearchingTarget = 0.5f;
				guardSettings.shootDamage = (float)AngryGuards.MatchlockGun.Damage;
				guardSettings.cooldownShot = (float)AngryGuards.MatchlockGun.Reload;
				guardSettings.range = AngryGuards.MatchlockGun.Range;
				guardSettings.recruitmentItem = new InventoryItem(BuiltinBlocks.MatchlockGun, 1);
				guardSettings.shootItem = new List<InventoryItem> {
					new InventoryItem(BuiltinBlocks.LeadBullet, 1),
					new InventoryItem(BuiltinBlocks.GunpowderPouch, 1)
				};
				guardSettings.sleepSafetyPeriod = 1f;
				guardSettings.sleepType = EGuardSleepType.Night;
				guardSettings.typeXN = ItemTypes.IndexLookup.GetIndex("angryguards.guardmatchlockgundayx-");
				guardSettings.typeXP = ItemTypes.IndexLookup.GetIndex("angryguards.guardmatchlockgundayx+");
				guardSettings.typeZN = ItemTypes.IndexLookup.GetIndex("angryguards.guardmatchlockgundayz-");
				guardSettings.typeZP = ItemTypes.IndexLookup.GetIndex("angryguards.guardmatchlockgundayz+");
				guardSettings.OnShootAudio = "matchlock";
				guardSettings.OnHitAudio = "fleshHit";
				AngryGuardMatchlockGunDayJob.CachedSettings = guardSettings;
			}
			return AngryGuardMatchlockGunDayJob.CachedSettings;
		}

		public override GuardSettings SetupSettings()
		{
			return AngryGuardMatchlockGunDayJob.GetGuardSettings();
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
			NpcSettings.printName = "AngryGuard Matchlock Day";
			NpcSettings.maskColor1 = new Color32(205, 207, 141, 255);
			NpcSettings.type = NPCTypeID.GetNextID();
			return NpcSettings;
		}

	} // class

} // namespace


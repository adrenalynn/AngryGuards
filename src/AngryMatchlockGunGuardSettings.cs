
namespace AngryGuards {

	public class AngryMatchlockGunGuardSettings: AngryGuardJobSettings
	{

		public AngryMatchlockGunGuardSettings(EGuardSleepType sleepType)
		{
			this.ident = "matchlockgun";
			this.SleepType = sleepType;

			this.BlockTypes = new ItemTypes.ItemType[4] {
				ItemTypes.GetType("angryguards.guard" + this.ident + this.SleepType + "x+"),
				ItemTypes.GetType("angryguards.guard" + this.ident + this.SleepType + "x-"),
				ItemTypes.GetType("angryguards.guard" + this.ident + this.SleepType + "z+"),
				ItemTypes.GetType("angryguards.guard" + this.ident + this.SleepType + "z-"),
			};

			this.NPCType = NPCType.GetByKeyNameOrDefault("angryguards.guard" + this.ident + this.SleepType);
			this.OnShootAudio = "bowShoot";
			this.RecruitmentItem = new InventoryItem(BuiltinBlocks.MatchlockGun);
			this.ShootItem = new List<InventoryItem> {
				new InventoryItem(BuiltinBlocks.LeadBullet),
				new InventoryItem(BuiltinBlocks.GunpowderPouch)
			};
			this.Range = AngryGuards.MatchlockGun.Range;
			this.Damage = AngryGuards.MatchlockGun.Damage;
			this.CooldDownShot = AngryGuards.MatchlockGun.Reload;
			this.OnShootResultItem = new ItemTypes.ItemTypeDrops(BuiltinBlocks.LinenPouch, 1, 0.9);
		}
	}

}



namespace AngryGuards {

	public class AngryBowGuardSettings: AngryGuardJobSettings
	{

		public AngryBowGuardSettings(EGuardSleepType sleepType)
		{
			this.ident = "bow";
			this.SleepType = sleepType;

			this.BlockTypes = new ItemTypes.ItemType[4] {
				ItemTypes.GetType("angryguards.guard" + this.ident + this.SleepType + "x+"),
				ItemTypes.GetType("angryguards.guard" + this.ident + this.SleepType + "x-"),
				ItemTypes.GetType("angryguards.guard" + this.ident + this.SleepType + "z+"),
				ItemTypes.GetType("angryguards.guard" + this.ident + this.SleepType + "z-"),
			};

			this.NPCType = NPCType.GetByKeyNameOrDefault("angryguards.guard" + this.ident + this.SleepType);
			this.OnShootAudio = "bowShoot";
			this.RecruitmentItem = new InventoryItem(BuiltinBlocks.Bow);
			this.ShootItem = new List<InventoryItem> {new InventoryItem(BuiltinBlocks.BronzeArrow)};
			this.Range = AngryGuards.Bow.Range;
			this.Damage = AngryGuards.Bow.Damage;
			this.CooldDownShot = AngryGuards.Bow.Reload;
			this.OnShootResultItem = null;
		}
	}

}


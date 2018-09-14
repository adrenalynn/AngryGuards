
namespace AngryGuards {

	public class AngryCrossbowGuardSettings: AngryGuardJobSettings
	{

		public AngryCrossbowGuardSettings(EGuardSleepType sleepType)
		{
			this.ident = "crossbow";
			this.SleepType = sleepType;

			this.BlockTypes = new ItemTypes.ItemType[4] {
				ItemTypes.GetType("angryguards.guard" + this.ident + this.SleepType + "x+"),
				ItemTypes.GetType("angryguards.guard" + this.ident + this.SleepType + "x-"),
				ItemTypes.GetType("angryguards.guard" + this.ident + this.SleepType + "z+"),
				ItemTypes.GetType("angryguards.guard" + this.ident + this.SleepType + "z-"),
			};

			this.NPCType = NPCType.GetByKeyNameOrDefault("angryguards.guard" + this.ident + this.SleepType);
			this.OnShootAudio = "bowShoot";
			this.RecruitmentItem = new InventoryItem(BuiltinBlocks.Crossbow);
			this.ShootItem = new List<InventoryItem> {new InventoryItem(BuiltinBlocks.CrossbowBolt)};
			this.Range = AngryGuards.Crossbow.Range;
			this.Damage = AngryGuards.Crossbow.Damage;
			this.CooldDownShot = AngryGuards.Crossbow.Reload;
			this.OnShootResultItem = null;
		}
	}

}


using ModLoaderInterfaces;

namespace AngryGuards {

	public class ModInterfaces: IOnAssemblyLoaded, IAfterItemTypesDefined, IAfterWorldLoad, IOnQuit, IOnNPCHit, IOnTryChangeBlock
	{

		public void OnAssemblyLoaded(string path)
		{
			AngryGuards.OnAssemblyLoaded(path);
		}

		public void AfterItemTypesDefined()
		{
			AngryGuards.AfterItemTypesDefined();
		}

		public void AfterWorldLoad()
		{
			AngryGuards.AfterWorldLoad();
		}

		public void OnQuit()
		{
			AngryGuards.OnQuit();
		}

		public void OnNPCHit(NPC.NPCBase npc, ModLoader.OnHitData data)
		{
			AngryGuards.OnNPCHit(npc, data);
		}

		public void OnTryChangeBlock(ModLoader.OnTryChangeBlockData data)
		{
			AngryGuards.OnTryChangeBlock(data);
		}

	}

}


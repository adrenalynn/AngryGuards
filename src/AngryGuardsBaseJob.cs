using Pipliz;
using Pipliz.Mods.APIProvider.Jobs;
using NPC;
using UnityEngine;

namespace AngryGuards {

	public class AngryGuardsBaseJob: GuardBaseJob
	{
		public new Players.Player target;
		public Vector3 eyePosition;

		public override void OnShoot()
		{
			if (this.guardSettings.OnShootAudio != null) {
				ServerManager.SendAudio(this.eyePosition, this.guardSettings.OnShootAudio);
			}
			if (this.guardSettings.OnHitAudio != null) {
				ServerManager.SendAudio(this.target.Position, this.guardSettings.OnHitAudio);
			}
			Vector3 positionToAimFor = this.target.Position;
			positionToAimFor[1] += 1;
			Vector3 normalized = Vector3.Normalize(positionToAimFor - this.eyePosition);
			ServerManager.SendParticleTrail(this.eyePosition + normalized * 0.15f, positionToAimFor - normalized * 0.15f, Pipliz.Random.NextFloat (1.5f, 2.5f));
			Players.TakeHit(this.target, this.guardSettings.shootDamage, (object)base.usedNPC, ModLoader.OnHitData.EHitSourceType.NPC);
		}

		public override void OnNPCAtJob(ref NPCBase.NPCState state)
		{
			this.eyePosition = base.position.Vector;
			this.eyePosition[1] += 1;

			this.target = PlayerTracker.FindTarget(base.Owner, this.eyePosition, this.guardSettings.range);

			if (this.target != null) {
				Vector3 positionToAimFor = this.target.Position;
				positionToAimFor[1] += 1;
				((NPCBase)base.usedNPC).LookAt(positionToAimFor);
				this.ShootAtTarget(ref state);
			} else {
				state.SetCooldown((double)this.guardSettings.cooldownSearchingTarget);
				if (base.worldTypeChecked) {
					Vector3 val = base.position.Vector;
					if (base.worldType == this.guardSettings.typeXP) {
						val += Vector3.right;
					} else if (base.worldType == this.guardSettings.typeXN) {
						val += Vector3.left;
					} else if (base.worldType == this.guardSettings.typeZP) {
						val += Vector3.forward;
					} else if (base.worldType == this.guardSettings.typeZN) {
						val += Vector3.back;
					}
					((NPCBase)base.usedNPC).LookAt(val);
				}
			}
			return;
		}

	} // class

} // namespace


// author: JackO'ManyNames
// date: 23/11/2021
// last edited: 6/2/2022

using ThunderRoad;
using UnityEngine;

namespace LifeSteal
{
    public class LifeSteal : MonoBehaviour
    {
		public void Start()
		{
			this.item = base.GetComponent<Item>();
			this.item.OnGrabEvent += GrabEventHandler;
			foreach (CollisionHandler collisionHandler in item.collisionHandlers)
			{
				collisionHandler.OnCollisionStartEvent += CollisionEventHandler;
			}
			item.OnUngrabEvent += ReleaseGrip;
		}

		/// <summary>
		/// If colliding with a creature, get the part of the creature that was targeted
		/// and if it wasn't the one using the weapon, health a certain amount of health
		/// </summary>
        private void CollisionEventHandler(CollisionInstance collisionInstance)
        {
			bool enabled = base.enabled;
			bool flag = collisionInstance.damageStruct.hitRagdollPart;
			if (enabled && flag)
			{
				Creature c = collisionInstance.targetCollider.GetComponentInParent<Creature>();

				RagdollPart.Type type = collisionInstance.damageStruct.hitRagdollPart.type;
				float damage = collisionInstance.damageStruct.damage;

				if (type == RagdollPart.Type.Torso && c != handlerCreature)
				{
					Player.local.creature.Heal(damage * 0.4f);
				}
				else if(c != handlerCreature)
				{
					float num = damage * (lifeStealPercent / 100f);
					Player.local.creature.currentHealth = Mathf.Clamp(handlerCreature.currentHealth + num, 0f, handlerCreature.maxHealth);
				}
			}
		}

		/// <summary>
		/// When releasing the grip on the weapon, unselect the one holding it
		/// </summary>
        private void ReleaseGrip(Handle handle, RagdollHand ragdollHand, bool throwing)
        {
			bool enabled = base.enabled;
			if (enabled)
			{
				bool flag = item.handlers.Count <= 0;
				if (flag)
				{
					handlerCreature = null;
				}
			}
		}	

		/// <summary>
		/// when grabbing the weapon, get the hand of the one holding it
		/// </summary>
        private void GrabEventHandler(Handle handle, RagdollHand ragdollHand)
        {
			bool enabled = base.enabled;
			if (enabled)
			{
				handlerCreature = ragdollHand.GetComponentInParent<Creature>();
			}
		}

		/// <summary>
		/// When holding the blade, slowly drain away health from the wielder over time
		/// </summary>
		private void Update()
		{
			bool flag = handlerCreature != null && item.mainHandler != null;
			if (flag)
			{
				bool flag2 = Time.time - startTime > 1f;
				if (flag2)
				{
					bool flag3 = handlerCreature.currentHealth > 1f;
					if (flag3)
					{
						float num = handlerCreature.maxHealth * (dmgPercentPerSec / 100f);
						handlerCreature.currentHealth = Mathf.Clamp(handlerCreature.currentHealth - num, 1f, handlerCreature.maxHealth);
						bool flag4 = item.mainHandler.side == 0;
						if (flag4)
						{
							PlayerControl.handRight.HapticPlayClip(Catalog.gameData.haptics.spellSelected, 1f);
						}
						else
						{
							PlayerControl.handLeft.HapticPlayClip(Catalog.gameData.haptics.spellSelected, 1f);
						}
					}
					startTime = Time.time;
				}
			}
		}
         
		public Item item;
		public float dmgPercentPerSec = 0.8f;
		public float lifeStealPercent = 30.0f;
		public Creature handlerCreature;
		protected RagdollHand rightInteractor;
		protected RagdollHand leftInteractor;
		private float startTime;
	}
}

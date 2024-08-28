using InsaneSystems.HealthbarsKit.UI;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace InsaneSystems.HealthbarsKit
{
	/// <summary>This component allows to make any game object having health and its own healthbar. Use this in your game or you can make your custom class, using its code as template. </summary>
	public sealed class Damageable : MonoBehaviour, IHealthed
	{
		enum HealthbarsMode
		{
			UI,
			Sprite
		}
		
		public event UIHealthbars.HealthChangedAction HealthWasChanged;

		[SerializeField] [Range(0.1f, 1000f)] float maxHealth = 100;
		[SerializeField] HealthbarsMode mode = HealthbarsMode.UI;
		
		float health;

        private PhotonView photonView;
		public GameObject deathVFX;
		//public GameObject winUIPanel; // Reference to the Win UI Panel
		//public GameObject loseUIPanel; // Reference to the Lose UI Panel
		private UIController uiController;

        void Awake() => health = maxHealth;
		void Start()
		{
            photonView = GetComponent<PhotonView>();
			// Find the UIController in the scene (or assign it manually)
            uiController = FindObjectOfType<UIController>();
            AddHealthbarToThisObject();
        }

		void AddHealthbarToThisObject()
		{
			if (mode == HealthbarsMode.UI)
			{
				var healthBar = UIHealthbars.AddHealthbar(gameObject, maxHealth);
				
				// setting up event to connect the Healthbar with this Damageable.
				// Now every time when it will take damage, Healthbar will be updated.
				HealthWasChanged += healthBar.OnHealthChanged; 
			}
			else if (mode == HealthbarsMode.Sprite)
			{
				SpriteHealthbars.AddHealthbar(this);
			}
			
			OnHealthChanged();
		}

		void OnHealthChanged() => HealthWasChanged?.Invoke(health);

		public void GetHit(float damage)
		{
            photonView.RPC(nameof(TakeDamageNew), RpcTarget.All, damage);
        }
		public void TakeDamage(float damage)
		{
			health = Mathf.Clamp(health - damage, 0, maxHealth);

			OnHealthChanged();

			if (health == 0)
				Die();
		}

		[PunRPC]
        public void TakeDamageNew(float damage)
        {
            health = Mathf.Clamp(health - damage, 0, maxHealth);

            OnHealthChanged();

            if (health == 0)
                Die();
        }

        public void Die()
		{
            // Sync death and check who caused the death
            photonView.RPC(nameof(HandleDeath), RpcTarget.All, photonView.Owner.ActorNumber);
            Destroy(gameObject);
        }

        [PunRPC]
        void HandleDeath(int actorNumberOfDeadPlayer)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumberOfDeadPlayer)
            {
				// This player died, show the lose UI
				//loseUIPanel.SetActive(true);
				uiController.ShowLoseUI();
            }
            else
            {
				// The local player didn't die, they won
				//winUIPanel.SetActive(true);
				uiController.ShowWinUI();
            }
        }

        public GameObject GetGameObject() => gameObject;
		public float GetHealth() => health;
		public float GetMaxHealth() => maxHealth;
	}
}
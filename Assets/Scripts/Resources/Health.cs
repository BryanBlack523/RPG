using RPG.Saving;
using UnityEngine;
using RPG.Stats;
using RPG.Core;
using System;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float health = 100f;
        private int cashedDeath = Animator.StringToHash("die");
        private bool isDead = false;

        private void Start()
        {
            health = GetComponent<BaseStats>().GetHealth();
        }

        public object CaptureState()
        {
            return health;
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void RestoreState(object state)
        {
            health = (float)state;
            if (health == 0)
                Die();
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            health = Mathf.Max(health - damage, 0);
            if (health == 0)
            {
                Die();
                AwardExpirience(instigator);
            }
        }

        public float GetPercentage()
        {
            return 100 * (health / GetComponent<BaseStats>().GetHealth());
        }

        private void Die()
        {
            if (isDead) return;
            
            isDead = true;
            GetComponent<Animator>().SetTrigger(cashedDeath);
            GetComponent<ActionScheduler>().CancelCurrentAction();
            
        }

        private void AwardExpirience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;
            
            experience.GainExperience(GetComponent<BaseStats>().GetExperienceReward());
        }
    }
}

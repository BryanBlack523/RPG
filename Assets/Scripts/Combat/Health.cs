
using UnityEngine;

namespace RPG.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float health = 100f;
        private int cashedDeath = Animator.StringToHash("die");
        private bool isDead = false;

        public bool IsDead()
        {
            return isDead;
        }
        public void TakeDamage(float damage)
        {
            health = Mathf.Max(health - damage, 0);
            if (isDead == false && health == 0)
            {
                isDead = true;
                GetComponent<Animator>().SetTrigger(cashedDeath);
            }
        }
    }
}

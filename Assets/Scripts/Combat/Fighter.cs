using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    
    public class Fighter : MonoBehaviour, IAction
    {
        Health target;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float attackCoolDown = 1f;
        [SerializeField] float weaponDamage = 10f;

        private int attackCashed = Animator.StringToHash("attack");
        private int stopAttackCashed = Animator.StringToHash("stopAttack");
        private float timeSinceLastAttack = Mathf.Infinity;

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;

            if (!IsInRange())
                GetComponent<Mover>().MoveTo(target.transform.position);
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);

            if (timeSinceLastAttack > attackCoolDown)
            {
                // This will trigger Hit()
                GetComponent<Animator>().ResetTrigger(stopAttackCashed);
                GetComponent<Animator>().SetTrigger(attackCashed);
                timeSinceLastAttack = 0;
            }
        }

        private bool IsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;

            Health testTarget = combatTarget.GetComponent<Health>();
            return testTarget != null && !testTarget.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            target = null;
            GetComponent<Animator>().ResetTrigger(attackCashed);
            GetComponent<Animator>().SetTrigger(stopAttackCashed);
        }

        //Animation event
        void Hit()
        {
            if (target == null) return;
            target.TakeDamage(weaponDamage);
        }
    }
}

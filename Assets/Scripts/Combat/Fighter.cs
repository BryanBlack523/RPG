using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;

namespace RPG.Combat
{
    
    public class Fighter : MonoBehaviour, IAction
    {
        Health target;
        
        [SerializeField] float attackCoolDown = 1f;
        [Range(0, 1)] [SerializeField] float speedFraction = 0.6f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;


        private int attackCashed = Animator.StringToHash("attack");
        private int stopAttackCashed = Animator.StringToHash("stopAttack");
        private float timeSinceLastAttack = Mathf.Infinity;
        Weapon currentWeapon = null;


        private void Start()
        {
            EquipWeapon(defaultWeapon);
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;

            if (!IsInRange())
                GetComponent<Mover>().MoveTo(target.transform.position, speedFraction);
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
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.GetRange();
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
            target.TakeDamage(currentWeapon.GetDamage());
        }
    }
}

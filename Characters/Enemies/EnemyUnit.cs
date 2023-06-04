using Gamekit3D;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SaligiaProofOfVision
{
    //TODO: Range Bonus for Unity with Melee Weapon --> increase range of melee collider?
    public abstract class EnemyUnit : BaseUnit
    {
        #region Animator Hashes
        public static readonly int hashAttack = Animator.StringToHash("Attack");
        public static readonly int hashInAttack = Animator.StringToHash("InAttack");
        public static readonly int hashCanAttackAgain = Animator.StringToHash("CanAttackAgain");
        public static readonly int hashDeath = Animator.StringToHash("Death");
        public static readonly int hashGrounded = Animator.StringToHash("Grounded");
        public static readonly int hashHit = Animator.StringToHash("Hit");
        public static readonly int hashInvulnerableHit = Animator.StringToHash("InvulnerableHit");
        public static readonly int hashIdleState = Animator.StringToHash("Idle");
        public static readonly int hashInPursuit = Animator.StringToHash("InPursuit");
        public static readonly int hashHitDirectionX = Animator.StringToHash("HitDirectionX");
        public static readonly int hashHitDirectionZ = Animator.StringToHash("HitDirectionZ");
        public static readonly int hashNearBase = Animator.StringToHash("NearBase");
        public static readonly int hashSpawnState = Animator.StringToHash("Spawn");
        public static readonly int hashSpeed = Animator.StringToHash("Speed");
        public static readonly int hashSpotted = Animator.StringToHash("Spotted");
        #endregion

        #region Stats
        [SerializeField] protected float movementSpeed;
        [SerializeField] protected int attackDamage;
        [SerializeField, Tooltip("Cooldown in seconds before the unit can attack again")] protected float attackCooldown;
        [SerializeField] protected float attackRange;
        [SerializeField] private bool _isBoss;
        public SinType sinType;
        public new float MaxHealth => maxHealth + bonusLife;
        public float MovementSpeed => movementSpeed;
        public float AttackDamage => attackDamage + bonusDmg;
        public float AttackCooldown => attackCooldown;
        public float AttackRange => attackRange + bonusRange;
        #endregion

        #region Damageable
        [Header("Damageable")]
        [Tooltip("The angle from the which that damageable is hitable. Always in the world XZ plane, with the forward being rotate by hitForwardRoation")]
        [SerializeField, Range(0.0f, 360.0f)]
        protected float hitAngle = 360.0f;
        [Tooltip("Allow to rotate the world forward vector of the damageable used to define the hitAngle zone")]
        [SerializeField, Range(0.0f, 360.0f)]
        protected float hitForwardRotation = 360.0f;
        protected float baseHitAngle;
        #endregion

        [field: Space]
        public Weapon Weapon { get; protected set; }

        #region Stat Bonuses
        public int WrathDmgBonus;
        public float PrideRangeBonus;
        public int GluttonyLifeBonus;

        protected float bonusDmg;
        protected float bonusLife;
        protected float bonusRange;
        #endregion

        #region Components
        protected PlayerController target = null;
        public PlayerController Target { get { return target; } set { target = value; } }

        protected EnemyController controller;
        public EnemyController Controller { get { return controller; } }

        protected TargetDistributor.TargetFollower followerInstance = null;
        public TargetDistributor.TargetFollower FollowerData { get { return followerInstance; } }

        public TargetScanner playerScanner;
        protected Collider col;
        #endregion

        #region Audio
        [field: Header("Audio")]
        [field: SerializeField] public RandomAudioPlayer AttackAudio { get; private set; }
        [field: SerializeField] public RandomAudioPlayer FrontStepAudio { get; private set; }
        [field: SerializeField] public RandomAudioPlayer BackStepAudio { get; private set; }
        [field: SerializeField] public RandomAudioPlayer WoundedAudio { get; private set; }
        [field: SerializeField] public RandomAudioPlayer GruntAudio { get; private set; }
        [field: SerializeField] public RandomAudioPlayer DeathAudio { get; private set; }
        [field: SerializeField] public RandomAudioPlayer SpottedAudio { get; private set; }
        [field: SerializeField] public RandomAudioPlayer InvulnerableAudio { get; private set; }
        #endregion

        protected float lastTimeAttacked = float.NegativeInfinity;
        public bool CanAttackAgain => (lastTimeAttacked + AttackCooldown) <= Time.time;

        [Tooltip("Time in seconds before the unit stops pursuing the player when the player is out of sight")]
        public float timeToStopPursuit;
        protected float m_TimerSinceLostTarget = 0.0f;

        [Header("Lifebar")]
        public Slider lifebar;
        public RawImage gluttonyIcon;
        public RawImage prideIcon;
        public RawImage wrathIcon;

        public Vector3 OriginalPosition { get; protected set; }
        public Quaternion OriginalRotation { get; protected set; }

        protected virtual void OnEnable()
        {
            controller = GetComponentInChildren<EnemyController>();
            col = GetComponent<Collider>();
            OriginalPosition = transform.position;
        }

        protected virtual void OnDisable()
        {
            if (followerInstance != null)
                followerInstance.distributor.UnregisterFollower(followerInstance);
        }

        protected override void Start()
        {
            if (gluttonyIcon != null)
                gluttonyIcon.enabled = false;
            if (prideIcon != null)
                prideIcon.enabled = false;
            if (wrathIcon != null)
                wrathIcon.enabled = false;
            bonusDmg = WrathDmgBonus * EnemyManager.Instance.EnemyScaling.GetSinMod(SinType.Ira);
            bonusLife = GluttonyLifeBonus * EnemyManager.Instance.EnemyScaling.GetSinMod(SinType.Gula);
            bonusRange = PrideRangeBonus * EnemyManager.Instance.EnemyScaling.GetSinMod(SinType.Superbia);
            controller.navmeshAgent.speed = MovementSpeed;
            SceneLinkedSMB<EnemyUnit>.Initialise(controller.animator, this);
            baseHitAngle = hitAngle;
            OriginalRotation = transform.rotation;
            ResetDamage();
            CheckforIcons();
        }

        protected virtual void FixedUpdate()
        {
            controller.animator.SetBool(hashGrounded, controller.grounded);

            Vector3 toBase = OriginalPosition - transform.position;
            toBase.y = 0;

            controller.animator.SetBool(hashNearBase, toBase.sqrMagnitude < 0.1 * 0.1f);
            if (toBase.sqrMagnitude < 0.1 * 0.1f && target == null)
            {
                ResetDamage();
                ResetHitAngle();
                transform.rotation = OriginalRotation;
            }
            controller.animator.SetFloat(hashSpeed, controller.navmeshAgent.velocity.magnitude / controller.navmeshAgent.speed);
        }

        #region Play Audio Methods
        public void Grunt()
        {
            if (GruntAudio != null)
                GruntAudio.PlayRandomClip();
        }

        public void Spotted()
        {
            if (SpottedAudio != null)
                SpottedAudio.PlayRandomClip();
        }
        #endregion

        #region Target Methods
        public void ForceFindTarget()
        {
            controller.animator.SetTrigger(hashSpotted);
            target = PlayerController.Instance;
            TargetDistributor distributor = target.GetComponentInChildren<TargetDistributor>();
            if (distributor != null)
                followerInstance = distributor.RegisterNewFollower();
        }

        public void FindTarget()
        {
            //we ignore height difference if the target was already seen
            PlayerController target = playerScanner.Detect(transform, this.target == null);

            if (this.target == null)
            {
                //we just saw the player for the first time, pick an empty spot to target around them
                if (target != null)
                {
                    controller.animator.SetTrigger(hashSpotted);
                    this.target = target;
                    TargetDistributor distributor = target.GetComponentInChildren<TargetDistributor>();
                    if (distributor != null)
                        followerInstance = distributor.RegisterNewFollower();
                }
            }
            else
            {
                //we lost the target. But chomper have a special behaviour : they only loose the player scent if they move past their detection range
                //and they didn't see the player for a given time. Not if they move out of their detectionAngle. So we check that this is the case before removing the target
                if (target == null)
                {
                    m_TimerSinceLostTarget += Time.deltaTime;
                    print(m_TimerSinceLostTarget);

                    if (m_TimerSinceLostTarget >= timeToStopPursuit)
                    {
                        Vector3 toTarget = this.target.transform.position - transform.position;
                        if (followerInstance != null)
                            followerInstance.distributor.UnregisterFollower(followerInstance);

                        //the target move out of range, reset the target
                        this.target = null;

                        //This will null the target if the player runs too far away from monster which is in itself a good test but needed at the moment
                        //if (toTarget.sqrMagnitude > playerScanner.detectionRadius * playerScanner.detectionRadius)
                        //{
                        //    if (followerInstance != null)
                        //        followerInstance.distributor.UnregisterFollower(followerInstance);

                        //    //the target move out of range, reset the target
                        //    this.target = null;
                        //}
                    }
                }
                else
                {
                    if (target != this.target)
                    {
                        if (followerInstance != null)
                            followerInstance.distributor.UnregisterFollower(followerInstance);

                        this.target = target;

                        TargetDistributor distributor = target.GetComponentInChildren<TargetDistributor>();
                        if (distributor != null)
                            followerInstance = distributor.RegisterNewFollower();
                    }

                    m_TimerSinceLostTarget = 0.0f;
                }
            }
        }

        public void RequestTargetPosition()
        {
            Vector3 fromTarget = transform.position - target.transform.position;
            fromTarget.y = 0;

            followerInstance.requiredPoint = target.transform.position + fromTarget.normalized * AttackRange * 0.9f;
        }
        #endregion

        #region Pursue Methods
        public void StartPursuit()
        {
            if (followerInstance != null)
            {
                followerInstance.requireSlot = true;
                RequestTargetPosition();
            }

            controller.animator.SetBool(hashInPursuit, true);
        }

        public void StopPursuit()
        {
            if (followerInstance != null)
            {
                followerInstance.requireSlot = false;
            }

            controller.animator.SetBool(hashInPursuit, false);
        }

        public void WalkBackToBase()
        {
            if (followerInstance != null)
                followerInstance.distributor.UnregisterFollower(followerInstance);
            target = null;
            StopPursuit();
            controller.SetTarget(OriginalPosition);
            controller.SetFollowNavmeshAgent(true);
        }
        #endregion

        #region Attack Methods
        public void TriggerAttack()
        {
            controller.animator.SetTrigger(hashAttack);
        }

        public abstract void AttackBegin();

        public abstract void AttackEnd();
        #endregion

        #region Take Damage Methods
        public override void ApplyDamage(DamageMessage msg, Action<EnemyUnit> killCallback = null)
        {
            if (IsInvulnerable || msg.damage <= 0 || CurrentHealth <= 0)
                return;

            // Determine if the unit can be hit from a certain angle
            Vector3 forward = transform.forward;
            forward = Quaternion.AngleAxis(hitForwardRotation, transform.up) * forward;

            //we project the direction to damager to the plane formed by the direction of damage
            Vector3 positionToDamager = msg.damageSource - transform.position;
            positionToDamager -= transform.up * Vector3.Dot(transform.up, positionToDamager);
            if (Vector3.Angle(forward, positionToDamager) > hitAngle * 0.5f)
            {
                controller.animator.SetBool(hashInvulnerableHit, true);
                if (InvulnerableAudio != null)
                    InvulnerableAudio.PlayRandomClip();
                return;
            }
            // Determine if the unit can be hit from a certain angle

            int actualDamage = (int)msg.damage;
            if (CurrentHealth - msg.damage < 0)
                actualDamage = actualDamage + (int)(CurrentHealth - msg.damage);
            if (actualDamage > 0)
                GameEvents.spawnCombatTextEvent?.Invoke(CombatText.CombatTextType.Damage, actualDamage.ToString(), transform.position + Vector3.up * col.bounds.size.y);
            CurrentHealth = Mathf.Clamp(CurrentHealth - msg.damage, 0, MaxHealth);

            if (lifebar != null)
                lifebar.value = (float)CurrentHealth / MaxHealth;

            msg.direction.y = 0;

            float hitDirectionX = Vector3.Dot(transform.right, msg.direction);
            float hitDirectionZ = Vector3.Dot(transform.forward * -1, msg.direction);

            Vector3 pushForce = transform.position - msg.damager.transform.position;

            pushForce.y = 0;

            //transform.forward = -pushForce.normalized;
            //controller.AddForce(pushForce.normalized * 5.5f, false);

            controller.animator.SetFloat(hashHitDirectionX, hitDirectionX);
            controller.animator.SetFloat(hashHitDirectionZ, hitDirectionZ);

            controller.animator.SetBool(hashHit, true);

            if (WoundedAudio != null)
                WoundedAudio.PlayRandomClip();

            if (CurrentHealth <= 0)
            {
                killCallback?.Invoke(this);
                Death(msg);
            }
        }

        public virtual void Death(DamageMessage msg)
        {
            if (_isBoss)
                GameEvents.bossEnemyDeathEvent?.Invoke(this);
            else
                GameEvents.trashEnemyDeathEvent?.Invoke(this);

            lifebar.gameObject.SetActive(false);

            //Vector3 pushForce = transform.position - msg.damager.transform.position;

            //pushForce.y = 0;

            //transform.forward = -pushForce.normalized;
            //controller.AddForce(pushForce.normalized * 7.0f - Physics.gravity * 0.6f);

            //controller.animator.SetTrigger(hashHit);
            controller.animator.SetTrigger(hashDeath);

            //We unparent the hit source, as it would destroy it with the gameobject when it get replaced by the ragdol otherwise
            if (DeathAudio != null)
            {
                DeathAudio.transform.SetParent(null, true);
                DeathAudio.PlayRandomClip();
                Destroy(DeathAudio, DeathAudio.clip == null ? 0.0f : DeathAudio.clip.length + 0.5f);
            }
        }
        #endregion

        public override void ResetDamage()
        {
            CurrentHealth = MaxHealth;
            if (lifebar != null)
                lifebar.value = (float)CurrentHealth / MaxHealth;
        }

        public virtual void UpdateBonus()
        {
            var previousBonusLife = bonusLife;

            bonusDmg = WrathDmgBonus * EnemyManager.Instance.EnemyScaling.GetSinMod(SinType.Ira);
            bonusLife = GluttonyLifeBonus * EnemyManager.Instance.EnemyScaling.GetSinMod(SinType.Gula);
            bonusRange = PrideRangeBonus * EnemyManager.Instance.EnemyScaling.GetSinMod(SinType.Superbia);
            CurrentHealth = Mathf.Clamp(CurrentHealth + (bonusLife - previousBonusLife), 0, MaxHealth);
            CheckforIcons();
        }

        private void CheckforIcons()
        {
            if (EnemyManager.Instance.EnemyScaling.currentWrathEnemyCount > 0)
            {
                wrathIcon.enabled = true;
            }
            if (EnemyManager.Instance.EnemyScaling.currentGluttonyEnemyCount > 0)
            {
                gluttonyIcon.enabled = true;
            }
            if (EnemyManager.Instance.EnemyScaling.currentPrideEnemyCount > 0)
            {
                prideIcon.enabled = true;
            }
        }

        public void ApplyHardCC(bool value)
        {
            if (value)
            {
                //TODO: Dont disable and enable, move into a state where the enemyunit can't exit until told otherwise
                controller.animator.enabled = false;
                controller.SetFollowNavmeshAgent(false);
            }
            else
            {
                controller.animator.enabled = true;
                controller.SetFollowNavmeshAgent(true);
            }
        }

        public void SetNewHitAngle(float hitAngle)
        {
            this.hitAngle = hitAngle;
        }

        public void ResetHitAngle()
        {
            hitAngle = baseHitAngle;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            playerScanner.EditorGizmo(transform);

            Gizmos.DrawWireSphere(transform.position, AttackRange);

            Vector3 forward = transform.forward;
            forward = Quaternion.AngleAxis(hitForwardRotation, transform.up) * forward;

            if (Event.current.type == EventType.Repaint)
            {
                UnityEditor.Handles.color = Color.blue;
                UnityEditor.Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(forward), 1.0f,
                    EventType.Repaint);
            }

            UnityEditor.Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            forward = Quaternion.AngleAxis(-hitAngle * 0.5f, transform.up) * forward;
            UnityEditor.Handles.DrawSolidArc(transform.position, transform.up, forward, hitAngle, 1.0f);
        }
#endif
    }
}
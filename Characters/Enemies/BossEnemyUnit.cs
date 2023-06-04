using SaligiaProofOfVision.Abilities;
using System;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

namespace SaligiaProofOfVision
{
    public class BossEnemyUnit : EnemyUnit
    {
        [field: SerializeField] public Leap LeapAbility { get; private set; }
        [field: SerializeField] public Slam SlamAbility { get; private set; }
        [SerializeField] private GameObject _abilityIndicator;
        private RectTransform _abilityRect;

        [Header("UI Components")]
        [SerializeField] private float _barAnimationSpeed;
        [SerializeField] private Image _healthBar;
        [SerializeField] private Image _healthBarAnimation;

        public bool LeapAbilityOngoing { get; set; }
        public bool SlamAbilityOngoing { get; set; }

        //private VisualElement _healthBar;

        //private void OnEnable()
        //{
        //    var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        //    _healthBar = rootVisualElement.Q<VisualElement>("Bar_Boss_Foreground");
        //}

        new protected void Start()
        {
            base.Start();
            //movementVector = Vector3.zero;
            LeapAbility.Cooldown.SetBase();
            LeapAbilityOngoing = false;
            SlamAbilityOngoing = false;
            _abilityRect = _abilityIndicator.GetComponent<RectTransform>();
            _healthBar.fillAmount = CurrentHealth / MaxHealth;
            _healthBarAnimation.fillAmount = _healthBar.fillAmount;
        }

        private void Update()
        {
            UpdateBars();
            if (!LeapAbility.IsUsable)
                LeapAbility.Tick();
            if (!SlamAbility.IsUsable)
                SlamAbility.Tick();
        }

        //public override void Attack(IDamageable damageable)
        //{
        //    if (lastTimeAttacked + (1 / attackSpeed) > Time.time)
        //        return;

        //    animator.SetTrigger("Attack");

        //    DamageMessage msg;
        //    msg.amount = attackDamage;
        //    msg.damager = this;
        //    msg.direction = Vector3.zero;

        //    damageable.ApplyDamage(msg);
        //    lastTimeAttacked = Time.time;
        //}

        public void UseAbility(Ability ability)
        {
            if (ability.GetType() == typeof(Leap))
            {
                _abilityIndicator.SetActive(true);
                _abilityRect.sizeDelta = Vector2.one * ((Leap)ability).Radius;
                //_abilityRect.transform.position = player.transform.position;
                LeapAbilityOngoing = true;
                StartCoroutine(ability.Use(this, _abilityRect));
            }
            else if (ability.GetType() == typeof(Slam))
            {
                _abilityIndicator.SetActive(true);
                _abilityRect.sizeDelta = Vector2.one * ((Slam)ability).Radius;
                var p = transform.position;
                p.y = 0.01f;
                _abilityRect.transform.position = p;
                SlamAbilityOngoing = true;
                StartCoroutine(ability.Use(this));
            }
        }

        public void SetAbilityIndicatorActive(bool enable)
        {
            _abilityIndicator.SetActive(enable);
        }

        public override void ApplyDamage(DamageMessage dmgMessage, Action<EnemyUnit> KillCallback)
        {
            base.ApplyDamage(dmgMessage);
            UpdateBars();
        }

        private void UpdateBars()
        {
            _healthBar.fillAmount = CurrentHealth / MaxHealth;
            _healthBarAnimation.fillAmount = Mathf.Lerp(_healthBarAnimation.fillAmount, _healthBar.fillAmount, _barAnimationSpeed * Time.deltaTime);
        }

        public override void AttackBegin()
        {
            throw new NotImplementedException();
        }

        public override void AttackEnd()
        {
            throw new NotImplementedException();
        }
    }
}
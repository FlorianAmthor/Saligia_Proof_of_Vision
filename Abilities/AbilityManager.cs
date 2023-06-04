using Gamekit3D;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SaligiaProofOfVision.Abilities
{
    public class AbilityManager : MonoBehaviour
    {
        [SerializeField] private List<Ability> _abilities;
        public Ability TargetingAbility { get; private set; }
        public ReadOnlyCollection<Ability> Abilities => _abilities.AsReadOnly();

        [Header("Targeting Indicators")]
        [SerializeField] private GameObject _circleIndicator;
        [SerializeField] private GameObject _linearIndicator;
        [SerializeField] private float _targetingSensitivity;

        [Space]
        [SerializeField] private Camera _mainCamera;

        private Vector3 _rotation;
        private Vector3 _targetingMove;
        private Player _player;
        private AbilitySlot _abilitySlotForEvent;

        private TargetingType _targetingType = TargetingType.None;

        private void Start()
        {
            TargetingAbility = null;
            _player = GetComponent<Player>();

            foreach (var ability in _abilities)
            {
                if (ability.GetType() == typeof(Scythe))
                {
                    var s = GetComponentInChildren<MeleeWeapon>();
                    ability.Init(s);
                    continue;
                }
                ability.Init();
            }
        }

        private void OnEnable()
        {
            GameEvents.resetGameEvent += OnResetGame;
            GameEvents.moveEvent += OnMove;
            GameEvents.primaryAbilityEvent += OnPrimaryAbility;
            GameEvents.secondaryAbilityOneEvent += OnSecondaryAbilityOne;
            GameEvents.secondaryAbilityTwoEvent += OnSecondaryAbilityTwo;
            GameEvents.movementAbilityEvent += OnMovementAbility;
            GameEvents.upgradeAbilityEvent += OnUpgradeAbility;
        }

        private void OnDisable()
        {
            GameEvents.resetGameEvent += OnResetGame;
            GameEvents.moveEvent -= OnMove;
            GameEvents.primaryAbilityEvent -= OnPrimaryAbility;
            GameEvents.secondaryAbilityOneEvent -= OnSecondaryAbilityOne;
            GameEvents.secondaryAbilityTwoEvent -= OnSecondaryAbilityTwo;
            GameEvents.movementAbilityEvent -= OnMovementAbility;
            GameEvents.upgradeAbilityEvent -= OnUpgradeAbility;
        }

        private void Update()
        {
            for (int i = 0; i < _abilities.Count; i++)
            {
                var ability = _abilities[i];
                if (!ability.IsUsable)
                    GameEvents.cooldownUpdateEvent?.Invoke(i, ability.Cooldown.Value, ability.Cooldown.BaseValue);
                else
                    GameEvents.cooldownEndEvent?.Invoke(i);

                ability.Tick(_player.gameObject);
            }

            if (_targetingType != TargetingType.None)
            {
                switch (_targetingType)
                {
                    case TargetingType.Circle:
                        if (!_circleIndicator.activeSelf)
                            _circleIndicator.SetActive(true);
                        var rectTransform = _circleIndicator.GetComponent<RectTransform>();
                        var radius = ((Barrage)TargetingAbility).Radius;
                        rectTransform.sizeDelta = new Vector2(radius, radius);
                        var newPos = _circleIndicator.transform.position + _targetingMove;
                        var direction = newPos - transform.position;
                        Barrage b = (Barrage)TargetingAbility;
                        if (direction.magnitude > b.MaxRange)
                            _circleIndicator.transform.position = transform.position + direction.normalized * b.MaxRange;
                        else
                            _circleIndicator.transform.position = newPos;
                        var updateVec = _circleIndicator.transform.position;
                        bool isTerrain = Terrain.activeTerrains.Length > 0;
                        if (isTerrain)
                            updateVec.y = Terrain.activeTerrain.SampleHeight(_circleIndicator.transform.position);
                        TargetingAbility.UpdateTarget(updateVec);
                        _rotation = _circleIndicator.transform.position - transform.position;
                        _rotation = Vector3.Scale(_rotation, Vector3.forward + Vector3.right);
                        break;
                    case TargetingType.Linear:
                        if (!_linearIndicator.activeSelf)
                            _linearIndicator.SetActive(true);
                        _linearIndicator.transform.position = transform.position + (transform.forward * 2.8f);
                        _linearIndicator.transform.forward = transform.forward;
                        TargetingAbility.UpdateTarget(transform.forward);
                        break;
                    default:
                        break;
                }
                if (_rotation != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(_rotation, Vector3.up);
            }
        }

        #region Input Callbacks
        private void OnMove(Vector2 vec)
        {
            _rotation = new Vector3(vec.x, 0, vec.y);
            _rotation = Quaternion.Euler(0, _mainCamera.transform.rotation.eulerAngles.y, 0) * _rotation;

            if (_rotation != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(_rotation, Vector3.up);

            _targetingMove = _rotation * Time.deltaTime * _targetingSensitivity;
        }

        public void OnResetGame()
        {
            _targetingType = TargetingType.None;
            if (_circleIndicator != null)
                _circleIndicator.SetActive(false);
            if (_linearIndicator != null)
                _linearIndicator.SetActive(false);

            foreach (var ability in _abilities)
            {
                ability.Cooldown.SetZero();
                ability.ApplyT1Mod(false);
                ability.ApplyT2Mod(false);
            }
        }

        private void OnPrimaryAbility(InputAction.CallbackContext context)
        {
            if (_isCasting)
                return;
            if (context.started)
                AbilityUsage(AbilitySlot.PrimaryAbiltiy, context);
        }

        private void OnSecondaryAbilityOne(InputAction.CallbackContext context)
        {
            if (_isCasting && TargetingAbility != _abilities[(int)AbilitySlot.SecondaryAbility1])
                return;
            if (context.started || context.canceled)
            {
                _circleIndicator.transform.position = transform.position;
                AbilityUsage(AbilitySlot.SecondaryAbility1, context);
            }
        }

        private void OnSecondaryAbilityTwo(InputAction.CallbackContext context)
        {
            if (_isCasting && TargetingAbility != _abilities[(int)AbilitySlot.SecondaryAbility2])
                return;
            if (context.started || context.canceled)
            {
                _circleIndicator.transform.position = transform.position;
                AbilityUsage(AbilitySlot.SecondaryAbility2, context);
            }
        }

        private void OnMovementAbility(InputAction.CallbackContext context)
        {
            if (context.started)
                AbilityUsage(AbilitySlot.MovementAbility, context);
        }
        #endregion

        private void OnUpgradeAbility(AbilitySlot abilitySlot, bool isT1)
        {
            if (isT1)
            {
                _abilities[(int)abilitySlot].ApplyT1Mod(true);
                switch (abilitySlot)
                {
                    case AbilitySlot.PrimaryAbiltiy:
                        _abilities[(int)AbilitySlot.SecondaryAbility1].ApplyT1Mod(false);
                        _abilities[(int)AbilitySlot.SecondaryAbility2].ApplyT1Mod(false);
                        break;
                    case AbilitySlot.SecondaryAbility1:
                        _abilities[(int)AbilitySlot.PrimaryAbiltiy].ApplyT1Mod(false);
                        _abilities[(int)AbilitySlot.SecondaryAbility2].ApplyT1Mod(false);
                        break;
                    case AbilitySlot.SecondaryAbility2:
                        _abilities[(int)AbilitySlot.PrimaryAbiltiy].ApplyT1Mod(false);
                        _abilities[(int)AbilitySlot.SecondaryAbility1].ApplyT1Mod(false);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                _abilities[(int)abilitySlot].ApplyT2Mod(true);
                switch (abilitySlot)
                {
                    case AbilitySlot.PrimaryAbiltiy:
                        _abilities[(int)AbilitySlot.SecondaryAbility1].ApplyT2Mod(false);
                        _abilities[(int)AbilitySlot.SecondaryAbility2].ApplyT2Mod(false);
                        break;
                    case AbilitySlot.SecondaryAbility1:
                        _abilities[(int)AbilitySlot.PrimaryAbiltiy].ApplyT2Mod(false);
                        _abilities[(int)AbilitySlot.SecondaryAbility2].ApplyT2Mod(false);
                        break;
                    case AbilitySlot.SecondaryAbility2:
                        _abilities[(int)AbilitySlot.PrimaryAbiltiy].ApplyT2Mod(false);
                        _abilities[(int)AbilitySlot.SecondaryAbility1].ApplyT2Mod(false);
                        break;
                    default:
                        break;
                }
            }
        }

        private void AbilityUsage(AbilitySlot abilitySlot, InputAction.CallbackContext context)
        {
            if (UseAbility(abilitySlot, context, out _targetingType))
            {
                _targetingType = TargetingType.None;
                _circleIndicator.SetActive(false);
                _linearIndicator.SetActive(false);
            }
            else if (TargetingAbility == null)
            {
                _targetingType = TargetingType.None;
                _circleIndicator.SetActive(false);
                _linearIndicator.SetActive(false);
            }
        }

        private bool _isCasting;

        private void TargetAbility(AbilitySlot abilitySlot)
        {
            GameEvents.targetingStartEvent?.Invoke();
            TargetingAbility = _abilities[(int)abilitySlot];
            _isCasting = true;
        }

        public bool UseAbility(AbilitySlot abilitySlot, InputAction.CallbackContext context, out TargetingType targetingType)
        {
            var ability = _abilities[(int)abilitySlot];
            targetingType = TargetingType.None;
            if (_player.CurrentMana < ability.ManaCost)
                return false;
            if (ability.IsUsable)
            {
                if (ability.HasPreview && context.started && !_isCasting)
                {
                    targetingType = ability.TargetingType;
                    TargetAbility(abilitySlot);
                    _player.Animator.SetTrigger("beginCast");
                    return false;
                }
                else if (!ability.HasPreview && context.started || ability.HasPreview && context.canceled && TargetingAbility != null)
                {
                    TargetingAbility = null;
                    GameEvents.targetingEndEvent?.Invoke();
                    _abilitySlotForEvent = abilitySlot;

                    if (ability.HasPreview)
                        _player.Animator.SetTrigger("endCast");
                    else
                        OnEndCast(null);

                    return true;
                }
            }
            return false;
        }

        public void OnEndCast(AnimationEvent animationEvent)
        {
            _isCasting = false;
            if (_abilitySlotForEvent < 0)
                return;
            var ability = _abilities[(int)_abilitySlotForEvent];
            StartCoroutine(ability.Use(_player));

            //deactivated UI cooldown for primary ability
            if (_abilitySlotForEvent != AbilitySlot.PrimaryAbiltiy)
                GameEvents.cooldownStartEvent?.Invoke((int)_abilitySlotForEvent, ability.Cooldown.BaseValue);

            if (_abilitySlotForEvent != AbilitySlot.PrimaryAbiltiy)
                _player.RecudeMana(ability.ManaCost);

            _abilitySlotForEvent = AbilitySlot.None;
        }

        public void StartAttack()
        {
            ((Scythe)_abilities[0]).MeleeWeapon.BeginAttack();
        }

        public void EndAttack()
        {
            ((Scythe)_abilities[0]).MeleeWeapon.EndAttack();
        }
    }
}
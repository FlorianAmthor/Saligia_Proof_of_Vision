using System.Collections;
using UnityEngine;

namespace SaligiaProofOfVision.Abilities
{
    public abstract partial class Ability : ScriptableObject
    {
        #region Exposed Protected Fields
        [SerializeField] protected new string name;
        [SerializeField] protected string description;
        [SerializeField, Tooltip("A negative value will mean this ability generates mana with each use.")]
        protected int baseManaCost;
        [SerializeField] protected bool hasPreview;
        [SerializeField] protected Sprite abilityImage;
        [SerializeField] protected LayerMask afflictedObjects;
        [SerializeField] protected Cooldown cooldown;
        [SerializeField] protected TargetingType targetingType;
        [SerializeField] protected float gulaModifier;
        #endregion

        #region Properties
        public string Name => name;

        public int ManaCost => (int)(baseManaCost * manaCostModifier);
        public string Description => description;
        public Sprite AbilityImage => abilityImage;
        public Cooldown Cooldown => cooldown;
        public virtual bool IsUsable => cooldown.Value <= 0;
        public bool HasPreview { get => hasPreview; protected set => hasPreview = value; }
        public TargetingType TargetingType => targetingType;
        
        [field: SerializeField]
        public bool HasT2Rune { get; protected set; }
        #endregion

        protected float manaCostModifier = 1.0f;

        private void OnEnable()
        {
            cooldown.SetZero();
        }

        public virtual void Init(params object[] optionalData)
        {
            cooldown.SetZero();
        }

        public virtual void UpdateTarget(Vector3 vec) { }
        public virtual void Tick(GameObject obj = null)
        {
            if (!IsUsable)
                OnCooldown();
        }
        public abstract IEnumerator Use(BaseUnit owner = null, params object[] args);

        protected void OnCooldown()
        {
            cooldown.Reduce();
        }

        public void ApplyT1Mod(bool apply)
        {
            if (apply)
                manaCostModifier = gulaModifier;
            else
                manaCostModifier = 1.0f;
        }

        public void ApplyT2Mod(bool apply)
        {
            HasT2Rune = apply;
        }
    }
}
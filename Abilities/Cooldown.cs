using System;
using UnityEngine;

namespace SaligiaProofOfVision.Abilities
{
    [Serializable]
    public class Cooldown
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private float _baseValue;
#pragma warning restore 649
        #endregion

        #region Properties
        public float Value { get; private set; }
        public float BaseValue { get => _baseValue; }
        #endregion

        /// <summary>
        /// Constructor for Cooldown
        /// </summary>
        /// <param name="baseValue">Base Cooldown Value</param>
        public Cooldown(float baseValue)
        {
            _baseValue = baseValue;
            SetZero();
        }

        /// <summary>
        /// Constructor for Cooldown
        /// </summary>
        /// <param name="coolDownObj">Existing Cooldown Object</param>
        public Cooldown(Cooldown coolDownObj)
        {
            _baseValue = coolDownObj.BaseValue;
            SetZero();
        }

        #region Public Methods
        /// <summary>
        /// Sets the cooldown value to zero.
        /// </summary>
        public void SetZero()
        {
            Value = 0;
        }
        /// <summary>
        /// Sets the cooldown value to the base value
        /// </summary>
        public void SetBase()
        {
            Value = _baseValue;
        }
        /// <summary>
        /// Reduces the current cooldown value by amount
        /// </summary>
        public void Reduce()
        {
            Value = Mathf.Clamp(Value - Time.deltaTime, 0, Value);
        }
        #endregion
    }
}
using SaligiaProofOfVision;
using SaligiaProofOfVision.Abilities;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static CombatText;

[CreateAssetMenu(fileName = "GameEvents", menuName = "Events/GameEvents")]
public class GameEvents : ScriptableObject
{
    //TODO: Custom Editor to Display the game events maybe with parameters
    #region Input Events
    public static Action<Vector2> moveEvent;
    public static Action<InputAction.CallbackContext> primaryAbilityEvent;
    public static Action<InputAction.CallbackContext> secondaryAbilityOneEvent;
    public static Action<InputAction.CallbackContext> secondaryAbilityTwoEvent;
    public static Action<InputAction.CallbackContext> movementAbilityEvent;
    public static Action<string> controlsChangedEvent;

    public static Action menuEvent;
    public static Action abilityOverlayEvent;
    #endregion

    #region Player Events
    public static Action<float, float> healthChangedEvent;
    public static Action<float, float> manaChangedEvent;
    public static Action playerDeathEvent;
    #endregion

    #region Camera Events
    public static Action<CameraController.CameraSettings> changeCameraSettingsEvent;
    public static Action<float> cameraRotateEvent;
    #endregion

    #region AbilityEvents
    public static Action<int, float> cooldownStartEvent;
    public static Action<int, float, float> cooldownUpdateEvent;
    public static Action<int> cooldownEndEvent;
    public static Action targetingStartEvent;
    public static Action targetingEndEvent;
    public static Action<AbilitySlot, bool> upgradeAbilityEvent;
    #endregion

    #region Enemy Events
    public static Action<EnemyUnit> trashEnemyDeathEvent;
    public static Action<EnemyUnit> bossEnemyDeathEvent;
    #endregion

    #region UI Events
    public static Action<CombatTextType, string, Vector3> spawnCombatTextEvent;
    #endregion

    #region General Events
    public static Action resetGameEvent;
    #endregion
}

using Gamekit3D;
using System.Collections;
using UnityEngine;

namespace SaligiaProofOfVision.Abilities
{
    [CreateAssetMenu(fileName = "Barrage", menuName = "Abilities/Barrage")]
    public class Barrage : Ability
    {
        [SerializeField] private GameObject _barrageArea;

        [SerializeField] private float _radius;
        [SerializeField] private float _timeBetweenTicks;
        [SerializeField] private float _tickDamage;
        [SerializeField] private float _duration;
        [SerializeField] private float _rootDuration;
        private Vector3 _targetPosition;

        public float Radius => _radius;
        [field: SerializeField]
        public float MaxRange { get; private set; }

        private BarrageArea _loadedBarrageArea = null;
        private ObjectPooler<BarrageArea> _barrageAreaPool;

        private void OnDisable()
        {
            _loadedBarrageArea = null;
            _barrageAreaPool = null;
        }

        public override void UpdateTarget(Vector3 vec)
        {
            base.UpdateTarget(vec);
            _targetPosition = vec;
        }

        public override IEnumerator Use(BaseUnit owner = null, params object[] args)
        {
            if (_barrageAreaPool == null)
            {
                _barrageAreaPool = new ObjectPooler<BarrageArea>();
                _barrageAreaPool.Initialize(3, _barrageArea.GetComponent<BarrageArea>());
            }

            cooldown.SetBase();

            _loadedBarrageArea = _barrageAreaPool.GetNew();
            _loadedBarrageArea.transform.position = _targetPosition;
            _loadedBarrageArea.InitialSetup(_targetPosition, _radius, _timeBetweenTicks, _tickDamage, _duration, HasT2Rune, _rootDuration);
            _loadedBarrageArea = null;
            yield return "";
        }
    }
}
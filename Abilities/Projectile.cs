using Gamekit3D;
using UnityEngine;

namespace SaligiaProofOfVision
{
    public abstract class Projectile : MonoBehaviour, IPooled<Projectile>
    {
        public BaseUnit Owner { get; protected set; }

        protected Vector3 direction;
        protected float speed;
        protected float timeToLive;
        protected Rigidbody rb;
        protected float impactDamage;
        protected float timeActivated;

        public int poolID { get; set; }
        public ObjectPooler<Projectile> pool { get; set; }

        public virtual void InitialSetup(Vector3 dir, BaseUnit owner, float impactDamage, float speed, float timeToLive = -1, params object[] data)
        {
            if (rb == null)
                rb = GetComponent<Rigidbody>();
            Owner = owner;
            if (timeToLive != -1)
                timeActivated = timeToLive;
            this.impactDamage = impactDamage;
            this.timeToLive = timeToLive;
            this.speed = speed;
            direction = dir;
            timeActivated = Time.time;
            transform.forward = direction - transform.position;
            //transform.LookAt(dir);
        }

        protected virtual void Update()
        {
            if (timeToLive != -1)
                if (timeActivated + timeToLive <= Time.time)
                    gameObject.SetActive(false);
        }

        protected virtual void FixedUpdate()
        {
            rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
        }

        protected abstract void OnCollisionEnter(Collision collision);
    }
}
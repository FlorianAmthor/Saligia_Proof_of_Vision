using System.Collections;
using UnityEngine;

namespace SaligiaProofOfVision.Abilities
{
    [CreateAssetMenu(fileName = "Roll", menuName = "Abilities/Roll")]
    public class Roll : Ability
    {
        [SerializeField] private string _animationTrigger;

        //[SerializeField] private float _dashForce;
        //[SerializeField] private float _dashDuration;

        public override IEnumerator Use(BaseUnit owner = null, params object[] args)
        {
            cooldown.SetBase();
            var player = owner.GetComponent<Player>();
            player.Animator.SetTrigger(_animationTrigger);
            yield return new WaitForEndOfFrame();
            //rb.AddForce(obj.transform.forward * _dashForce, ForceMode.VelocityChange);
            //player.Animator.SetTrigger("Dash");
            //yield return new WaitForSeconds(_dashDuration);
            //if(player.RunAnimation)
            //    player.Animator.SetTrigger("Run");
            //else
            //    player.Animator.SetTrigger("Stand"); 
            //rb.velocity = Vector3.zero;
        }
    }
}
namespace SaligiaProofOfVision
{
    public class GhoulRedUnit : MeleeEnemyUnit
    {
        new protected void Start()
        {
            base.Start();
            SceneLinkedSMB<GhoulRedUnit>.Initialise(controller.animator, this);
        }
    }
}
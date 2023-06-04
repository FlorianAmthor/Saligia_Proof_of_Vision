namespace SaligiaProofOfVision
{
    public class GhoulBlack : MeleeEnemyUnit
    {
        //TODO: Block Ability --> see skelton warrior for reference
        new protected void Start()
        {
            base.Start();
            SceneLinkedSMB<GhoulBlack>.Initialise(controller.animator, this);
        }
    }
}
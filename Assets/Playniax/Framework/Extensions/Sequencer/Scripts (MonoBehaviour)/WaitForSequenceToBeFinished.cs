using Playniax.Ignition;

namespace Playniax.Sequencer
{
    public class WaitForSequenceToBeFinished : SequenceBase
    {
        public override void OnSequencerUpdate()
        {
            if (Register.count == 0)
            {
                if (GameData.spawned > 0 && GameData.bodyCount > 0 && GameData.spawned == GameData.bodyCount) GameData.completed += 1;

                GameData.spawned = 0;
                GameData.bodyCount = 0;

                enabled = false;
            }
        }
    }
}
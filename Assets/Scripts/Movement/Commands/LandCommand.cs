using Movement.Components;

namespace Movement.Commands
{
    class LandCommand : AMoveCommand
    {
        public LandCommand(IJumperReceiver receiver) : base(receiver)
        {
        }

        public override void Execute()
        {
            ((IJumperReceiver)Player).Jump(IJumperReceiver.JumpStage.Landing);
        }
    }
}

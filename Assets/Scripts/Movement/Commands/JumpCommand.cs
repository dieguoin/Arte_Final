using Movement.Components;

namespace Movement.Commands
{
    class JumpCommand : AMoveCommand
    {
        public JumpCommand(IJumperReceiver receiver) : base(receiver)
        {
        }

        public override void Execute()
        {
            ((IJumperReceiver)Player).Jump(IJumperReceiver.JumpStage.Jumping);
        }
    }
}

using Movement.Components;

namespace Movement.Commands
{
    public class WalkRightCommand : AMoveCommand
    {
        public WalkRightCommand(IMoveableReceiver receiver) : base(receiver)
        {
        }

        public override void Execute()
        {
            ((IMoveableReceiver)Player).Move(IMoveableReceiver.Direction.Right);
        }
    }
}

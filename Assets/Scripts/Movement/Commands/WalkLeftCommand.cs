using Movement.Components;

namespace Movement.Commands
{
    public class WalkLeftCommand : AMoveCommand
    {
        public WalkLeftCommand(IMoveableReceiver receiver) : base(receiver)
        {
        }

        public override void Execute()
        {
            ((IMoveableReceiver)Player).Move(IMoveableReceiver.Direction.Left);
        }
    }
}

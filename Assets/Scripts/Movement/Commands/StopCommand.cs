using Movement.Components;

namespace Movement.Commands
{
    public class StopCommand : AMoveCommand
    {
        public StopCommand(IMoveableReceiver receiver) : base(receiver)
        {
        }

        public override void Execute()
        {
            ((IMoveableReceiver)Player).Move(IMoveableReceiver.Direction.None);
        }
    }
}

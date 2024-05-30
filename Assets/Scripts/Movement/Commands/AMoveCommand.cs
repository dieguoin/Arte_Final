using Movement.Components;

namespace Movement.Commands
{
    public abstract class AMoveCommand : ICommand
    {
        protected readonly IRecevier Player;

        protected AMoveCommand(IRecevier receiver)
        {
            Player = receiver;
        }

        public abstract void Execute();
    }
}

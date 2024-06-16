using Movement.Components;

namespace Movement.Commands
{
    public interface ICommand
    {
        public void Execute();
    }

    public abstract class AFightCommand : ICommand
    {
        protected readonly IFighterReceiver Player;

        protected AFightCommand(IFighterReceiver receiver)
        {
            Player = receiver;
        }

        public abstract void Execute();
    }

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
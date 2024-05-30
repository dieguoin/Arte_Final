using Movement.Components;

namespace Movement.Commands
{
    public abstract class AFightCommand : ICommand
    {
        protected readonly IFighterReceiver Player;

        protected AFightCommand(IFighterReceiver receiver)
        {
            Player = receiver;
        }

        public abstract void Execute();
    }
}
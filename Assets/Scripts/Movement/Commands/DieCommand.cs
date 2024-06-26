﻿using Movement.Components;

namespace Movement.Commands
{
    public class DieCommand : AFightCommand
    {
        public DieCommand(IFighterReceiver receiver) : base(receiver)
        {
        }

        public override void Execute()
        {
            Player.Die(1);
        }
    }
}
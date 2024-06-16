using Movement.Components;

namespace Movement.Commands
{
    public class DodgeCommand : AFightCommand
    {
        public DodgeCommand(IFighterReceiver receiver) : base(receiver)
        {

        }

        public override void Execute()
        {
            Player.Dodge();
        }
    }

    public class AttackCommand : AFightCommand
    {
        public AttackCommand(IFighterReceiver receiver) : base(receiver) { }

        public override void Execute()
        {
            Player.Attack();
        }
    }

    public class BlockCommand : AFightCommand
    {
        public BlockCommand(IFighterReceiver receiver) : base(receiver)
        { }

        public override void Execute()
        {
            Player.Block();
        }
    }

    public class CancelBlockCommand : AFightCommand
    {
        public CancelBlockCommand(IFighterReceiver receiver) : base(receiver) { }

        public override void Execute()
        {
            Player.CancelBlock();
        }
    }

    public class AdvancedAttackCommand : AFightCommand
    {
        public AdvancedAttackCommand(IFighterReceiver receiver) : base(receiver) { }

        public override void Execute()
        {
            Player.AdvancedAttack();
        }
    }

    public class AdvancedParryCommand : AFightCommand
    {
        public AdvancedParryCommand(IFighterReceiver receiver) : base(receiver) { }

        public override void Execute()
        {
            Player.AdvancedParry();
        }
    }

    public class AdvancedDodgeCommand : AFightCommand
    {
        public AdvancedDodgeCommand(IFighterReceiver receiver) : base(receiver) { }

        public override void Execute()
        {
            Player.AdvancedDodge();
        }
    }

    public class HealCommand : AFightCommand
    {
        public HealCommand(IFighterReceiver receiver) : base(receiver) { }

        public override void Execute()
        {
            Player.Heal();
        }
    }
    

    //public class DieCommand : AFightCommand
    //{
    //    public DieCommand(IFighterReceiver receiver) : base(receiver)
    //    {
    //    }

    //    public override void Execute()
    //    {
    //        Player.Die(1);
    //    }
    //}

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

    //public class TakeHitCommand : AFightCommand
    //{
    //    public TakeHitCommand(IFighterReceiver receiver) : base(receiver)
    //    {
    //    }

    //    public override void Execute()
    //    {
    //        Player.TakeHit();
    //    }
    //}

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
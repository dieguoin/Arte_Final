namespace Movement.Components
{
    public interface IFighterReceiver : IRecevier
    {
        public void Dodge();
        public void Attack();
        public void Block();
        public void CancelBlock();
        public void AdvancedAttack();
        public void AdvancedParry();
        public void AdvancedDodge();
        public void Heal();
    }
}
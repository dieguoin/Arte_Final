namespace Movement.Components
{
    public interface ITakeDamage
    {
        public void TakeDamage(uint damage);
        public void EnviromentHeal(uint healPoints);
    }
}

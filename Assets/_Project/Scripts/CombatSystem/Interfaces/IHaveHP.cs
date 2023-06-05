namespace CombatSystem
{
    public interface IHaveHP
    {
        public void Damage(int amount);
        public void Heal(int amount);
    }
}

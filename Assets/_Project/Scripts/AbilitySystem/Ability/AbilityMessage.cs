namespace AbilitySystem
{
    public enum AbilityEvent
    {
        None,
        SuccessfulCast,
        NotEnoughMana,
        OnCooldown,
        InvalidTarget,
        NoValidTargets,
        AbilityUpgraded,
        UpgradeNotAllowed,

    }

    public struct AbilityMessage
    {
        public Ability Ability;
        public AbilityEvent Event;

        public AbilityMessage(Ability ability, AbilityEvent abilityEvent = AbilityEvent.None)
        {
            Ability = ability;
            Event = abilityEvent;
        }
    }
}

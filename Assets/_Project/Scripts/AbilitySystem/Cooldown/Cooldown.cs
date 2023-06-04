using System;
using System.Collections.Generic;

namespace InventorySystem
{
    public class Cooldown
    {
        private static Dictionary<Ability, Cooldown> _cooldowns;
        
        public static Cooldown GetCooldown()
        {

            return null;
        }

        public Cooldown(float duration, bool start = true)
        {
            if (start) StartCooldown();
        }

        public void StartCooldown()
        {
            throw new NotImplementedException();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boonDefinition : MonoBehaviour
{
    public class Boon
    {
        public string Mechanic;
        public string Rarity;
        public double Modifier;

        public Boon(string mechanic, string rarity, double modifier)
        {
            Mechanic = mechanic;
            Rarity = rarity;
            Modifier = modifier;
        }
    }
    //                              Mechanic       Rarity   Modifier
    Boon speedWheels = new Boon("Top Speed", "Common", 1.10);
    Boon springBoard = new Boon("Ollie Jump", "Common", 1.10);
    Boon pointBooaster = new Boon("Point Gain", "Rare", 1.10); // Later
    Boon coolTricks = new Boon("Crit Power", "Rare", 1.10); // Later
    Boon hqHelmet = new Boon("Recovery", "Rare", 1.10); // Later
    Boon ollieOllie = new Boon("Double Jump", "Epic", 2.00); // Later

    // The Rarity will affect how likely a Boon is to show up inbetween
    // levels. If that Boon is selected, the modifier needs to be sent to
    // the appropriate stat to multiply it.

    bool enableSpeedWheels()
    {
        return false;
        // When called, enableSpeedWheels will send its modifier to
        // the player's stat table 
    }

}

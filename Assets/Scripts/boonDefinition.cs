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

    Hashtable rarityWeight = new Hashtable();

    //                              Mechanic       Rarity   Modifier
    Boon speedWheels    = new Boon("Top Speed",   "Common", 1.10);
    Boon springBoard    = new Boon("Ollie Jump",  "Common", 1.10);
    Boon pointBooaster  = new Boon("Point Gain",  "Rare",   1.10);
    Boon coolTricks     = new Boon("Crit Power",  "Rare",   1.10);
    Boon hqHelmet       = new Boon("Recovery",    "Rare",   1.10);
    Boon ollieOllie     = new Boon("Double Jump", "Epic",   2.00);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rarityWeight.Add("Common", 0.50);
        rarityWeight.Add("Rare",   0.35);
        rarityWeight.Add("Epic",   0.15);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class playerStatDefinition : MonoBehaviour
// {
//     [Header("Player Stats")]
//     // [Tooltip("Text element to display the score that will be earned after a combo")]
//     public int kickoffSpeed;
//     // [Tooltip("Text element to display the score that will be earned after a combo")]
//     public int torqueForce;
//     // [Tooltip("Text element to display the score that will be earned after a combo")]
//     public int maxSpeed;
//     // [Tooltip("Text element to display the score that will be earned after a combo")]
//     public int ollieForce;

//     // For the love of god, DO NOT TOUCH
//     [Header("Physics Stats")]
//     [Tooltip("DEBUG: DO NOT TOUCH!!!!!\n")]
//     public int strength;
//     [Tooltip("DEBUG: DO NOT TOUCH!!!!!\n")]
//     public int length;
//     [Tooltip("DEBUG: DO NOT TOUCH!!!!!\n")]
//     public int dampening;

//     private double[] playerStats;

//     public class Boon
//     {
//         public string Name;
//         public string Rarity;
//         public double Modifier;

//         public Boon(string name, string rarity, double modifier)
//         {
//             Name = name;
//             Rarity = rarity;
//             Modifier = modifier;
//         }
//     }

//     //                             Variable       Rarity   Modifier
//     Boon speedWheels = new Boon("maxSpeed", "Common", 1.10);
//     Boon springBoard = new Boon("ollieForce", "Common", 1.10);
//     // Boon springBoard   = new Boon("kickoffSpeed",  "Common", 1.10);
//     // Boon springBoard   = new Boon("torqueForce",  "Common", 0.90);
//     Boon pointBooaster = new Boon("Point Gain", "Rare", 1.10); // Later
//     Boon coolTricks = new Boon("Crit Power", "Rare", 1.10); // Later
//     Boon hqHelmet = new Boon("Recovery", "Rare", 1.10); // Later
//     Boon ollieOllie = new Boon("Double Jump", "Epic", 2.00); // Later

//     // Overcomplicating

//     void Start()
//     {



//         Hashtable boons = new Hashtable();
//         Hashtable playerStats = new Hashtable();

//         double[] playerStats = { kickoffSpeed, torqueForce, maxSpeed, ollieForce, strength, length, dampening };
        
//     }

//     double[] LoadPlayerStats()
//     {
//         // When ran, Player Stats get updated and sent to the playerController script

//         // Step 1, check to see what boons have been activated
//         // Step 2, for boons activated, modify appropriate stats
//         boons.Add("maxSpeed", 1.10);

//         // maxSpeed *=

//         // playerStats.Add(maxSpeed, 1.10);

//         // foreach (DictionaryEntry boon in boons)
//         // {
//         //      playerStats.Add(boon.Key * boon.Value)
//         // }

//         // Step 3, package player stats into array
//         return playerStats;
//     }


// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipData : MonoBehaviour
{
    public enum ShipType
    {
        //Striker
        Fighter,
        Bomber,
        //Assault
        Gunship,
        Transport,
        Gunboat,
        //Warship
        Corvette,
        Destroyer,
        Frigate,
        //Capital
        Cruiser,
        Battleship,
        Carrier,
        //Dreadnought
        Dreadnought
    }
    public ShipType thisShip;
    public bool isBoardable;
    public List<CurrentTurretSetTarget> turretsTargetingThisShip = new List<CurrentTurretSetTarget>();
    public bool shipInCombat;

    public float maxHealth = 0;
    public float health = 0;
    public float maxShield = 0;
    public float shield = 0;

    int MGturretCount = 0;
    int MissileRackCount = 0;
    int BombBay = 0;
    int MissileTurretCount = 0;
    int LightTurretCount = 0;
    int MedTurretCount = 0;
    int HeavyTurretCount = 0;
    int LanceTurretCount = 0;

    float numComplementRefits;    //Value related to how many times a ship can repair and replace damaged ships its deployed with
    int supplyCount;            //Galactic map, how long the ship can remain deployed

    public enum CurrentTurretSetTarget
    {
        MGTurret,
        MissileRack,
        BombBay,
        MissileTurret,
        LightTurret,
        MedTurret,
        HeavyTurret,
        Lance,
    }

    int CrewCount = 0;
    int MarineCount = 0;    //Marines are 1:.8 kills, with hit rate adjustments. Marines to personnel is 1:7.
    
    int DeployableFighterCount = 0;
    int DeployableBomberCount = 0;
    int DeployableTransportCount = 0;
    int DeployableGunshipCount = 0;
    int DeployableGunboatCount = 0;
    int DeployableFrigateCount = 0;

    void InitializeShipArmament()
    {
        switch (thisShip)
        {
            case ShipType.Fighter:  //1 crew
                {
                    health = 30;
                    shield = 15;
                    maxHealth = 30;
                    maxShield = 15;
                    MGturretCount = 1;
                    MissileRackCount = 1;

                    isBoardable = false;
                    CrewCount = 1;
                    break;
                }
            case ShipType.Bomber:   //4 crew
                {
                    health = 100;
                    shield = 45;
                    maxHealth = 100;
                    maxShield = 45;
                    MGturretCount = 3;
                    MissileRackCount = 2;
                    BombBay = 1;

                    isBoardable = false;
                    CrewCount = 4;
                    break;
                }
            case ShipType.Gunship:  //2 crew
                {
                    health = 150;
                    shield = 75;
                    maxHealth = 150;
                    maxShield = 75;
                    MGturretCount = 2;
                    MissileRackCount = 2;

                    isBoardable = false;
                    CrewCount = 2;
                    break;
                }
            case ShipType.Transport:    //2 crew, 30 marines
                {
                    health = 70;
                    shield = 45;
                    maxHealth = 70;
                    maxShield = 45;
                    MGturretCount = 2;
                    isBoardable = false;

                    CrewCount = 2;
                    MarineCount = 30;
                    break;
                }
            case ShipType.Gunboat:  //12 crew
                {
                    health = 220;
                    shield = 105;
                    maxHealth = 220;
                    maxShield = 105;
                    MGturretCount = 6;
                    MissileTurretCount = 2;

                    isBoardable = false;
                    CrewCount = 12;
                    break;
                }
            case ShipType.Corvette: //60 crew
                {
                    health = 200;
                    shield = 200;
                    maxHealth = 200;
                    maxShield = 200;
                    MGturretCount = 4;
                    MissileTurretCount = 3;
                    LightTurretCount = 1;
                    DeployableFighterCount = 2;

                    isBoardable = false;
                    CrewCount = 60;
                    break;
                }
            case ShipType.Destroyer:    //220 crew, 40 marines
                {
                    health = 600;
                    shield = 600;
                    maxHealth = 600;
                    maxShield = 600;
                    MGturretCount = 8;
                    MissileTurretCount = 4;
                    LightTurretCount = 2;
                    MedTurretCount = 1;

                    isBoardable = true;
                    CrewCount = 220;
                    MarineCount = 40;
                    break;
                }
            case ShipType.Frigate:  //500 crew, 70 marines
                {
                    health = 450;
                    shield = 450;
                    maxHealth = 450;
                    maxShield = 450;
                    MGturretCount = 12;
                    MissileTurretCount = 4;
                    MedTurretCount = 2;

                    DeployableFighterCount = 6;
                    DeployableBomberCount = 2;
                    DeployableTransportCount = 1;

                    isBoardable = true;
                    CrewCount = 500;
                    MarineCount = 70;
                    break;
                }
            case ShipType.Cruiser:  //1600 crew, 200 marines
                {
                    health = 1200;
                    shield = 1200;
                    maxHealth = 1200;
                    maxShield = 1200;
                    MGturretCount = 22;
                    MissileTurretCount = 8;
                    LightTurretCount = 4;
                    MedTurretCount = 2;
                    HeavyTurretCount = 1;

                    DeployableFighterCount = 18;
                    DeployableBomberCount = 6;
                    DeployableTransportCount = 4;
                    DeployableGunshipCount = 2;

                    isBoardable = true;
                    CrewCount = 1600;
                    MarineCount = 200;
                    break;
                }
            case ShipType.Battleship:   //2300 crew, 400 marines
                {
                    health = 1500;
                    shield = 1500;
                    maxHealth = 1500;
                    maxShield = 1500;
                    MGturretCount = 40;
                    MissileTurretCount = 14;
                    LightTurretCount = 12;
                    MedTurretCount = 8;
                    HeavyTurretCount = 4;

                    DeployableFighterCount = 12;
                    DeployableBomberCount = 2;
                    DeployableTransportCount = 4;

                    isBoardable = true;
                    CrewCount = 2300;
                    MarineCount = 400;
                    break;
                }
            case ShipType.Carrier:  //4200 crew, 2500 marines (500/2100 for complement operation, accounted for)
                {
                    health = 1300;
                    shield = 800;
                    maxHealth = 1300;
                    maxShield = 800;
                    MGturretCount = 30;
                    MissileTurretCount = 10;
                    LightTurretCount = 6;
                    MedTurretCount = 2;

                    DeployableFighterCount = 120;   //120 crew
                    DeployableBomberCount = 40;     //160 crew
                    DeployableTransportCount = 70;  //140 crew, 2100 marines
                    DeployableGunshipCount = 40;    //80 crew

                    isBoardable = true;
                    CrewCount = 4200;
                    MarineCount = 2500;
                    break;
                }
            case ShipType.Dreadnought:  //8000 crew, 3300 marines (1384, 1040 complement)
                {
                    health = 2400;
                    shield = 3200;
                    maxHealth = 2400;
                    maxShield = 3200;
                    MGturretCount = 60;
                    MissileTurretCount = 24;
                    LightTurretCount = 20;
                    MedTurretCount = 14;
                    HeavyTurretCount = 8;
                    LanceTurretCount = 2;

                    DeployableFighterCount = 80;    //80
                    DeployableBomberCount = 40;     //160
                    DeployableTransportCount = 30;  //60, 900
                    DeployableGunshipCount = 18;    //36
                    DeployableGunboatCount = 4;     //48
                    DeployableFrigateCount = 2;     //1000/140

                    isBoardable = true;
                    CrewCount = 8000;
                    MarineCount = 3300;
                    break;
                }
        }
    }

    //Called during combat. Counts weapons, and gives individual assignment to each turret
    public void SetTurretTargets(FleetData targetShipList)
    {

        if (MGturretCount != 0)
        {
            for (int i = 0; i > MGturretCount; i++)
            {
                AdjustAttackData(i, targetShipList, CurrentTurretSetTarget.MGTurret, 1);
            }
        }
        if (MissileRackCount != 0)
        {
            for (int i = 0; i > MissileRackCount; i++)
            {
                AdjustAttackData(i, targetShipList, CurrentTurretSetTarget.MissileRack, 2);
            }
        }
        if (BombBay != 0)
        {
            for (int i = 0; i > BombBay; i++)
            {
                AdjustAttackData(i, targetShipList, CurrentTurretSetTarget.BombBay, 8);
            }
        }
        if (MissileTurretCount != 0)
        {
            for (int i = 0; i > MissileTurretCount; i++)
            {
                AdjustAttackData(i, targetShipList, CurrentTurretSetTarget.MissileTurret, 4);
            }
        }
        if (LightTurretCount != 0)
        {
            for (int i = 0; i > LightTurretCount; i++)
            {
                AdjustAttackData(i, targetShipList, CurrentTurretSetTarget.LightTurret, 3);
            }
        }
        if (MedTurretCount != 0)
        {
            for (int i = 0; i > MedTurretCount; i++)
            {
                AdjustAttackData(i, targetShipList, CurrentTurretSetTarget.MedTurret, 4);
            }
        }
        if (HeavyTurretCount != 0)
        {
            for (int i = 0; i > HeavyTurretCount; i++)
            {
                AdjustAttackData(i, targetShipList, CurrentTurretSetTarget.HeavyTurret, 6);
            }
        }
        if (LanceTurretCount != 0)
        {
            for (int i = 0; i > LanceTurretCount; i++)
            {
                AdjustAttackData(i, targetShipList, CurrentTurretSetTarget.Lance, 10);
            }
        }

    }

    //Adjusts based on the target information   
    float hitChance;
    float critChance;
    int attackTimer;
    int damageToSend;
    void AdjustAttackData(int currentTurret, FleetData targetShipList, CurrentTurretSetTarget turretType, int attackFrequency)
    {
        //Assess all ships
        foreach (ShipData ship in targetShipList.shipsInFleet)
        {
            switch (ship.thisShip)
            {
                //Pick most ideal target
                case ShipType.Fighter:
                    {
                        if (turretType == CurrentTurretSetTarget.MGTurret)
                        {
                            if (attackTimer != attackFrequency)
                            {
                                //Set hit chance and Check num turrets targeting list condition
                                if (HitDiceRoll(60)) //&& ship.turretsTargetingThisShip.)   //Contains 3 mgturrets targeting this target
                                {
                                    foreach (CurrentTurretSetTarget turret in ship.turretsTargetingThisShip)
                                    {
                                        if (turret == CurrentTurretSetTarget.MGTurret)
                                        {

                                        }
                                    }

                                    ship.turretsTargetingThisShip.Add(CurrentTurretSetTarget.MGTurret);
                                    if (HitDiceRoll(20))
                                    {
                                        //Crit for double damage
                                        ship.turretsTargetingThisShip.Add(CurrentTurretSetTarget.MGTurret);
                                    }
                                    else
                                    {
                                        //Apply attack
                                    }
                                }
                                //Miss target
                                attackTimer = 0;
                            }
                            else
                            {
                                attackTimer++;
                            }
                        }
                        break;
                    }
            }
            //~Else condition just picks a target
        }
    }

    bool HitDiceRoll(float hitChance)
    {
        float diceRoll = Random.Range(0, 100);
        if (diceRoll >= hitChance)
        {
            return (false);
        }
        else
        {
            return (true);
        }
    }

    void SetMainTarget()
    {

    }
}

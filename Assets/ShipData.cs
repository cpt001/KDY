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

    float health;
    float attack;
    float attackRate;
    //This adjusts the damage this ship will do to enemy ship types
    private Dictionary<ShipType, float> attackBonusMatrix = new Dictionary<ShipType, float>();

    void InitializeShip()
    {

    }
}

public class AttackData
{
    int MGCount;
    int missileCount;
    int MGturretCount;
    int MissileTurretCount;
    int FlakTurretCount;
    int LightTurretCount;
    int MedTurretCount;
    int HeavyTurretCount;
}

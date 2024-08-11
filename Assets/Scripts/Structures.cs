using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is a collection point for all structures in the game
/// It sets structures with input from the UI, or from hotkey presses
/// 
/// 
/// When adding a new structure, it's referenced in 2 scripts.
/// -> Here, where the structure is set for construction
/// -> Select machinery, where hotkey information is set
/// </summary>
public class Structures : MonoBehaviour
{
    private GridCheck gridCheck;
    [SerializeField] private GridConveyor gridConveyor;

    #region Conveyance
    [Header("Conveyance Prefabs")]
    [SerializeField] private GameObject conveyorStart;
    [SerializeField] private GameObject splitter;
    [SerializeField] private GameObject inserter;
    [SerializeField] private GameObject craneTrack;
    [SerializeField] private GameObject monorail;
    [SerializeField] private GameObject signal;
    [SerializeField] private GameObject station;
    [SerializeField] private GameObject massDriver;
    [SerializeField] private GameObject receiver;
    public void Co_SetConveyor() { gridConveyor.StartPlacingConveyor(); }
    public void Co_SetSplitter() { gridCheck.buildingGhost = splitter; }
    public void Co_SetInserter() { gridCheck.buildingGhost = inserter; }
    public void Co_SetCraneTrack() { gridCheck.buildingGhost = craneTrack; }
    public void Co_SetMonorail() { gridCheck.buildingGhost = monorail; }
    public void Co_SetSignal() { gridCheck.buildingGhost = signal; }
    public void Co_SetStation() { gridCheck.buildingGhost = station; }
    public void Co_SetMassDriver() { gridCheck.buildingGhost = massDriver; }
    public void Co_SetReceiver() { gridCheck.buildingGhost = receiver; }
    #endregion
    #region Refinement
    [Header("Refinement Prefabs")]
    [SerializeField] private GameObject acidBath;
    [SerializeField] private GameObject arcFurnace;
    [SerializeField] private GameObject crusher;
    [SerializeField] private GameObject furnace;
    [SerializeField] private GameObject loom;
    [SerializeField] private GameObject washer;
    [SerializeField] private GameObject wiringLoom;
    public void R_SetAcidBath() { gridCheck.buildingGhost = acidBath; }
    public void R_SetArcFurnace() { gridCheck.buildingGhost = arcFurnace; }
    public void R_SetCrusher() { gridCheck.buildingGhost = crusher; }
    public void R_SetFurnace() { gridCheck.buildingGhost = furnace; }
    public void R_SetLoom() { gridCheck.buildingGhost = loom; }
    public void R_SetWasher() { gridCheck.buildingGhost = washer; }
    public void R_SetWiringLoom() { gridCheck.buildingGhost = wiringLoom; }
    #endregion
    #region Machining
    [Header("Machining Prefabs")]
    [SerializeField] private GameObject extruder;
    [SerializeField] private GameObject hammer;
    [SerializeField] private GameObject industrialSewingMachine;
    [SerializeField] private GameObject jumper;
    [SerializeField] private GameObject lathe;
    [SerializeField] private GameObject pillowMagazine;
    [SerializeField] private GameObject welder;
    public void M_SetExtruder() { gridCheck.buildingGhost = extruder; }
    public void M_SetHammer() { gridCheck.buildingGhost = hammer; }
    public void M_SetIndustrialSewingMachine() { gridCheck.buildingGhost = industrialSewingMachine; }
    public void M_SetJumper() { gridCheck.buildingGhost = jumper; }
    public void M_SetLathe() { gridCheck.buildingGhost = lathe; }
    public void M_SetPillowMagazine() { gridCheck.buildingGhost = pillowMagazine; }
    public void M_SetWelder() { gridCheck.buildingGhost = welder; }
    #endregion
    #region Advanced
    [Header("Advanced Prefabs")]
    [SerializeField] private GameObject centrifuge;
    [SerializeField] private GameObject cuttingMachine;
    [SerializeField] private GameObject energizer;
    [SerializeField] private GameObject pneumaticHammer;
    [SerializeField] private GameObject rivetingMachine;
    [SerializeField] private GameObject particleCollider;
    [SerializeField] private GameObject compressor;
    public void A_SetCentrifuge() { gridCheck.buildingGhost = centrifuge; }
    public void A_SetCuttingMachine() { gridCheck.buildingGhost = cuttingMachine; }
    public void A_SetEnergizer() { gridCheck.buildingGhost = energizer; }
    public void A_SetPneumaticHammer() { gridCheck.buildingGhost = pneumaticHammer; }
    public void A_SetRivetingMachine() { gridCheck.buildingGhost = rivetingMachine; }
    public void A_SetParticleCollider() { gridCheck.buildingGhost = particleCollider; }
    public void A_SetCompressor() { gridCheck.buildingGhost = compressor; }
    #endregion
    #region Classified
    [Header("Classified Prefabs")]
    [SerializeField] private GameObject liquidInundator;
    [SerializeField] private GameObject prefaber;
    [SerializeField] private GameObject programmer;
    [SerializeField] private GameObject weaverWormTank;
    public void C_SetLiquidInundator() { gridCheck.buildingGhost = liquidInundator; }
    public void C_SetPrefaber() { gridCheck.buildingGhost = prefaber; }
    public void C_SetProgrammer() { gridCheck.buildingGhost = programmer; }
    public void C_SetWeaverWormTank() { gridCheck.buildingGhost = weaverWormTank; }
    #endregion
    #region Finalizing
    [Header("Finalizing Prefabs")]
    [SerializeField] private GameObject automatedAssemblyYard;
    [SerializeField] private GameObject heavyAssemblyYard;
    [SerializeField] private GameObject precisionAssemblyYard;
    [SerializeField] private GameObject pressurizedAssemblyBay;
    [SerializeField] private GameObject stagingAssemblyArea;
    [SerializeField] private GameObject sterileAssemblyBay;
    [SerializeField] private GameObject highSecurityAssembly;
    public void F_SetAutomatedAssemblyYard() { gridCheck.buildingGhost = automatedAssemblyYard; }
    public void F_SetHeavyAssemblyYard() { gridCheck.buildingGhost = heavyAssemblyYard; }
    public void F_SetPrecisionAssemblyYard() { gridCheck.buildingGhost = precisionAssemblyYard; }
    public void F_SetPressurizedAssemblyBay() { gridCheck.buildingGhost = pressurizedAssemblyBay; }
    public void F_SetStagingAssemblyArea() { gridCheck.buildingGhost = stagingAssemblyArea; }
    public void F_SetSterileAssemblyBay() { gridCheck.buildingGhost = sterileAssemblyBay; }
    public void F_SetHighSecurityAssembly() { gridCheck.buildingGhost = highSecurityAssembly; }
    #endregion
    #region Drydocks
    [Header("Drydock Prefabs")]
    [SerializeField] private GameObject striker;
    [SerializeField] private GameObject assault;
    [SerializeField] private GameObject warship;
    [SerializeField] private GameObject capital;
    [SerializeField] private GameObject dreadnought;
    [SerializeField] private GameObject micro;
    public void D_SetStriker() { gridCheck.buildingGhost = striker; }
    public void D_SetAssault() { gridCheck.buildingGhost = assault; }
    public void D_SetWarship() { gridCheck.buildingGhost = warship; }
    public void D_SetCapital() { gridCheck.buildingGhost = capital; }
    public void D_SetDreadnought() { gridCheck.buildingGhost = dreadnought; }
    public void D_SetMicro() { gridCheck.buildingGhost = micro; }
    #endregion

    #region Structural
    private GameObject girderLattice;
    private GameObject pressurizedStructure;
    private GameObject smFreightDock;
    private GameObject mdFreightDock;
    private GameObject lgFreightDock;
    private GameObject xlFreightDock;
    private GameObject containerZone;
    public void S_SetGirderLattice() { gridCheck.buildingGhost = girderLattice; }
    public void S_SetPressurizedStructure() { gridCheck.buildingGhost = pressurizedStructure; }
    public void S_SmallFreightDock() { gridCheck.buildingGhost = smFreightDock; }
    public void S_MediumFreightDock() { gridCheck.buildingGhost = mdFreightDock; }
    public void S_LargeFreightDock() { gridCheck.buildingGhost = lgFreightDock; }
    public void S_XLFreightDock() { gridCheck.buildingGhost = xlFreightDock; }
    public void S_ContainerZone() { gridCheck.buildingGhost = containerZone; }

    #endregion
}

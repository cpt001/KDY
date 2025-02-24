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
    private GridCheck gridCheck => Camera.main.GetComponent<GridCheck>();
    [SerializeField] private ConveyorPlacement gridConveyor;

    //Sets and resets the gameobject to be what the player wants
    void SetGhost(GameObject targetPreview, MachineryPooling targetPool)
    {
        if (gridCheck.buildingGhost != null) { gridCheck.buildingGhost.SetActive(false); }
        gridCheck.buildingGhost = targetPreview;
        gridCheck.buildingGhost.SetActive(true);

        gridCheck.targetPullPool = targetPool;
        gridCheck.buildingColliderCheck = targetPreview.GetComponent<ColliderCheck>();
    }

    #region Conveyance
    [Header("Conveyance Prefabs")]
    [SerializeField] private MachineryPooling splitter;
    [SerializeField] private MachineryPooling inserter;
    [SerializeField] private MachineryPooling craneTrack;
    [SerializeField] private MachineryPooling monorail;
    [SerializeField] private MachineryPooling signal;
    [SerializeField] private MachineryPooling station;
    [SerializeField] private MachineryPooling massDriver;
    [SerializeField] private MachineryPooling receiver;
    [SerializeField] private GameObject splitterPreview;
    [SerializeField] private GameObject inserterPreview;
    [SerializeField] private GameObject craneTrackPreview;
    [SerializeField] private GameObject monorailPreview;
    [SerializeField] private GameObject signalPreview;
    [SerializeField] private GameObject stationPreview;
    [SerializeField] private GameObject massDriverPreview;
    [SerializeField] private GameObject receiverPreview;
    public void Co_SetConveyor() { gridConveyor.enabled = true; }
    public void Co_SetSplitter() { SetGhost(splitterPreview, splitter);}
    public void Co_SetInserter() { SetGhost(inserterPreview, inserter);}
    public void Co_SetCraneTrack() { SetGhost(craneTrackPreview, craneTrack);}
    public void Co_SetMonorail() {SetGhost(monorailPreview, monorail);}
    public void Co_SetSignal() {SetGhost(signalPreview, signal);}
    public void Co_SetStation() {SetGhost(stationPreview, station);}
    public void Co_SetMassDriver() {SetGhost(massDriverPreview, massDriver);}
    public void Co_SetReceiver() {SetGhost(receiverPreview, receiver);}
    #endregion
    #region Refinement
    [Header("Refinement Prefabs")]
    [SerializeField] private MachineryPooling acidBath;
    [SerializeField] private MachineryPooling arcFurnace;
    [SerializeField] private MachineryPooling crusher;
    [SerializeField] private MachineryPooling furnace;
    [SerializeField] private MachineryPooling spinner;
    [SerializeField] private MachineryPooling washer;
    [SerializeField] private MachineryPooling wiringLoom;
    [SerializeField] private GameObject acidBathPreview;
    [SerializeField] private GameObject arcFurnacePreview;
    [SerializeField] private GameObject crusherPreview;
    [SerializeField] private GameObject furnacePreview;
    [SerializeField] private GameObject spinnerPreview;
    [SerializeField] private GameObject washerPreview;
    [SerializeField] private GameObject wiringLoomPreview;
    public void R_SetAcidBath() {SetGhost(acidBathPreview, acidBath);}
    public void R_SetArcFurnace() {SetGhost(arcFurnacePreview, arcFurnace);}
    public void R_SetCrusher() {SetGhost(crusherPreview, crusher);}//Debug.Log(""); }
    public void R_SetFurnace() {SetGhost(furnacePreview, furnace);}
    public void R_SetSpinner() {SetGhost(spinnerPreview, spinner);}
    public void R_SetWasher() {SetGhost(washerPreview, washer);}
    public void R_SetWiringLoom() {SetGhost(wiringLoomPreview, wiringLoom);}
    #endregion
    #region Machining
    [Header("Machining Prefabs")]
    [SerializeField] private MachineryPooling extruder;
    [SerializeField] private MachineryPooling hammer;
    [SerializeField] private MachineryPooling industrialSewingMachine;
    [SerializeField] private MachineryPooling jumper;
    [SerializeField] private MachineryPooling lathe;
    [SerializeField] private MachineryPooling pillowMagazine;
    [SerializeField] private MachineryPooling welder;
    [SerializeField] private GameObject extruderPreview;
    [SerializeField] private GameObject hammerPreview;
    [SerializeField] private GameObject industrialSewingMachinePreview;
    [SerializeField] private GameObject jumperPreview;
    [SerializeField] private GameObject lathePreview;
    [SerializeField] private GameObject pillowMagazinePreview;
    [SerializeField] private GameObject welderPreview;
    public void M_SetExtruder() {SetGhost(extruderPreview, extruder);}
    public void M_SetHammer() {SetGhost(hammerPreview, hammer);}
    public void M_SetIndustrialSewingMachine() {SetGhost(industrialSewingMachinePreview, industrialSewingMachine);}
    public void M_SetJumper() {SetGhost(jumperPreview, jumper);}
    public void M_SetLathe() {SetGhost(lathePreview, lathe);}
    public void M_SetPillowMagazine() {SetGhost(pillowMagazinePreview, pillowMagazine);}
    public void M_SetWelder() {SetGhost(welderPreview, welder);}
    #endregion
    #region Advanced
    [Header("Advanced Prefabs")]
    [SerializeField] private MachineryPooling centrifuge;
    [SerializeField] private MachineryPooling cuttingMachine;
    [SerializeField] private MachineryPooling energizer;
    [SerializeField] private MachineryPooling pneumaticHammer;
    [SerializeField] private MachineryPooling rivetingMachine;
    [SerializeField] private MachineryPooling particleCollider;
    [SerializeField] private MachineryPooling compressor;
    [SerializeField] private GameObject centrifugePreview;
    [SerializeField] private GameObject cuttingMachinePreview;
    [SerializeField] private GameObject energizerPreview;
    [SerializeField] private GameObject pneumaticHammerPreview;
    [SerializeField] private GameObject rivetingMachinePreview;
    [SerializeField] private GameObject particleColliderPreview;
    [SerializeField] private GameObject compressorPreview;
    public void A_SetCentrifuge() {SetGhost(centrifugePreview, centrifuge);}
    public void A_SetCuttingMachine() {SetGhost(cuttingMachinePreview, cuttingMachine);}
    public void A_SetEnergizer() {SetGhost(energizerPreview, energizer);}
    public void A_SetPneumaticHammer() {SetGhost(pneumaticHammerPreview, pneumaticHammer);}
    public void A_SetRivetingMachine() {SetGhost(rivetingMachinePreview, rivetingMachine);}
    public void A_SetParticleCollider() {SetGhost(particleColliderPreview, particleCollider);}
    public void A_SetCompressor() {SetGhost(compressorPreview, compressor);}
    #endregion
    #region Classified
    [Header("Classified Prefabs")]
    [SerializeField] private MachineryPooling liquidInundator;
    [SerializeField] private MachineryPooling prefaber;
    [SerializeField] private MachineryPooling programmer;
    [SerializeField] private MachineryPooling weaverWormTank;
    [SerializeField] private GameObject liquidInundatorPreview;
    [SerializeField] private GameObject prefaberPreview;
    [SerializeField] private GameObject programmerPreview;
    [SerializeField] private GameObject weaverWormTankPreview;
    public void C_SetLiquidInundator() {SetGhost(liquidInundatorPreview, liquidInundator);}
    public void C_SetPrefaber() {SetGhost(prefaberPreview, prefaber);}
    public void C_SetProgrammer() {SetGhost(programmerPreview, programmer);}
    public void C_SetWeaverWormTank() {SetGhost(weaverWormTankPreview, weaverWormTank);}
    #endregion
    #region Finalizing
    [Header("Finalizing Prefabs")]
    [SerializeField] private MachineryPooling automatedAssemblyYard;
    [SerializeField] private MachineryPooling heavyAssemblyYard;
    [SerializeField] private MachineryPooling precisionAssemblyYard;
    [SerializeField] private MachineryPooling pressurizedAssemblyBay;
    [SerializeField] private MachineryPooling stagingAssemblyArea;
    [SerializeField] private MachineryPooling sterileAssemblyBay;
    [SerializeField] private MachineryPooling highSecurityAssembly;
    [SerializeField] private GameObject automatedAssemblyYardPreview;
    [SerializeField] private GameObject heavyAssemblyYardPreview;
    [SerializeField] private GameObject precisionAssemblyYardPreview;
    [SerializeField] private GameObject pressurizedAssemblyBayPreview;
    [SerializeField] private GameObject stagingAssemblyAreaPreview;
    [SerializeField] private GameObject sterileAssemblyBayPreview;
    [SerializeField] private GameObject highSecurityAssemblyPreview;
    public void F_SetAutomatedAssemblyYard() {SetGhost(automatedAssemblyYardPreview, automatedAssemblyYard);}
    public void F_SetHeavyAssemblyYard() {SetGhost(heavyAssemblyYardPreview, heavyAssemblyYard);}
    public void F_SetPrecisionAssemblyYard() {SetGhost(precisionAssemblyYardPreview, precisionAssemblyYard);}
    public void F_SetPressurizedAssemblyBay() {SetGhost(pressurizedAssemblyBayPreview, pressurizedAssemblyBay);}
    public void F_SetStagingAssemblyArea() {SetGhost(stagingAssemblyAreaPreview, stagingAssemblyArea);}
    public void F_SetSterileAssemblyBay() {SetGhost(sterileAssemblyBayPreview, sterileAssemblyBay);}
    public void F_SetHighSecurityAssembly() {SetGhost(highSecurityAssemblyPreview, highSecurityAssembly);}
    #endregion
    #region Drydocks
    [Header("Drydock Prefabs")]
    [SerializeField] private MachineryPooling striker;
    [SerializeField] private MachineryPooling assault;
    [SerializeField] private MachineryPooling warship;
    [SerializeField] private MachineryPooling capital;
    [SerializeField] private MachineryPooling dreadnought;
    [SerializeField] private MachineryPooling micro;
    [SerializeField] private GameObject strikerPreview;
    [SerializeField] private GameObject assaultPreview;
    [SerializeField] private GameObject warshipPreview;
    [SerializeField] private GameObject capitalPreview;
    [SerializeField] private GameObject dreadnoughtPreview;
    [SerializeField] private GameObject microPreview;
    public void D_SetStriker() {SetGhost(strikerPreview, striker);}
    public void D_SetAssault() {SetGhost(assaultPreview, assault);}
    public void D_SetWarship() {SetGhost(warshipPreview, warship);}
    public void D_SetCapital() {SetGhost(capitalPreview, capital);}
    public void D_SetDreadnought() {SetGhost(dreadnoughtPreview, dreadnought);}
    public void D_SetMicro() {SetGhost(microPreview, micro);}
    #endregion

    #region Structural
    [Header("Structural Prefabs")]
    [SerializeField] private MachineryPooling girderLattice;
    [SerializeField] private MachineryPooling pressurizedStructure;
    [SerializeField] private MachineryPooling smFreightDock;
    [SerializeField] private MachineryPooling mdFreightDock;
    [SerializeField] private MachineryPooling lgFreightDock;
    [SerializeField] private MachineryPooling xlFreightDock;
    [SerializeField] private MachineryPooling containerZone;
    [SerializeField] private GameObject girderLatticePreview;
    [SerializeField] private GameObject pressurizedStructurePreview;
    [SerializeField] private GameObject smFreightDockPreview;
    [SerializeField] private GameObject mdFreightDockPreview;
    [SerializeField] private GameObject lgFreightDockPreview;
    [SerializeField] private GameObject xlFreightDockPreview;
    [SerializeField] private GameObject containerZonePreview;
    public void S_SetGirderLattice() {SetGhost(girderLatticePreview, girderLattice);}
    public void S_SetPressurizedStructure() {SetGhost(pressurizedStructurePreview, pressurizedStructure);}
    public void S_SmallFreightDock() {SetGhost(smFreightDockPreview, smFreightDock);}
    public void S_MediumFreightDock() {SetGhost(mdFreightDockPreview, mdFreightDock);}
    public void S_LargeFreightDock() {SetGhost(lgFreightDockPreview, lgFreightDock);}
    public void S_XLFreightDock() {SetGhost(xlFreightDockPreview, xlFreightDock);}
    public void S_ContainerZone() {SetGhost(containerZonePreview, containerZone);}

    #endregion
}

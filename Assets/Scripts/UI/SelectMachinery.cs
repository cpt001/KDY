using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script manages UI and hotkey requests
/// </summary>
public class SelectMachinery : MonoBehaviour
{
    [SerializeField] private GameObject MovingUI;
    private RectTransform targetRect => MovingUI.GetComponent<RectTransform>();
    private bool uiDropped = false;
    [SerializeField] private GameObject activeUI, ConveyPanel, RefinePanel, MachinePanel, AdvPanel, ClassPanel, FinalPanel, DryPanel;
    [SerializeField] private Structures structureSetup;

    //Which enum is targeted for hotkey input
    private int modifyingSelectionNumber = -1;
    private enum ConveySelection
    {
        Belt,
        Splitter,
        Inserter,
        CraneTrack,
        Monorail,
        Signal,
        Station,
        MassDriver,
        Receiver,
    }
    private ConveySelection conveyanceSelection;
    private enum RefineSelection
    {
        AcidBath,
        ArcFurnace,
        Crusher,
        Furnace,
        Loom,
        Washer,
        WiringLoom,
    }
    private RefineSelection refineSelection;

    private enum MachiningSelection
    {
        Extruder,
        Hammer,
        IndusSewMachine,
        Jumper,
        Lathe,
        PillowMagazine,
        Welder,
    }
    private MachiningSelection machiningSelection;

    private enum AdvancedSelection
    {
        Centrifuge,
        CuttingMachine,
        Energizer,
        PneumaticHammer,
        RivetingInnit,
        ParticleCollider,
        Compressor,
    }
    private AdvancedSelection advancedSelection;

    private enum ClassifiedSelection
    {
        LiquidInundator,
        Prefaber,
        Programmer,
        WeaverWormTank,
    }
    private ClassifiedSelection classifiedSelection;

    private enum FinalizingSelection
        {
            AutomatedAssYard,
            HeavyAssYard,
            PrecisionAssembler,
            PressurizeAssBay,
            StagingAssArea,
            SterileAssBay,
            HighSecAss,
        }
    private FinalizingSelection finalizingSelection;

    private enum DrydockSelection
        {
            Striker,
            Assault,
            Micro,
            Warship,
            Capital,
            Dreadnought,
        }
    private DrydockSelection drydockSelection;


    private void Update()
    {
        SetCategorySelection();
    }

    #region UI button category selections
    public void SetConvey()
    {
        modifyingSelectionNumber = 1;
        ManageUI(ConveyPanel);
        if (!uiDropped)
            StartCoroutine(LerpUI(true, .2f));
    }
    public void SetRefine()
    {
        modifyingSelectionNumber = 2;
        ManageUI(RefinePanel);
        if (!uiDropped)
            StartCoroutine(LerpUI(true, .2f));
    }
    public void SetMachine()
    {
        modifyingSelectionNumber = 3;
        ManageUI(MachinePanel);
        if (!uiDropped)
            StartCoroutine(LerpUI(true, .2f));
    }
    public void SetAdvanced()
    {
        modifyingSelectionNumber = 4;
        ManageUI(AdvPanel);
        if (!uiDropped)
            StartCoroutine(LerpUI(true, .2f));
    }
    public void SetClassified()
    {
        modifyingSelectionNumber = 5;
        ManageUI(ClassPanel);
        if (!uiDropped)
            StartCoroutine(LerpUI(true, .2f));
    }
    public void SetFinal()
    {
        modifyingSelectionNumber = 6;
        ManageUI(FinalPanel);
        if (!uiDropped)
            StartCoroutine(LerpUI(true, .2f));
    }
    public void SetDrydock()
    {
        modifyingSelectionNumber = 7;
        ManageUI(DryPanel);
        if (!uiDropped)
            StartCoroutine(LerpUI(true, .2f));
    }
    #endregion
    #region Hotkey category selection
    //This sets which category is currently active in the UI
    public void SetCategorySelection()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            modifyingSelectionNumber = -1;
            if (uiDropped)
                StartCoroutine(LerpUI(false, .5f));
            ManageUI(null);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            ManageUI(ConveyPanel);
            modifyingSelectionNumber = 1;
            if (!uiDropped) 
                StartCoroutine(LerpUI(true, .2f));
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ManageUI(RefinePanel);
            modifyingSelectionNumber = 2;
            if (!uiDropped)
                StartCoroutine(LerpUI(true, .2f));
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            ManageUI(MachinePanel);
            modifyingSelectionNumber = 3;
            if (!uiDropped)
                StartCoroutine(LerpUI(true, .2f));
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            ManageUI(AdvPanel);
            modifyingSelectionNumber = 4;
            if (!uiDropped)
                StartCoroutine(LerpUI(true, .2f));
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ManageUI(ClassPanel);
            modifyingSelectionNumber = 5;
            if (!uiDropped)
                StartCoroutine(LerpUI(true, .2f));
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            ManageUI(FinalPanel);
            modifyingSelectionNumber = 6;
            if (!uiDropped)
                StartCoroutine(LerpUI(true, .2f));
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            ManageUI(DryPanel);
            modifyingSelectionNumber = 7;
            if (!uiDropped)
                StartCoroutine(LerpUI(true, .2f));
        }
    }
    #endregion

    void ManageUI(GameObject setUI)
    {
        if (activeUI != null)
            activeUI.gameObject.SetActive(false);
        if (setUI != null)
        {
            activeUI = setUI;
            activeUI.gameObject.SetActive(true);
        }
    }

    public void SetStructureSelection()
    {
        if (!uiDropped)
        {
            return;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))   //Player presses 1
            {
                switch (modifyingSelectionNumber)
                {   
                    //Switch case depending on what UI is active
                    case 1: { conveyanceSelection = ConveySelection.Belt; structureSetup.Co_SetConveyor(); break; }
                    case 2: { refineSelection = RefineSelection.AcidBath; structureSetup.R_SetAcidBath(); break; }
                    case 3: { machiningSelection = MachiningSelection.Extruder; structureSetup.M_SetExtruder(); break; }
                    case 4: { advancedSelection = AdvancedSelection.Centrifuge; structureSetup.A_SetCentrifuge(); break; }
                    case 5: { classifiedSelection = ClassifiedSelection.LiquidInundator; structureSetup.C_SetLiquidInundator(); break; }
                    case 6: { finalizingSelection = FinalizingSelection.AutomatedAssYard; structureSetup.F_SetAutomatedAssemblyYard(); break; }
                    case 7: { drydockSelection = DrydockSelection.Striker; structureSetup.D_SetStriker(); break; }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                switch (modifyingSelectionNumber)
                {   
                    //Switch case depending on what UI is active
                    case 1: { conveyanceSelection = ConveySelection.Splitter; structureSetup.Co_SetSplitter(); break; }
                    case 2: { refineSelection = RefineSelection.ArcFurnace; structureSetup.R_SetArcFurnace(); break; }
                    case 3: { machiningSelection = MachiningSelection.Hammer; structureSetup.M_SetHammer(); break; }
                    case 4: { advancedSelection = AdvancedSelection.CuttingMachine; structureSetup.A_SetCuttingMachine(); break; }
                    case 5: { classifiedSelection = ClassifiedSelection.Prefaber; structureSetup.C_SetPrefaber(); break; }
                    case 6: { finalizingSelection = FinalizingSelection.HeavyAssYard; structureSetup.F_SetHeavyAssemblyYard(); break; }
                    case 7: { drydockSelection = DrydockSelection.Assault; structureSetup.D_SetAssault(); break; }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                switch (modifyingSelectionNumber)
                {   
                    //Switch case depending on what UI is active
                    case 1: { conveyanceSelection = ConveySelection.Inserter; structureSetup.Co_SetInserter(); break; }
                    case 2: { refineSelection = RefineSelection.Crusher; structureSetup.R_SetCrusher(); break; }
                    case 3: { machiningSelection = MachiningSelection.IndusSewMachine; structureSetup.M_SetIndustrialSewingMachine(); break; }
                    case 4: { advancedSelection = AdvancedSelection.Energizer; structureSetup.A_SetEnergizer(); break; }
                    case 5: { classifiedSelection = ClassifiedSelection.Programmer; structureSetup.C_SetProgrammer(); break; }
                    case 6: { finalizingSelection = FinalizingSelection.PrecisionAssembler; structureSetup.F_SetPrecisionAssemblyYard(); break; }
                    case 7: { drydockSelection = DrydockSelection.Warship; structureSetup.D_SetWarship(); break; }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                switch (modifyingSelectionNumber)
                {   
                    //Switch case depending on what UI is active
                    case 1: { conveyanceSelection = ConveySelection.CraneTrack; structureSetup.Co_SetCraneTrack(); break; }
                    case 2: { refineSelection = RefineSelection.Furnace; structureSetup.R_SetFurnace(); break; }
                    case 3: { machiningSelection = MachiningSelection.Jumper; structureSetup.M_SetJumper(); break; }
                    case 4: { advancedSelection = AdvancedSelection.PneumaticHammer; structureSetup.A_SetPneumaticHammer(); break; }
                    case 5: { classifiedSelection = ClassifiedSelection.WeaverWormTank; structureSetup.C_SetWeaverWormTank(); break; }
                    case 6: { finalizingSelection = FinalizingSelection.PressurizeAssBay; structureSetup.F_SetPressurizedAssemblyBay(); break; }
                    case 7: { drydockSelection = DrydockSelection.Capital; structureSetup.D_SetCapital(); break; }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                switch (modifyingSelectionNumber)
                {   
                    //Switch case depending on what UI is active
                    case 1: { conveyanceSelection = ConveySelection.Monorail; structureSetup.Co_SetMonorail(); break; }
                    case 2: { refineSelection = RefineSelection.Loom; structureSetup.R_SetLoom(); break; }
                    case 3: { machiningSelection = MachiningSelection.Lathe; structureSetup.M_SetLathe(); break; }
                    case 4: { advancedSelection = AdvancedSelection.RivetingInnit; structureSetup.A_SetRivetingMachine(); break; }
                    case 6: { finalizingSelection = FinalizingSelection.StagingAssArea; structureSetup.F_SetStagingAssemblyArea(); break; }
                    case 7: { drydockSelection = DrydockSelection.Dreadnought; structureSetup.D_SetDreadnought(); break; }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                switch (modifyingSelectionNumber)
                {   
                    //Switch case depending on what UI is active
                    case 1: { conveyanceSelection = ConveySelection.Signal; structureSetup.Co_SetSignal(); break; }
                    case 2: { refineSelection = RefineSelection.Washer; structureSetup.R_SetWasher(); break; }
                    case 3: { machiningSelection = MachiningSelection.PillowMagazine; structureSetup.M_SetPillowMagazine(); break; }
                    case 4: { advancedSelection = AdvancedSelection.ParticleCollider; structureSetup.A_SetParticleCollider(); break; }
                    case 6: { finalizingSelection = FinalizingSelection.SterileAssBay; structureSetup.F_SetSterileAssemblyBay(); break; }
                    case 7: { drydockSelection = DrydockSelection.Micro; structureSetup.D_SetMicro(); break; }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                switch (modifyingSelectionNumber)
                {   
                    //Switch case depending on what UI is active
                    case 1: { conveyanceSelection = ConveySelection.Station; structureSetup.Co_SetStation(); break; }
                    case 2: { refineSelection = RefineSelection.WiringLoom; structureSetup.R_SetWiringLoom(); break; }
                    case 3: { machiningSelection = MachiningSelection.Welder; structureSetup.M_SetWelder(); break; }
                    case 4: { advancedSelection = AdvancedSelection.Compressor; structureSetup.A_SetCompressor(); break; }
                    case 6: { finalizingSelection = FinalizingSelection.HighSecAss; structureSetup.F_SetHighSecurityAssembly(); break; }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                switch (modifyingSelectionNumber)
                {   
                    //Switch case depending on what UI is active
                    case 1: { conveyanceSelection = ConveySelection.MassDriver; structureSetup.Co_SetMassDriver(); break; }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                switch (modifyingSelectionNumber)
                {   
                    //Switch case depending on what UI is active
                    case 1: { conveyanceSelection = ConveySelection.Receiver; structureSetup.Co_SetReceiver(); break; }
                }
            }

        }
    }

    private IEnumerator LerpUI(bool deployed, float duration)
    {
        uiDropped = deployed;
        float time = 0;
        Vector3 startPos = targetRect.localPosition;
        Vector3 targetPos = targetRect.localPosition;
        if (!deployed) { targetPos.y = targetRect.localPosition.y + 50f; }
        else if (deployed) { targetPos.y = targetRect.localPosition.y - 50f; }

        while (time < duration)
        {
            targetRect.localPosition = Vector3.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }
}

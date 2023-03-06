using System.Collections.Generic;
using static ModAdjuster.DefinitionStructure;
using static ModAdjuster.DefinitionStructure.BlockDef;
using static ModAdjuster.DefinitionStructure.BlockDef.BlockAction.BlockMod;

namespace ModAdjuster
{
    public class BlockDefinitions
    {
        public string AdminComponent = "MyObjectBuilder_Component/SomeComponment"; // Component to insert into disabled blocks to prevent building from projection
        public List<string> DisabledBlocks = new List<string>() // List of blocks to disable
        {
            "MyObjectBuilder_WindTurbine/HugeWindTurbine",
            "MyObjectBuilder_OxygenFarm/LargeBlockOxygenFarm",
        };

        public List<BlockDef> Definitions = new List<BlockDef>()
        {
            // List of blocks to modify. Can be as many or few as desired
            new BlockDef()
            {
                BlockName = "MyObjectBuilder_WindTurbine/HugeWindTurbine", // Name of the block to modify. Format is "MyObjectBuilder_Type/Subtype" in the same format as BlockVariantGroups
                BlockActions = new[] // List of modifications to make. Can be as many or few as desired
                {
                    // The following modifications can be used on any type of block
                    new BlockAction
                    {
                        Action = DisableBlockDefinition // Block will still exist in world and can be built from projections unfortunately
                    },
                    new BlockAction
                    {
                        Action = ChangeBlockName, // Change Display Name of the block
                        NewText = "New Block Name"
                    },
                    new BlockAction
                    {
                        Action = ChangeBlockDescription, // Change the Description of the block
                        NewText = "New Block Description"
                    },
                    new BlockAction
                    {
                        Action = ChangeBlockPublicity // Sets <Public> to the opposite of its setting in sbc
                    },
                    new BlockAction
                    {
                        Action = ChangeBlockGuiVisibility // Sets <GuiVisible> to the opposite of its setting in sbc
                    },
                    new BlockAction
                    {
                        Action = ChangePCU, // Sets block PCU
                        Value = 100
                    },
                    new BlockAction
                    {
                        Action = ChangeBuildTime, // Sets <BuildTimeSeconds>
                        Value = 45
                    },
                    new BlockAction
                    {
                        Action = ChangeDeformationRatio, // Controls damage taken from collisions and explosions
                        Value = 0.3f
                    },
                    new BlockAction
                    {
                        Action = ChangeResistance, // <GeneralDamageMultiplier>
                        Value = 0.5f
                    },
                    new BlockAction
                    {
                        Action = InsertComponent, // Inserts the given number of the given component at the given index of a block's component list
                        Component = "MyObjectBuilder_Component/Superconductor",
                        Index = 4,
                        Count = 20
                    },
                    new BlockAction
                    {
                        Action = ReplaceComponent, // Replaces the component at the given index with a new component
                        Component = "MyObjectBuilder_Component/Construction",
                        Index = 1,
                        Count = 32 // This field is optional. If not specified or set to 0, the component count will stay the same
                    },
                    new BlockAction
                    {
                        Action = ChangeComponentCount, // Changes the required number of the component at the given index
                        Index = 6,
                        Count = 100
                    },
                    new BlockAction
                    {
                        Action = ChangeComponentDeconstructId, // Sets what the component at the given index grinds down into.
                        Component = "MyObjectBuilder_Ore/Scrap",
                        Index = 5
                    },
                    new BlockAction
                    {
                        Action = ChangeCriticalComponentIndex, // Sets the critical component which must be welded up for the block to function
                        Index = 7
                    },
                }
            },

            new BlockDef()
            {
                BlockName = "MyObjectBuilder_HydrogenEngine/LG_Fuelcell_T0",
                BlockActions = new[]
                {
                    // These modifications are specific to power production blocks
                    new BlockAction
                    {
                        Action = ChangeFuelMultiplier, // Sets <FuelProductionToCapacityMultiplier> for Hydrogen Engines
                        Value = 0.05f
                    },
                    new BlockAction
                    {
                        Action = ChangeMaxPowerOutput, // Sets <MaxPowerOutput> for any reactor, hydrogen engine, solar panel, wind turbine, or battery
                        Value = 1.5f
                    },
                }
            },


            new BlockDef()
            {
                BlockName = "MyObjectBuilder_UpgradeModule/MA_at_upgrade_productivity",
                BlockActions = new[]
                {
                    // This modification is specific to upgrade modules
                    new BlockAction
                    {
                        Action = ChangeUpgradeModifier, // Sets the modifier for an upgrade module's upgrade
                        Value = 0.8f
                    }
                }
            },

            new BlockDef()
            {
                BlockName = "MyObjectBuilder_Drill/HeavyImpactDrillLarge",
                BlockActions = new[]
                {
                    // These modifications are specific to ship tools
                    new BlockAction
                    {
                        Action = ChangeSensorRadius, // Set the effect radius of any ship tool (welder, grinder, drill)
                        Value = 7.2f
                    },
                    new BlockAction
                    {
                        Action = ChangeCutOutRadius, // Set the mining radius of a Ship Drill
                        Value = 7.5f
                    }
                }
            },

        };

    }
}

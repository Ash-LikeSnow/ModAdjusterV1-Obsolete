using System.Collections.Generic;
using static ModAdjuster.DefinitionStructure;
using static ModAdjuster.DefinitionStructure.PhysicalItemDef;
using static ModAdjuster.DefinitionStructure.PhysicalItemDef.ItemAction.ItemMod;

namespace ModAdjuster
{
    public class ItemDefinitions
    {
        public List<PhysicalItemDef> Definitions = new List<PhysicalItemDef>()
        {
            new PhysicalItemDef()
            {
                ItemName = "MyObjectBuilder_GasContainerObject/HydrogenBottle",
                ItemActions = new[]
                {
                    new ItemAction()
                    {
                        Action = ChangeItemPublicity, // Prevent item from being listed in the admin spawn menu
                    },
                }
            },

            new PhysicalItemDef()
            {
                ItemName = "MyObjectBuilder_Component/Superconductor",
                ItemActions = new[]
                {
                    new ItemAction()
                    {
                        Action = ChangeItemName, // Change Display Name of item
                        Text = "Hyperconductor",
                    },
                    new ItemAction()
                    {
                        Action = ChangeIcon, // Change Icon of item
                        Text = "\\Textures\\GUI\\Icons\\Cubes\\Fake.dds",
                    },
                    new ItemAction()
                    {
                        Action = ChangeMass, // Change item Mass
                        Value = 7.5f,
                    },
                    new ItemAction()
                    {
                        Action = ChangeVolume, // Change item Volume
                        Value = 5f,
                    },
                    new ItemAction()
                    {
                        Action = ChangeMaxIntegrity, // Change item Integrity
                        Count = 10,
                    }
                }
            },

            new PhysicalItemDef()
            {
                ItemName = "MyObjectBuilder_PhysicalGunObject/UltimateAutomaticRifleItem",
                ItemActions = new[]
                {
                    new ItemAction()
                    {
                        Action = ChangeItemDescription, // Change item Description (only relevant for weapons and tools) 
                        Text = "Pew Pew Gun",
                    },
                }
            },
        };

    }
}

namespace ModAdjuster
{
    public class DefinitionStructure
    {
        public struct BlockDef
        {
            internal string BlockName;
            internal BlockAction[] BlockActions;

            public struct BlockAction
            {
                internal BlockMod Action;
                internal int Index;
                internal string Component;
                internal int Count;
                internal float Value;
                internal string NewText;

                public enum BlockMod
                {
                    DisableBlockDefinition,
                    ChangeBlockName,
                    ChangeBlockDescription,
                    ChangeBlockPublicity,
                    ChangeBlockGuiVisibility,
                    ChangePCU,
                    ChangeBuildTime,
                    ChangeDeformationRatio,
                    ChangeResistance,
                    InsertComponent,
                    ReplaceComponent,
                    RemoveComponent,
                    ChangeComponentCount,
                    ChangeComponentDeconstructId,
                    ChangeCriticalComponentIndex,
                    ChangeFuelMultiplier,
                    ChangeMaxPowerOutput,
                    ChangeUpgradeModifier,
                    ChangeSensorRadius,
                    ChangeCutOutRadius,
                    ChangeThrustForce,
                    ChangeThrustPowerConsumption,
                    ChangeGyroForce,
                    ChangeIcon,
                }
            }
        }

        public struct BlueprintDef
        {
            internal string BlueprintName;
            internal BPAction[] BPActions;

            public struct BPAction
            {
                internal BPMod Action;
                internal string Item;
                internal int Index;
                internal float Amount;

                public enum BPMod
                {
                    InsertPrerequisite,
                    InsertResult,
                    ReplacePrerequisite,
                    ReplaceResult,
                    RemovePrerequisite,
                    RemoveResult,
                    ChangeAmountPrerequisite,
                    ChangeAmountResult,
                    ChangeBpPublicity,
                    ChangeBpDisplayName,
                    ChangeProductionTime,
                }
            }

        }

        public struct PhysicalItemDef
        {
            internal string ItemName;
            internal ItemAction[] ItemActions;

            public struct ItemAction
            {
                internal ItemMod Action;
                internal string Text;
                internal int Count;
                internal float Value;

                public enum ItemMod
                {
                    DisableItemDefinition,
                    ChangeItemPublicity,
                    ChangeItemName,
                    ChangeItemDescription,
                    ChangeIcon,
                    ChangeMass,
                    ChangeVolume,
                    ChangeMaxIntegrity,
                }
            }

        }
    }
}

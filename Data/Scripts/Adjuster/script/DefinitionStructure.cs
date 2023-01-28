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
                    ChangeAmountPrerequisite,
                    ChangeAmountResult,
                    ChangeBpPublicity
                }
            }

        }
    }
}

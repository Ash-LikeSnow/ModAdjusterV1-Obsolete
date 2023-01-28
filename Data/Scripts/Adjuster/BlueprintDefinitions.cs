using System.Collections.Generic;
using static ModAdjuster.DefinitionStructure;
using static ModAdjuster.DefinitionStructure.BlueprintDef;
using static ModAdjuster.DefinitionStructure.BlueprintDef.BPAction.BPMod;

namespace ModAdjuster
{
    public class BlueprintDefinitions
    {
        public List<BlueprintDef> Definitions = new List<BlueprintDef>()
        {   
            // List of blueprints to modify. Can be as many or few as desired
            new BlueprintDef()
            {
                BlueprintName = "MyObjectBuilder_BlueprintDefinition/40mmATLASBP", // Name of the blueprint to modify
                BPActions = new[] // List of modifications to make. Can be as many or few as desired
                {
                    new BPAction
                    {
                        Action = ChangeAmountPrerequisite, // Change the required amount of the item at the given index
                        Index = 0,
                        Amount = 50f
                    },
                    new BPAction
                    {
                        Action = ReplacePrerequisite, // Replace the required item at the given index with a new item
                        Index = 2,
                        Item = "MyObjectBuilder_Ingot/Cobalt"
                    },
                    new BPAction
                    {
                        Action = InsertPrerequisite, //Insert the given amount of a new required item at the given index
                        Index = 3,
                        Item = "MyObjectBuilder_Ingot/Platinum",
                        Amount = 5f
                    },
                }
            },

            new BlueprintDef()
            {
                BlueprintName = "MyObjectBuilder_BlueprintDefinition/StoneOreToIngot",
                BPActions = new[]
                {
                    new BPAction
                    {
                        Action = ChangeAmountResult, // Change the produced amount of the item at the given index
                        Index = 0,
                        Amount = 20f
                    },
                    new BPAction
                    {
                        Action = ReplaceResult, // Replace the produced item at the given index with a new item
                        Index = 3,
                        Item = "MyObjectBuilder_Ingot/Cobalt"
                    },
                    new BPAction
                    {
                        Action = InsertResult, //Insert the given amount of a new produced item at the given index
                        Index = 4,
                        Item = "MyObjectBuilder_Ingot/Silver",
                        Amount = 1.5f
                    },
                }
            },

            new BlueprintDef()
            {
                BlueprintName = "MyObjectBuilder_BlueprintDefinition/Concrete", // Name of the blueprint to modify
                BPActions = new[] // List of modifications to make. Can be as many or few as desired
                {
                    new BPAction
                    {
                        Action = ChangeBpPublicity, // Sets <Public> to the opposite of its setting in sbc
                    },
                }
            },
        };
    }
}
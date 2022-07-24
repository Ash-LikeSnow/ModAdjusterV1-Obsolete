using Sandbox.Definitions;
using System.Collections.Generic;
using VRage;
using VRage.Game;
using VRage.Game.Components;
using VRage.Utils;
using static ModAdjuster.DefinitionStructure.BlockDef.BlockAction;
using static ModAdjuster.DefinitionStructure.BlueprintDef.BPAction;

namespace ModAdjuster
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class AdjustSBC : MySessionComponentBase
    {
        internal MyCubeBlockDefinition blockDef = new MyCubeBlockDefinition();
        internal Dictionary<MyCubeBlockDefinition, float> resist = new Dictionary<MyCubeBlockDefinition, float>();
        internal MyBlueprintDefinitionBase bpDef;

        public override void LoadData()
        {
            MyLog.Default.WriteLine($"[ModAdjuster] Starting changes!");
            AdjustBlocks();
            AdjustBlueprints();
            MyLog.Default.WriteLine($"[ModAdjuster] Changes complete!");
        }

        public override void BeforeStart()
        {
            BeforeStartStuff();
        }

        internal void AdjustBlocks()
        {
            foreach (var block in BlockDefinitions.BlockDefs)
            {
                var defId = MyDefinitionId.Parse(block.BlockName);
                if (MyDefinitionManager.Static.TryGetCubeBlockDefinition(defId, out blockDef))
                {
                    foreach (var action in block.BlockActions)
                    {
                        MyDefinitionId id;
                        MyComponentDefinition comp = null;
                        MyPhysicalItemDefinition item = null;

                        if (action.Component != null)
                        {
                            if (!MyDefinitionId.TryParse(action.Component, out id))
                            {
                                MyLog.Default.WriteLine($"[ModAdjuster] No valid TypeId in {action.Component}");
                                continue;
                            }

                            if (id.TypeId.ToString() == "MyObjectBuilder_Component")
                                comp = MyDefinitionManager.Static.GetComponentDefinition(id);
                            else
                                item = MyDefinitionManager.Static.GetPhysicalItemDefinition(id);

                            if (comp == null && item == null)
                            {
                                MyLog.Default.WriteLine($"[ModAdjuster] Failed to find definition for {action.Component}");
                                continue;
                            }

                        }

                        if (action.Index != 0 && action.Index >= blockDef.Components.Length || action.Index < 0)
                        {
                            MyLog.Default.WriteLine($"[ModAdjuster] Index out of range: {block.BlockName}, Index = {action.Index}");
                            continue;
                        }

                        switch (action.Action)
                        {
                            case BlockMod.DisableBlockDefinition:
                                blockDef.Enabled = false;
                                break;

                            case BlockMod.ChangeBlockPublicity:
                                blockDef.Public = !blockDef.Public;
                                break;

                            case BlockMod.ChangeBlockGuiVisibility:
                                blockDef.GuiVisible = !blockDef.GuiVisible;
                                break;

                            case BlockMod.ChangeBlockName:
                                blockDef.DisplayNameString = action.NewText;
                                break;

                            case BlockMod.InsertComponent:
                                InsertComponent(action.Index, comp, action.Count);
                                break;

                            case BlockMod.ReplaceComponent:
                                ReplaceComponent(action.Index, comp, action.Count);
                                break;

                            case BlockMod.ChangeComponentCount:
                                ChangeCompCount(action.Index, action.Count);
                                break;

                            case BlockMod.ChangeCriticalComponentIndex:
                                blockDef.CriticalGroup = (ushort)action.Index;
                                SetRatios(action.Index);
                                break;

                            case BlockMod.ChangeComponentDeconstructId:
                                blockDef.Components[action.Index].DeconstructItem = item ?? comp;
                                break;

                            case BlockMod.ChangeBlockDescription:
                                blockDef.DescriptionString = action.NewText;
                                break;

                            case BlockMod.ChangePCU:
                                blockDef.PCU = (int)action.Value;
                                break;

                            case BlockMod.ChangeDeformationRatio:
                                blockDef.DeformationRatio = action.Value;
                                break;

                            case BlockMod.ChangeResistance:
                                resist[blockDef] = action.Value;
                                break;

                            case BlockMod.ChangeBuildTime:
                                blockDef.IntegrityPointsPerSec = blockDef.MaxIntegrity / action.Value;
                                break;

                            case BlockMod.ChangeFuelMultiplier:
                                (blockDef as MyHydrogenEngineDefinition).FuelProductionToCapacityMultiplier = action.Value;
                                break;

                            case BlockMod.ChangeMaxPowerOutput:
                                (blockDef as MyPowerProducerDefinition).MaxPowerOutput = action.Value;
                                break;

                            case BlockMod.ChangeUpgradeModifier:
                                (blockDef as MyUpgradeModuleDefinition).Upgrades[0].Modifier = action.Value;
                                break;

                            case BlockMod.ChangeSensorRadius:
                                (blockDef as MyShipToolDefinition).SensorRadius = action.Value;
                                break;

                            case BlockMod.ChangeCutOutRadius:
                                (blockDef as MyShipDrillDefinition).CutOutRadius = action.Value;
                                break;

                            case BlockMod.ChangeThrustForce:
                                (blockDef as MyThrustDefinition).ForceMagnitude = action.Value;
                                break;

                            case BlockMod.ChangeThrustPowerConsumption:
                                (blockDef as MyThrustDefinition).MaxPowerConsumption = action.Value;
                                break;
                        }
                    }
                }
                else MyLog.Default.WriteLine($"[ModAdjuster] Block {block.BlockName} not found!");
            }
        }

        internal void AdjustBlueprints()
        {
            foreach (var bp in BlueprintDefinitions.BlueprintDefs)
            {
                var defId = MyDefinitionId.Parse(bp.BlueprintName);
                bpDef = MyDefinitionManager.Static.GetBlueprintDefinition(defId);
                if (bpDef == null)
                {
                    MyLog.Default.WriteLine($"[ModAdjuster] Blueprint {bp.BlueprintName} not found!");
                    continue;
                }

                foreach (var action in bp.BPActions)
                {
                    switch (action.Action)
                    {
                        case BPMod.InsertPrerequisite:
                            InsertItem(action.Index, MyDefinitionId.Parse(action.Item), (MyFixedPoint)action.Amount, bpDef.Prerequisites, out bpDef.Prerequisites);
                            break;

                        case BPMod.InsertResult:
                            InsertItem(action.Index, MyDefinitionId.Parse(action.Item), (MyFixedPoint)action.Amount, bpDef.Results, out bpDef.Results);
                            break;

                        case BPMod.ReplacePrerequisite:
                            bpDef.Prerequisites[action.Index].Id = MyDefinitionId.Parse(action.Item);
                            break;

                        case BPMod.ReplaceResult:
                            bpDef.Results[action.Index].Id = MyDefinitionId.Parse(action.Item);
                            break;

                        case BPMod.ChangeAmountPrerequisite:
                            bpDef.Prerequisites[action.Index].Amount = (MyFixedPoint)action.Amount;
                            break;

                        case BPMod.ChangeAmountResult:
                            bpDef.Results[action.Index].Amount = (MyFixedPoint)action.Amount;
                            break;
                    }
                }

            }
        }

        internal void ReplaceComponent(int index, MyComponentDefinition newComp, int newCount)
        {
            var comp = blockDef.Components[index];
            int oldCount = comp.Count;
            float intDiff;
            float massDiff;
            if (newCount > 0)
            {
                intDiff = newComp.MaxIntegrity * newCount - comp.Definition.MaxIntegrity * oldCount;
                massDiff = newComp.Mass * newCount - comp.Definition.Mass * oldCount;

                blockDef.Components[index].Count = newCount;
            }
            else
            {
                intDiff = (newComp.MaxIntegrity - comp.Definition.MaxIntegrity) * oldCount;
                massDiff = (newComp.Mass - comp.Definition.Mass) * oldCount;
            }

            comp.Definition = newComp;
            comp.DeconstructItem = newComp;

            blockDef.MaxIntegrity += intDiff;
            blockDef.Mass += massDiff;

            SetRatios(blockDef.CriticalGroup);
        }

        internal void InsertComponent(int index, MyComponentDefinition comp, int count)
        {
            float intDiff = comp.MaxIntegrity * count;
            float massDiff = comp.Mass * count;

            blockDef.CriticalGroup += 1;

            blockDef.MaxIntegrity += intDiff;
            blockDef.Mass += massDiff;

            var newComps = new MyCubeBlockDefinition.Component[blockDef.Components.Length + 1];

            int i;
            for (i = 0; i < newComps.Length; i++)
            {
                if (i < index)
                    newComps[i] = blockDef.Components[i];
                else if (i == index)
                    newComps[i] = new MyCubeBlockDefinition.Component();
                else
                    newComps[i] = blockDef.Components[i - 1];
            }
            newComps[index].Definition = comp;
            newComps[index].DeconstructItem = comp;
            newComps[index].Count = count;

            blockDef.Components = newComps;

            SetRatios(blockDef.CriticalGroup);
        }

        internal void ChangeCompCount(int index, int newCount)
        {
            var comp = blockDef.Components[index];
            int oldCount = comp.Count;
            float intDiff = comp.Definition.MaxIntegrity * (newCount - oldCount);
            float massDiff = comp.Definition.Mass * (newCount - oldCount);

            comp.Count = newCount;

            blockDef.MaxIntegrity += intDiff;
            blockDef.Mass += massDiff;

            SetRatios(blockDef.CriticalGroup);
        }

        internal void SetRatios(int criticalIndex)
        {
            var criticalIntegrity = 0f;
            var ownershipIntegrity = 0f;
            for (var index = 0; index <= criticalIndex; index++)
            {
                var component = blockDef.Components[index];
                if (ownershipIntegrity == 0f && component.Definition.Id.SubtypeName == "Computer")
                {
                    ownershipIntegrity = criticalIntegrity + component.Definition.MaxIntegrity;
                }
                criticalIntegrity += component.Count * component.Definition.MaxIntegrity;
                if (index == criticalIndex)
                {
                    criticalIntegrity -= component.Definition.MaxIntegrity;
                }
            }

            blockDef.CriticalIntegrityRatio = criticalIntegrity / blockDef.MaxIntegrity;
            blockDef.OwnershipIntegrityRatio = ownershipIntegrity / blockDef.MaxIntegrity;

            var count = blockDef.BuildProgressModels.Length;
            for (var index = 0; index < count; index++)
            {
                var buildPercent = (index + 1f) / count;
                blockDef.BuildProgressModels[index].BuildRatioUpperBound = buildPercent * blockDef.CriticalIntegrityRatio;
            }
        }

        internal void InsertItem(int index, MyDefinitionId id, MyFixedPoint amount, MyBlueprintDefinitionBase.Item[] items, out MyBlueprintDefinitionBase.Item[] updatedItems)
        {
            var newItems = new MyBlueprintDefinitionBase.Item[items.Length + 1];

            int i;
            for (i = 0; i < newItems.Length; i++)
            {
                if (i < index)
                    newItems[i] = items[i];
                else if (i == index)
                    newItems[i] = new MyBlueprintDefinitionBase.Item();
                else
                    newItems[i] = items[i - 1];
            }
            newItems[index].Id = id;
            newItems[index].Amount = amount;

            updatedItems = newItems;
        }

        internal void BeforeStartStuff()
        {
            foreach (var blockDef in resist.Keys)
            {
                blockDef.GeneralDamageMultiplier = resist[blockDef];
            } 
        }

    }
}

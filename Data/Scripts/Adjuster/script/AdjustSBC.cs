using Sandbox.Definitions;
using System.Collections.Generic;
using VRage;
using VRage.Game;
using VRage.Game.Components;
using VRage.Utils;
using static ModAdjuster.DefinitionStructure.BlockDef.BlockAction;
using static ModAdjuster.DefinitionStructure.BlueprintDef.BPAction;
using static ModAdjuster.DefinitionStructure.PhysicalItemDef.ItemAction;

namespace ModAdjuster
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class AdjustSBC : MySessionComponentBase
    {
        internal BlockDefinitions BlockDefs = new BlockDefinitions();
        internal BlueprintDefinitions BlueprintDefs = new BlueprintDefinitions();
        internal ItemDefinitions ItemDefs = new ItemDefinitions();

        internal MyCubeBlockDefinition BlockDef = new MyCubeBlockDefinition();
        internal MyBlueprintDefinitionBase BpDef;
        internal MyPhysicalItemDefinition ItemDef;

        internal Dictionary<MyCubeBlockDefinition, float> Resists = new Dictionary<MyCubeBlockDefinition, float>();

        public override void LoadData()
        {
            MyLog.Default.WriteLine($"[ModAdjuster] Starting changes!");
            AdjustItems();
            AdjustBlueprints();
            AdjustBlocks();
            MyLog.Default.WriteLine($"[ModAdjuster] Changes complete!");
        }

        public override void BeforeStart()
        {
            BeforeStartStuff();

            Clean();
        }

        internal void Clean()
        {
            BlockDefs.Definitions.Clear();
            BlueprintDefs.Definitions.Clear();
            BlockDefs = null;
            BlueprintDefs = null;
            BlockDef = null;
            BpDef = null;

            Resists.Clear();
        }

        internal void AdjustBlocks()
        {
            foreach (var block in BlockDefs.Definitions)
            {
                var defId = MyDefinitionId.Parse(block.BlockName);
                if (MyDefinitionManager.Static.TryGetCubeBlockDefinition(defId, out BlockDef))
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

                        var maxIndex = action.Action == BlockMod.InsertComponent ? BlockDef.Components.Length : BlockDef.Components.Length - 1;
                        if (action.Index > maxIndex || action.Index < 0)
                        {
                            MyLog.Default.WriteLine($"[ModAdjuster] Index out of range: {block.BlockName}, Index = {action.Index}");
                            continue;
                        }

                        switch (action.Action)
                        {
                            case BlockMod.DisableBlockDefinition:
                                BlockDef.Enabled = false;
                                break;

                            case BlockMod.ChangeBlockPublicity:
                                BlockDef.Public = !BlockDef.Public;
                                break;

                            case BlockMod.ChangeBlockGuiVisibility:
                                BlockDef.GuiVisible = !BlockDef.GuiVisible;
                                break;

                            case BlockMod.ChangeBlockName:
                                BlockDef.DisplayNameEnum = null;
                                BlockDef.DisplayNameString = action.NewText;
                                break;

                            case BlockMod.InsertComponent:
                                InsertComponent(action.Index, comp, action.Count);
                                break;

                            case BlockMod.ReplaceComponent:
                                ReplaceComponent(action.Index, comp, action.Count);
                                break;

                            case BlockMod.RemoveComponent:
                                RemoveComponent(action.Index);
                                break;

                            case BlockMod.ChangeComponentCount:
                                ChangeCompCount(action.Index, action.Count);
                                break;

                            case BlockMod.ChangeCriticalComponentIndex:
                                BlockDef.CriticalGroup = (ushort)action.Index;
                                SetRatios(action.Index);
                                break;

                            case BlockMod.ChangeComponentDeconstructId:
                                BlockDef.Components[action.Index].DeconstructItem = item ?? comp;
                                break;

                            case BlockMod.ChangeBlockDescription:
                                BlockDef.DescriptionString = action.NewText;
                                break;

                            case BlockMod.ChangeIcon:
                                BlockDef.Icons = new string[] { action.NewText };
                                break;

                            case BlockMod.ChangePCU:
                                BlockDef.PCU = (int)action.Value;
                                break;

                            case BlockMod.ChangeDeformationRatio:
                                BlockDef.DeformationRatio = action.Value;
                                break;

                            case BlockMod.ChangeResistance:
                                Resists[BlockDef] = action.Value;
                                break;

                            case BlockMod.ChangeBuildTime:
                                BlockDef.IntegrityPointsPerSec = BlockDef.MaxIntegrity / action.Value;
                                break;

                            case BlockMod.ChangeFuelMultiplier:
                                (BlockDef as MyHydrogenEngineDefinition).FuelProductionToCapacityMultiplier = action.Value;
                                break;

                            case BlockMod.ChangeMaxPowerOutput:
                                (BlockDef as MyPowerProducerDefinition).MaxPowerOutput = action.Value;
                                break;

                            case BlockMod.ChangeUpgradeModifier:
                                (BlockDef as MyUpgradeModuleDefinition).Upgrades[0].Modifier = action.Value;
                                break;

                            case BlockMod.ChangeSensorRadius:
                                (BlockDef as MyShipToolDefinition).SensorRadius = action.Value;
                                break;

                            case BlockMod.ChangeCutOutRadius:
                                (BlockDef as MyShipDrillDefinition).CutOutRadius = action.Value;
                                break;

                            case BlockMod.ChangeThrustForce:
                                (BlockDef as MyThrustDefinition).ForceMagnitude = action.Value;
                                break;

                            case BlockMod.ChangeThrustPowerConsumption:
                                (BlockDef as MyThrustDefinition).MaxPowerConsumption = action.Value;
                                break;
                            case BlockMod.ChangeGyroForce:
                                (BlockDef as MyGyroDefinition).ForceMagnitude = action.Value;
                                break;
                        }
                    }
                }
                else MyLog.Default.WriteLine($"[ModAdjuster] Block {block.BlockName} not found!");
            }
        }

        internal void AdjustBlueprints()
        {
            foreach (var bp in BlueprintDefs.Definitions)
            {
                var defId = MyDefinitionId.Parse(bp.BlueprintName);
                BpDef = MyDefinitionManager.Static.GetBlueprintDefinition(defId);
                if (BpDef == null)
                {
                    MyLog.Default.WriteLine($"[ModAdjuster] Blueprint {bp.BlueprintName} not found!");
                    continue;
                }

                foreach (var action in bp.BPActions)
                {
                    switch (action.Action)
                    {
                        case BPMod.InsertPrerequisite:
                            InsertItem(action.Index, MyDefinitionId.Parse(action.Item), (MyFixedPoint)action.Amount, BpDef.Prerequisites, out BpDef.Prerequisites);
                            break;

                        case BPMod.InsertResult:
                            InsertItem(action.Index, MyDefinitionId.Parse(action.Item), (MyFixedPoint)action.Amount, BpDef.Results, out BpDef.Results);
                            break;

                        case BPMod.ReplacePrerequisite:
                            BpDef.Prerequisites[action.Index].Id = MyDefinitionId.Parse(action.Item);
                            break;

                        case BPMod.ReplaceResult:
                            BpDef.Results[action.Index].Id = MyDefinitionId.Parse(action.Item);
                            break;

                        case BPMod.RemovePrerequisite:
                            RemoveItem(action.Index, BpDef.Prerequisites, out BpDef.Prerequisites);
                            break;

                        case BPMod.RemoveResult:
                            RemoveItem(action.Index, BpDef.Results, out BpDef.Results);
                            break;

                        case BPMod.ChangeAmountPrerequisite:
                            BpDef.Prerequisites[action.Index].Amount = (MyFixedPoint)action.Amount;
                            break;

                        case BPMod.ChangeAmountResult:
                            BpDef.Results[action.Index].Amount = (MyFixedPoint)action.Amount;
                            break;

                        case BPMod.ChangeBpPublicity:
                            BpDef.Public = !BpDef.Public;
                            break;

                        case BPMod.ChangeBpDisplayName:
                            BpDef.DisplayNameEnum = null;
                            BpDef.DisplayNameString = action.Item;
                            break;

                        case BPMod.ChangeProductionTime:
                            BpDef.BaseProductionTimeInSeconds = action.Amount;
                            break;
                    }
                }

            }
        }

        internal void AdjustItems()
        {
            foreach (var item in ItemDefs.Definitions)
            {
                var defId = MyDefinitionId.Parse(item.ItemName);
                ItemDef = MyDefinitionManager.Static.GetPhysicalItemDefinition(defId);
                if (ItemDef == null)
                {
                    MyLog.Default.WriteLine($"[ModAdjuster] Physical Item {item.ItemName} not found!");
                    continue;
                }

                foreach (var action in item.ItemActions)
                {
                    switch (action.Action)
                    {
                        case ItemMod.DisableItemDefinition:
                            ItemDef.Enabled = false;
                            break;

                        case ItemMod.ChangeItemPublicity:
                            ItemDef.Public = !ItemDef.Public;
                            break;

                        case ItemMod.ChangeItemName:
                            ItemDef.DisplayNameEnum = null;
                            ItemDef.DisplayNameString = action.Text;
                            break;

                        case ItemMod.ChangeItemDescription:
                            ItemDef.DescriptionEnum = null;
                            ItemDef.DescriptionString = action.Text;
                            break;

                        case ItemMod.ChangeIcon:
                            ItemDef.Icons = new string[] { action.Text };
                            break;

                        case ItemMod.ChangeMass:
                            ItemDef.Mass = action.Value;
                            break;

                        case ItemMod.ChangeVolume:
                            ItemDef.Volume = action.Value;
                            break;

                        case ItemMod.ChangeMaxIntegrity:
                            (ItemDef as MyComponentDefinition).MaxIntegrity = action.Count;
                            break;

                    }
                }
            }
        }

        internal void InsertComponent(int index, MyComponentDefinition comp, int count)
        {
            float intDiff = comp.MaxIntegrity * count;
            float massDiff = comp.Mass * count;

            if (index <= BlockDef.CriticalGroup)
            {
                BlockDef.CriticalGroup += 1;
            }

            BlockDef.MaxIntegrity += intDiff;
            BlockDef.Mass += massDiff;

            var newComps = new MyCubeBlockDefinition.Component[BlockDef.Components.Length + 1];

            int i;
            for (i = 0; i < newComps.Length; i++)
            {
                if (i < index)
                    newComps[i] = BlockDef.Components[i];
                else if (i == index)
                    newComps[i] = new MyCubeBlockDefinition.Component();
                else
                    newComps[i] = BlockDef.Components[i - 1];
            }
            newComps[index].Definition = comp;
            newComps[index].DeconstructItem = comp;
            newComps[index].Count = count;

            BlockDef.Components = newComps;

            SetRatios(BlockDef.CriticalGroup);
        }

        internal void ReplaceComponent(int index, MyComponentDefinition newComp, int newCount)
        {
            var comp = BlockDef.Components[index];
            int oldCount = comp.Count;
            float intDiff;
            float massDiff;
            if (newCount > 0)
            {
                intDiff = newComp.MaxIntegrity * newCount - comp.Definition.MaxIntegrity * oldCount;
                massDiff = newComp.Mass * newCount - comp.Definition.Mass * oldCount;

                BlockDef.Components[index].Count = newCount;
            }
            else
            {
                intDiff = (newComp.MaxIntegrity - comp.Definition.MaxIntegrity) * oldCount;
                massDiff = (newComp.Mass - comp.Definition.Mass) * oldCount;
            }

            comp.Definition = newComp;
            comp.DeconstructItem = newComp;

            BlockDef.MaxIntegrity += intDiff;
            BlockDef.Mass += massDiff;

            SetRatios(BlockDef.CriticalGroup);
        }

        internal void RemoveComponent(int index)
        {
            var comp = BlockDef.Components[index];
            var def = comp.Definition;
            var count = comp.Count;
            float intDiff = def.MaxIntegrity * count;
            float massDiff = def.Mass * count;

            if (index <= BlockDef.CriticalGroup)
            {
                BlockDef.CriticalGroup -= 1;
            }

            BlockDef.MaxIntegrity -= intDiff;
            BlockDef.Mass -= massDiff;

            var newComps = new MyCubeBlockDefinition.Component[BlockDef.Components.Length - 1];

            int i;
            for (i = 0; i < newComps.Length; i++)
            {
                var j = i < index ? i : i + 1;
                newComps[i] = BlockDef.Components[j];
            }

            BlockDef.Components = newComps;

            SetRatios(BlockDef.CriticalGroup);
        }

        internal void ChangeCompCount(int index, int newCount)
        {
            var comp = BlockDef.Components[index];
            int oldCount = comp.Count;
            float intDiff = comp.Definition.MaxIntegrity * (newCount - oldCount);
            float massDiff = comp.Definition.Mass * (newCount - oldCount);

            comp.Count = newCount;

            BlockDef.MaxIntegrity += intDiff;
            BlockDef.Mass += massDiff;

            SetRatios(BlockDef.CriticalGroup);
        }

        internal void SetRatios(int criticalIndex)
        {
            var criticalIntegrity = 0f;
            var ownershipIntegrity = 0f;
            for (var index = 0; index <= criticalIndex; index++)
            {
                var component = BlockDef.Components[index];
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

            BlockDef.CriticalIntegrityRatio = criticalIntegrity / BlockDef.MaxIntegrity;
            BlockDef.OwnershipIntegrityRatio = ownershipIntegrity / BlockDef.MaxIntegrity;

            var count = BlockDef.BuildProgressModels.Length;
            for (var index = 0; index < count; index++)
            {
                var buildPercent = (index + 1f) / count;
                BlockDef.BuildProgressModels[index].BuildRatioUpperBound = buildPercent * BlockDef.CriticalIntegrityRatio;
            }
        }

        internal void InsertItem(int index, MyDefinitionId id, MyFixedPoint amount, MyBlueprintDefinitionBase.Item[] items, out MyBlueprintDefinitionBase.Item[] updatedItems)
        {
            var newItems = new MyBlueprintDefinitionBase.Item[items.Length + 1];

            for (int i = 0; i < newItems.Length; i++)
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

        internal void RemoveItem(int index, MyBlueprintDefinitionBase.Item[] items, out MyBlueprintDefinitionBase.Item[] updatedItems)
        {
            var newItems = new MyBlueprintDefinitionBase.Item[items.Length - 1];
            for (int i = 0; i < newItems.Length; i++)
            {
                var j = i < index ? i : i + 1;
                newItems[i] = items[j];
            }
            updatedItems = newItems;
        }

        internal void BeforeStartStuff()
        {
            foreach (var blockDef in Resists.Keys)
            {
                blockDef.GeneralDamageMultiplier = Resists[blockDef];
            } 
        }

    }
}

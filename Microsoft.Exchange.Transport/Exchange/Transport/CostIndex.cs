using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class CostIndex : ICostIndex
	{
		public CostIndex(Cost[] costIndices, int maxConditionsPerBucket, GetCost getCost, IsBelowCost isBelowCost, ShouldAddToIndex shouldAddToIndex, Trace tracer)
		{
			if (costIndices == null)
			{
				throw new ArgumentNullException("costIndices");
			}
			if (maxConditionsPerBucket <= 0)
			{
				throw new ArgumentException("maxConditionsPerBucket has to be positive", "maxConditionsPerBucket");
			}
			if (getCost == null)
			{
				throw new ArgumentNullException("isIndex");
			}
			if (isBelowCost == null)
			{
				throw new ArgumentNullException("isBelowCost");
			}
			if (shouldAddToIndex == null)
			{
				throw new ArgumentNullException("shouldAddToIndex");
			}
			if (tracer == null)
			{
				throw new ArgumentNullException("tracer");
			}
			this.tracer = tracer;
			this.costIndicesPositionMap = new Dictionary<Cost, int>();
			this.maxConditionsPerBucket = maxConditionsPerBucket;
			this.getCost = getCost;
			this.isBelowCost = isBelowCost;
			this.shouldAddToIndex = shouldAddToIndex;
			this.costIndices = new List<Cost>(costIndices);
			this.costIndexMap = new List<LinkedList<WaitCondition>>();
			for (int i = 0; i < this.costIndices.Count; i++)
			{
				this.costIndicesPositionMap.Add(costIndices[i], i);
				this.costIndexMap.Add(new LinkedList<WaitCondition>());
			}
		}

		public void Add(WaitCondition waitCondition)
		{
			if (waitCondition == null)
			{
				throw new ArgumentNullException("waitCondition");
			}
			if (!this.shouldAddToIndex(waitCondition))
			{
				this.tracer.TraceDebug<WaitCondition>((long)this.GetHashCode(), "Skipping adding WaitCondition ({0}) as ShouldAddToIndex returned false.", waitCondition);
				return;
			}
			Cost cost = this.getCost(waitCondition);
			if (cost == null)
			{
				throw new InvalidOperationException("Failed to get cost for the condition");
			}
			int num;
			if (!this.costIndicesPositionMap.TryGetValue(cost, out num))
			{
				throw new InvalidOperationException(string.Format("Condition {0} with {1} threads is not found in Cost Index list which has {2} entries", waitCondition, cost.UsedThreads, this.costIndicesPositionMap.Count));
			}
			LinkedList<WaitCondition> linkedList = this.costIndexMap[num];
			lock (linkedList)
			{
				if (num == 0 || linkedList.Count < this.maxConditionsPerBucket)
				{
					linkedList.AddLast(waitCondition);
					this.tracer.TraceDebug<WaitCondition, int, int>((long)this.GetHashCode(), "Adding WaitCondition ({0}) to bucket #{1} (Item Count = {2}).", waitCondition, num, linkedList.Count);
				}
				else
				{
					this.tracer.TraceDebug((long)this.GetHashCode(), "Skipping adding WaitCondition ({0}) to bucket #{1} as it has {2} items and maximum allowed conditions are {3}.", new object[]
					{
						waitCondition,
						num,
						linkedList.Count,
						this.maxConditionsPerBucket
					});
				}
			}
		}

		public WaitCondition[] TryRemove(bool allowAboveThreshold, object state)
		{
			List<WaitCondition> list = new List<WaitCondition>();
			for (int i = 0; i < this.costIndexMap.Count; i++)
			{
				Cost costIndex = this.costIndices[i];
				LinkedList<WaitCondition> linkedList = this.costIndexMap[i];
				LinkedListNode<WaitCondition> linkedListNode;
				lock (linkedList)
				{
					linkedListNode = linkedList.First;
				}
				bool flag2 = false;
				while (linkedListNode != null)
				{
					IsBelowCostResult belowCostResult = this.isBelowCost(linkedListNode.Value, costIndex, state, allowAboveThreshold);
					LinkedListNode<WaitCondition> linkedListNode2;
					this.ProcessEntry(belowCostResult, linkedListNode, linkedList, i, list, out linkedListNode2, out flag2);
					if (flag2)
					{
						break;
					}
					linkedListNode = linkedListNode2;
				}
				if (flag2)
				{
					break;
				}
			}
			return list.ToArray();
		}

		private void ProcessEntry(IsBelowCostResult belowCostResult, LinkedListNode<WaitCondition> entry, LinkedList<WaitCondition> bucket, int bucketIndex, List<WaitCondition> removedConditions, out LinkedListNode<WaitCondition> nextNode, out bool atCapacity)
		{
			atCapacity = false;
			lock (bucket)
			{
				nextNode = entry.Next;
				WaitCondition value = entry.Value;
				switch (belowCostResult)
				{
				case IsBelowCostResult.BelowCost:
					removedConditions.Add(value);
					this.tracer.TraceDebug<WaitCondition, int>((long)this.GetHashCode(), "Removing WaitCondition ({0}) from bucket #{1} as it is below cost).", value, bucketIndex);
					this.TryRemoveEntry(bucket, entry);
					break;
				case IsBelowCostResult.InvalidCondition:
					this.tracer.TraceDebug<WaitCondition, int>((long)this.GetHashCode(), "Removing invalid WaitCondition ({0}) from bucket #{1}.", value, bucketIndex);
					this.TryRemoveEntry(bucket, entry);
					break;
				case IsBelowCostResult.AtCapacity:
					atCapacity = true;
					this.tracer.TraceDebug((long)this.GetHashCode(), "Breaking out of TryRemove as we're running at or above allowed capacity.");
					break;
				}
			}
		}

		private void TryRemoveEntry(LinkedList<WaitCondition> list, LinkedListNode<WaitCondition> entry)
		{
			try
			{
				list.Remove(entry);
			}
			catch (InvalidOperationException)
			{
			}
		}

		protected readonly List<Cost> costIndices;

		private readonly Dictionary<Cost, int> costIndicesPositionMap;

		private Trace tracer;

		protected List<LinkedList<WaitCondition>> costIndexMap;

		protected int maxConditionsPerBucket;

		protected GetCost getCost;

		protected IsBelowCost isBelowCost;

		protected ShouldAddToIndex shouldAddToIndex;
	}
}

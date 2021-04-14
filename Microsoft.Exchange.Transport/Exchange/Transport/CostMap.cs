using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class CostMap
	{
		public CostMap(CostConfiguration config, ICostFactory factory, IsLocked isLocked, IsLockedOnQueue isLockedOnQueue, Trace tracer)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			if (config.MaxThreads <= 0)
			{
				throw new ArgumentException("maxThreads has to be positive", "maxThreads");
			}
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			this.tracer = tracer;
			this.config = config;
			this.lastCleanupTime = this.TimeProvider();
			this.isLocked = isLocked;
			this.isLockedOnQueue = isLockedOnQueue;
			this.costs = new ConcurrentDictionary<WaitCondition, Cost>();
			this.freeCost = new Cost(new CostConfiguration(config.Config, true, config.MaxThreads, config.ProcessingCapacity, config.QuotaOverride, config.TimeProvider), 0, this.tracer);
			Cost[] array = new Cost[config.MaxThreads + 1];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Cost(this.config, i, this.tracer);
			}
			this.costIndex = factory.CreateIndex(array, config.MaxThreads, new GetCost(this.GetCost), new IsBelowCost(this.IsBelowCost), new ShouldAddToIndex(this.ShouldAddToIndex), this.tracer);
		}

		internal bool Allow(WaitCondition condition, out Cost conditionCost)
		{
			conditionCost = null;
			Cost cost;
			if (!this.costs.TryGetValue(condition, out cost))
			{
				if (this.config.QuotaOverride != null && (this.config.OverrideEnabled || this.config.TestOverrideEnabled))
				{
					ProcessingQuotaComponent.ProcessingData quotaOverride = this.config.QuotaOverride.GetQuotaOverride(condition);
					if (quotaOverride != null && quotaOverride.ThrottlingFactor == 0.0)
					{
						if (this.config.OverrideEnabled)
						{
							this.costs.GetOrAdd(condition, new Cost(condition, this.config, this.tracer));
							return false;
						}
						this.tracer.TraceDebug<WaitCondition>((long)this.GetHashCode(), "Quota for tenant {0} would be set to block all", condition);
					}
				}
				conditionCost = this.AddThread(condition);
				this.StartProcessing(condition);
				return true;
			}
			if (this.ConditionalThreadSafeAdd((Cost c) => c.CompareWithoutAccessToken(this.freeCost) < 0, condition, delegate(Cost c)
			{
				c.RecordThreadStart();
			}, out conditionCost))
			{
				this.freeCost.RecordThreadStart();
				this.costIndex.Add(condition);
				this.StartProcessing(condition);
				return true;
			}
			return false;
		}

		internal WaitCondition[] Unlock(NextHopSolutionKey queue)
		{
			WaitCondition[] array = this.TryRemoveConditions(queue, true);
			if (array.Length == 0 && this.config.AboveThresholdBehaviorEnabled)
			{
				array = this.TryRemoveConditions(queue, false);
			}
			return array;
		}

		internal Cost AddThread(WaitCondition condition)
		{
			Cost result = this.ThreadSafeAdd(condition, delegate(Cost c)
			{
				c.RecordThreadStart();
			});
			this.freeCost.RecordThreadStart();
			this.costIndex.Add(condition);
			return result;
		}

		internal void RemoveThread(WaitCondition condition)
		{
			Cost cost;
			if (this.costs.TryGetValue(condition, out cost))
			{
				cost.RecordThreadEnd();
				this.freeCost.RecordThreadEnd();
				this.costIndex.Add(condition);
				return;
			}
			throw new InvalidOperationException("Cost not found to remove thread from");
		}

		internal void ReturnToken(WaitCondition condition, out Cost conditionCost)
		{
			Cost cost;
			if (this.costs.TryGetValue(condition, out cost))
			{
				cost.ReturnToken();
				this.freeCost.ReturnToken();
				cost.StartProcessing();
				this.freeCost.StartProcessing();
				conditionCost = cost;
				return;
			}
			throw new InvalidOperationException("Cost not found to return token for");
		}

		internal void FailToken(WaitCondition condition)
		{
			Cost cost;
			if (this.costs.TryGetValue(condition, out cost))
			{
				cost.FailToken();
				this.freeCost.FailToken();
				this.costIndex.Add(condition);
				return;
			}
			throw new InvalidOperationException("Cost not found to remove thread from");
		}

		internal void StartProcessing(WaitCondition condition)
		{
			Cost orAdd = this.costs.GetOrAdd(condition, (WaitCondition c) => new Cost(condition, this.config, this.tracer));
			orAdd.StartProcessing();
			this.freeCost.StartProcessing();
		}

		internal void CompleteProcessing(WaitCondition condition, DateTime startTime)
		{
			Cost cost;
			if (this.costs.TryGetValue(condition, out cost))
			{
				cost.CompleteProcessing(startTime);
				this.freeCost.CompleteProcessing(startTime);
				return;
			}
			throw new InvalidOperationException("Cost not found to complete processing for");
		}

		internal Cost RecordMemoryUse(WaitCondition condition, ByteQuantifiedSize bytesUsed)
		{
			Cost orAdd = this.costs.GetOrAdd(condition, (WaitCondition c) => new Cost(condition, this.config, this.tracer));
			if (orAdd.AddMemoryCost(bytesUsed))
			{
				return orAdd;
			}
			return null;
		}

		internal void TimedUpdate()
		{
			this.ClearEmptyCostsFromMap();
		}

		internal void AddToIndex(WaitCondition condition)
		{
			this.costIndex.Add(condition);
		}

		internal XElement GetDiagnosticInfo(bool verbose)
		{
			XElement xelement = new XElement("CostMap");
			xelement.Add(new XElement("type", base.GetType().Name));
			xelement.Add(new XElement("CostCount", this.costs.Count));
			xelement.Add(new XElement(this.freeCost.GetDiagnosticInfo()));
			xelement.Add(new XElement("inFlightUnlocks", this.inFlightUnlocks));
			if (verbose)
			{
				XElement xelement2 = new XElement("Costs");
				foreach (KeyValuePair<WaitCondition, Cost> keyValuePair in this.costs)
				{
					XElement content = new XElement("Cost", new object[]
					{
						new XElement("Condition", keyValuePair.Key),
						new XElement(keyValuePair.Value.GetDiagnosticInfo())
					});
					xelement2.Add(content);
				}
				xelement.Add(xelement2);
			}
			return xelement;
		}

		internal string GetDiagnosticString(WaitCondition condition)
		{
			Cost cost;
			if (this.costs.TryGetValue(condition, out cost))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("Threads: {0}/{1}", cost.UsedThreads, this.freeCost.UsedThreads);
				if (this.config.ProcessingHistoryEnabled)
				{
					stringBuilder.AppendFormat("Processing Total: {0}/{1}", cost.ProcessingTotal, this.freeCost.ProcessingTotal);
				}
				if (this.config.MemoryCollectionEnabled)
				{
					stringBuilder.AppendFormat("Memory used: {0}/{1}", cost.MemoryTotal, this.config.MemoryThreshold);
				}
				if ((this.config.OverrideEnabled || this.config.TestOverrideEnabled) && cost.LastThrottleFactor != -1.0)
				{
					stringBuilder.AppendFormat(", ThrottleFactor: {0}", cost.LastThrottleFactor);
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		private void ClearEmptyCostsFromMap()
		{
			TimeSpan t = this.TimeProvider() - this.lastCleanupTime;
			if (t < this.config.EmptyCostRemovalInterval)
			{
				return;
			}
			this.lastCleanupTime = this.TimeProvider();
			IEnumerable<WaitCondition> enumerable = from mapItem in this.costs.AsParallel<KeyValuePair<WaitCondition, Cost>>()
			where mapItem.Value.MarkEmptyCostForDeletion()
			select mapItem.Key;
			foreach (WaitCondition waitCondition in enumerable)
			{
				Cost cost;
				if (this.costs.TryGetValue(waitCondition, out cost))
				{
					lock (cost)
					{
						if (cost.IsEmpty && cost.ObjectState != CostObjectState.Live)
						{
							this.tracer.TraceDebug<WaitCondition>((long)this.GetHashCode(), "Removing condition ({0}) as it has empty cost.", waitCondition);
							Cost cost2;
							if (this.costs.TryRemove(waitCondition, out cost2))
							{
								cost.MarkObjectDeleted();
							}
						}
					}
				}
			}
		}

		private WaitCondition[] TryRemoveConditions(NextHopSolutionKey queue, bool belowThresholdOnly)
		{
			return this.costIndex.TryRemove(!belowThresholdOnly, queue);
		}

		private bool ShouldAddToIndex(WaitCondition condition)
		{
			return this.isLocked(condition);
		}

		private IsBelowCostResult IsBelowCost(WaitCondition condition, Cost index, object state, bool allowAboveThreshold)
		{
			IsBelowCostResult result;
			try
			{
				int num = Interlocked.Increment(ref this.inFlightUnlocks);
				if (!this.freeCost.HasCapacity(allowAboveThreshold, num - 1))
				{
					this.tracer.TraceDebug<Cost, int>((long)this.GetHashCode(), "Returning AtCapacity as the system ({0}) has reached maximum capacity with {1} messages currently being unlocked.", this.freeCost, this.inFlightUnlocks);
					result = IsBelowCostResult.AtCapacity;
				}
				else
				{
					NextHopSolutionKey queue = (NextHopSolutionKey)state;
					if (queue.IsEmpty)
					{
						throw new ArgumentNullException("expecting NextHopSolutionKey as state object");
					}
					Cost cost;
					if (this.costs.TryGetValue(condition, out cost))
					{
						if (cost.GetIndexOf() != index.GetIndexOf())
						{
							this.tracer.TraceDebug<Cost, Cost>((long)this.GetHashCode(), "Returning InvalidCondition as cost ({0}) doesn't match with original cost ({1})", cost, index);
							return IsBelowCostResult.InvalidCondition;
						}
						if (cost >= this.freeCost && (cost.HasOverride || !allowAboveThreshold))
						{
							if (this.config.TestOverrideEnabled && cost.HasOverride)
							{
								this.tracer.TraceDebug<WaitCondition>((long)this.GetHashCode(), "Tenant {0} would not be unlocked because override is present", condition);
							}
							if (this.config.OverrideEnabled || !allowAboveThreshold)
							{
								this.tracer.TraceDebug<Cost, Cost>((long)this.GetHashCode(), "Returning AboveCost as cost ({0}) is more than free cost ({1})", cost, this.freeCost);
								return IsBelowCostResult.AboveCost;
							}
						}
					}
					if (this.isLockedOnQueue(condition, queue))
					{
						this.tracer.TraceDebug<WaitCondition, Cost>((long)this.GetHashCode(), "Returning Below as condition ({0}) has cost less than free cost ({1}) and has items locked on the queue.", condition, this.freeCost);
						this.AddToken(condition);
						result = IsBelowCostResult.BelowCost;
					}
					else
					{
						result = IsBelowCostResult.AboveCost;
					}
				}
			}
			finally
			{
				Interlocked.Decrement(ref this.inFlightUnlocks);
			}
			return result;
		}

		private Cost GetCost(WaitCondition condition)
		{
			Cost cost;
			if (this.costs.TryGetValue(condition, out cost))
			{
				return new Cost(this.config, cost.UsedThreads, this.tracer);
			}
			return new Cost(this.config, 0, this.tracer);
		}

		private void AddToken(WaitCondition condition)
		{
			this.ThreadSafeAdd(condition, delegate(Cost cost)
			{
				cost.AddToken();
			});
			this.costIndex.Add(condition);
			this.freeCost.AddToken();
		}

		private bool ConditionalThreadSafeAdd(Func<Cost, bool> shouldAdd, WaitCondition condition, Action<Cost> action, out Cost cost)
		{
			Cost orAdd = this.costs.GetOrAdd(condition, new Cost(condition, this.config, this.tracer));
			cost = null;
			lock (orAdd)
			{
				if (shouldAdd(orAdd))
				{
					cost = this.InternalAdd(condition, orAdd, action);
					return true;
				}
			}
			return false;
		}

		private Cost ThreadSafeAdd(WaitCondition condition, Action<Cost> action)
		{
			Cost orAdd = this.costs.GetOrAdd(condition, new Cost(condition, this.config, this.tracer));
			Cost result;
			lock (orAdd)
			{
				result = this.InternalAdd(condition, orAdd, action);
			}
			return result;
		}

		private Cost InternalAdd(WaitCondition condition, Cost cost, Action<Cost> action)
		{
			if (cost.ObjectState == CostObjectState.Deleted)
			{
				Cost orAdd = this.costs.GetOrAdd(condition, new Cost(condition, this.config, this.tracer));
				action(orAdd);
				return orAdd;
			}
			action(cost);
			return cost;
		}

		private DateTime TimeProvider()
		{
			if (this.config.TimeProvider == null)
			{
				return DateTime.UtcNow;
			}
			return this.config.TimeProvider();
		}

		protected ConcurrentDictionary<WaitCondition, Cost> costs;

		protected ICostIndex costIndex;

		protected Cost freeCost;

		private readonly CostConfiguration config;

		private readonly IsLocked isLocked;

		private readonly IsLockedOnQueue isLockedOnQueue;

		private DateTime lastCleanupTime;

		private int inFlightUnlocks;

		private Trace tracer;
	}
}

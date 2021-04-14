using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal class TreeLatencyTracker : LatencyTracker
	{
		internal TreeLatencyTracker()
		{
			this.root = new TreeLatencyTracker.LatencyRecordNode(121, LatencyTracker.StopwatchProvider(), null);
			this.trackedCount += 1;
			this.pendingLeaf = this.root;
		}

		public override bool SupportsTreeFormatting
		{
			get
			{
				return true;
			}
		}

		public override bool HasCompletedComponents
		{
			get
			{
				return this.hasCompletedComponents;
			}
		}

		public override bool HasPendingComponents
		{
			get
			{
				return this.root.IsPending;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("TreeLatencyTracker:");
			stringBuilder.AppendLine(string.Format("too many latency: {0}", (this.tooMany != null) ? this.tooMany.Value.CalculateLatency().ToString() : "null"));
			stringBuilder.AppendLine(string.Format("unknown pending start: {0}", (this.unknownComponentPending != null) ? this.unknownComponentPending.Value.StartTime.ToString() : "null"));
			stringBuilder.AppendLine(string.Format("has completed hasCompletedComponents: {0}", this.hasCompletedComponents));
			stringBuilder.AppendLine(string.Format("tracked count: {0}", this.trackedCount));
			stringBuilder.Append("preprocess latencies :");
			bool flag = false;
			foreach (LatencyRecord latencyRecord in this.preProcessLatencies)
			{
				if (flag)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(string.Format("{0}:{1}", latencyRecord.ComponentShortName, latencyRecord.Latency));
				flag = true;
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(string.Format("root: {0}", (this.root == null) ? "null" : this.root.ToString()));
			stringBuilder.AppendLine(string.Format("pending leaf: {0}", (this.pendingLeaf == null) ? "null" : this.pendingLeaf.ToString()));
			return stringBuilder.ToString();
		}

		public override IEnumerable<LatencyRecord> GetCompletedRecords()
		{
			List<LatencyRecord> list = new List<LatencyRecord>();
			list.AddRange(this.preProcessLatencies);
			list.AddRange(this.root.GetCompletedRecords());
			if (this.tooMany != null)
			{
				list.Add(this.tooMany.Value.AsCompletedRecord());
			}
			return list;
		}

		public override IEnumerable<PendingLatencyRecord> GetPendingRecords()
		{
			return this.root.GetPendingRecords();
		}

		public override void AppendLatencyString(StringBuilder builder, bool useTreeFormat, bool haveTotal, bool enableHeaderFolding)
		{
			if (!this.HasCompletedComponents && !this.HasPendingComponents)
			{
				return;
			}
			int lastFoldingPoint = builder.Length;
			if (haveTotal)
			{
				builder.Append('|');
				lastFoldingPoint = LatencyFormatter.AddFolding(builder, lastFoldingPoint, enableHeaderFolding);
			}
			bool flag = false;
			foreach (LatencyRecord record in this.preProcessLatencies)
			{
				if (flag)
				{
					builder.Append('|');
				}
				LatencyFormatter.AppendLatencyRecord(builder, record, null);
				flag = true;
				lastFoldingPoint = LatencyFormatter.AddFolding(builder, lastFoldingPoint, enableHeaderFolding);
			}
			if (flag)
			{
				builder.Append('|');
			}
			lastFoldingPoint = this.root.AppendComponentLatencyString(builder, lastFoldingPoint, enableHeaderFolding, useTreeFormat);
			if (this.tooMany != null)
			{
				LatencyFormatter.AddFolding(builder, lastFoldingPoint, enableHeaderFolding);
				builder.Append('|');
				LatencyFormatter.AppendLatencyRecord(builder, this.tooMany.Value.AsCompletedRecord(), null);
			}
		}

		protected override void Complete()
		{
			this.EndTrackLatency(121, 121, LatencyTracker.StopwatchProvider(), false);
		}

		protected override LatencyTracker Clone()
		{
			TreeLatencyTracker treeLatencyTracker = new TreeLatencyTracker();
			treeLatencyTracker.root = this.root.DeepCopy(null);
			treeLatencyTracker.preProcessLatencies = new List<LatencyRecord>(this.preProcessLatencies);
			treeLatencyTracker.pendingLeaf = treeLatencyTracker.root.GetPendingLeaf();
			treeLatencyTracker.tooMany = this.tooMany;
			treeLatencyTracker.unknownComponentPending = this.unknownComponentPending;
			treeLatencyTracker.hasCompletedComponents = this.hasCompletedComponents;
			treeLatencyTracker.trackedCount = this.trackedCount;
			return treeLatencyTracker;
		}

		protected override void BeginTrackLatency(ushort componentId, long startTime)
		{
			if (this.unknownComponentPending != null)
			{
				TimeSpan latency = LatencyTracker.TimeSpanFromTicks(this.unknownComponentPending.Value.StartTime, startTime);
				if (LatencyTracker.ShouldTrackComponent(latency, 118))
				{
					this.pendingLeaf.AddCompletedRecord(118, latency);
				}
				this.unknownComponentPending = null;
			}
			if (this.trackedCount >= 1000)
			{
				if (this.tooMany == null)
				{
					this.tooMany = new LatencyRecordPlus?(new LatencyRecordPlus(122, startTime));
				}
				return;
			}
			this.trackedCount += 1;
			this.pendingLeaf = this.pendingLeaf.BeginTrackLatency(componentId, startTime);
		}

		protected override TimeSpan EndTrackLatency(ushort pendingComponentId, ushort trackingComponentId, long endTime, bool shouldAggregate)
		{
			TimeSpan result = TimeSpan.Zero;
			if (this.pendingLeaf == null)
			{
				TreeLatencyTracker.EventLogger.LogEvent(TransportEventLogConstants.Tuple_NullLatencyTreeLeaf, "NullPendingLeaf", new object[]
				{
					this.ToString()
				});
				return result;
			}
			TreeLatencyTracker.LatencyRecordNode parent = this.pendingLeaf;
			bool flag = false;
			bool flag2 = false;
			while (parent.HasParent)
			{
				if (parent.ComponentId == pendingComponentId)
				{
					flag2 = true;
					result = parent.Parent.CompleteChild(trackingComponentId, endTime, shouldAggregate, out flag);
					this.pendingLeaf = parent.Parent;
					this.hasCompletedComponents = true;
					break;
				}
				parent = parent.Parent;
			}
			if (object.Equals(parent, this.root) && parent.ComponentId == pendingComponentId)
			{
				result = parent.Complete(trackingComponentId, endTime);
				this.pendingLeaf = null;
				this.hasCompletedComponents = true;
				LatencyTracker.UpdatePerfCounter(trackingComponentId, (long)(result.TotalSeconds + 0.5));
				return result;
			}
			if (!flag && flag2)
			{
				base.AggregatedUnderThresholdTicks += result.Ticks;
			}
			if (object.Equals(this.pendingLeaf, this.root) && this.unknownComponentPending == null)
			{
				this.unknownComponentPending = new LatencyRecordPlus?(new LatencyRecordPlus(118, LatencyTracker.StopwatchProvider()));
			}
			LatencyTracker.UpdatePerfCounter(trackingComponentId, (long)(result.TotalSeconds + 0.5));
			return result;
		}

		protected override void TrackPreProcessLatency(ushort componentId, DateTime startTime)
		{
			TimeSpan latency = LatencyTracker.TimeProvider() - startTime;
			if (this.trackedCount >= 1000)
			{
				return;
			}
			this.preProcessLatencies.Add(new LatencyRecord(componentId, latency));
			this.trackedCount += 1;
			this.hasCompletedComponents = true;
			LatencyTracker.UpdatePerfCounter(componentId, (long)latency.TotalSeconds);
			if (this.preProcessLatencies.Count > 1)
			{
				string text = string.Join(", ", from x in this.preProcessLatencies
				select x.ComponentShortName);
				TreeLatencyTracker.EventLogger.LogEvent(TransportEventLogConstants.Tuple_MultiplePreProcessLatencies, "MultiplePreProcessLatencies", new object[]
				{
					text
				});
			}
		}

		protected override void TrackExternalLatency(ushort componentId, TimeSpan latency)
		{
			if (this.trackedCount >= 1000)
			{
				return;
			}
			this.pendingLeaf.AddCompletedRecord(componentId, latency);
			this.trackedCount += 1;
			this.hasCompletedComponents = true;
			LatencyTracker.UpdatePerfCounter(componentId, (long)latency.TotalSeconds);
		}

		internal static readonly ExEventLog EventLogger = new ExEventLog(ExTraceGlobals.GeneralTracer.Category, TransportEventLog.GetEventSource());

		private List<LatencyRecord> preProcessLatencies = new List<LatencyRecord>();

		private TreeLatencyTracker.LatencyRecordNode root;

		private TreeLatencyTracker.LatencyRecordNode pendingLeaf;

		private LatencyRecordPlus? tooMany;

		private LatencyRecordPlus? unknownComponentPending;

		private bool hasCompletedComponents;

		private ushort trackedCount;

		private class LatencyRecordNode
		{
			public LatencyRecordNode(ushort componentId, long startTime, TreeLatencyTracker.LatencyRecordNode parent)
			{
				this.latencyRecord = new LatencyRecordPlus(componentId, startTime);
				this.parent = parent;
			}

			private LatencyRecordNode(TreeLatencyTracker.LatencyRecordNode parent)
			{
				this.parent = parent;
			}

			private LatencyRecordNode(LatencyRecordPlus latencyRecord, TreeLatencyTracker.LatencyRecordNode parent)
			{
				this.latencyRecord = latencyRecord;
				this.parent = parent;
			}

			public TreeLatencyTracker.LatencyRecordNode Parent
			{
				get
				{
					return this.parent;
				}
			}

			public bool HasParent
			{
				get
				{
					return this.parent != null;
				}
			}

			public bool HasPendingChild
			{
				get
				{
					return this.pendingChild != null;
				}
			}

			public bool HasCompletedChild
			{
				get
				{
					return this.completedChildren.Any<TreeLatencyTracker.LatencyRecordNode>();
				}
			}

			public bool IsPending
			{
				get
				{
					return !this.latencyRecord.IsComplete;
				}
			}

			public bool IsComplete
			{
				get
				{
					return this.latencyRecord.IsComplete;
				}
			}

			public ushort ComponentId
			{
				get
				{
					return this.latencyRecord.ComponentId;
				}
			}

			private ushort LastCompletedChildComponentId
			{
				get
				{
					if (this.HasCompletedChild)
					{
						return this.completedChildren.Last<TreeLatencyTracker.LatencyRecordNode>().latencyRecord.ComponentId;
					}
					return 0;
				}
			}

			private TimeSpan LastCompletedChildLatency
			{
				get
				{
					if (this.HasCompletedChild)
					{
						return this.completedChildren.Last<TreeLatencyTracker.LatencyRecordNode>().latencyRecord.CalculateLatency();
					}
					return TimeSpan.Zero;
				}
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append((LatencyComponent)this.latencyRecord.ComponentId);
				foreach (TreeLatencyTracker.LatencyRecordNode latencyRecordNode in this.completedChildren)
				{
					stringBuilder.Append("(" + latencyRecordNode.ToString() + ")");
				}
				stringBuilder.Append("[" + ((this.pendingChild != null) ? this.pendingChild.ToString() : string.Empty) + "]");
				return stringBuilder.ToString();
			}

			public int AppendComponentLatencyString(StringBuilder builder, int lastFoldingPoint, bool enableHeaderFolding, bool useTreeFormat)
			{
				bool flag = false;
				if (this.ComponentId != 121)
				{
					flag = true;
					if (this.IsComplete)
					{
						LatencyFormatter.AppendLatencyRecord(builder, this.latencyRecord.AsCompletedRecord(), this.latencyRecord.IsImplicitlyComplete ? "INC" : string.Empty);
					}
					else
					{
						LatencyFormatter.AppendPendingLatencyRecord(builder, this.latencyRecord.AsPendingRecord(), this.CalculateLatency(LatencyTracker.StopwatchProvider()));
					}
				}
				lastFoldingPoint = LatencyFormatter.AddFolding(builder, lastFoldingPoint, enableHeaderFolding);
				bool flag2 = false;
				if (this.HasCompletedChild || this.HasPendingChild)
				{
					if (useTreeFormat && flag)
					{
						builder.Append('(');
					}
					else if (flag)
					{
						builder.Append('|');
					}
					foreach (TreeLatencyTracker.LatencyRecordNode latencyRecordNode in this.completedChildren)
					{
						if (flag2)
						{
							builder.Append('|');
						}
						lastFoldingPoint = latencyRecordNode.AppendComponentLatencyString(builder, lastFoldingPoint, enableHeaderFolding, useTreeFormat);
						flag2 = true;
					}
					if (this.pendingChild != null)
					{
						if (flag2)
						{
							builder.Append('|');
						}
						lastFoldingPoint = this.pendingChild.AppendComponentLatencyString(builder, lastFoldingPoint, enableHeaderFolding, useTreeFormat);
					}
					if (useTreeFormat && flag)
					{
						builder.Append(')');
					}
				}
				return lastFoldingPoint;
			}

			public TreeLatencyTracker.LatencyRecordNode DeepCopy(TreeLatencyTracker.LatencyRecordNode newParent)
			{
				TreeLatencyTracker.LatencyRecordNode latencyRecordNode = new TreeLatencyTracker.LatencyRecordNode(newParent);
				List<TreeLatencyTracker.LatencyRecordNode> list = new List<TreeLatencyTracker.LatencyRecordNode>(this.completedChildren);
				foreach (TreeLatencyTracker.LatencyRecordNode latencyRecordNode2 in list)
				{
					latencyRecordNode.completedChildren.Add(latencyRecordNode2.DeepCopy(latencyRecordNode));
				}
				TreeLatencyTracker.LatencyRecordNode latencyRecordNode3 = this.pendingChild;
				if (latencyRecordNode3 != null)
				{
					latencyRecordNode.pendingChild = latencyRecordNode3.DeepCopy(latencyRecordNode);
				}
				latencyRecordNode.latencyRecord = this.latencyRecord;
				return latencyRecordNode;
			}

			public IEnumerable<LatencyRecord> GetCompletedRecords()
			{
				List<LatencyRecord> list = new List<LatencyRecord>();
				if (this.IsComplete)
				{
					list.Add(this.latencyRecord.AsCompletedRecord());
				}
				foreach (TreeLatencyTracker.LatencyRecordNode latencyRecordNode in this.completedChildren)
				{
					list.AddRange(latencyRecordNode.GetCompletedRecords());
				}
				if (this.HasPendingChild)
				{
					list.AddRange(this.pendingChild.GetCompletedRecords());
				}
				return list;
			}

			public IEnumerable<PendingLatencyRecord> GetPendingRecords()
			{
				List<PendingLatencyRecord> list = new List<PendingLatencyRecord>();
				if (this.IsPending)
				{
					list.Add(this.latencyRecord.AsPendingRecord());
				}
				if (this.pendingChild != null)
				{
					list.AddRange(this.pendingChild.GetPendingRecords());
				}
				return list;
			}

			public TreeLatencyTracker.LatencyRecordNode BeginTrackLatency(ushort componentId, long startTime)
			{
				this.pendingChild = new TreeLatencyTracker.LatencyRecordNode(componentId, startTime, this);
				return this.pendingChild;
			}

			public TimeSpan CompleteChild(ushort trackingComponentId, long endTime, bool shouldAggregate, out bool childTracked)
			{
				childTracked = true;
				TimeSpan timeSpan = this.pendingChild.Complete(trackingComponentId, endTime);
				if (shouldAggregate && this.HasCompletedChild && this.LastCompletedChildComponentId == trackingComponentId)
				{
					this.completedChildren.Last<TreeLatencyTracker.LatencyRecordNode>().latencyRecord = new LatencyRecordPlus(trackingComponentId, this.LastCompletedChildLatency + this.pendingChild.CalculateLatency(endTime));
					this.completedChildren.Last<TreeLatencyTracker.LatencyRecordNode>().completedChildren.AddRange(this.pendingChild.completedChildren);
					this.pendingChild = null;
					return timeSpan;
				}
				if (TreeLatencyTracker.LatencyRecordNode.ShouldTrackComponent(timeSpan))
				{
					this.completedChildren.Add(this.pendingChild);
				}
				else
				{
					childTracked = false;
				}
				this.pendingChild = null;
				return timeSpan;
			}

			public TimeSpan Complete(ushort trackingComponentId, long endTime)
			{
				if (this.IsComplete)
				{
					TreeLatencyTracker.EventLogger.LogEvent(TransportEventLogConstants.Tuple_MultipleCompletions, "MultipleCompletions", new object[]
					{
						this.ToString(),
						trackingComponentId
					});
					return TimeSpan.Zero;
				}
				if (this.pendingChild != null)
				{
					this.pendingChild.ForceImplicitComplete(endTime);
					this.completedChildren.Add(this.pendingChild);
					this.pendingChild = null;
				}
				return this.latencyRecord.Complete(endTime, trackingComponentId, false);
			}

			public TimeSpan CalculateLatency(long currentTime)
			{
				return this.latencyRecord.CalculateLatency(currentTime);
			}

			public void AddCompletedRecord(ushort componentId, TimeSpan latency)
			{
				TreeLatencyTracker.LatencyRecordNode item = new TreeLatencyTracker.LatencyRecordNode(new LatencyRecordPlus(componentId, latency), this);
				this.completedChildren.Add(item);
			}

			public TreeLatencyTracker.LatencyRecordNode GetPendingLeaf()
			{
				if (this.HasPendingChild)
				{
					return this.pendingChild.GetPendingLeaf();
				}
				return this;
			}

			private static bool ShouldTrackComponent(TimeSpan latency)
			{
				return LatencyTracker.ComponentLatencyTrackingEnabled && (LatencyTracker.HighPrecisionThresholdInterval == TimeSpan.Zero || !(latency < LatencyTracker.HighPrecisionThresholdInterval));
			}

			private void ForceImplicitComplete(long endTime)
			{
				if (this.pendingChild != null)
				{
					this.pendingChild.ForceImplicitComplete(endTime);
					this.completedChildren.Add(this.pendingChild);
					this.pendingChild = null;
				}
				this.latencyRecord.Complete(endTime, true);
			}

			private readonly TreeLatencyTracker.LatencyRecordNode parent;

			private readonly List<TreeLatencyTracker.LatencyRecordNode> completedChildren = new List<TreeLatencyTracker.LatencyRecordNode>();

			private TreeLatencyTracker.LatencyRecordNode pendingChild;

			private LatencyRecordPlus latencyRecord;
		}
	}
}

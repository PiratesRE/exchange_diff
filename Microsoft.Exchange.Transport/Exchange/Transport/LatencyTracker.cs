using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Transport
{
	internal class LatencyTracker
	{
		protected LatencyTracker()
		{
			this.latencies = new List<LatencyRecord>();
			this.pendingComponents = new List<PendingLatencyRecord>();
		}

		public static TransportAppConfig.LatencyTrackerConfig Configuration
		{
			get
			{
				return LatencyTracker.config;
			}
			set
			{
				LatencyTracker.config = value;
			}
		}

		public static string ProcessShortName { get; private set; }

		public static bool MessageLatencyEnabled
		{
			get
			{
				TransportAppConfig.LatencyTrackerConfig latencyTrackerConfig = LatencyTracker.config;
				return latencyTrackerConfig == null || latencyTrackerConfig.MessageLatencyEnabled;
			}
		}

		public static bool ComponentLatencyTrackingEnabled
		{
			get
			{
				return LatencyTracker.HighPrecisionThresholdInterval != TransportAppConfig.LatencyTrackerConfig.MaxLatency;
			}
		}

		public static TimeSpan ServerLatencyThreshold
		{
			get
			{
				TransportAppConfig.LatencyTrackerConfig latencyTrackerConfig = LatencyTracker.config;
				if (latencyTrackerConfig != null)
				{
					return latencyTrackerConfig.ServerLatencyThreshold;
				}
				return TransportAppConfig.LatencyTrackerConfig.DefaultServerLatencyThreshold;
			}
		}

		public static TimeSpan HighPrecisionThresholdInterval
		{
			get
			{
				TransportAppConfig.LatencyTrackerConfig latencyTrackerConfig = LatencyTracker.config;
				if (latencyTrackerConfig != null)
				{
					return latencyTrackerConfig.ComponentThresholdInterval;
				}
				return TransportAppConfig.LatencyTrackerConfig.DefaultComponentThresholdInterval;
			}
		}

		public static TimeSpan MinInterestingUnknownInterval
		{
			get
			{
				TransportAppConfig.LatencyTrackerConfig latencyTrackerConfig = LatencyTracker.config;
				if (latencyTrackerConfig != null)
				{
					return latencyTrackerConfig.MinInterestingUnknownInterval;
				}
				return TransportAppConfig.LatencyTrackerConfig.DefaultMinInterestingUnknownInterval;
			}
		}

		public static bool TrustExternalPickupReceivedHeaders
		{
			get
			{
				TransportAppConfig.LatencyTrackerConfig latencyTrackerConfig = LatencyTracker.config;
				return latencyTrackerConfig != null && latencyTrackerConfig.TrustExternalPickupReceivedHeaders;
			}
		}

		public static bool ServerLatencyTrackingEnabled
		{
			get
			{
				return LatencyTracker.ServerLatencyThreshold != TransportAppConfig.LatencyTrackerConfig.MaxLatency;
			}
		}

		public static bool TreeLatencyTrackingEnabled
		{
			get
			{
				TransportAppConfig.LatencyTrackerConfig latencyTrackerConfig = LatencyTracker.config;
				return latencyTrackerConfig != null && latencyTrackerConfig.TreeLatencyTrackingEnabled;
			}
		}

		public virtual bool SupportsTreeFormatting
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasCompletedComponents
		{
			get
			{
				return this.latencies.Count > 0;
			}
		}

		public virtual bool HasPendingComponents
		{
			get
			{
				return this.pendingComponents.Count > 0;
			}
		}

		public long AggregatedUnderThresholdTicks { get; protected set; }

		public static TimeSpan TimeSpanFromTicks(long startTime, long endTime)
		{
			return TimeSpan.FromTicks((endTime - startTime) * (10000000L / Stopwatch.Frequency));
		}

		public static DateTime DefaultTimeProvider()
		{
			return DateTime.UtcNow;
		}

		public static long DefaultStopWatchProvider()
		{
			return Stopwatch.GetTimestamp();
		}

		public static void Start(TransportAppConfig.LatencyTrackerConfig configuration, ProcessTransportRole transportRole)
		{
			LatencyPerformanceCounter.SetCategoryNames(LatencyTracker.perfCounterCategoryMap[transportRole] + " Component Latency", LatencyTracker.perfCounterCategoryMap[transportRole] + " End To End Latency");
			LatencyTracker.config = configuration;
			foreach (LatencyTracker.ComponentDefinition componentDefinition in LatencyTracker.componentsArray)
			{
				componentDefinition.Initialize(transportRole);
			}
			if (transportRole == ProcessTransportRole.Hub || transportRole == ProcessTransportRole.Edge || transportRole == ProcessTransportRole.MailboxDelivery)
			{
				LatencyTracker.endToEndPerformanceCounter = PrioritizedLatencyPerformanceCounter.CreateInstance("Total", LatencyTracker.Configuration.PercentileLatencyExpiryInterval, (long)((ulong)LatencyTracker.Configuration.PercentileLatencyInfinitySeconds), LatencyPerformanceCounterType.EndToEnd);
			}
			LatencyTracker.timer = new GuardedTimer(new TimerCallback(LatencyTracker.PeriodicUpdate), null, LatencyTracker.PeriodicUpdateInterval, LatencyTracker.PeriodicUpdateInterval);
			switch (transportRole)
			{
			case ProcessTransportRole.Hub:
				LatencyTracker.ProcessShortName = "HUB";
				return;
			case ProcessTransportRole.Edge:
				LatencyTracker.ProcessShortName = "EDGE";
				return;
			case ProcessTransportRole.FrontEnd:
				LatencyTracker.ProcessShortName = "FE";
				return;
			case ProcessTransportRole.MailboxSubmission:
				LatencyTracker.ProcessShortName = "SUB";
				return;
			case ProcessTransportRole.MailboxDelivery:
				LatencyTracker.ProcessShortName = "DEL";
				return;
			default:
				LatencyTracker.ProcessShortName = "UNK";
				return;
			}
		}

		public static void Stop()
		{
			if (LatencyTracker.timer != null)
			{
				LatencyTracker.timer.Dispose(true);
				LatencyTracker.timer = null;
			}
		}

		public static LatencyTracker CreateInstance(LatencyComponent sourceComponent)
		{
			if (sourceComponent == LatencyComponent.Dumpster || sourceComponent == LatencyComponent.Heartbeat)
			{
				return null;
			}
			if (!LatencyTracker.ComponentLatencyTrackingEnabled)
			{
				return null;
			}
			if (LatencyTracker.TreeLatencyTrackingEnabled)
			{
				return new TreeLatencyTracker();
			}
			return new LatencyTracker();
		}

		public static LatencyTracker Clone(LatencyTracker tracker)
		{
			if (tracker != null)
			{
				return tracker.Clone();
			}
			return null;
		}

		public static void UpdateExternalServerLatency(long latencySeconds)
		{
			if (latencySeconds >= 0L)
			{
				LatencyTracker.components[49].UpdatePerformanceCounter(latencySeconds);
			}
		}

		public static void UpdateExternalPartnerServerLatency(long latencySeconds)
		{
			if (latencySeconds >= 0L)
			{
				LatencyTracker.components[50].UpdatePerformanceCounter(latencySeconds);
			}
		}

		public static void UpdateTotalEndToEndLatency(DeliveryPriority priority, long latency)
		{
			if (LatencyTracker.endToEndPerformanceCounter == null)
			{
				return;
			}
			LatencyTracker.endToEndPerformanceCounter.AddValue(latency, priority);
			LatencyTracker.endToEndPerformanceCounter.Update();
		}

		public static void TrackPreProcessLatency(LatencyComponent component, LatencyTracker tracker, DateTime startTime)
		{
			if (tracker == null)
			{
				return;
			}
			tracker.TrackPreProcessLatency((ushort)component, startTime);
		}

		public static void TrackExternalComponentLatency(LatencyComponent component, LatencyTracker tracker, TimeSpan latency)
		{
			if (tracker == null)
			{
				return;
			}
			tracker.TrackExternalLatency((ushort)component, latency);
		}

		public static void BeginTrackLatency(LatencyComponent component, LatencyTracker tracker)
		{
			if (tracker == null)
			{
				return;
			}
			tracker.BeginTrackLatency((ushort)component, LatencyTracker.StopwatchProvider());
		}

		public static void BeginTrackLatency(LatencyComponent eventComponent, int agentSequenceNumber, LatencyTracker tracker)
		{
			if (tracker == null)
			{
				return;
			}
			ushort componentId = LatencyTracker.ToComponentId(eventComponent, agentSequenceNumber);
			tracker.BeginTrackLatency(componentId, LatencyTracker.StopwatchProvider());
		}

		public static TimeSpan EndTrackLatency(LatencyComponent component, LatencyTracker tracker, bool shouldAggregate)
		{
			return LatencyTracker.EndTrackLatency(component, component, tracker, shouldAggregate);
		}

		public static TimeSpan EndTrackLatency(LatencyComponent component, LatencyTracker tracker)
		{
			return LatencyTracker.EndTrackLatency(component, component, tracker, false);
		}

		public static TimeSpan EndTrackLatency(LatencyComponent pendingComponent, LatencyComponent trackingComponent, LatencyTracker tracker)
		{
			if (tracker != null)
			{
				return tracker.EndTrackLatency((ushort)pendingComponent, (ushort)trackingComponent, LatencyTracker.StopwatchProvider(), false);
			}
			return TimeSpan.Zero;
		}

		public static TimeSpan EndTrackLatency(LatencyComponent pendingComponent, LatencyComponent trackingComponent, LatencyTracker tracker, bool shouldAggregate)
		{
			if (tracker != null)
			{
				return tracker.EndTrackLatency((ushort)pendingComponent, (ushort)trackingComponent, LatencyTracker.StopwatchProvider(), shouldAggregate);
			}
			return TimeSpan.Zero;
		}

		public static TimeSpan EndTrackLatency(LatencyComponent eventComponent, int agentSequenceNumber, LatencyTracker tracker)
		{
			if (tracker == null)
			{
				return TimeSpan.Zero;
			}
			ushort num = LatencyTracker.ToComponentId(eventComponent, agentSequenceNumber);
			return tracker.EndTrackLatency(num, num, LatencyTracker.StopwatchProvider(), false);
		}

		public static TimeSpan EndAndBeginTrackLatency(LatencyComponent endingComponent, LatencyComponent beginningComponent, LatencyTracker tracker)
		{
			if (tracker == null)
			{
				return TimeSpan.Zero;
			}
			long num = LatencyTracker.StopwatchProvider();
			TimeSpan result = tracker.EndTrackLatency((ushort)endingComponent, (ushort)endingComponent, num, false);
			tracker.BeginTrackLatency((ushort)beginningComponent, num);
			return result;
		}

		public static void Complete(LatencyTracker tracker)
		{
			if (tracker != null)
			{
				tracker.Complete();
			}
		}

		public static void UpdateTotalPerfCounter(TimeSpan latency, DeliveryPriority priority)
		{
			LatencyTracker.components[120].UpdatePerformanceCounter((long)latency.TotalSeconds, priority);
		}

		public static bool TryGetTotalLatencyRecord(TimeSpan totalLatency, bool ignoreThreshold, out LatencyRecord totalRecord)
		{
			if (!LatencyTracker.ServerLatencyTrackingEnabled || (!ignoreThreshold && !LatencyTracker.ShouldTrackTotal(totalLatency)))
			{
				totalRecord = LatencyRecord.Empty;
				return false;
			}
			totalRecord = new LatencyRecord(120, totalLatency);
			return true;
		}

		public static bool TryGetComponentLatencyRecord(LatencyComponent componentId, TimeSpan latency, out LatencyRecord record)
		{
			if (!LatencyTracker.ServerLatencyTrackingEnabled || !LatencyTracker.ShouldTrackComponent(latency, (ushort)componentId))
			{
				record = LatencyRecord.Empty;
				return false;
			}
			record = new LatencyRecord((ushort)componentId, latency);
			return true;
		}

		public static void SetAgentNames(LatencyAgentGroup agentGroup, string[] agentNames)
		{
			LatencyTracker.agentNames[(int)agentGroup] = agentNames;
		}

		public static string GetShortName(ushort componentId)
		{
			if (componentId < 1000)
			{
				return LatencyTracker.GetComponentDefinition((LatencyComponent)componentId).ShortName;
			}
			ushort component = componentId / 1000;
			ushort num = componentId % 1000;
			LatencyTracker.ComponentDefinition componentDefinition = LatencyTracker.GetComponentDefinition((LatencyComponent)component);
			string text;
			if (num == 999)
			{
				text = "__TOO_MANY__";
			}
			else if (componentDefinition.AgentGroup > (LatencyAgentGroup)LatencyTracker.agentNames.Length)
			{
				text = "UnknownGroup" + componentDefinition.AgentGroup;
			}
			else
			{
				string[] array = LatencyTracker.agentNames[(int)componentDefinition.AgentGroup];
				if (array != null && (int)num < array.Length)
				{
					text = array[(int)num];
				}
				else
				{
					text = string.Concat(new object[]
					{
						"UnknownAgent",
						componentDefinition.AgentGroup,
						"-",
						num
					});
				}
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[]
			{
				componentDefinition.ShortName,
				'-',
				text
			});
		}

		public static string GetAgentName(string shortName)
		{
			if (string.IsNullOrEmpty(shortName))
			{
				return string.Empty;
			}
			string text = string.Empty;
			int num = shortName.IndexOf('-');
			if (num >= 0 && num + 1 < shortName.Length)
			{
				text = shortName.Substring(num + 1);
				text = LatencyTracker.DecodeEscapedUnicode(text);
			}
			return text;
		}

		public static LocalizedString GetFullName(string shortName)
		{
			if (string.IsNullOrEmpty(shortName))
			{
				return LocalizedString.Empty;
			}
			string text = LatencyTracker.GetAgentName(shortName);
			if (string.Equals(text, "__TOO_MANY__", StringComparison.OrdinalIgnoreCase))
			{
				text = Strings.TooManyAgents;
			}
			LatencyTracker.ComponentDefinition componentDefinition;
			if (!LatencyTracker.ShortNameToDefinitionMap.TryGetValue(shortName, out componentDefinition))
			{
				return LocalizedString.Empty;
			}
			if (!string.IsNullOrEmpty(text))
			{
				return Strings.EventAgentComponent(componentDefinition.FullName, text);
			}
			return componentDefinition.FullName;
		}

		public static LatencyComponent GetDeliveryQueueLatencyComponent(DeliveryType deliveryType)
		{
			LatencyComponent result;
			if (deliveryType == DeliveryType.SmtpDeliveryToMailbox)
			{
				result = LatencyComponent.DeliveryQueueMailbox;
			}
			else if (TransportDeliveryTypes.internalDeliveryTypes.Contains(deliveryType))
			{
				result = LatencyComponent.DeliveryQueueInternal;
			}
			else
			{
				if (!TransportDeliveryTypes.externalDeliveryTypes.Contains(deliveryType))
				{
					throw new InvalidOperationException("cannot determine LatencyComponent from DeliveryType: " + deliveryType.ToString());
				}
				result = LatencyComponent.DeliveryQueueExternal;
			}
			return result;
		}

		public virtual IEnumerable<LatencyRecord> GetCompletedRecords()
		{
			return new List<LatencyRecord>(this.latencies);
		}

		public virtual IEnumerable<PendingLatencyRecord> GetPendingRecords()
		{
			return new List<PendingLatencyRecord>(this.pendingComponents);
		}

		public virtual void AppendLatencyString(StringBuilder builder, bool useTreeFormat, bool haveTotal, bool enableHeaderFolding)
		{
			if (this.HasCompletedComponents)
			{
				int lastFoldingPoint = builder.Length;
				if (haveTotal)
				{
					builder.Append('|');
					lastFoldingPoint = LatencyFormatter.AddFolding(builder, lastFoldingPoint, enableHeaderFolding);
				}
				this.AppendComponentLatencyString(builder, lastFoldingPoint, enableHeaderFolding, useTreeFormat);
			}
			if (this.HasPendingComponents)
			{
				if (haveTotal || this.HasCompletedComponents)
				{
					builder.Append(';');
				}
				this.AppendPendingComponentLatencyString(builder, useTreeFormat);
			}
		}

		protected static bool ShouldTrackComponent(TimeSpan latency, ushort componentId)
		{
			if (!LatencyTracker.ComponentLatencyTrackingEnabled)
			{
				return false;
			}
			if (componentId == 118)
			{
				return LatencyTracker.MinInterestingUnknownInterval == TimeSpan.Zero || LatencyTracker.MinInterestingUnknownInterval < latency;
			}
			return LatencyTracker.HighPrecisionThresholdInterval == TimeSpan.Zero || LatencyTracker.HighPrecisionThresholdInterval < latency;
		}

		protected static void UpdatePerfCounter(ushort componentId, long latencySeconds)
		{
			if (componentId < 1000)
			{
				LatencyTracker.components[(int)componentId].UpdatePerformanceCounter(latencySeconds);
				return;
			}
			string agentName = LatencyTracker.GetAgentName(LatencyTracker.GetShortName(componentId));
			ILatencyPerformanceCounter latencyPerformanceCounter = LatencyTracker.agentPerformanceCounters.Get(agentName);
			if (latencyPerformanceCounter != null)
			{
				latencyPerformanceCounter.AddValue(latencySeconds);
			}
		}

		protected virtual void Complete()
		{
		}

		protected virtual LatencyTracker Clone()
		{
			LatencyTracker latencyTracker = new LatencyTracker();
			lock (this.latencies)
			{
				latencyTracker.latencies = new List<LatencyRecord>(this.latencies);
				latencyTracker.pendingComponents = new List<PendingLatencyRecord>(this.pendingComponents);
			}
			return latencyTracker;
		}

		protected virtual void BeginTrackLatency(ushort componentId, long startTime)
		{
			lock (this.latencies)
			{
				this.pendingComponents.Add(new PendingLatencyRecord(componentId, startTime));
			}
		}

		protected virtual TimeSpan EndTrackLatency(ushort pendingComponentId, ushort trackingComponentId, long endTime, bool shouldAggregate)
		{
			lock (this.latencies)
			{
				int num = this.FindPendingRecord(pendingComponentId);
				if (num >= 0)
				{
					TimeSpan timeSpan = LatencyTracker.TimeSpanFromTicks(this.pendingComponents[num].StartTime, endTime);
					LatencyTracker.UpdatePerfCounter(trackingComponentId, (long)(timeSpan.TotalSeconds + 0.5));
					this.AddLatency(trackingComponentId, timeSpan, shouldAggregate);
					this.pendingComponents.RemoveAt(num);
					return timeSpan;
				}
			}
			return TimeSpan.Zero;
		}

		protected virtual void TrackPreProcessLatency(ushort componentId, DateTime startTime)
		{
			TimeSpan latencyTimeSpan = LatencyTracker.TimeProvider() - startTime;
			LatencyTracker.UpdatePerfCounter(componentId, (long)latencyTimeSpan.TotalSeconds);
			this.AddLatency(componentId, latencyTimeSpan, false);
		}

		protected virtual void TrackExternalLatency(ushort componentId, TimeSpan latency)
		{
			LatencyTracker.UpdatePerfCounter(componentId, (long)latency.TotalSeconds);
			this.AddLatency(componentId, latency, false);
		}

		private static string DecodeEscapedUnicode(string input)
		{
			return LatencyTracker.EscapedUnicodeRegex.Replace(input, (Match m) => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString());
		}

		private static bool ShouldTrackTotal(TimeSpan totalLatency)
		{
			return LatencyTracker.ServerLatencyTrackingEnabled && (LatencyTracker.ServerLatencyThreshold == TimeSpan.Zero || !(totalLatency < LatencyTracker.ServerLatencyThreshold));
		}

		private static ushort ToComponentId(LatencyComponent eventComponent, int agentSequenceNumber)
		{
			if (eventComponent == LatencyComponent.None || eventComponent > LatencyComponent.MaxMExEventComponent)
			{
				string message = string.Format("eventCoponentId {0} does not fit the event range", (ushort)eventComponent);
				throw new ArgumentOutOfRangeException("eventComponent", message);
			}
			ushort num = (ushort)((agentSequenceNumber > 999) ? 999 : agentSequenceNumber);
			return (ushort)(eventComponent * (LatencyComponent)1000 + num);
		}

		private static LatencyTracker.ComponentDefinition GetComponentDefinition(LatencyComponent component)
		{
			LatencyTracker.ComponentDefinition result;
			if (!LatencyTracker.components.TryGetValue((int)component, out result))
			{
				throw new InvalidOperationException(string.Format("LatencyComponent value {0} could not be found Did you forget to update one of the lists?", component));
			}
			return result;
		}

		private static void PeriodicUpdate(object state)
		{
			foreach (LatencyTracker.ComponentDefinition componentDefinition in LatencyTracker.componentsArray)
			{
				componentDefinition.UpdatePerformanceCounter();
			}
			LatencyTracker.agentPerformanceCounters.ForEach(delegate(string name, ILatencyPerformanceCounter perfCounter)
			{
				LatencyTracker.UpdateAgentPerfCounter(name, perfCounter);
			});
			if (LatencyTracker.endToEndPerformanceCounter != null)
			{
				LatencyTracker.endToEndPerformanceCounter.Update();
			}
		}

		private static void UpdateAgentPerfCounter(string agentName, ILatencyPerformanceCounter perfCounter)
		{
			if (perfCounter != null)
			{
				perfCounter.Update();
			}
		}

		private int FindPendingRecord(ushort componentId)
		{
			int num = this.pendingComponents.Count;
			while (--num >= 0 && this.pendingComponents[num].ComponentId != componentId)
			{
			}
			return num;
		}

		private void AddLatency(ushort componentId, TimeSpan latencyTimeSpan, bool shouldAggregate)
		{
			if (!LatencyTracker.ShouldTrackComponent(latencyTimeSpan, componentId))
			{
				if (LatencyTracker.ComponentLatencyTrackingEnabled)
				{
					this.AggregatedUnderThresholdTicks += latencyTimeSpan.Ticks;
				}
				return;
			}
			TimeSpan timeSpan = latencyTimeSpan;
			lock (this.latencies)
			{
				if (shouldAggregate && this.latencies.Count > 0 && this.latencies[this.latencies.Count - 1].ComponentId == componentId)
				{
					this.latencies[this.latencies.Count - 1] = new LatencyRecord(componentId, this.latencies[this.latencies.Count - 1].Latency + timeSpan);
				}
				else if (this.latencies.Count == 1000)
				{
					TimeSpan latency = this.latencies[999].Latency;
					this.latencies[999] = new LatencyRecord(122, timeSpan + latency);
				}
				else
				{
					this.latencies.Add(new LatencyRecord(componentId, timeSpan));
				}
			}
		}

		private void AppendPendingComponentLatencyString(StringBuilder builder, bool useTreeFormat)
		{
			if (useTreeFormat)
			{
				return;
			}
			bool flag = true;
			foreach (PendingLatencyRecord pendingLatencyRecord in this.GetPendingRecords())
			{
				if (!flag)
				{
					builder.Append('|');
				}
				builder.Append(LatencyTracker.GetShortName(pendingLatencyRecord.ComponentId));
				flag = false;
			}
		}

		private void AppendComponentLatencyString(StringBuilder builder, int lastFoldingPoint, bool enableHeaderFolding, bool useTreeFormat)
		{
			if (useTreeFormat)
			{
				return;
			}
			bool flag = false;
			foreach (LatencyRecord record in this.GetCompletedRecords())
			{
				if (flag)
				{
					builder.Append('|');
					lastFoldingPoint = LatencyFormatter.AddFolding(builder, lastFoldingPoint, enableHeaderFolding);
				}
				LatencyFormatter.AppendLatencyRecord(builder, record, null);
				flag = true;
			}
		}

		public const int MaxRecordCount = 1000;

		public const ushort MaxAgentCount = 1000;

		public const string TotalShortName = "TOTAL";

		public const string TooManyAgentsShortName = "__TOO_MANY__";

		public static Func<DateTime> TimeProvider = new Func<DateTime>(LatencyTracker.DefaultTimeProvider);

		public static Func<long> StopwatchProvider = new Func<long>(LatencyTracker.DefaultStopWatchProvider);

		private static readonly IDictionary<ProcessTransportRole, string> perfCounterCategoryMap = new Dictionary<ProcessTransportRole, string>
		{
			{
				ProcessTransportRole.Edge,
				"MSExchangeTransport"
			},
			{
				ProcessTransportRole.Hub,
				"MSExchangeTransport"
			},
			{
				ProcessTransportRole.FrontEnd,
				"MSExchangeFrontEndTransport"
			},
			{
				ProcessTransportRole.MailboxDelivery,
				"MSExchange Delivery"
			},
			{
				ProcessTransportRole.MailboxSubmission,
				"MSExchange Submission"
			}
		};

		private static readonly TimeSpan PeriodicUpdateInterval = TimeSpan.FromMinutes(1.0);

		private static readonly Regex EscapedUnicodeRegex = new Regex("\\\\u(?<Value>[a-fA-F0-9]{4})");

		private static readonly LatencyTracker.ComponentDefinition[] componentsArray = new LatencyTracker.ComponentDefinition[]
		{
			new LatencyTracker.ComponentDefinition(LatencyComponent.None, "NONE", Strings.LatencyComponentNone, LatencyCounterType.None, ProcessTransportRoleFlags.All, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.CategorizerOnResolvedMessage, "CATRS", Strings.LatencyComponentCategorizerOnResolvedMessage, LatencyAgentGroup.Categorizer, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.CategorizerOnRoutedMessage, "CATRT", Strings.LatencyComponentCategorizerOnRoutedMessage, LatencyAgentGroup.Categorizer, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.CategorizerOnSubmittedMessage, "CATSM", Strings.LatencyComponentCategorizerOnSubmittedMessage, LatencyAgentGroup.Categorizer, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.CategorizerOnCategorizedMessage, "CATCM", Strings.LatencyComponentCategorizerOnCategorizedMessage, LatencyAgentGroup.Categorizer, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryAgentOnOpenConnection, "DAOC", Strings.LatencyComponentDeliveryAgentOnOpenConnection, LatencyAgentGroup.Delivery, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryAgentOnDeliverMailItem, "DADM", Strings.LatencyComponentDeliveryAgentOnDeliverMailItem, LatencyAgentGroup.Delivery, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SmtpReceiveOnDataCommand, "SMRDC", Strings.LatencyComponentSmtpReceiveOnDataCommand, LatencyAgentGroup.SmtpReceive, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge | ProcessTransportRoleFlags.FrontEnd, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SmtpReceiveOnEndOfData, "SMRED", Strings.LatencyComponentSmtpReceiveOnEndOfData, LatencyAgentGroup.SmtpReceive, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SmtpReceiveOnEndOfHeaders, "SMREH", Strings.LatencyComponentSmtpReceiveOnEndOfHeaders, LatencyAgentGroup.SmtpReceive, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge | ProcessTransportRoleFlags.FrontEnd, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SmtpReceiveOnRcptCommand, "SMRRC", Strings.LatencyComponentSmtpReceiveOnRcptCommand, LatencyAgentGroup.SmtpReceive, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge | ProcessTransportRoleFlags.FrontEnd, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SmtpReceiveOnRcpt2Command, "SMRR2C", Strings.LatencyComponentSmtpReceiveOnRcpt2Command, LatencyAgentGroup.SmtpReceive, LatencyCounterType.None, ProcessTransportRoleFlags.FrontEnd, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SmtpReceiveOnProxyInboundMessage, "SMRPI", Strings.LatencyComponentSmtpReceiveOnProxyInboundMessage, LatencyAgentGroup.SmtpReceive, LatencyCounterType.None, ProcessTransportRoleFlags.FrontEnd, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverOnCreatedMessage, "SDDCM", Strings.LatencyComponentStoreDriverOnCreatedMessage, LatencyAgentGroup.StoreDriver, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxDelivery, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverOnInitializedMessage, "SDDIM", Strings.LatencyComponentStoreDriverOnInitializedMessage, LatencyAgentGroup.StoreDriver, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxDelivery, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverOnPromotedMessage, "SDDPM", Strings.LatencyComponentStoreDriverOnPromotedMessage, LatencyAgentGroup.StoreDriver, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxDelivery, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverOnDeliveredMessage, "SDDDLV", Strings.LatencyComponentStoreDriverOnDeliveredMessage, LatencyAgentGroup.StoreDriver, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxDelivery, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverOnDemotedMessage, "SDSDM", Strings.LatencyComponentStoreDriverOnDemotedMessage, LatencyAgentGroup.StoreDriver, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxDelivery, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionOnDemotedMessage, "MTSSDSDM", Strings.LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionOnDemotedMessage, LatencyAgentGroup.MailboxTransportSubmissionStoreDriverSubmission, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverOnCompletedMessage, "SDDCPM", Strings.LatencyComponentStoreDriverOnCompletedMessage, LatencyAgentGroup.StoreDriver, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxDelivery, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.Agent, "AGNT", Strings.LatencyComponentAgent, LatencyAgentGroup.UnassignedAgentGroup, LatencyCounterType.None, ProcessTransportRoleFlags.All, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.Categorizer, "CAT", Strings.LatencyComponentCategorizer, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.CategorizerBifurcation, "CBIF", Strings.LatencyComponentCategorizerBifurcation, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.CategorizerContentConversion, "CCC", Strings.LatencyComponentCategorizerContentConversion, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.CategorizerFinal, "CFIN", Strings.LatencyComponentCategorizerFinal, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.CategorizerLocking, "CL", Strings.LatencyComponentCategorizerLocking, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.CategorizerResolver, "CRSL", Strings.LatencyComponentCategorizerResolver, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.CategorizerRouting, "CRT", Strings.LatencyComponentCategorizerRouting, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.ContentAggregation, "CA", Strings.LatencyComponentContentAggregation, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.ContentAggregationMailItemCommit, "CAMIC", Strings.LatencyComponentContentAggregationMailItemCommit, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.Deferral, "DFR", Strings.LatencyComponentDeferral, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.Delivery, "D", Strings.LatencyComponentDelivery, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryAgent, "DAD", Strings.LatencyComponentDeliveryAgent, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryQueueInternal, "QDI", Strings.LatencyComponentDeliveryQueueInternal, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryQueueExternal, "QDE", Strings.LatencyComponentDeliveryQueueExternal, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryQueueMailbox, "QDM", Strings.LatencyComponentDeliveryQueueMailbox, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryQueueMailboxDeliverAgentTransientFailure, "QDMDATF", Strings.LatencyComponentDeliveryQueueMailboxDeliverAgentTransientFailure, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryQueueMailboxDynamicMailboxDatabaseThrottlingLimitExceeded, "QDMDMDTLE", Strings.LatencyComponentDeliveryQueueMailboxDynamicMailboxDatabaseThrottlingLimitExceeded, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryQueueMailboxInsufficientResources, "QDMIR", Strings.LatencyComponentDeliveryQueueMailboxInsufficientResources, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryQueueMailboxMailboxDatabaseOffline, "QDMMDO", Strings.LatencyComponentDeliveryQueueMailboxMailboxDatabaseOffline, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryQueueMailboxMailboxServerOffline, "QDMMSO", Strings.LatencyComponentDeliveryQueueMailboxMailboxServerOffline, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryQueueMailboxMapiExceptionLockViolation, "QDMMELV", Strings.LatencyComponentDeliveryQueueMailboxMapiExceptionLockViolation, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryQueueMailboxMapiExceptionTimeout, "QDMMET", Strings.LatencyComponentDeliveryQueueMailboxMapiExceptionTimeout, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryQueueMailboxMaxConcurrentMessageSizeLimitExceeded, "QDMMCMSLE", Strings.LatencyComponentDeliveryQueueMailboxMaxConcurrentMessageSizeLimitExceeded, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryQueueMailboxRecipientThreadLimitExceeded, "QDMRTLE", Strings.LatencyComponentDeliveryQueueMailboxRecipientThreadLimitExceeded, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DeliveryQueueLocking, "QDL", Strings.LatencyComponentDeliveryQueueLocking, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.DsnGenerator, "DSN", Strings.LatencyComponentDsnGenerator, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.Dumpster, "DMP", Strings.LatencyComponentDumpster, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.ExternalServers, "ES", Strings.LatencyComponentExternalServers, LatencyCounterType.LongRangePercentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge | ProcessTransportRoleFlags.FrontEnd, LatencyComponentAction.SkipEndToEnd),
			new LatencyTracker.ComponentDefinition(LatencyComponent.ExternalPartnerServers, "EPS", Strings.LatencyComponentExternalPartnerServers, LatencyCounterType.LongRangePercentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge | ProcessTransportRoleFlags.FrontEnd, LatencyComponentAction.SkipEndToEnd),
			new LatencyTracker.ComponentDefinition(LatencyComponent.Heartbeat, "HB", Strings.LatencyComponentHeartbeat, LatencyCounterType.None, ProcessTransportRoleFlags.MailboxDelivery, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailboxMove, "MM", Strings.LatencyComponentMailboxMove, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailboxRules, "MR", Strings.LatencyComponentMailboxRules, LatencyCounterType.None, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailboxTransportSubmissionService, "MTSS", Strings.LatencyComponentMailboxTransportSubmissionService, LatencyCounterType.Percentile, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SubmissionAssistant, "SA", Strings.LatencyComponentSubmissionAssistant, LatencyCounterType.None, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SubmissionAssistantThrottling, "SAT", Strings.LatencyComponentSubmissionAssistantThrottling, LatencyCounterType.None, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmission, "MTSSD", Strings.LatencyComponentMailboxTransportSubmissionStoreDriverSubmission, LatencyCounterType.None, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionAD, "MTSSDA", Strings.LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionAD, LatencyCounterType.Percentile, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionContentConversion, "MTSSDC", Strings.LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionContentConversion, LatencyCounterType.Percentile, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionHubSelector, "MTSSDH", Strings.LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionHubSelector, LatencyCounterType.Percentile, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionPerfContextLdap, "MTSSDPL", Strings.LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionPerfContextLdap, LatencyCounterType.None, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionSmtp, "MTSSDM", Strings.LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionSmtp, LatencyCounterType.None, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionSmtpOut, "MTSSDMO", Strings.LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionSmtpOut, LatencyCounterType.Percentile, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionStoreOpenSession, "MTSSDSOS", Strings.LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionStoreOpenSession, LatencyCounterType.Percentile, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionStoreDisposeSession, "MTSSDSDS", Strings.LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionStoreDisposeSession, LatencyCounterType.Percentile, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionStoreStats, "MTSSDSS", Strings.LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionStoreStats, LatencyCounterType.None, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailSubmissionService, "MSS", Strings.LatencyComponentMailSubmissionService, LatencyCounterType.None, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailSubmissionServiceFailedAttempt, "MSSFA", Strings.LatencyComponentMailSubmissionServiceFailedAttempt, LatencyCounterType.None, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailSubmissionServiceNotify, "MSSN", Strings.LatencyComponentMailSubmissionServiceNotify, LatencyCounterType.None, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailSubmissionServiceNotifyRetrySchedule, "MSSNRS", Strings.LatencyComponentMailSubmissionServiceNotifyRetrySchedule, LatencyCounterType.None, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailSubmissionServiceShadowResubmitDecision, "MSSSRD", Strings.LatencyComponentMailSubmissionServiceShadowResubmitDecision, LatencyCounterType.None, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MailSubmissionServiceThrottling, "MSST", Strings.LatencyComponentMailSubmissionServiceThrottling, LatencyCounterType.None, ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.MexRuntimeThreadpoolQueue, "MEXRTPQ", Strings.LatencyComponentMexRuntimeThreadpoolQueue, LatencyCounterType.None, ProcessTransportRoleFlags.All, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.NonSmtpGateway, "NSGW", Strings.LatencyComponentNonSmtpGateway, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.OriginalMailDsn, "OMDSN", Strings.LatencyComponentOriginalMailDsn, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.Pickup, "PCK", Strings.LatencyComponentPickup, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.PoisonQueue, "QP", Strings.LatencyComponentPoisonQueue, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.ProcessingScheduler, "PSC", Strings.LatencyComponentProcessingScheduler, LatencyCounterType.PrioritizedPercentile, ProcessTransportRoleFlags.Hub, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.ProcessingSchedulerScoped, "PSCSQ", Strings.LatencyComponentProcessingSchedulerScoped, LatencyCounterType.PrioritizedPercentile, ProcessTransportRoleFlags.Hub, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.QuarantineReleaseOrReport, "QUAR", Strings.LatencyComponentQuarantineReleaseOrReport, LatencyCounterType.None, ProcessTransportRoleFlags.FrontEnd, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.Replay, "RPL", Strings.LatencyComponentReplay, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.RmsAcquireB2BRac, "RMABR", Strings.LatencyComponentRmsAcquireB2BRac, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.RmsAcquireB2BLicense, "RMABL", Strings.LatencyComponentRmsAcquireB2BLicense, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.RmsAcquireCertificationMexData, "RMACM", Strings.LatencyComponentRmsAcquireCertificationMexData, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.RmsAcquireClc, "RMAC", Strings.LatencyComponentRmsAcquireClc, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.RmsAcquireLicense, "RMAL", Strings.LatencyComponentRmsAcquireLicense, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.RmsAcquireServerLicensingMexData, "RMASLM", Strings.LatencyComponentRmsAcquireServerLicensingMexData, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.RmsAcquirePrelicense, "RMAPL", Strings.LatencyComponentRmsAcquirePreLicense, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.RmsAcquireServerBoxRac, "RMASR", Strings.LatencyComponentRmsAcquireServerBoxRac, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.RmsAcquireTemplateInfo, "RMATI", Strings.LatencyComponentRmsAcquireTemplateInfo, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.RmsAcquireTemplates, "RMAT", Strings.LatencyComponentRmsAcquireTemplates, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.RmsFindServiceLocation, "RMFSL", Strings.LatencyComponentRmsFindServiceLocation, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.RmsRequestDelegationToken, "RMRDT", Strings.LatencyComponentRmsRequestDelegationToken, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.ServiceRestart, "RST", Strings.LatencyComponentServiceRestart, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.ShadowQueue, "QSH", Strings.LatencyComponentShadowQueue, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.SkipEndToEnd),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SmtpReceive, "SMR", Strings.LatencyComponentSmtpReceive, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge | ProcessTransportRoleFlags.MailboxDelivery, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SmtpReceiveCommit, "SMRC", Strings.LatencyComponentSmtpReceiveCommit, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SmtpReceiveCommitLocal, "SMRCL", Strings.LatencyComponentSmtpReceiveCommitLocal, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SmtpReceiveCommitRemote, "SMRCR", Strings.LatencyComponentSmtpReceiveCommitRemote, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SmtpReceiveDataInternal, "SMRDI", Strings.LatencyComponentSmtpReceiveDataInternal, LatencyCounterType.Percentile, ProcessTransportRoleFlags.All, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SmtpReceiveDataExternal, "SMRDE", Strings.LatencyComponentSmtpReceiveDataExternal, LatencyCounterType.Percentile, ProcessTransportRoleFlags.All, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SmtpSend, "SMS", Strings.LatencyComponentSmtpSend, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge | ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SmtpSendConnect, "SMSC", Strings.LatencyComponentSmtpSendConnect, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge | ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SmtpSendMailboxDelivery, "SMSMBXD", Strings.LatencyComponentSmtpSendMailboxDelivery, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverDelivery, "SDD", Strings.LatencyComponentStoreDriverDelivery, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxDelivery, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverDeliveryAD, "SDDAD", Strings.LatencyComponentStoreDriverDeliveryAD, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxDelivery, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverDeliveryContentConversion, "SDDCC", Strings.LatencyComponentStoreDriverDeliveryContentConversion, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxDelivery, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverDeliveryMessageConcurrency, "SDDMC", Strings.LatencyComponentStoreDriverDeliveryMessageConcurrency, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxDelivery, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverDeliveryRpc, "SDDR", Strings.LatencyComponentStoreDriverDeliveryRpc, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxDelivery, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverDeliveryStore, "SDDS", Strings.LatencyComponentStoreDriverDeliveryStore, LatencyCounterType.None, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxDelivery, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverSubmissionAD, "SDSAD", Strings.LatencyComponentStoreDriverSubmissionAD, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverSubmissionRpc, "SDSR", Strings.LatencyComponentStoreDriverSubmissionRpc, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverSubmissionStore, "SDSS", Strings.LatencyComponentStoreDriverSubmissionStore, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.StoreDriverSubmit, "SDS", Strings.LatencyComponentStoreDriverSubmit, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.MailboxSubmission, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.SubmissionQueue, "QS", Strings.LatencyComponentSubmissionQueue, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.UnderThreshold, "UTH", Strings.LatencyComponentUnderThreshold, LatencyCounterType.None, ProcessTransportRoleFlags.All, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.Unknown, "UNK", Strings.LatencyComponentUnknown, LatencyCounterType.None, ProcessTransportRoleFlags.All, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.UnreachableQueue, "QU", Strings.LatencyComponentUnreachableQueue, LatencyCounterType.Percentile, ProcessTransportRoleFlags.Hub | ProcessTransportRoleFlags.Edge, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.Total, "TOTAL", Strings.LatencyComponentTotal, LatencyCounterType.PrioritizedPercentile, ProcessTransportRoleFlags.All, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.Process, "P", Strings.LatencyComponentProcess, LatencyCounterType.PrioritizedPercentile, ProcessTransportRoleFlags.All, LatencyComponentAction.Normal),
			new LatencyTracker.ComponentDefinition(LatencyComponent.TooManyComponents, "MANY", Strings.LatencyComponentTooManyComponents, LatencyCounterType.None, ProcessTransportRoleFlags.All, LatencyComponentAction.Normal)
		};

		private static readonly Dictionary<int, LatencyTracker.ComponentDefinition> components = LatencyTracker.componentsArray.ToDictionary((LatencyTracker.ComponentDefinition cd) => (int)cd.ComponentId);

		private static readonly Dictionary<string, LatencyTracker.ComponentDefinition> ShortNameToDefinitionMap = LatencyTracker.componentsArray.ToDictionary((LatencyTracker.ComponentDefinition cd) => cd.ShortName);

		private static readonly AutoReadThroughCache<string, ILatencyPerformanceCounter> agentPerformanceCounters = new AutoReadThroughCache<string, ILatencyPerformanceCounter>((string name) => LatencyPerformanceCounter.CreateInstance(name, LatencyTracker.Configuration.PercentileLatencyExpiryInterval, (long)((ulong)LatencyTracker.Configuration.PercentileLatencyInfinitySeconds)));

		private static ILatencyPerformanceCounter endToEndPerformanceCounter;

		private static TransportAppConfig.LatencyTrackerConfig config;

		private static string[][] agentNames = new string[6][];

		private static GuardedTimer timer;

		private List<LatencyRecord> latencies;

		private List<PendingLatencyRecord> pendingComponents;

		public delegate ILatencyPerformanceCounter CreateLatencyPerformanceCounterDelegate(string instanceName);

		internal class ComponentDefinition
		{
			public ComponentDefinition(LatencyComponent componentId, string shortName, LocalizedString fullName, LatencyCounterType latencyCounterType, ProcessTransportRoleFlags transportRoles, LatencyComponentAction componentAction = LatencyComponentAction.Normal) : this(componentId, shortName, fullName, LatencyAgentGroup.UnassignedAgentGroup, latencyCounterType, transportRoles, componentAction)
			{
			}

			public ComponentDefinition(LatencyComponent componentId, string shortName, LocalizedString fullName, LatencyAgentGroup agentGroup, LatencyCounterType latencyCounterType, ProcessTransportRoleFlags transportRoles, LatencyComponentAction componentAction = LatencyComponentAction.Normal)
			{
				this.ComponentId = componentId;
				this.AgentGroup = agentGroup;
				this.ShortName = shortName;
				this.FullName = fullName;
				this.latencyCounterType = latencyCounterType;
				this.transportRoles = transportRoles;
				this.latencyComponentAction = componentAction;
			}

			public static void SetLatencyPerformanceCounterTestStubCreator(LatencyTracker.CreateLatencyPerformanceCounterDelegate createDelegate)
			{
				LatencyTracker.ComponentDefinition.latencyPerformanceCounterTestStubCreator = createDelegate;
			}

			public static ProcessTransportRoleFlags MapTransportRoleToFlag(ProcessTransportRole transportRole)
			{
				switch (transportRole)
				{
				case ProcessTransportRole.Hub:
					return ProcessTransportRoleFlags.Hub;
				case ProcessTransportRole.Edge:
					return ProcessTransportRoleFlags.Edge;
				case ProcessTransportRole.FrontEnd:
					return ProcessTransportRoleFlags.FrontEnd;
				case ProcessTransportRole.MailboxSubmission:
					return ProcessTransportRoleFlags.MailboxSubmission;
				case ProcessTransportRole.MailboxDelivery:
					return ProcessTransportRoleFlags.MailboxDelivery;
				default:
					return ProcessTransportRoleFlags.None;
				}
			}

			public void Initialize(ProcessTransportRole transportRole)
			{
				if (LatencyTracker.ComponentDefinition.latencyPerformanceCounterTestStubCreator != null)
				{
					this.perfCounter = LatencyTracker.ComponentDefinition.latencyPerformanceCounterTestStubCreator(this.FullName.ToString());
					return;
				}
				ProcessTransportRoleFlags processTransportRoleFlags = LatencyTracker.ComponentDefinition.MapTransportRoleToFlag(transportRole);
				if ((processTransportRoleFlags & this.transportRoles) != processTransportRoleFlags)
				{
					return;
				}
				switch (this.latencyCounterType)
				{
				case LatencyCounterType.None:
					break;
				case LatencyCounterType.Percentile:
					this.perfCounter = LatencyPerformanceCounter.CreateInstance(this.FullName.ToString(), LatencyTracker.Configuration.PercentileLatencyExpiryInterval, (long)((ulong)LatencyTracker.Configuration.PercentileLatencyInfinitySeconds));
					return;
				case LatencyCounterType.PrioritizedPercentile:
					if (transportRole == ProcessTransportRole.Hub || transportRole == ProcessTransportRole.Edge || transportRole == ProcessTransportRole.MailboxDelivery)
					{
						this.perfCounter = PrioritizedLatencyPerformanceCounter.CreateInstance(this.FullName.ToString(), LatencyTracker.Configuration.PercentileLatencyExpiryInterval, (long)((ulong)LatencyTracker.Configuration.PercentileLatencyInfinitySeconds));
						return;
					}
					this.perfCounter = LatencyPerformanceCounter.CreateInstance(this.FullName.ToString(), LatencyTracker.Configuration.PercentileLatencyExpiryInterval, (long)((ulong)LatencyTracker.Configuration.PercentileLatencyInfinitySeconds));
					return;
				case LatencyCounterType.LongRangePercentile:
					this.perfCounter = LatencyPerformanceCounter.CreateInstance(this.FullName.ToString(), LatencyTracker.Configuration.PercentileLatencyExpiryInterval, (long)((ulong)LatencyTracker.Configuration.PercentileLatencyInfinitySeconds), true);
					break;
				default:
					return;
				}
			}

			public void UpdatePerformanceCounter(long latencySeconds)
			{
				if (this.perfCounter != null)
				{
					if (this.latencyComponentAction == LatencyComponentAction.SkipEndToEnd && this.perfCounter.CounterType == LatencyPerformanceCounterType.EndToEnd)
					{
						return;
					}
					if (this.latencyComponentAction == LatencyComponentAction.ResetEndToEnd && this.perfCounter.CounterType == LatencyPerformanceCounterType.EndToEnd)
					{
						this.perfCounter.Reset();
						return;
					}
					this.perfCounter.AddValue(latencySeconds);
				}
			}

			public void UpdatePerformanceCounter(long latencySeconds, DeliveryPriority priority)
			{
				if (this.perfCounter != null)
				{
					if (this.latencyComponentAction == LatencyComponentAction.SkipEndToEnd && this.perfCounter.CounterType == LatencyPerformanceCounterType.EndToEnd)
					{
						return;
					}
					if (this.latencyComponentAction == LatencyComponentAction.ResetEndToEnd && this.perfCounter.CounterType == LatencyPerformanceCounterType.EndToEnd)
					{
						this.perfCounter.Reset();
						return;
					}
					this.perfCounter.AddValue(latencySeconds, priority);
				}
			}

			public void UpdatePerformanceCounter()
			{
				if (this.perfCounter != null)
				{
					this.perfCounter.Update();
				}
			}

			public readonly LatencyComponent ComponentId;

			public readonly string ShortName;

			public readonly LocalizedString FullName;

			public readonly LatencyAgentGroup AgentGroup;

			private static LatencyTracker.CreateLatencyPerformanceCounterDelegate latencyPerformanceCounterTestStubCreator;

			private readonly ProcessTransportRoleFlags transportRoles;

			private readonly LatencyCounterType latencyCounterType;

			private ILatencyPerformanceCounter perfCounter;

			private LatencyComponentAction latencyComponentAction;
		}
	}
}

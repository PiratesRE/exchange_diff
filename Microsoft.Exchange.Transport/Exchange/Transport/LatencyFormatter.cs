using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport
{
	internal class LatencyFormatter
	{
		public LatencyFormatter(LatencyTracker latencyTracker, string localServerFqdn, DateTime localArrivalTime, DateTime orgArrivalTime, bool includeOrgArrivalTime, bool reportEndToEnd, bool setUpdateTotalPerfCounter = false)
		{
			if (setUpdateTotalPerfCounter)
			{
				this.flags |= LatencyFormatter.FormatFlags.UpdateTotalPerfCounter;
			}
			this.priority = DeliveryPriority.Normal;
			this.localTracker = latencyTracker;
			this.localArrivalTime = localArrivalTime;
			this.orgArrivalTime = orgArrivalTime;
			this.localServerFqdn = (string.IsNullOrEmpty(localServerFqdn) ? ComputerInformation.DnsPhysicalFullyQualifiedDomainName : localServerFqdn);
			if (reportEndToEnd)
			{
				this.flags |= LatencyFormatter.FormatFlags.ReportEndToEnd;
			}
			if (includeOrgArrivalTime)
			{
				this.flags |= LatencyFormatter.FormatFlags.IncludeOrgArrivalTime;
			}
			this.externalDeliveryEnqueueTime = DateTime.MinValue;
		}

		public LatencyFormatter(IReadOnlyMailItem mailItem, string localServerFqdn, bool reportEndToEnd, DateTime externalDeliveryEnqueueTime, DateTime orgArrivalTime) : this(mailItem, localServerFqdn, LatencyFormatter.FormatFlags.IncludeOrgArrivalTime | LatencyFormatter.FormatFlags.UpdateTotalPerfCounter | (reportEndToEnd ? LatencyFormatter.FormatFlags.ReportEndToEnd : LatencyFormatter.FormatFlags.None), externalDeliveryEnqueueTime, orgArrivalTime)
		{
		}

		public LatencyFormatter(IReadOnlyMailItem mailItem, string localServerFqdn, bool reportEndToEnd) : this(mailItem, localServerFqdn, LatencyFormatter.FormatFlags.IncludeOrgArrivalTime | LatencyFormatter.FormatFlags.UpdateTotalPerfCounter | (reportEndToEnd ? LatencyFormatter.FormatFlags.ReportEndToEnd : LatencyFormatter.FormatFlags.None), DateTime.MinValue, DateTime.MinValue)
		{
		}

		private LatencyFormatter(IReadOnlyMailItem mailItem, string localServerFqdn, LatencyFormatter.FormatFlags flags, DateTime externalDeliveryEnqueueTime, DateTime orgArrivalTime)
		{
			this.priority = mailItem.Priority;
			this.localTracker = mailItem.LatencyTracker;
			this.flags = flags;
			this.localServerFqdn = (string.IsNullOrEmpty(localServerFqdn) ? ComputerInformation.DnsPhysicalFullyQualifiedDomainName : localServerFqdn);
			this.localArrivalTime = mailItem.DateReceived;
			if (orgArrivalTime != DateTime.MinValue)
			{
				this.orgArrivalTime = orgArrivalTime;
			}
			else
			{
				Util.TryGetOrganizationalMessageArrivalTime(mailItem, out this.orgArrivalTime);
			}
			this.externalDeliveryEnqueueTime = externalDeliveryEnqueueTime;
			if (this.ReportEndToEnd)
			{
				this.previousHops = LatencyHeaderManager.GetPreviousHops(mailItem);
			}
		}

		private bool EnableHeaderFolding
		{
			get
			{
				return (this.flags & LatencyFormatter.FormatFlags.EnableHeaderFolding) != LatencyFormatter.FormatFlags.None;
			}
		}

		private bool IgnoreTotalThreshold
		{
			get
			{
				return (this.flags & LatencyFormatter.FormatFlags.IgnoreTotalThreshold) != LatencyFormatter.FormatFlags.None;
			}
		}

		private bool IncludeCurrentTime
		{
			get
			{
				return (this.flags & LatencyFormatter.FormatFlags.IncludeCurrentTime) != LatencyFormatter.FormatFlags.None;
			}
		}

		private bool IncludeOrgArrivalTime
		{
			get
			{
				return (this.flags & LatencyFormatter.FormatFlags.IncludeOrgArrivalTime) != LatencyFormatter.FormatFlags.None;
			}
		}

		private bool ReportEndToEnd
		{
			get
			{
				return (this.flags & LatencyFormatter.FormatFlags.ReportEndToEnd) != LatencyFormatter.FormatFlags.None;
			}
		}

		private bool UpdateTotalPerfCounter
		{
			get
			{
				return (this.flags & LatencyFormatter.FormatFlags.UpdateTotalPerfCounter) != LatencyFormatter.FormatFlags.None;
			}
		}

		public TimeSpan EndToEndLatency
		{
			get
			{
				return this.endToEndLatency;
			}
		}

		public TimeSpan ExternalSendLatency
		{
			get
			{
				return this.externalSendLatency;
			}
		}

		private bool TotalFromOrgArrivalTime
		{
			get
			{
				return (this.flags & LatencyFormatter.FormatFlags.CalculateTotalFromOrgArrivalTime) != LatencyFormatter.FormatFlags.None;
			}
		}

		public static string FormatLatencyInProgressHeader(IReadOnlyMailItem mailItem, string localServerFqdn, DateTime orgArrivalTime, bool useTreeFormat)
		{
			LatencyFormatter.FormatFlags formatFlags = LatencyFormatter.FormatFlags.None;
			if (orgArrivalTime != DateTime.MinValue)
			{
				formatFlags = LatencyFormatter.FormatFlags.CalculateTotalFromOrgArrivalTime;
			}
			LatencyFormatter latencyFormatter = new LatencyFormatter(mailItem, localServerFqdn, LatencyFormatter.FormatFlags.EnableHeaderFolding | LatencyFormatter.FormatFlags.IgnoreTotalThreshold | LatencyFormatter.FormatFlags.IncludeCurrentTime | formatFlags, DateTime.MinValue, orgArrivalTime);
			return latencyFormatter.FormatAndUpdatePerfCounters(useTreeFormat);
		}

		public static string FormatExternalLatencyHeader(string fqdn, TimeSpan totalLatency)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			LatencyFormatter.AppendServerFqdn(stringBuilder, true, fqdn, 0, fqdn.Length);
			LatencyRecord record;
			if (LatencyTracker.TryGetTotalLatencyRecord(totalLatency, false, out record))
			{
				LatencyFormatter.AppendLatencyRecord(stringBuilder, record, null);
				return stringBuilder.ToString();
			}
			return null;
		}

		public static string FormatLatencyHeader(LatencyInProgressParser parser, DateTime localArrivalTime, LatencyComponent previousHopDeliveryComponent, LatencyComponent previousHopSubComponent, TimeSpan previousHopSubComponentLatency)
		{
			TimeSpan timeSpan;
			if (localArrivalTime < parser.PreDeliveryTime)
			{
				timeSpan = TimeSpan.Zero;
			}
			else
			{
				timeSpan = localArrivalTime - parser.PreDeliveryTime;
			}
			StringBuilder stringBuilder = new StringBuilder();
			LatencyFormatter.AppendServerFqdn(stringBuilder, true, parser.StringToParse, parser.ServerFqdnStartIndex, parser.ServerFqdnLength);
			bool flag = false;
			LatencyRecord record;
			if ((ushort)parser.TotalSeconds != 65535 && LatencyTracker.TryGetTotalLatencyRecord(timeSpan + TimeSpan.FromSeconds((double)parser.TotalSeconds), false, out record))
			{
				LatencyFormatter.AppendLatencyRecord(stringBuilder, record, parser.PreviousHopProcess);
				flag = true;
			}
			if (parser.ComponentsStartIndex != -1)
			{
				if (flag)
				{
					stringBuilder.Append('|');
				}
				stringBuilder.Append(parser.StringToParse, parser.ComponentsStartIndex, parser.ComponentsLength);
				flag = true;
			}
			LatencyRecord record2;
			if (previousHopSubComponent != LatencyComponent.None && LatencyTracker.TryGetComponentLatencyRecord(previousHopSubComponent, previousHopSubComponentLatency, out record2))
			{
				if (flag)
				{
					stringBuilder.Append('|');
				}
				LatencyFormatter.AppendLatencyRecord(stringBuilder, record2, null);
				flag = true;
			}
			LatencyRecord record3;
			if (LatencyTracker.TryGetComponentLatencyRecord(previousHopDeliveryComponent, timeSpan, out record3))
			{
				if (flag)
				{
					stringBuilder.Append('|');
				}
				LatencyFormatter.AppendLatencyRecord(stringBuilder, record3, null);
				flag = true;
			}
			if (parser.PendingStartIndex != -1)
			{
				if (flag)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append(parser.StringToParse, parser.PendingStartIndex, parser.PreDeliveryTimeSeparatorIndex - parser.PendingStartIndex);
				flag = true;
			}
			if (!flag)
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		public static bool IsSeparator(char c)
		{
			return c == ';' || c == ':' || c == '=' || c == '|' || c == '(';
		}

		public static XElement GetDiagnosticInfo(LatencyTracker latencyTracker)
		{
			XElement xelement = new XElement("Latency");
			foreach (LatencyRecord latencyRecord in latencyTracker.GetCompletedRecords())
			{
				xelement.Add(new XElement("item", new object[]
				{
					new XAttribute("name", latencyRecord.ComponentShortName),
					latencyRecord.Latency
				}));
			}
			return xelement;
		}

		public string FormatAndUpdatePerfCounters()
		{
			return this.FormatAndUpdatePerfCounters(LatencyTracker.TreeLatencyTrackingEnabled);
		}

		public string FormatAndUpdatePerfCounters(bool useTreeFormat)
		{
			if (useTreeFormat && !this.localTracker.SupportsTreeFormatting)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			this.ComputeEndToEndLatencies();
			string text = this.FormatOrgArrivalTime();
			DateTime d = this.TotalFromOrgArrivalTime ? this.orgArrivalTime : this.localArrivalTime;
			TimeSpan timeSpan = LatencyTracker.TimeProvider() - d;
			if (this.UpdateTotalPerfCounter)
			{
				LatencyTracker.UpdateTotalPerfCounter((this.externalDeliveryEnqueueTime != DateTime.MinValue) ? (this.externalDeliveryEnqueueTime - d) : timeSpan, this.priority);
			}
			LatencyRecord record;
			bool flag = LatencyTracker.TryGetTotalLatencyRecord(timeSpan, this.IgnoreTotalThreshold, out record);
			bool flag2 = false;
			bool flag3 = false;
			if (this.localTracker != null)
			{
				flag2 = this.localTracker.HasCompletedComponents;
				flag3 = this.localTracker.HasPendingComponents;
			}
			bool flag4 = false;
			if (text != null)
			{
				stringBuilder.Append(text);
				flag4 = true;
			}
			if (this.previousHops != null)
			{
				if (flag4)
				{
					stringBuilder.Append(';');
					flag4 = false;
				}
				this.AppendPreviousHops(stringBuilder);
			}
			if (this.ReportEndToEnd)
			{
				TimeSpan t = (this.externalSendLatency != TimeSpan.MinValue) ? this.externalSendLatency : this.endToEndLatency;
				if (t != TimeSpan.MinValue)
				{
					LatencyTracker.UpdateTotalEndToEndLatency(this.priority, (long)t.TotalSeconds);
				}
			}
			if (flag || flag2 || flag3)
			{
				if (flag4)
				{
					stringBuilder.Append(';');
				}
				else if (this.previousHops != null)
				{
					stringBuilder.Append(';');
				}
				LatencyFormatter.AppendServerFqdn(stringBuilder, this.ReportEndToEnd, this.localServerFqdn, 0, this.localServerFqdn.Length);
			}
			if (flag)
			{
				LatencyFormatter.AppendLatencyRecord(stringBuilder, record, LatencyTracker.ProcessShortName);
			}
			if (this.localTracker != null && flag2 && this.localTracker.AggregatedUnderThresholdTicks > 0L)
			{
				flag |= LatencyFormatter.AppendComponent(stringBuilder, flag, LatencyComponent.UnderThreshold, TimeSpan.FromTicks(this.localTracker.AggregatedUnderThresholdTicks));
			}
			if (this.localTracker != null)
			{
				this.localTracker.AppendLatencyString(stringBuilder, useTreeFormat, flag, this.EnableHeaderFolding);
			}
			if (this.IncludeCurrentTime && (flag || flag2 || flag3))
			{
				stringBuilder.Append(';');
				stringBuilder.Append(LatencyFormatter.FormatDateTime(LatencyTracker.TimeProvider()));
			}
			return stringBuilder.ToString();
		}

		public string FormatOrgArrivalTime()
		{
			if (this.orgArrivalTime == DateTime.MinValue || !this.IncludeOrgArrivalTime)
			{
				return null;
			}
			return LatencyFormatter.FormatDateTime(this.orgArrivalTime);
		}

		private void ComputeEndToEndLatencies()
		{
			DateTime dateTime = LatencyTracker.TimeProvider();
			DateTime dateTime2 = (this.orgArrivalTime == DateTime.MinValue) ? DateTime.MinValue : this.orgArrivalTime.ToUniversalTime();
			DateTime dateTime3 = (this.externalDeliveryEnqueueTime == DateTime.MinValue) ? DateTime.MinValue : this.externalDeliveryEnqueueTime.ToUniversalTime();
			if (dateTime2 != DateTime.MinValue && dateTime2 < dateTime)
			{
				this.endToEndLatency = dateTime - dateTime2;
				if (dateTime3 != DateTime.MinValue && dateTime2 < dateTime3 && dateTime3 < dateTime)
				{
					this.externalSendLatency = dateTime3 - dateTime2;
				}
			}
		}

		private static void AppendServerFqdn(StringBuilder builder, bool endToEnd, string s, int startIndex, int count)
		{
			builder.Append(endToEnd ? "SRV" : "LSRV");
			builder.Append('=');
			builder.Append(s, startIndex, count);
			builder.Append(':');
		}

		internal static void AppendLatencyRecord(StringBuilder builder, LatencyRecord record, string suffix = null)
		{
			builder.Append(record.ComponentShortName);
			if (!string.IsNullOrEmpty(suffix))
			{
				builder.Append('-');
				builder.Append(suffix);
			}
			builder.Append('=');
			builder.Append(record.Latency.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture));
		}

		internal static void AppendPendingLatencyRecord(StringBuilder builder, PendingLatencyRecord record, TimeSpan latency)
		{
			builder.Append(record.ComponentShortName);
			builder.Append('-');
			builder.Append("PEN");
			builder.Append('=');
			builder.Append(latency.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture));
		}

		internal static int AddFolding(StringBuilder builder, int lastFoldingPoint, bool enableHeaderFoldering)
		{
			if (enableHeaderFoldering && builder.Length - lastFoldingPoint > 80)
			{
				builder.Append(' ');
				lastFoldingPoint = builder.Length;
			}
			return lastFoldingPoint;
		}

		private static string FormatDateTime(DateTime dt)
		{
			return dt.ToUniversalTime().ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffZ", DateTimeFormatInfo.InvariantInfo);
		}

		private static bool AppendComponent(StringBuilder builder, bool needsRecordSeparator, LatencyComponent componentId, TimeSpan latencyTimeSpan)
		{
			LatencyRecord record;
			if (!LatencyTracker.TryGetComponentLatencyRecord(componentId, latencyTimeSpan, out record))
			{
				return false;
			}
			if (needsRecordSeparator)
			{
				builder.Append('|');
			}
			LatencyFormatter.AppendLatencyRecord(builder, record, null);
			return true;
		}

		private void AppendPreviousHops(StringBuilder builder)
		{
			for (int i = 0; i < this.previousHops.Count; i++)
			{
				if (i > 0)
				{
					builder.Append(';');
				}
				builder.Append(this.previousHops[i]);
			}
		}

		public const char AgentNameSeparator = '-';

		public const char ComponentSuffixSeparator = '-';

		public const char OrgArrivalTimeSeparator = ';';

		public const char CurrentTimeSeparator = ';';

		public const char ServerSeparator = ';';

		public const char ServerFqdnSeparator = ':';

		public const char ComponentValueSeparator = '=';

		public const char LatencyRecordSeparator = '|';

		public const char ChildBegin = '(';

		public const char ChildEnd = ')';

		public const char PendingPartSeparator = ';';

		public const string EndToEndServerPrefix = "SRV";

		public const string LocalServerPrefix = "LSRV";

		public const string PendingSuffix = "PEN";

		public const string IncompleteSuffix = "INC";

		public const string FormatSpecifier = "F3";

		public const int HeaderFoldingThreshold = 80;

		private readonly DateTime externalDeliveryEnqueueTime = DateTime.MinValue;

		private LatencyTracker localTracker;

		private string localServerFqdn;

		private DateTime localArrivalTime;

		private DateTime orgArrivalTime;

		private TimeSpan endToEndLatency = TimeSpan.MinValue;

		private TimeSpan externalSendLatency = TimeSpan.MinValue;

		private IList<string> previousHops;

		private LatencyFormatter.FormatFlags flags;

		private DeliveryPriority priority;

		[Flags]
		private enum FormatFlags
		{
			None = 0,
			EnableHeaderFolding = 1,
			IgnoreTotalThreshold = 2,
			IncludeCurrentTime = 4,
			IncludeOrgArrivalTime = 8,
			ReportEndToEnd = 16,
			UpdateTotalPerfCounter = 32,
			CalculateTotalFromOrgArrivalTime = 64
		}
	}
}

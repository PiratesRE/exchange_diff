using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class Report
	{
		internal Report(ReportData reportData)
		{
			SessionStatistics stats = new SessionStatistics();
			SessionStatistics stats2 = new SessionStatistics();
			this.SessionStatistics = new SessionStatistics();
			this.ArchiveSessionStatistics = new SessionStatistics();
			foreach (ReportEntry reportEntry in reportData.Entries)
			{
				if ((reportEntry.Flags & (ReportEntryFlags.TargetThrottleDurations | ReportEntryFlags.SourceThrottleDurations)) != ReportEntryFlags.None)
				{
					if ((reportEntry.Flags & ReportEntryFlags.SourceThrottleDurations) != ReportEntryFlags.None)
					{
						this.SourceThrottles = new Throttles(reportEntry.SourceThrottleDurations);
					}
					if ((reportEntry.Flags & ReportEntryFlags.TargetThrottleDurations) != ReportEntryFlags.None)
					{
						this.TargetThrottles = new Throttles(reportEntry.TargetThrottleDurations);
					}
				}
				else
				{
					if (reportEntry.Type == ReportEntryType.Debug)
					{
						this.AddToLazyList<ReportEntry>(ref this.debugEntries, reportEntry);
					}
					else
					{
						this.AddToLazyList<ReportEntry>(ref this.entries, reportEntry);
					}
					if (reportEntry.Type == ReportEntryType.Warning || reportEntry.Type == ReportEntryType.WarningCondition)
					{
						this.AddToLazyList<ReportEntry>(ref this.warnings, reportEntry);
					}
					if (reportEntry.Failure != null)
					{
						this.AddToLazyList<FailureRec>(ref this.failures, reportEntry.Failure);
					}
					if (reportEntry.BadItem != null)
					{
						BadItemKind kind = reportEntry.BadItem.Kind;
						if (kind == BadItemKind.LargeItem)
						{
							this.AddToLazyList<BadMessageRec>(ref this.largeItems, reportEntry.BadItem);
						}
						else
						{
							this.AddToLazyList<BadMessageRec>(ref this.badItems, reportEntry.BadItem);
						}
					}
					if ((reportEntry.Flags & ReportEntryFlags.MailboxVerificationResults) != ReportEntryFlags.None && reportEntry.MailboxVerificationResults != null)
					{
						this.MailboxVerification = new Report.ListWithToString<FolderSizeRec>();
						this.MailboxVerification.AddRange(reportEntry.MailboxVerificationResults);
					}
					if ((reportEntry.Flags & ReportEntryFlags.SessionStatistics) != ReportEntryFlags.None)
					{
						if (reportEntry.SessionStatistics != null)
						{
							if (reportEntry.SessionStatistics.SessionId != this.SessionStatistics.SessionId)
							{
								this.SessionStatistics += stats;
							}
							stats = reportEntry.SessionStatistics;
						}
						if (reportEntry.ArchiveSessionStatistics != null)
						{
							if (reportEntry.ArchiveSessionStatistics.SessionId != this.ArchiveSessionStatistics.SessionId)
							{
								this.ArchiveSessionStatistics += stats2;
							}
							stats2 = reportEntry.ArchiveSessionStatistics;
						}
					}
					if ((reportEntry.Flags & ReportEntryFlags.ConfigObject) != ReportEntryFlags.None && reportEntry.ConfigObject != null)
					{
						if ((reportEntry.Flags & ReportEntryFlags.Before) != ReportEntryFlags.None)
						{
							if ((reportEntry.Flags & ReportEntryFlags.Source) != ReportEntryFlags.None)
							{
								this.SourceMailboxBeforeMove = reportEntry.ConfigObject;
							}
							if ((reportEntry.Flags & ReportEntryFlags.Target) != ReportEntryFlags.None)
							{
								this.TargetMailUserBeforeMove = reportEntry.ConfigObject;
							}
						}
						if ((reportEntry.Flags & ReportEntryFlags.After) != ReportEntryFlags.None)
						{
							if ((reportEntry.Flags & ReportEntryFlags.Source) != ReportEntryFlags.None)
							{
								this.SourceMailUserAfterMove = reportEntry.ConfigObject;
							}
							if ((reportEntry.Flags & ReportEntryFlags.Target) != ReportEntryFlags.None)
							{
								this.TargetMailboxAfterMove = reportEntry.ConfigObject;
							}
						}
					}
					if ((reportEntry.Flags & ReportEntryFlags.MailboxSize) != ReportEntryFlags.None && reportEntry.MailboxSize != null)
					{
						if ((reportEntry.Flags & ReportEntryFlags.Source) != ReportEntryFlags.None)
						{
							if ((reportEntry.Flags & ReportEntryFlags.Primary) != ReportEntryFlags.None)
							{
								this.SourceMailboxSize = reportEntry.MailboxSize;
							}
							if ((reportEntry.Flags & ReportEntryFlags.Archive) != ReportEntryFlags.None)
							{
								this.SourceArchiveMailboxSize = reportEntry.MailboxSize;
							}
						}
						if ((reportEntry.Flags & ReportEntryFlags.Target) != ReportEntryFlags.None)
						{
							if ((reportEntry.Flags & ReportEntryFlags.Primary) != ReportEntryFlags.None)
							{
								this.TargetMailboxSize = reportEntry.MailboxSize;
							}
							if ((reportEntry.Flags & ReportEntryFlags.Archive) != ReportEntryFlags.None)
							{
								this.TargetArchiveMailboxSize = reportEntry.MailboxSize;
							}
						}
					}
					if (reportEntry.Connectivity != null)
					{
						this.AddToLazyList<Report.ConnectivityRecWithTimestamp>(ref this.connectivity, new Report.ConnectivityRecWithTimestamp(reportEntry.CreationTime, reportEntry.Connectivity));
					}
				}
			}
			this.SessionStatistics += stats;
			this.ArchiveSessionStatistics += stats2;
			Comparison<DurationInfo> comparison = (DurationInfo x, DurationInfo y) => y.Duration.CompareTo(x.Duration);
			this.SessionStatistics.SourceProviderInfo.Durations.Sort(comparison);
			this.SessionStatistics.DestinationProviderInfo.Durations.Sort(comparison);
			this.ArchiveSessionStatistics.SourceProviderInfo.Durations.Sort(comparison);
			this.ArchiveSessionStatistics.DestinationProviderInfo.Durations.Sort(comparison);
		}

		public Report.ListWithToString<ReportEntry> Entries
		{
			get
			{
				return this.entries;
			}
		}

		public Report.ListWithToString<ReportEntry> Warnings
		{
			get
			{
				return this.warnings;
			}
		}

		public Report.ListWithToString<ReportEntry> DebugEntries
		{
			get
			{
				return this.debugEntries;
			}
		}

		public Report.ListWithToString<FailureRec> Failures
		{
			get
			{
				return this.failures;
			}
		}

		public Report.ListWithToString<BadMessageRec> BadItems
		{
			get
			{
				return this.badItems;
			}
		}

		public Report.ListWithToString<BadMessageRec> LargeItems
		{
			get
			{
				return this.largeItems;
			}
		}

		public Report.ListWithToString<Report.ConnectivityRecWithTimestamp> Connectivity
		{
			get
			{
				return this.connectivity;
			}
		}

		public ConfigurableObjectXML SourceMailboxBeforeMove { get; private set; }

		public ConfigurableObjectXML TargetMailboxAfterMove { get; private set; }

		public ConfigurableObjectXML TargetMailUserBeforeMove { get; private set; }

		public ConfigurableObjectXML SourceMailUserAfterMove { get; private set; }

		public MailboxSizeRec SourceMailboxSize { get; private set; }

		public MailboxSizeRec TargetMailboxSize { get; private set; }

		public MailboxSizeRec SourceArchiveMailboxSize { get; private set; }

		public MailboxSizeRec TargetArchiveMailboxSize { get; private set; }

		public Report.ListWithToString<FolderSizeRec> MailboxVerification { get; private set; }

		public SessionStatistics SessionStatistics { get; private set; }

		public SessionStatistics ArchiveSessionStatistics { get; private set; }

		public Throttles SourceThrottles { get; private set; }

		public Throttles TargetThrottles { get; private set; }

		public override string ToString()
		{
			if (this.Entries != null)
			{
				return this.Entries.ToString();
			}
			return string.Empty;
		}

		private void AddToLazyList<T>(ref Report.ListWithToString<T> list, T entry)
		{
			if (list == null)
			{
				list = new Report.ListWithToString<T>();
			}
			list.Add(entry);
		}

		private Report.ListWithToString<ReportEntry> entries;

		private Report.ListWithToString<ReportEntry> debugEntries;

		private Report.ListWithToString<ReportEntry> warnings;

		private Report.ListWithToString<FailureRec> failures;

		private Report.ListWithToString<BadMessageRec> badItems;

		private Report.ListWithToString<BadMessageRec> largeItems;

		private Report.ListWithToString<Report.ConnectivityRecWithTimestamp> connectivity;

		[Serializable]
		public class ListWithToString<T> : List<T>
		{
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (T t in this)
				{
					string value = t.ToString();
					if (!string.IsNullOrEmpty(value))
					{
						stringBuilder.AppendLine(value);
					}
				}
				return stringBuilder.ToString();
			}
		}

		[Serializable]
		public class ConnectivityRecWithTimestamp : ConnectivityRec
		{
			public ConnectivityRecWithTimestamp(DateTime timestamp, ConnectivityRec entry)
			{
				this.Timestamp = timestamp;
				base.ServerKind = entry.ServerKind;
				base.ServerName = entry.ServerName;
				base.ServerVersion = entry.ServerVersion;
				base.ProxyName = entry.ProxyName;
				base.ProxyVersion = entry.ProxyVersion;
				base.ProviderName = entry.ProviderName;
			}

			public DateTime Timestamp { get; private set; }

			public override string ToString()
			{
				return string.Format("{0} {1}", this.Timestamp.ToLocalTime().ToString(), base.ToString());
			}
		}
	}
}

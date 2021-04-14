using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal sealed class ReportData
	{
		internal ReportData(Guid guid, ReportVersion version) : this(guid, version, "MailboxReplicationService Move Reports", "IPM.MS-Exchange.MailboxMoveReports")
		{
		}

		internal ReportData(Guid guid, ReportVersion version, string reportFolder, string reportMessage)
		{
			this.MessageId = null;
			this.IdentifyingGuid = guid;
			this.Version = version;
			this.existingEntries = new List<ReportEntry>();
			this.newEntries = new List<ReportEntry>();
			switch (this.Version)
			{
			case ReportVersion.ReportE14R4:
				this.reportHelper = new DownlevelReportHelper(reportFolder, reportMessage);
				return;
			case ReportVersion.ReportE14R6Compression:
				this.reportHelper = new CompressedReportHelper(reportFolder, reportMessage);
				return;
			default:
				return;
			}
		}

		public List<ReportEntry> Entries
		{
			get
			{
				List<ReportEntry> result;
				lock (this.locker)
				{
					result = ReportHelper<CompressedReport>.MergeEntries(this.existingEntries, this.newEntries);
				}
				return result;
			}
			set
			{
				lock (this.locker)
				{
					this.existingEntries = (value ?? new List<ReportEntry>());
					this.newEntries = new List<ReportEntry>();
				}
			}
		}

		public bool HasNewEntries
		{
			get
			{
				bool result;
				lock (this.locker)
				{
					result = (this.newEntries != null && this.newEntries.Count > 0);
				}
				return result;
			}
		}

		public List<ReportEntry> NewEntries
		{
			get
			{
				List<ReportEntry> result = null;
				lock (this.locker)
				{
					result = new List<ReportEntry>(this.newEntries);
				}
				return result;
			}
		}

		public byte[] MessageId { get; set; }

		public Guid IdentifyingGuid { get; private set; }

		public ReportVersion Version { get; private set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ReportEntry reportEntry in this.Entries)
			{
				stringBuilder.AppendLine(reportEntry.ToString());
			}
			return stringBuilder.ToString();
		}

		public void Clear()
		{
			lock (this.locker)
			{
				this.existingEntries.Clear();
				this.newEntries.Clear();
			}
		}

		public void Append(LocalizedString msg)
		{
			ReportEntry entry = new ReportEntry(msg);
			this.Append(entry);
		}

		public void Append(LocalizedString msg, Exception failure, ReportEntryFlags flags)
		{
			this.Append(new ReportEntry(msg, ReportEntryType.Error)
			{
				Failure = FailureRec.Create(failure),
				Flags = (flags | ReportEntryFlags.Failure)
			});
		}

		public void Append(LocalizedString msg, FailureRec failureRec, ReportEntryFlags flags)
		{
			this.Append(new ReportEntry(msg, ReportEntryType.Error)
			{
				Failure = failureRec,
				Flags = (flags | ReportEntryFlags.Failure)
			});
		}

		public void Append(LocalizedString msg, BadMessageRec badItem)
		{
			this.Append(new ReportEntry(msg)
			{
				BadItem = badItem,
				Failure = badItem.Failure,
				Flags = (ReportEntryFlags.BadItem | ReportEntryFlags.Failure)
			});
		}

		public void Append(LocalizedString msg, ConfigurableObjectXML configObject, ReportEntryFlags flags)
		{
			this.Append(new ReportEntry(msg)
			{
				ConfigObject = configObject,
				Flags = (flags | ReportEntryFlags.ConfigObject)
			});
		}

		public void Append(LocalizedString msg, MailboxSizeRec mailboxSizeRec, ReportEntryFlags flags)
		{
			this.Append(new ReportEntry(msg)
			{
				MailboxSize = mailboxSizeRec,
				Flags = (flags | ReportEntryFlags.MailboxSize)
			});
		}

		public void Append(LocalizedString msg, List<FolderSizeRec> mailboxVerificationResults)
		{
			ReportEntry reportEntry = new ReportEntry(msg);
			reportEntry.MailboxVerificationResults = mailboxVerificationResults;
			reportEntry.Flags |= ReportEntryFlags.MailboxVerificationResults;
			this.Append(reportEntry);
		}

		public void Append(LocalizedString msg, SessionStatistics sessionStatistics, SessionStatistics archiveSessionStatistics)
		{
			if (sessionStatistics == null && archiveSessionStatistics == null)
			{
				this.Append(new ReportEntry(msg));
				return;
			}
			ReportEntry reportEntry = new ReportEntry(msg);
			reportEntry.Flags |= ReportEntryFlags.SessionStatistics;
			if (sessionStatistics != null)
			{
				reportEntry.SessionStatistics = sessionStatistics;
				reportEntry.Flags |= ReportEntryFlags.Primary;
			}
			if (archiveSessionStatistics != null)
			{
				reportEntry.ArchiveSessionStatistics = archiveSessionStatistics;
				reportEntry.Flags |= ReportEntryFlags.Archive;
			}
			this.Append(reportEntry);
		}

		public void Append(RequestJobTimeTracker timeTracker, ReportEntryFlags flags)
		{
			ThrottleDurations sourceThrottleDurations;
			ThrottleDurations targetThrottleDurations;
			timeTracker.GetThrottledDurations(out sourceThrottleDurations, out targetThrottleDurations);
			ReportEntry reportEntry = new ReportEntry();
			reportEntry.Flags = flags;
			if ((flags & ReportEntryFlags.SourceThrottleDurations) != ReportEntryFlags.None)
			{
				reportEntry.SourceThrottleDurations = sourceThrottleDurations;
			}
			if ((flags & ReportEntryFlags.TargetThrottleDurations) != ReportEntryFlags.None)
			{
				reportEntry.TargetThrottleDurations = targetThrottleDurations;
			}
			this.Append(reportEntry);
		}

		public void Append(LocalizedString msg, ConnectivityRec connectivityRec)
		{
			this.Append(new ReportEntry(msg)
			{
				Connectivity = connectivityRec
			});
		}

		public void AppendDebug(string debugData)
		{
			this.Append(new ReportEntry(LocalizedString.Empty, ReportEntryType.Debug)
			{
				DebugData = debugData
			});
		}

		public void Append(ReportEntry entry)
		{
			if (entry != null)
			{
				lock (this.locker)
				{
					this.newEntries.Add(entry);
				}
				MrsTracer.Common.Debug("{0}", new object[]
				{
					entry
				});
			}
		}

		public void Append(ICollection<ReportEntry> reportEntries)
		{
			if (reportEntries != null)
			{
				foreach (ReportEntry entry in reportEntries)
				{
					this.Append(entry);
				}
			}
		}

		public void Load(MapiStore storeToUse)
		{
			this.reportHelper.Load(this, storeToUse);
		}

		public void Flush(MapiStore storeToUse)
		{
			this.reportHelper.Flush(this, storeToUse);
		}

		public void Delete(MapiStore storeToUse)
		{
			this.reportHelper.Delete(this, storeToUse);
		}

		public Report ToReport()
		{
			return new Report(this);
		}

		private const string ReportFolderName = "MailboxReplicationService Move Reports";

		private const string ReportMessageClass = "IPM.MS-Exchange.MailboxMoveReports";

		private List<ReportEntry> existingEntries;

		private List<ReportEntry> newEntries;

		private IReportHelper reportHelper;

		private object locker = new object();
	}
}

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[XmlType(TypeName = "MoveReportEntry")]
	[Serializable]
	public sealed class ReportEntry : XMLSerializableBase, ILocalizedString
	{
		public ReportEntry()
		{
		}

		public ReportEntry(LocalizedString message) : this(message, ReportEntryType.Informational)
		{
		}

		public ReportEntry(LocalizedString message, ReportEntryType type) : this(message, type, null, ReportEntryFlags.None)
		{
		}

		public ReportEntry(LocalizedString message, ReportEntryType type, Exception failure, ReportEntryFlags flags)
		{
			this.Message = message;
			this.CreationTime = (DateTime)ExDateTime.UtcNow;
			this.ServerName = CommonUtils.LocalShortComputerName;
			this.Type = type;
			this.Flags = flags;
			if (failure != null)
			{
				this.Failure = FailureRec.Create(failure);
				this.Flags |= ReportEntryFlags.Failure;
			}
		}

		private ReportEntry(DateTime creationTime)
		{
			this.CreationTime = creationTime;
		}

		[XmlElement(ElementName = "CreationTime")]
		public DateTime CreationTime { get; set; }

		[XmlElement(ElementName = "ServerName")]
		public string ServerName { get; set; }

		[XmlIgnore]
		public ReportEntryType Type { get; internal set; }

		[XmlElement(ElementName = "Type")]
		public int TypeInt
		{
			get
			{
				return (int)this.Type;
			}
			set
			{
				this.Type = (ReportEntryType)value;
			}
		}

		[XmlIgnore]
		public ReportEntryFlags Flags { get; set; }

		[XmlElement(ElementName = "Flags")]
		public int FlagsInt
		{
			get
			{
				return (int)this.Flags;
			}
			set
			{
				this.Flags = (ReportEntryFlags)value;
			}
		}

		[XmlIgnore]
		public LocalizedString Message { get; private set; }

		[XmlElement(ElementName = "Message")]
		public byte[] MessageData
		{
			get
			{
				return CommonUtils.ByteSerialize(this.Message);
			}
			set
			{
				this.Message = CommonUtils.ByteDeserialize(value);
			}
		}

		[XmlElement(ElementName = "Failure")]
		public FailureRec Failure { get; set; }

		[XmlElement(ElementName = "BadItem")]
		public BadMessageRec BadItem { get; set; }

		[XmlElement(ElementName = "ConfigObject")]
		public ConfigurableObjectXML ConfigObject { get; set; }

		[XmlElement(ElementName = "MailboxSize")]
		public MailboxSizeRec MailboxSize { get; set; }

		[XmlElement(ElementName = "SessionStatistics")]
		public SessionStatistics SessionStatistics { get; set; }

		[XmlElement(ElementName = "ArchiveSessionStatistics")]
		public SessionStatistics ArchiveSessionStatistics { get; set; }

		[XmlArrayItem("FolderSize")]
		[XmlArray("MailboxVerificationResults")]
		public List<FolderSizeRec> MailboxVerificationResults { get; set; }

		[XmlElement(ElementName = "DebugData")]
		public string DebugData { get; set; }

		[XmlElement(ElementName = "Connectivity")]
		public ConnectivityRec Connectivity { get; set; }

		[XmlElement(ElementName = "SourceThrottles")]
		public ThrottleDurations SourceThrottleDurations { get; set; }

		[XmlElement(ElementName = "TargetThrottles")]
		public ThrottleDurations TargetThrottleDurations { get; set; }

		LocalizedString ILocalizedString.LocalizedString
		{
			get
			{
				LocalizedString message = (this.Type == ReportEntryType.Debug) ? new LocalizedString(this.DebugData) : this.Message;
				return MrsStrings.MoveReportEntryMessage(this.CreationTime.ToLocalTime().ToString(), this.ServerName, message);
			}
		}

		public override string ToString()
		{
			return ((ILocalizedString)this).LocalizedString.ToString();
		}

		internal static ReportEntry MaxEntry = new ReportEntry(DateTime.MaxValue);
	}
}

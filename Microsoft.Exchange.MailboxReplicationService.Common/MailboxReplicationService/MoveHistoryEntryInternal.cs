using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class MoveHistoryEntryInternal : XMLSerializableBase, IComparable<MoveHistoryEntryInternal>
	{
		public MoveHistoryEntryInternal()
		{
			this.hasReadCompressedEntries = false;
		}

		internal MoveHistoryEntryInternal(RequestJobBase requestJob, ReportData report)
		{
			this.hasReadCompressedEntries = false;
			this.Status = (int)requestJob.Status;
			this.Flags = (int)requestJob.Flags;
			this.SourceDatabase = ADObjectIdXML.Serialize(requestJob.SourceDatabase);
			this.SourceVersion = requestJob.SourceVersion;
			this.SourceServer = requestJob.SourceServer;
			this.SourceArchiveDatabase = ADObjectIdXML.Serialize(requestJob.SourceArchiveDatabase);
			this.SourceArchiveVersion = requestJob.SourceArchiveVersion;
			this.SourceArchiveServer = requestJob.SourceArchiveServer;
			this.DestinationDatabase = ADObjectIdXML.Serialize(requestJob.TargetDatabase);
			this.DestinationVersion = requestJob.TargetVersion;
			this.DestinationServer = requestJob.TargetServer;
			this.DestinationArchiveDatabase = ADObjectIdXML.Serialize(requestJob.TargetArchiveDatabase);
			this.DestinationArchiveVersion = requestJob.TargetArchiveVersion;
			this.DestinationArchiveServer = requestJob.TargetArchiveServer;
			this.RemoteHostName = requestJob.RemoteHostName;
			if (requestJob.RemoteCredential == null)
			{
				this.RemoteCredentialUserName = null;
			}
			else if (requestJob.RemoteCredential.Domain == null)
			{
				this.RemoteCredentialUserName = requestJob.RemoteCredential.UserName;
			}
			else
			{
				this.RemoteCredentialUserName = requestJob.RemoteCredential.Domain + "\\" + requestJob.RemoteCredential.UserName;
			}
			this.RemoteDatabaseName = requestJob.RemoteDatabaseName;
			this.RemoteArchiveDatabaseName = requestJob.RemoteArchiveDatabaseName;
			this.BadItemLimit = requestJob.BadItemLimit;
			this.BadItemsEncountered = requestJob.BadItemsEncountered;
			this.LargeItemLimit = requestJob.LargeItemLimit;
			this.LargeItemsEncountered = requestJob.LargeItemsEncountered;
			this.MissingItemsEncountered = requestJob.MissingItemsEncountered;
			this.TimeTracker = requestJob.TimeTracker;
			this.MRSServerName = requestJob.MRSServerName;
			this.TotalMailboxSize = requestJob.TotalMailboxSize;
			this.TotalMailboxItemCount = requestJob.TotalMailboxItemCount;
			this.TotalArchiveSize = requestJob.TotalArchiveSize;
			this.TotalArchiveItemCount = requestJob.TotalArchiveItemCount;
			this.TargetDeliveryDomain = requestJob.TargetDeliveryDomain;
			this.ArchiveDomain = requestJob.ArchiveDomain;
			this.FailureCode = requestJob.FailureCode;
			this.FailureType = requestJob.FailureType;
			this.MessageData = CommonUtils.ByteSerialize(requestJob.Message);
			this.report = report;
		}

		[XmlElement(ElementName = "Status")]
		public int Status { get; set; }

		[XmlElement(ElementName = "Flags")]
		public int Flags { get; set; }

		[XmlElement(ElementName = "SourceDatabase")]
		public ADObjectIdXML SourceDatabase { get; set; }

		[XmlElement(ElementName = "SourceVersion")]
		public int SourceVersion { get; set; }

		[XmlElement(ElementName = "SourceServer")]
		public string SourceServer { get; set; }

		[XmlElement(ElementName = "SourceArchiveDatabase")]
		public ADObjectIdXML SourceArchiveDatabase { get; set; }

		[XmlElement(ElementName = "SourceArchiveVersion")]
		public int SourceArchiveVersion { get; set; }

		[XmlElement(ElementName = "SourceArchiveServer")]
		public string SourceArchiveServer { get; set; }

		[XmlElement(ElementName = "DestinationDatabase")]
		public ADObjectIdXML DestinationDatabase { get; set; }

		[XmlElement(ElementName = "DestinationVersion")]
		public int DestinationVersion { get; set; }

		[XmlElement(ElementName = "DestinationServer")]
		public string DestinationServer { get; set; }

		[XmlElement(ElementName = "DestinationArchiveDatabase")]
		public ADObjectIdXML DestinationArchiveDatabase { get; set; }

		[XmlElement(ElementName = "DestinationArchiveVersion")]
		public int DestinationArchiveVersion { get; set; }

		[XmlElement(ElementName = "DestinationArchiveServer")]
		public string DestinationArchiveServer { get; set; }

		[XmlElement(ElementName = "RemoteHostName")]
		public string RemoteHostName { get; set; }

		[XmlElement(ElementName = "RemoteCredentialUserName")]
		public string RemoteCredentialUserName { get; set; }

		[XmlElement(ElementName = "RemoteDatabaseName")]
		public string RemoteDatabaseName { get; set; }

		[XmlElement(ElementName = "RemoteArchiveDatabaseName")]
		public string RemoteArchiveDatabaseName { get; set; }

		[XmlElement(ElementName = "BadItemLimit")]
		public int BadItemLimitInt
		{
			get
			{
				if (!this.BadItemLimit.IsUnlimited)
				{
					return this.BadItemLimit.Value;
				}
				return -1;
			}
			set
			{
				this.BadItemLimit = ((value < 0) ? Unlimited<int>.UnlimitedValue : new Unlimited<int>(value));
			}
		}

		[XmlIgnore]
		public Unlimited<int> BadItemLimit { get; set; }

		[XmlElement(ElementName = "BadItemsEncountered")]
		public int BadItemsEncountered { get; set; }

		[XmlElement(ElementName = "LargeItemLimit")]
		public int LargeItemLimitInt
		{
			get
			{
				if (!this.LargeItemLimit.IsUnlimited)
				{
					return this.LargeItemLimit.Value;
				}
				return -1;
			}
			set
			{
				this.LargeItemLimit = ((value < 0) ? Unlimited<int>.UnlimitedValue : new Unlimited<int>(value));
			}
		}

		[XmlIgnore]
		public Unlimited<int> LargeItemLimit { get; set; }

		[XmlElement(ElementName = "LargeItemsEncountered")]
		public int LargeItemsEncountered { get; set; }

		[XmlElement(ElementName = "MissingItemsEncountered")]
		public int MissingItemsEncountered { get; set; }

		[XmlElement(ElementName = "TimeTracker")]
		public RequestJobTimeTracker TimeTracker { get; set; }

		[XmlElement(ElementName = "MoveServerName")]
		public string MRSServerName { get; set; }

		[XmlElement(ElementName = "TotalMailboxSize")]
		public ulong TotalMailboxSize { get; set; }

		[XmlElement(ElementName = "TotalMailboxItemCount")]
		public ulong TotalMailboxItemCount { get; set; }

		[XmlElement(ElementName = "TotalArchiveSize")]
		public ulong? TotalArchiveSize { get; set; }

		[XmlElement(ElementName = "TotalArchiveItemCount")]
		public ulong? TotalArchiveItemCount { get; set; }

		[XmlElement(ElementName = "TargetDeliveryDomain")]
		public string TargetDeliveryDomain { get; set; }

		[XmlElement(ElementName = "ArchiveDomain")]
		public string ArchiveDomain { get; set; }

		[XmlElement(ElementName = "FailureCode")]
		public int? FailureCode { get; set; }

		[XmlElement(ElementName = "FailureType")]
		public string FailureType { get; set; }

		[XmlElement(ElementName = "FailureMessageData")]
		public byte[] MessageData { get; set; }

		[XmlElement(ElementName = "MoveReport")]
		public ReportEntry[] ReportData
		{
			get
			{
				return new ReportEntry[]
				{
					new ReportEntry(new LocalizedString("Your version of Exchange is unable to display Compressed Reports."))
				};
			}
			set
			{
				if (!this.hasReadCompressedEntries)
				{
					this.report = new ReportData(MoveHistoryEntryInternal.FakeReportGuid, ReportVersion.ReportE14R4);
					if (value != null)
					{
						this.report.Entries = new List<ReportEntry>(value);
					}
				}
			}
		}

		[XmlElement(ElementName = "CompressedReport")]
		public CompressedReport CompressedReportData
		{
			get
			{
				return new CompressedReport((this.report != null) ? this.report.Entries : null);
			}
			set
			{
				this.report = new ReportData(MoveHistoryEntryInternal.FakeReportGuid, ReportVersion.ReportE14R6Compression);
				if (value != null)
				{
					this.report.Entries = value.Entries;
					this.hasReadCompressedEntries = true;
				}
			}
		}

		internal Report Report
		{
			get
			{
				return this.report.ToReport();
			}
		}

		private DateTime ComparisonTime
		{
			get
			{
				if (this.TimeTracker == null)
				{
					return DateTime.MinValue;
				}
				DateTime? timestamp = this.TimeTracker.GetTimestamp(RequestJobTimestamp.Completion);
				DateTime? timestamp2 = this.TimeTracker.GetTimestamp(RequestJobTimestamp.Failure);
				DateTime? timestamp3 = this.TimeTracker.GetTimestamp(RequestJobTimestamp.Creation);
				if (timestamp != null)
				{
					return timestamp.Value;
				}
				if (timestamp2 != null)
				{
					return timestamp2.Value;
				}
				DateTime? dateTime = timestamp3;
				if (dateTime == null)
				{
					return DateTime.MinValue;
				}
				return dateTime.GetValueOrDefault();
			}
		}

		int IComparable<MoveHistoryEntryInternal>.CompareTo(MoveHistoryEntryInternal other)
		{
			return -this.ComparisonTime.CompareTo(other.ComparisonTime);
		}

		internal static List<MoveHistoryEntryInternal> LoadMoveHistory(MapiStore mailbox)
		{
			MrsTracer.Common.Function("MoveHistoryEntryInternal.LoadMoveHistory", new object[0]);
			List<MoveHistoryEntryInternal> list = new List<MoveHistoryEntryInternal>();
			using (MapiFolder mapiFolder = MapiUtils.OpenFolderUnderRoot(mailbox, MoveHistoryEntryInternal.MHEFolderName, false))
			{
				if (mapiFolder == null)
				{
					return list;
				}
				using (MapiTable contentsTable = mapiFolder.GetContentsTable(ContentsTableFlags.DeferredErrors))
				{
					PropValue[][] array = MapiUtils.QueryAllRows(contentsTable, null, new PropTag[]
					{
						PropTag.EntryId
					});
					foreach (PropValue[] array3 in array)
					{
						byte[] bytes = array3[0].GetBytes();
						string subject = string.Format("MoveHistoryEntry {0}", TraceUtils.DumpEntryId(bytes));
						MoveObjectInfo<MoveHistoryEntryInternal> moveObjectInfo = new MoveObjectInfo<MoveHistoryEntryInternal>(Guid.Empty, mailbox, bytes, MoveHistoryEntryInternal.MHEFolderName, MoveHistoryEntryInternal.MHEMessageClass, subject, null);
						using (moveObjectInfo)
						{
							MoveHistoryEntryInternal moveHistoryEntryInternal = null;
							try
							{
								moveHistoryEntryInternal = moveObjectInfo.ReadObject(ReadObjectFlags.DontThrowOnCorruptData);
							}
							catch (MailboxReplicationPermanentException ex)
							{
								MrsTracer.Common.Warning("Failed to read move history entry: {0}", new object[]
								{
									ex.ToString()
								});
							}
							if (moveHistoryEntryInternal != null)
							{
								list.Add(moveHistoryEntryInternal);
							}
							else if (moveObjectInfo.CreationTimestamp < DateTime.UtcNow - TimeSpan.FromDays(365.0))
							{
								MrsTracer.Common.Warning("Removing old corrupt MHEI entry {0}", new object[]
								{
									TraceUtils.DumpEntryId(bytes)
								});
								moveObjectInfo.DeleteMessage();
							}
						}
					}
				}
			}
			list.Sort();
			return list;
		}

		internal static List<MoveHistoryEntryInternal> LoadMoveHistory(Guid mailboxGuid, Guid mdbGuid, UserMailboxFlags flags)
		{
			List<MoveHistoryEntryInternal> result;
			using (MapiStore userMailbox = MapiUtils.GetUserMailbox(mailboxGuid, mdbGuid, flags))
			{
				if (userMailbox == null)
				{
					result = null;
				}
				else
				{
					result = MoveHistoryEntryInternal.LoadMoveHistory(userMailbox);
				}
			}
			return result;
		}

		internal void SaveToMailbox(MapiStore mailbox, int maxMoveHistoryLength)
		{
			MrsTracer.Common.Function("MoveHistoryEntryInternal.SaveToMailbox(maxHistoryLength={0})", new object[]
			{
				maxMoveHistoryLength
			});
			List<byte[]> list = new List<byte[]>();
			using (MapiFolder folder = MapiUtils.OpenFolderUnderRoot(mailbox, MoveHistoryEntryInternal.MHEFolderName, true))
			{
				using (MapiTable contentsTable = folder.GetContentsTable(ContentsTableFlags.DeferredErrors))
				{
					contentsTable.SortTable(new SortOrder(PropTag.LastModificationTime, SortFlags.Ascend), SortTableFlags.None);
					PropValue[][] array = MapiUtils.QueryAllRows(contentsTable, null, new PropTag[]
					{
						PropTag.EntryId
					});
					foreach (PropValue[] array3 in array)
					{
						list.Add(array3[0].GetBytes());
					}
				}
				MrsTracer.Common.Debug("Move history contains {0} items.", new object[]
				{
					list.Count
				});
				List<byte[]> list2 = new List<byte[]>();
				while (list.Count >= maxMoveHistoryLength && list.Count > 0)
				{
					list2.Add(list[0]);
					list.RemoveAt(0);
				}
				if (list2.Count > 0)
				{
					MrsTracer.Common.Debug("Clearing {0} entries from move history", new object[]
					{
						list2.Count
					});
					MapiUtils.ProcessMapiCallInBatches<byte[]>(list2.ToArray(), delegate(byte[][] batch)
					{
						folder.DeleteMessages(DeleteMessagesFlags.ForceHardDelete, batch);
					});
				}
			}
			if (maxMoveHistoryLength <= 0)
			{
				MrsTracer.Common.Debug("Move history saving is disabled.", new object[0]);
				return;
			}
			DateTime dateTime = this.TimeTracker.GetTimestamp(RequestJobTimestamp.Creation) ?? DateTime.MinValue;
			string subject = string.Format("MoveHistoryEntry {0}", dateTime.ToString());
			byte[] bytes = BitConverter.GetBytes(dateTime.ToBinary());
			MoveObjectInfo<MoveHistoryEntryInternal> moveObjectInfo = new MoveObjectInfo<MoveHistoryEntryInternal>(Guid.Empty, mailbox, null, MoveHistoryEntryInternal.MHEFolderName, MoveHistoryEntryInternal.MHEMessageClass, subject, bytes);
			using (moveObjectInfo)
			{
				moveObjectInfo.SaveObject(this);
			}
		}

		public static readonly string MHEFolderName = "MailboxMoveHistory";

		private static readonly string MHEMessageClass = "IPM.MS-Exchange.MailboxMoveHistory";

		private static readonly Guid FakeReportGuid = new Guid("d1df9b83-96bf-428b-8557-85c71cb1640f");

		private bool hasReadCompressedEntries;

		private ReportData report;
	}
}

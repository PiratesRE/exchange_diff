using System;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class BadMessageRec : XMLSerializableBase
	{
		[XmlIgnore]
		public BadItemKind Kind { get; set; }

		[XmlElement(ElementName = "Kind")]
		public int KindInt
		{
			get
			{
				return (int)this.Kind;
			}
			set
			{
				this.Kind = (BadItemKind)value;
			}
		}

		[XmlElement(ElementName = "EntryId")]
		public byte[] EntryId { get; set; }

		[XmlIgnore]
		public string EntryIdHex
		{
			get
			{
				return TraceUtils.DumpBytes(this.EntryId);
			}
		}

		[XmlElement(ElementName = "CloudId")]
		public string CloudId { get; set; }

		[XmlElement(ElementName = "Data")]
		public string XmlData { get; set; }

		[XmlElement(ElementName = "FolderId")]
		public byte[] FolderId { get; set; }

		[XmlIgnore]
		public string FolderIdHex
		{
			get
			{
				return TraceUtils.DumpBytes(this.FolderId);
			}
		}

		[XmlElement(ElementName = "FolderName")]
		public string FolderName
		{
			get
			{
				if (SuppressingPiiContext.NeedPiiSuppression)
				{
					return SuppressingPiiData.Redact(this.folderName);
				}
				return this.folderName;
			}
			set
			{
				this.folderName = value;
			}
		}

		[XmlIgnore]
		public WellKnownFolderType WellKnownFolderType { get; set; }

		[XmlElement(ElementName = "WKFType")]
		public int WellKnownFolderTypeInt
		{
			get
			{
				return (int)this.WellKnownFolderType;
			}
			set
			{
				this.WellKnownFolderType = (WellKnownFolderType)value;
			}
		}

		[XmlElement(ElementName = "Sender")]
		public string Sender
		{
			get
			{
				if (SuppressingPiiContext.NeedPiiSuppression)
				{
					return SuppressingPiiData.Redact(this.sender);
				}
				return this.sender;
			}
			set
			{
				this.sender = value;
			}
		}

		[XmlElement(ElementName = "Recipient")]
		public string Recipient
		{
			get
			{
				if (SuppressingPiiContext.NeedPiiSuppression)
				{
					return SuppressingPiiData.Redact(this.recipient);
				}
				return this.recipient;
			}
			set
			{
				this.recipient = value;
			}
		}

		[XmlElement(ElementName = "Subject")]
		public string Subject
		{
			get
			{
				if (SuppressingPiiContext.NeedPiiSuppression)
				{
					return SuppressingPiiData.Redact(this.subject);
				}
				return this.subject;
			}
			set
			{
				this.subject = value;
			}
		}

		[XmlElement(ElementName = "MessageClass")]
		public string MessageClass { get; set; }

		[XmlElement(ElementName = "MessageSize")]
		public int? MessageSize { get; set; }

		[XmlElement(ElementName = "DateSent")]
		public DateTime? DateSent { get; set; }

		[XmlElement(ElementName = "DateReceived")]
		public DateTime? DateReceived { get; set; }

		[XmlElement(ElementName = "Failure")]
		public FailureRec Failure { get; set; }

		[XmlElement(ElementName = "Category")]
		public string Category { get; set; }

		[XmlIgnore]
		internal Exception RawFailure { get; set; }

		public override string ToString()
		{
			return this.ToLocalizedString();
		}

		internal static BadMessageRec MissingItem(MessageRec msg)
		{
			return new BadMessageRec
			{
				Kind = BadItemKind.MissingItem,
				EntryId = msg.EntryId,
				FolderId = msg.FolderId
			};
		}

		internal static BadMessageRec MissingItem(Exception ex)
		{
			return new BadMessageRec
			{
				Kind = BadItemKind.MissingItem,
				Failure = FailureRec.Create(ex),
				EntryId = BadMessageRec.ComputeKey(BitConverter.GetBytes(ex.GetHashCode()), BadItemKind.MissingItem)
			};
		}

		internal static BadMessageRec MissingFolder(FolderRec folderRec, string folderPath, WellKnownFolderType wkfType)
		{
			return new BadMessageRec
			{
				Kind = BadItemKind.MissingFolder,
				EntryId = BadMessageRec.ComputeKey(folderRec.EntryId, BadItemKind.MissingFolder),
				FolderId = folderRec.EntryId,
				FolderName = folderPath,
				WellKnownFolderType = wkfType
			};
		}

		internal static BadMessageRec MisplacedFolder(FolderRec folderRec, string folderPath, WellKnownFolderType wkfType, byte[] destParentId)
		{
			return new BadMessageRec
			{
				Kind = BadItemKind.MisplacedFolder,
				EntryId = BadMessageRec.ComputeKey(folderRec.EntryId, BadItemKind.MisplacedFolder),
				FolderId = folderRec.EntryId,
				FolderName = folderPath,
				WellKnownFolderType = wkfType
			};
		}

		internal static BadMessageRec Item(MessageRec msgData, FolderRec folderRec, Exception exception)
		{
			BadMessageRec badMessageRec = new BadMessageRec();
			if (exception == null)
			{
				badMessageRec.Kind = BadItemKind.MissingItem;
			}
			else if (CommonUtils.ExceptionIs(exception, new WellKnownException[]
			{
				WellKnownException.MapiMaxSubmissionExceeded
			}))
			{
				badMessageRec.Kind = BadItemKind.LargeItem;
			}
			else
			{
				badMessageRec.Kind = BadItemKind.CorruptItem;
			}
			badMessageRec.EntryId = msgData.EntryId;
			badMessageRec.Sender = (msgData[PropTag.SenderName] as string);
			badMessageRec.Recipient = (msgData[PropTag.ReceivedByName] as string);
			badMessageRec.Subject = (msgData[PropTag.Subject] as string);
			badMessageRec.MessageClass = (msgData[PropTag.MessageClass] as string);
			badMessageRec.MessageSize = (int?)msgData[PropTag.MessageSize];
			badMessageRec.DateSent = (DateTime?)msgData[PropTag.ClientSubmitTime];
			badMessageRec.DateReceived = (DateTime?)msgData[PropTag.MessageDeliveryTime];
			badMessageRec.FolderId = msgData.FolderId;
			badMessageRec.FolderName = folderRec.FolderName;
			badMessageRec.Failure = FailureRec.Create(exception);
			badMessageRec.RawFailure = exception;
			return badMessageRec;
		}

		internal static BadMessageRec Folder(FolderRec folderRec, BadItemKind kind, Exception failure)
		{
			return new BadMessageRec
			{
				Kind = kind,
				EntryId = BadMessageRec.ComputeKey(folderRec.EntryId, kind),
				FolderId = folderRec.EntryId,
				FolderName = folderRec.FolderName,
				Failure = FailureRec.Create(failure),
				RawFailure = failure
			};
		}

		internal static BadMessageRec InferenceData(Exception failure, byte[] entryId)
		{
			return new BadMessageRec
			{
				Kind = BadItemKind.CorruptInferenceProperties,
				Failure = FailureRec.Create(failure),
				EntryId = entryId,
				RawFailure = failure
			};
		}

		internal static BadMessageRec MailboxSetting(Exception failure, ItemPropertiesBase item)
		{
			return new BadMessageRec
			{
				Kind = BadItemKind.CorruptMailboxSetting,
				Failure = FailureRec.Create(failure),
				RawFailure = failure,
				MessageClass = item.GetType().ToString(),
				EntryId = item.GetId()
			};
		}

		internal static BadMessageRec FolderProperty(FolderRec folderRec, PropTag propTag, string sourceValue, string destValue)
		{
			BadMessageRec badMessageRec = new BadMessageRec();
			badMessageRec.FolderId = folderRec.EntryId;
			badMessageRec.FolderName = folderRec.FolderName;
			badMessageRec.Kind = BadItemKind.FolderPropertyMismatch;
			badMessageRec.EntryId = BadMessageRec.ComputeKey(folderRec.EntryId, badMessageRec.Kind, propTag);
			PropertyMismatchException ex = new PropertyMismatchException((uint)propTag, sourceValue, destValue);
			badMessageRec.Failure = FailureRec.Create(ex);
			badMessageRec.RawFailure = ex;
			return badMessageRec;
		}

		private static byte[] ComputeKey(byte[] entryId, BadItemKind kind)
		{
			return CommonUtils.GetSHA512Hash(string.Format("{0}{1}", TraceUtils.DumpEntryId(entryId), kind));
		}

		private static byte[] ComputeKey(byte[] entryId, BadItemKind kind, PropTag propTag)
		{
			return CommonUtils.GetSHA512Hash(string.Format("{0}{1}{2}", TraceUtils.DumpEntryId(entryId), kind, propTag));
		}

		internal static byte[] ComputeKey(PropValueData[] pvda)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PropValueData propValueData in pvda)
			{
				stringBuilder.Append(propValueData.ToString());
			}
			return CommonUtils.GetSHA512Hash(stringBuilder.ToString());
		}

		internal LocalizedString ToLocalizedString()
		{
			switch (this.Kind)
			{
			case BadItemKind.MissingItem:
				return MrsStrings.BadItemMissingItem(this.MessageClass, this.Subject, this.FolderName);
			case BadItemKind.LargeItem:
			{
				int num = this.MessageSize ?? 0;
				return MrsStrings.BadItemLarge(this.MessageClass, this.Subject, new ByteQuantifiedSize((ulong)((long)num)).ToString(), this.FolderName);
			}
			case BadItemKind.CorruptSearchFolderCriteria:
				return MrsStrings.BadItemSearchFolder(this.FolderName);
			case BadItemKind.CorruptFolderACL:
				return MrsStrings.BadItemFolderACL(this.FolderName);
			case BadItemKind.CorruptFolderRule:
				return MrsStrings.BadItemFolderRule(this.FolderName);
			case BadItemKind.MissingFolder:
				return MrsStrings.BadItemMissingFolder(this.FolderName);
			case BadItemKind.MisplacedFolder:
				return MrsStrings.BadItemMisplacedFolder(this.FolderName);
			case BadItemKind.CorruptFolderProperty:
				return MrsStrings.BadItemFolderProperty(this.FolderName);
			case BadItemKind.CorruptMailboxSetting:
				return MrsStrings.BadItemCorruptMailboxSetting(this.MessageClass);
			case BadItemKind.FolderPropertyMismatch:
				return MrsStrings.BadItemFolderPropertyMismatch(this.folderName, this.RawFailure.ToString());
			}
			return MrsStrings.BadItemCorrupt(this.MessageClass, this.Subject, this.FolderName);
		}

		private string sender;

		private string recipient;

		private string subject;

		private string folderName;

		internal static readonly PropTag[] BadItemPtags = new PropTag[]
		{
			PropTag.Subject,
			PropTag.SenderName,
			PropTag.ReceivedByName,
			PropTag.MessageClass,
			PropTag.MessageSize,
			PropTag.ClientSubmitTime,
			PropTag.MessageDeliveryTime
		};
	}
}

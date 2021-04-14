using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Storage.Messaging.Null
{
	internal class MessagingDatabase : IMessagingDatabase
	{
		public DataSource DataSource
		{
			get
			{
				return null;
			}
		}

		public ServerInfoTable ServerInfoTable
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public QueueTable QueueTable
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void SuspendDataCleanup()
		{
		}

		public void ResumeDataCleanup()
		{
		}

		public IMailRecipientStorage NewRecipientStorage(long messageId)
		{
			return new MessagingDatabase.MailRecipientStorage
			{
				MsgId = messageId
			};
		}

		public IMailItemStorage NewMailItemStorage(bool loadDefaults)
		{
			return new MessagingDatabase.MailItemStorage();
		}

		public IMailItemStorage LoadMailItemFromId(long msgId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IMailRecipientStorage> LoadMailRecipientsFromMessageId(long messageId)
		{
			throw new NotImplementedException();
		}

		public IMailRecipientStorage LoadMailRecipientFromId(long recipientId)
		{
			throw new NotImplementedException();
		}

		public Transaction BeginNewTransaction()
		{
			throw new NotImplementedException();
		}

		public void Attach(IMessagingDatabaseConfig config)
		{
		}

		public void Detach()
		{
		}

		public IReplayRequest NewReplayRequest(Guid correlationId, Destination destination, DateTime startTime, DateTime endTime, bool isTestRequest)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IReplayRequest> GetAllReplayRequests()
		{
			throw new NotImplementedException();
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return new XElement("NullMessagingDatabase");
		}

		public virtual IEnumerable<MailItemAndRecipients> GetMessages(List<long> messageIds)
		{
			throw new NotImplementedException();
		}

		public void BootLoadCompleted()
		{
		}

		public virtual MessagingDatabaseResultStatus ReadUnprocessedMessageIds(out Dictionary<byte, List<long>> unprocessedMessageIds)
		{
			throw new NotImplementedException();
		}

		public void Start()
		{
			throw new NotImplementedException();
		}

		public string CurrentState
		{
			get
			{
				return string.Empty;
			}
		}

		internal class MailItemStorage : IMailItemStorage
		{
			public MailItemStorage()
			{
				this.IsNew = true;
				this.mimeCache = new MimeCache(this);
				this.XExch50Blob = new XExch50Blob();
				this.FastIndexBlob = new LazyBytes();
				this.ExtendedProperties = new ExtendedPropertyDictionary();
				this.TransportPropertiesHeader = new MultiValueHeader(this, "X-MS-Exchange-Organization-Transport-Properties");
				this.IsActive = true;
				this.DateReceived = DateTime.UtcNow;
				this.MimeNotSequential = false;
				this.FromAddress = string.Empty;
				this.MimeFrom = string.Empty;
				this.MimeSenderAddress = string.Empty;
				this.DsnFormat = DsnFormat.Default;
				this.HeloDomain = null;
				this.EnvId = string.Empty;
				this.Auth = string.Empty;
				this.BodyType = BodyType.Default;
				this.ReceiveConnectorName = string.Empty;
				this.Subject = string.Empty;
				this.InternetMessageId = string.Empty;
				this.ShadowServerDiscardId = string.Empty;
				this.Directionality = MailDirectionality.Undefined;
				this.ShadowMessageId = Guid.NewGuid();
				this.SourceIPAddress = IPAddress.None;
				this.PoisonCount = 0;
			}

			public bool IsNew { get; private set; }

			public bool IsDeleted { get; private set; }

			public bool IsActive { get; set; }

			public bool IsReadOnly { get; set; }

			public bool PendingDatabaseUpdates { get; private set; }

			public bool IsInAsyncCommit { get; private set; }

			public long MsgId { get; private set; }

			public string FromAddress { get; set; }

			public string AttributedFromAddress { get; set; }

			public DateTime DateReceived { get; set; }

			public TimeSpan ExtensionToExpiryDuration { get; set; }

			public DsnFormat DsnFormat { get; set; }

			public bool IsDiscardPending { get; set; }

			public MailDirectionality Directionality { get; set; }

			public string HeloDomain { get; set; }

			public string Auth { get; set; }

			public string EnvId { get; set; }

			public BodyType BodyType { get; set; }

			public string Oorg { get; set; }

			public string ReceiveConnectorName { get; set; }

			public int PoisonCount { get; set; }

			public IPAddress SourceIPAddress { get; set; }

			public string Subject { get; set; }

			public string InternetMessageId { get; set; }

			public Guid ShadowMessageId { get; set; }

			public string ShadowServerContext { get; set; }

			public string ShadowServerDiscardId { get; set; }

			public IDataExternalComponent Recipients { get; set; }

			public int Scl { get; set; }

			public string PerfCounterAttribution { get; set; }

			public IExtendedPropertyCollection ExtendedProperties { get; private set; }

			public XExch50Blob XExch50Blob { get; private set; }

			public LazyBytes FastIndexBlob { get; private set; }

			public bool IsJournalReport { get; private set; }

			public List<string> MoveToHosts { get; set; }

			public bool RetryDeliveryIfRejected { get; set; }

			public MultiValueHeader TransportPropertiesHeader { get; private set; }

			public DeliveryPriority? Priority { get; set; }

			public DeliveryPriority BootloadingPriority { get; set; }

			public string PrioritizationReason { get; set; }

			public Guid NetworkMessageId { get; set; }

			public Guid SystemProbeId { get; set; }

			public RiskLevel RiskLevel { get; set; }

			public string ExoAccountForest { get; set; }

			public string ExoTenantContainer { get; set; }

			public Guid ExternalOrganizationId { get; set; }

			public MimeDocument MimeDocument
			{
				get
				{
					return this.mimeCache.MimeDocument;
				}
				set
				{
					this.mimeCache.SetMimeDocument(value);
				}
			}

			public EmailMessage Message
			{
				get
				{
					return this.mimeCache.Message;
				}
			}

			public string MimeFrom { get; set; }

			public string MimeSenderAddress { get; set; }

			public bool MimeNotSequential { get; set; }

			public bool FallbackToRawOverride
			{
				get
				{
					return this.mimeCache.FallbackToRawOverride;
				}
				set
				{
					this.mimeCache.FallbackToRawOverride = value;
				}
			}

			public Encoding MimeDefaultEncoding
			{
				get
				{
					return this.mimeCache.DefaultEncoding;
				}
				set
				{
					this.mimeCache.DefaultEncoding = value;
				}
			}

			public bool MimeWriteStreamOpen
			{
				get
				{
					return this.mimeCache.MimeWriteStreamOpen;
				}
			}

			public long MimeSize { get; set; }

			public MimePart RootPart
			{
				get
				{
					return this.mimeCache.RootPart;
				}
			}

			public string ProbeName { get; set; }

			public bool PersistProbeTrace { get; set; }

			public Stream OpenMimeReadStream(bool downConvert)
			{
				return this.mimeCache.OpenMimeReadStream(downConvert);
			}

			public Stream OpenMimeWriteStream(MimeLimits mimeLimits, bool expectBinaryContent)
			{
				return this.mimeCache.OpenMimeWriteStream(mimeLimits, expectBinaryContent);
			}

			public void OpenMimeDBWriter(DataTableCursor cursor, bool update, Func<bool> checkpointCallback, out Stream mimeMap, out CreateFixedStream mimeCreateFixedStream)
			{
				throw new NotImplementedException();
			}

			public Stream OpenMimeDBReader()
			{
				throw new NotImplementedException();
			}

			public MimeDocument LoadMimeFromDB(DecodingOptions decodingOptions)
			{
				throw new NotImplementedException();
			}

			public long GetCurrrentMimeSize()
			{
				return this.mimeCache.MimeStreamSize;
			}

			public long RefreshMimeSize()
			{
				return this.mimeCache.MimeStreamSize;
			}

			public void UpdateCachedHeaders()
			{
				this.mimeCache.PromoteHeaders();
			}

			public void RestoreLastSavedMime()
			{
				this.mimeCache.RestoreLastSavedMime();
			}

			public void ResetMimeParserEohCallback()
			{
				this.mimeCache.ResetMimeParserEohCallback();
			}

			public void MinimizeMemory()
			{
			}

			public void Commit(TransactionCommitMode commitMode)
			{
				this.Materialize(null);
			}

			public void Materialize(Transaction transaction)
			{
				if (this.MsgId == 0L)
				{
					this.MsgId = Interlocked.Increment(ref MessagingDatabase.MailItemStorage.lastId);
				}
				this.UpdateCachedHeaders();
				this.IsNew = false;
			}

			public IAsyncResult BeginCommit(AsyncCallback asyncCallback, object asyncState)
			{
				this.Commit(TransactionCommitMode.Lazy);
				return new AsyncResult(asyncCallback, asyncState, true);
			}

			public bool EndCommit(IAsyncResult ar, out Exception exception)
			{
				exception = null;
				return true;
			}

			public void MarkToDelete()
			{
				this.IsDeleted = true;
				this.IsNew = false;
			}

			public IMailItemStorage Clone()
			{
				throw new NotImplementedException();
			}

			public void ReleaseFromActive()
			{
				this.IsActive = false;
				this.mimeCache.CleanupEmailMessage();
				this.mimeCache.CleanupMimeDocument();
			}

			public IMailItemStorage CloneCommitted(Action<IMailItemStorage> cloneVisitor)
			{
				return this.Clone();
			}

			public void AtomicChange(Action<IMailItemStorage> changeAction)
			{
				changeAction(this);
			}

			public IMailItemStorage CopyCommitted(Action<IMailItemStorage> copyVisitor)
			{
				return this.Clone();
			}

			private static long lastId;

			private readonly MimeCache mimeCache;
		}

		internal class MailRecipientStorage : IMailRecipientStorage
		{
			public MailRecipientStorage()
			{
				this.ExtendedProperties = new ExtendedPropertyDictionary();
				this.Email = string.Empty;
				this.DsnRequested = DsnRequestedFlags.Default;
				this.DsnNeeded = DsnFlags.None;
				this.DsnCompleted = DsnFlags.None;
				this.Status = Status.Ready;
				this.RetryCount = 0;
				this.AdminActionStatus = AdminActionStatus.None;
				this.IsActive = true;
			}

			public long RecipId { get; private set; }

			public long MsgId { get; set; }

			public AdminActionStatus AdminActionStatus { get; set; }

			public DateTime? DeliveryTime { get; set; }

			public DsnFlags DsnCompleted { get; set; }

			public DsnFlags DsnNeeded { get; set; }

			public DsnRequestedFlags DsnRequested { get; set; }

			public Destination DeliveredDestination { get; set; }

			public string Email { get; set; }

			public string ORcpt { get; set; }

			public string PrimaryServerFqdnGuid { get; set; }

			public int RetryCount { get; set; }

			public Status Status { get; set; }

			public RequiredTlsAuthLevel? TlsAuthLevel { get; set; }

			public int OutboundIPPool { get; set; }

			public IExtendedPropertyCollection ExtendedProperties { get; private set; }

			public bool IsDeleted { get; private set; }

			public bool IsInSafetyNet { get; private set; }

			public bool IsActive { get; private set; }

			public bool PendingDatabaseUpdates
			{
				get
				{
					return false;
				}
			}

			public void MarkToDelete()
			{
				this.IsDeleted = true;
				this.IsActive = false;
			}

			public void Materialize(Transaction transaction)
			{
				if (this.RecipId == 0L)
				{
					this.RecipId = Interlocked.Increment(ref MessagingDatabase.MailRecipientStorage.lastRecipId);
				}
			}

			public void Commit(TransactionCommitMode commitMode)
			{
				this.Materialize(null);
			}

			public IMailRecipientStorage MoveTo(long targetMsgId)
			{
				this.MsgId = targetMsgId;
				return this;
			}

			public IMailRecipientStorage CopyTo(long target)
			{
				throw new NotImplementedException();
			}

			public void MinimizeMemory()
			{
			}

			public void ReleaseFromActive()
			{
				this.IsActive = false;
			}

			public void AddToSafetyNet()
			{
				this.IsInSafetyNet = true;
			}

			private static long lastRecipId;
		}
	}
}

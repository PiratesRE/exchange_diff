using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;

namespace Microsoft.Exchange.Transport.Storage.Messaging.Utah
{
	internal class MailItemStorage : DataRow, IMailItemStorage
	{
		private static void TransactionCallback(IAsyncResult asyncResult)
		{
			Tuple<Transaction, AsyncResult> tuple = (Tuple<Transaction, AsyncResult>)asyncResult.AsyncState;
			try
			{
				tuple.Item1.EndAsyncCommit(asyncResult);
			}
			catch (Exception exception)
			{
				tuple.Item2.Exception = exception;
			}
			tuple.Item2.IsCompleted = true;
		}

		private void InitComponents()
		{
			this.blobCollection = new BlobCollection(this.Table.Schemas[21], this);
			this.componentMimeCache = new MimeCache(this);
			this.componentXexch50 = new XExch50Blob(this, this.blobCollection, 5);
			this.componentFastIndexBlob = new LazyBytes(this, this.blobCollection, 6);
			this.componentExtendedProperties = new ExtendedPropertyDictionary(this, this.blobCollection, 3);
			this.componentInternalProperties = new ExtendedPropertyDictionary(this, this.blobCollection, 4);
			base.AddComponent(this.componentExtendedProperties);
			base.AddComponent(this.componentInternalProperties);
			base.AddComponent(this.componentXexch50);
			base.AddComponent(this.componentFastIndexBlob);
			base.AddFirstComponent(this.componentMimeCache);
		}

		private void LoadDefaults()
		{
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
			this.IsDiscardPending = false;
		}

		public MailItemStorage(DataTable dataTable, bool loadDefaults) : base(dataTable)
		{
			this.InitComponents();
			if (loadDefaults)
			{
				this.LoadDefaults();
			}
			base.PerfCounters.NewMailItem.Increment();
		}

		public MailItemStorage(DataTableCursor cursor) : base(cursor.Table)
		{
			this.InitComponents();
			base.LoadFromCurrentRow(cursor);
			if (this.IsActive)
			{
				this.Table.IncrementActiveMessageCount();
			}
			if (this.IsPending)
			{
				this.Table.IncrementPendingMessageCount();
			}
		}

		public new string PerfCounterAttribution
		{
			get
			{
				if (this.perfCounters != null)
				{
					return this.perfCounters.Name;
				}
				return string.Empty;
			}
			set
			{
				base.PerfCounterAttribution = value;
			}
		}

		public long MsgId
		{
			get
			{
				return this.Table.Generation.CombineIds(this.MessageRowId);
			}
		}

		public int MessageRowId
		{
			get
			{
				return ((ColumnCache<int>)base.Columns[0]).Value;
			}
			protected set
			{
				((ColumnCache<int>)base.Columns[0]).Value = value;
			}
		}

		public string HeloDomain
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[14]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				((ColumnCache<string>)base.Columns[14]).Value = value;
			}
		}

		public DateTime DateReceived
		{
			get
			{
				return ((ColumnCache<DateTime>)base.Columns[2]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				((ColumnCache<DateTime>)base.Columns[2]).Value = value;
			}
		}

		public string Auth
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[16]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				((ColumnCache<string>)base.Columns[16]).Value = value;
			}
		}

		public string EnvId
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[12]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				((ColumnCache<string>)base.Columns[12]).Value = value;
			}
		}

		public string ReceiveConnectorName
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[13]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				((ColumnCache<string>)base.Columns[13]).Value = value;
			}
		}

		public int PoisonCount
		{
			get
			{
				return (int)((ColumnCache<byte>)base.Columns[6]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				((ColumnCache<byte>)base.Columns[6]).Value = (byte)value;
			}
		}

		public IPAddress SourceIPAddress
		{
			get
			{
				return ((ColumnCache<IPvxAddress>)base.Columns[3]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				IPvxAddress value2 = new IPvxAddress(value);
				((ColumnCache<IPvxAddress>)base.Columns[3]).Value = value2;
			}
		}

		public Guid ShadowMessageId
		{
			get
			{
				return ((ColumnCache<Guid>)base.Columns[8]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				((ColumnCache<Guid>)base.Columns[8]).Value = value;
			}
		}

		public string ShadowServerContext
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[10]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				((ColumnCache<string>)base.Columns[10]).Value = value;
			}
		}

		public string ShadowServerDiscardId
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[9]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				((ColumnCache<string>)base.Columns[9]).Value = value;
			}
		}

		public string FromAddress
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[15]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				((ColumnCache<string>)base.Columns[15]).Value = value;
			}
		}

		public TimeSpan ExtensionToExpiryDuration
		{
			get
			{
				ColumnCache<int> columnCache = (ColumnCache<int>)base.Columns[7];
				if (!columnCache.HasValue)
				{
					return TimeSpan.Zero;
				}
				return TimeSpan.FromSeconds((double)columnCache.Value);
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				if (value == TimeSpan.Zero)
				{
					((ColumnCache<int>)base.Columns[7]).HasValue = false;
					return;
				}
				double totalSeconds = value.TotalSeconds;
				if (totalSeconds > 2147483647.0)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "ExtensionToExpiryDuration.TotalSeconds = '{0}' which is greater than Int32.MaxValue", new object[]
					{
						totalSeconds
					}));
				}
				((ColumnCache<int>)base.Columns[7]).Value = (int)totalSeconds;
			}
		}

		public bool IsDiscardPending
		{
			get
			{
				return this.GetPendingReasonValue(MailItemStorage.PendingReasons.DiscardPending);
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.SetPendingReasonValue(value, MailItemStorage.PendingReasons.DiscardPending);
			}
		}

		public bool IsActive
		{
			get
			{
				return this.GetPendingReasonValue(MailItemStorage.PendingReasons.Active);
			}
			set
			{
				if (value == this.IsActive)
				{
					return;
				}
				this.ThrowIfDeleted();
				this.SetPendingReasonValue(value, MailItemStorage.PendingReasons.Active);
				if (value)
				{
					this.Table.IncrementActiveMessageCount();
					return;
				}
				this.Table.DecrementActiveMessageCount();
			}
		}

		public bool IsPending
		{
			get
			{
				ColumnCache<byte> columnCache = (ColumnCache<byte>)base.Columns[22];
				return columnCache.HasValue && (columnCache.Value & 15) != 0;
			}
		}

		public DeliveryPriority BootloadingPriority
		{
			get
			{
				ColumnCache<byte> columnCache = (ColumnCache<byte>)base.Columns[22];
				if (!columnCache.HasValue)
				{
					return DeliveryPriority.Normal;
				}
				return (DeliveryPriority)(columnCache.Value >> 4);
			}
			set
			{
				this.ThrowIfDeleted();
				ColumnCache<byte> columnCache = (ColumnCache<byte>)base.Columns[22];
				if (columnCache.HasValue)
				{
					columnCache.Value = (byte)((int)(columnCache.Value & 15) | (int)((byte)value) << 4);
				}
			}
		}

		public string Oorg
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[11]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				((ColumnCache<string>)base.Columns[11]).Value = value;
			}
		}

		public string MimeFrom
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[18]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				((ColumnCache<string>)base.Columns[18]).Value = value;
			}
		}

		public BodyType BodyType
		{
			get
			{
				return (BodyType)((ColumnCache<byte>)base.Columns[4]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				((ColumnCache<byte>)base.Columns[4]).Value = (byte)value;
			}
		}

		public string Subject
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[19]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				((ColumnCache<string>)base.Columns[19]).Value = value;
			}
		}

		public string InternetMessageId
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[20]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				((ColumnCache<string>)base.Columns[20]).Value = value;
			}
		}

		public string MimeSenderAddress
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[17]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				((ColumnCache<string>)base.Columns[17]).Value = value;
			}
		}

		public bool IsInAsyncCommit
		{
			get
			{
				return false;
			}
		}

		protected bool UnderConstruction
		{
			get
			{
				int value = ((ColumnCache<int>)base.Columns[1]).Value;
				return (value & 64) != 0;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				int num = ((ColumnCache<int>)base.Columns[1]).Value;
				if (value)
				{
					num |= 64;
				}
				else
				{
					num &= -65;
				}
				((ColumnCache<int>)base.Columns[1]).Value = num;
			}
		}

		public DsnFormat DsnFormat
		{
			get
			{
				int value = ((ColumnCache<int>)base.Columns[1]).Value;
				return (DsnFormat)(value & 15);
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				int num = ((ColumnCache<int>)base.Columns[1]).Value;
				num = ((-16 & num) | (int)((DsnFormat)15 & value));
				((ColumnCache<int>)base.Columns[1]).Value = num;
			}
		}

		public int Scl
		{
			get
			{
				int num = ((ColumnCache<int>)base.Columns[1]).Value;
				num = ((num & 3840) >> 8 & 15);
				return num - 2;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				value += 2;
				int num = ((ColumnCache<int>)base.Columns[1]).Value;
				num = ((-3841 & num) | (3840 & value << 8));
				((ColumnCache<int>)base.Columns[1]).Value = num;
			}
		}

		public MailDirectionality Directionality
		{
			get
			{
				int value = ((ColumnCache<int>)base.Columns[1]).Value;
				return (MailDirectionality)Convert.ToByte((value & 28672) >> 12 & 7);
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				int num = ((ColumnCache<int>)base.Columns[1]).Value;
				num = ((-28673 & num) | (28672 & Convert.ToInt32(value) << 12));
				((ColumnCache<int>)base.Columns[1]).Value = num;
			}
		}

		public IExtendedPropertyCollection ExtendedProperties
		{
			get
			{
				return this.componentExtendedProperties;
			}
		}

		public string PrioritizationReason
		{
			get
			{
				return this.componentInternalProperties.GetValue<string>("Microsoft.Exchange.Transport.TransportMailItem.PrioritizationReason", string.Empty);
			}
			set
			{
				this.componentInternalProperties.SetValue<string>("Microsoft.Exchange.Transport.TransportMailItem.PrioritizationReason", value);
			}
		}

		public bool IsJournalReport
		{
			get
			{
				string value;
				this.componentInternalProperties.TryGetValue<string>("Microsoft.Exchange.ContentIdentifier", out value);
				if (!string.IsNullOrEmpty(value))
				{
					return "EXJournalData".Equals(value, StringComparison.OrdinalIgnoreCase);
				}
				Header header = this.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Journal-Report");
				if (header != null)
				{
					if (!this.IsReadOnly)
					{
						this.componentInternalProperties.SetValue<string>("Microsoft.Exchange.ContentIdentifier", "EXJournalData");
					}
					return true;
				}
				return false;
			}
		}

		public List<string> MoveToHosts
		{
			get
			{
				ReadOnlyCollection<string> collection;
				if (this.componentInternalProperties.TryGetListValue<string>("Microsoft.Exchange.Transport.DirectoryData.RedirectHosts", out collection))
				{
					return new List<string>(collection);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.componentInternalProperties.Remove("Microsoft.Exchange.Transport.DirectoryData.RedirectHosts");
					return;
				}
				this.componentInternalProperties.SetValue<List<string>>("Microsoft.Exchange.Transport.DirectoryData.RedirectHosts", value);
			}
		}

		public bool RetryDeliveryIfRejected
		{
			get
			{
				return this.retryDeliveryIfRejected;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.retryDeliveryIfRejected = value;
			}
		}

		public MultiValueHeader TransportPropertiesHeader
		{
			get
			{
				this.ThrowIfReadOnly();
				MultiValueHeader result;
				if ((result = this.transportPropertiesHeader) == null)
				{
					result = (this.transportPropertiesHeader = new MultiValueHeader(this, "X-MS-Exchange-Organization-Transport-Properties"));
				}
				return result;
			}
		}

		public DeliveryPriority? Priority
		{
			get
			{
				return this.priority;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				if (this.priority == value)
				{
					return;
				}
				this.priority = value;
				this.TransportPropertiesHeader.SetStringValue("DeliveryPriority", value.ToString());
				if (value == DeliveryPriority.Normal)
				{
					this.componentInternalProperties.Remove("Microsoft.Exchange.Transport.TransportMailItem.PrioritizationReason");
				}
				this.BootloadingPriority = (value ?? DeliveryPriority.Normal);
			}
		}

		public Guid NetworkMessageId
		{
			get
			{
				Guid result;
				if (this.componentInternalProperties.TryGetValue<Guid>("Microsoft.Exchange.Transport.MailRecipient.NetworkMessageId", out result))
				{
					return result;
				}
				return Guid.Empty;
			}
			set
			{
				this.componentInternalProperties.SetValue<Guid>("Microsoft.Exchange.Transport.MailRecipient.NetworkMessageId", value);
			}
		}

		public Guid SystemProbeId
		{
			get
			{
				Guid result;
				if (this.componentInternalProperties.TryGetValue<Guid>("Microsoft.Exchange.Transport.TransportMailItem.SystemProbeId", out result))
				{
					return result;
				}
				return Guid.Empty;
			}
			set
			{
				this.componentInternalProperties.SetValue<Guid>("Microsoft.Exchange.Transport.TransportMailItem.SystemProbeId", value);
			}
		}

		public RiskLevel RiskLevel
		{
			get
			{
				return (RiskLevel)this.componentInternalProperties.GetValue<int>("Microsoft.Exchange.Transport.TransportMailItem.RiskLevel", 0);
			}
			set
			{
				this.componentInternalProperties.SetValue<int>("Microsoft.Exchange.Transport.TransportMailItem.RiskLevel", (int)value);
			}
		}

		public string ExoAccountForest
		{
			get
			{
				string result;
				if (this.componentInternalProperties.TryGetValue<string>("Microsoft.Exchange.Transport.TransportMailItem.ExoAccountForest", out result))
				{
					return result;
				}
				return null;
			}
			set
			{
				this.componentInternalProperties.SetValue<string>("Microsoft.Exchange.Transport.TransportMailItem.ExoAccountForest", value);
			}
		}

		public string ExoTenantContainer
		{
			get
			{
				string result;
				if (this.componentInternalProperties.TryGetValue<string>("Microsoft.Exchange.Transport.TransportMailItem.ExoTenantContainer", out result))
				{
					return result;
				}
				return null;
			}
			set
			{
				this.componentInternalProperties.SetValue<string>("Microsoft.Exchange.Transport.TransportMailItem.ExoTenantContainer", value);
			}
		}

		public Guid ExternalOrganizationId
		{
			get
			{
				Guid result;
				if (this.componentInternalProperties.TryGetValue<Guid>("Microsoft.Exchange.Transport.TransportMailItem.ExternalOrganizationId", out result))
				{
					return result;
				}
				return Guid.Empty;
			}
			set
			{
				this.componentInternalProperties.SetValue<Guid>("Microsoft.Exchange.Transport.TransportMailItem.ExternalOrganizationId", value);
			}
		}

		public string AttributedFromAddress
		{
			get
			{
				string result;
				if (this.componentInternalProperties.TryGetValue<string>("Microsoft.Exchange.Transport.TransportMailItem.AttributedFromAddress", out result))
				{
					return result;
				}
				return null;
			}
			set
			{
				this.componentInternalProperties.SetValue<string>("Microsoft.Exchange.Transport.TransportMailItem.AttributedFromAddress", value);
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
			set
			{
				this.componentMimeCache.SetReadOnly(value);
				this.componentExtendedProperties.IsReadOnly = value;
				this.componentInternalProperties.IsReadOnly = value;
				this.componentXexch50.IsReadOnly = value;
				this.componentFastIndexBlob.IsReadOnly = value;
				this.isReadOnly = value;
			}
		}

		public new MessageTable Table
		{
			get
			{
				return (MessageTable)base.Table;
			}
		}

		public MimeDocument MimeDocument
		{
			get
			{
				return this.componentMimeCache.MimeDocument;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				this.componentMimeCache.SetMimeDocument(value);
			}
		}

		public EmailMessage Message
		{
			get
			{
				return this.componentMimeCache.Message;
			}
		}

		public MimePart RootPart
		{
			get
			{
				return this.componentMimeCache.RootPart;
			}
		}

		public long MimeSize
		{
			get
			{
				return ((ColumnCache<long>)base.Columns[5]).Value;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				((ColumnCache<long>)base.Columns[5]).Value = value;
			}
		}

		public bool MimeNotSequential
		{
			get
			{
				int value = ((ColumnCache<int>)base.Columns[1]).Value;
				return (value & 128) != 0;
			}
			set
			{
				this.ThrowIfDeleted();
				int num = ((ColumnCache<int>)base.Columns[1]).Value;
				if (value)
				{
					num |= 128;
				}
				else
				{
					num &= -129;
				}
				((ColumnCache<int>)base.Columns[1]).Value = num;
			}
		}

		public bool FallbackToRawOverride
		{
			get
			{
				return this.componentMimeCache.FallbackToRawOverride;
			}
			set
			{
				this.componentMimeCache.FallbackToRawOverride = value;
			}
		}

		public bool MimeWriteStreamOpen
		{
			get
			{
				return this.componentMimeCache.MimeWriteStreamOpen;
			}
		}

		public Encoding MimeDefaultEncoding
		{
			get
			{
				return this.componentMimeCache.DefaultEncoding;
			}
			set
			{
				this.componentMimeCache.DefaultEncoding = value;
			}
		}

		public XExch50Blob XExch50Blob
		{
			get
			{
				return this.componentXexch50;
			}
		}

		public LazyBytes FastIndexBlob
		{
			get
			{
				return this.componentFastIndexBlob;
			}
		}

		public string ProbeName { get; set; }

		public bool PersistProbeTrace { get; set; }

		public IDataExternalComponent Recipients
		{
			get
			{
				return this.componentRecipients;
			}
			set
			{
				if (value != this.componentRecipients)
				{
					if (this.componentRecipients != null)
					{
						base.ReplaceComponent(this.componentRecipients, value);
					}
					else
					{
						base.AddComponent(value);
					}
					this.componentRecipients = value;
				}
			}
		}

		public static MailItemStorage LoadFromRow(DataTableCursor cursor)
		{
			MailItemStorage mailItemStorage = new MailItemStorage(cursor);
			mailItemStorage.PerfCounters.LoadMailItem.Increment();
			return mailItemStorage;
		}

		public override void MinimizeMemory()
		{
			base.MinimizeMemory();
			base.PerfCounters.DehydrateMailItem.Increment();
		}

		public void ReleaseFromActive()
		{
			this.IsActive = false;
		}

		public override void MarkToDelete()
		{
			base.MarkToDelete();
			this.Table.DecrementMessageCount();
			if (this.IsActive)
			{
				this.Table.DecrementActiveMessageCount();
			}
			if (this.IsPending)
			{
				this.Table.DecrementPendingMessageCount();
			}
		}

		public Stream OpenMimeReadStream(bool downConvert)
		{
			return this.componentMimeCache.OpenMimeReadStream(downConvert);
		}

		public Stream OpenMimeWriteStream(MimeLimits mimeLimits, bool expectBinaryContent)
		{
			this.ThrowIfDeletedOrReadOnly();
			this.ThrowIfInAsyncCommit();
			return this.componentMimeCache.OpenMimeWriteStream(mimeLimits, expectBinaryContent);
		}

		public long GetCurrrentMimeSize()
		{
			this.ThrowIfInAsyncCommit();
			return this.componentMimeCache.MimeStreamSize;
		}

		public long RefreshMimeSize()
		{
			this.ThrowIfDeletedOrReadOnly();
			return this.GetCurrrentMimeSize();
		}

		public void RestoreLastSavedMime()
		{
			this.ThrowIfDeletedOrReadOnly();
			this.ThrowIfInAsyncCommit();
			this.componentMimeCache.RestoreLastSavedMime();
		}

		public void UpdateCachedHeaders()
		{
			this.ThrowIfDeletedOrReadOnly();
			this.ThrowIfInAsyncCommit();
			this.componentMimeCache.PromoteHeaders();
		}

		public void ResetMimeParserEohCallback()
		{
			this.componentMimeCache.ResetMimeParserEohCallback();
		}

		public new void Commit(TransactionCommitMode commitMode)
		{
			this.audit.Drop((commitMode == TransactionCommitMode.Lazy) ? Breadcrumb.CommitLazy : Breadcrumb.CommitNow);
			base.Commit(commitMode);
			((commitMode == TransactionCommitMode.Immediate) ? base.PerfCounters.CommitImmediateMailItem : base.PerfCounters.CommitLazyMailItem).Increment();
			if (this.IsDeleted)
			{
				((commitMode == TransactionCommitMode.Immediate) ? base.PerfCounters.Delete : base.PerfCounters.DeleteLazyMailItem).Increment();
			}
		}

		public new void Materialize(Transaction transaction)
		{
			base.Materialize(transaction);
		}

		protected override void MaterializeToCursor(Transaction transaction, DataTableCursor cursor, Func<bool> checkpointCallback)
		{
			if (this.MessageRowId == 0)
			{
				this.AssignMessageId(this.Table.GetNextMessageRowId());
			}
			base.MaterializeToCursor(transaction, cursor, checkpointCallback);
			base.PerfCounters.MaterializeMailItem.Increment();
		}

		public IAsyncResult BeginCommit(AsyncCallback asyncCallback, object asyncState)
		{
			AsyncResult asyncResult = new AsyncResult(asyncCallback, asyncState);
			if (!base.PendingDatabaseUpdates)
			{
				asyncResult.CompletedSynchronously = true;
				asyncResult.IsCompleted = true;
				return asyncResult;
			}
			base.PerfCounters.BeginCommitMailItem.Increment();
			try
			{
				using (DataConnection dataConnection = this.Table.DataSource.DemandNewConnection())
				{
					using (Transaction transaction = dataConnection.BeginTransaction())
					{
						this.Materialize(transaction);
						transaction.BeginAsyncCommit(MailItemStorage.DefaultAsyncCommitTimeout, Tuple.Create<Transaction, AsyncResult>(transaction, asyncResult), new AsyncCallback(MailItemStorage.TransactionCallback));
					}
				}
			}
			catch (Exception exception)
			{
				asyncResult.Exception = exception;
				asyncResult.CompletedSynchronously = true;
				asyncResult.IsCompleted = true;
			}
			return asyncResult;
		}

		public bool EndCommit(IAsyncResult ar, out Exception exception)
		{
			AsyncResult asyncResult = (AsyncResult)ar;
			exception = null;
			try
			{
				asyncResult.End();
			}
			catch (Exception ex)
			{
				exception = ex;
			}
			return null == exception;
		}

		public void CloneTo(MailItemStorage newMailItemStorage)
		{
			newMailItemStorage.Columns.CloneFrom(base.Columns);
			newMailItemStorage.componentExtendedProperties.CloneFrom(this.componentExtendedProperties);
			newMailItemStorage.componentInternalProperties.CloneFrom(this.componentInternalProperties);
			newMailItemStorage.ProbeName = this.ProbeName;
			newMailItemStorage.PersistProbeTrace = this.PersistProbeTrace;
			if (this.Table == newMailItemStorage.Table)
			{
				newMailItemStorage.SetCloneOrMoveSource(this, true);
				((IDataObjectComponent)newMailItemStorage.componentXexch50).CloneFrom(this.componentXexch50);
				((IDataObjectComponent)newMailItemStorage.componentFastIndexBlob).CloneFrom(this.componentFastIndexBlob);
				((IDataObjectComponent)newMailItemStorage.componentMimeCache).CloneFrom(this.componentMimeCache);
			}
			else
			{
				newMailItemStorage.Columns.MarkDirtyForReload();
				newMailItemStorage.componentExtendedProperties.Dirty = true;
				newMailItemStorage.componentInternalProperties.Dirty = true;
				newMailItemStorage.componentXexch50.Value = this.componentXexch50.Value;
				newMailItemStorage.componentFastIndexBlob.Value = this.componentFastIndexBlob.Value;
				using (Stream stream = newMailItemStorage.componentMimeCache.OpenMimeWriteStream(MimeLimits.Unlimited, true))
				{
					using (Stream stream2 = this.componentMimeCache.OpenMimeReadStream(false))
					{
						byte[] array = new byte[65536];
						int count;
						while ((count = stream2.Read(array, 0, array.Length)) > 0)
						{
							stream.Write(array, 0, count);
						}
						stream2.Close();
						stream.Close();
					}
				}
			}
			newMailItemStorage.MessageRowId = 0;
			newMailItemStorage.ShadowMessageId = Guid.NewGuid();
			if (newMailItemStorage.IsActive)
			{
				newMailItemStorage.Table.IncrementActiveMessageCount();
			}
			if (newMailItemStorage.IsPending)
			{
				newMailItemStorage.Table.IncrementPendingMessageCount();
			}
		}

		public IMailItemStorage CloneCommitted(Action<IMailItemStorage> cloneVisitor)
		{
			MailItemStorage mailItemStorage = new MailItemStorage(this.Table, false);
			IMailItemStorage result;
			using (Transaction transaction = this.Table.DataSource.BeginNewTransaction())
			{
				this.Materialize(transaction);
				this.CloneTo(mailItemStorage);
				cloneVisitor(mailItemStorage);
				mailItemStorage.Materialize(transaction);
				transaction.Commit();
				result = mailItemStorage;
			}
			return result;
		}

		public void AtomicChange(Action<IMailItemStorage> changeAction)
		{
			using (Transaction transaction = this.Table.DataSource.BeginNewTransaction())
			{
				this.Materialize(transaction);
				changeAction(this);
				transaction.Commit();
			}
		}

		public IMailItemStorage CopyCommitted(Action<IMailItemStorage> copyVisitor)
		{
			base.Commit();
			DataGeneration generation = this.Table.Generation;
			MailItemStorage mailItemStorage = (MailItemStorage)generation.MessagingDatabase.NewMailItemStorage(false);
			IMailItemStorage result;
			using (Transaction transaction = mailItemStorage.Table.DataSource.BeginNewTransaction())
			{
				this.CloneTo(mailItemStorage);
				copyVisitor(mailItemStorage);
				mailItemStorage.Materialize(transaction);
				transaction.Commit();
				result = mailItemStorage;
			}
			return result;
		}

		IMailItemStorage IMailItemStorage.Clone()
		{
			DataGeneration generation = this.Table.Generation;
			MailItemStorage mailItemStorage = (MailItemStorage)generation.MessagingDatabase.NewMailItemStorage(false);
			this.CloneTo(mailItemStorage);
			return mailItemStorage;
		}

		private void AssignMessageId(int newId)
		{
			this.MessageRowId = newId;
			if (this.Recipients != null)
			{
				this.Recipients.ParentPrimaryKeyChanged();
			}
		}

		private void ThrowIfReadOnly()
		{
			if (this.IsReadOnly)
			{
				throw new InvalidOperationException("This operation cannot be performed in read-only mode. Mail item queued for delivery?");
			}
		}

		private void ThrowIfInAsyncCommit()
		{
			if (this.IsInAsyncCommit)
			{
				throw new InvalidOperationException("This operation cannot be performed when mail item is in Async Commit");
			}
		}

		private void ThrowIfDeleted()
		{
			if (this.IsDeleted)
			{
				throw new InvalidOperationException("operations not allowed on a deleted mail item");
			}
		}

		private void ThrowIfDeletedOrReadOnly()
		{
			this.ThrowIfDeleted();
			this.ThrowIfReadOnly();
		}

		public void OpenMimeDBWriter(DataTableCursor cursor, bool update, Func<bool> checkpointCallback, out Stream mimeMap, out CreateFixedStream mimeCreateFixedStream)
		{
			mimeCreateFixedStream = (() => this.blobCollection.OpenWriter(1, cursor, update, true, checkpointCallback));
			mimeMap = this.blobCollection.OpenWriter(2, cursor, update, false, null);
		}

		public MimeDocument LoadMimeFromDB(DecodingOptions decodingOptions)
		{
			MimeDocument result;
			using (DataTableCursor cursor = this.Table.GetCursor())
			{
				using (cursor.BeginTransaction())
				{
					base.SeekCurrent(cursor);
					using (Stream stream = this.blobCollection.OpenReader(2, cursor, false))
					{
						result = MimeCacheMap.Load(stream, () => this.OpenMimeDBReader(cursor), decodingOptions);
					}
				}
			}
			return result;
		}

		public Stream OpenMimeDBReader(DataTableCursor cursor)
		{
			return this.blobCollection.OpenReader(1, cursor, true);
		}

		public Stream OpenMimeDBReader()
		{
			Stream result;
			using (DataTableCursor cursor = this.Table.GetCursor())
			{
				base.SeekCurrent(cursor);
				result = this.OpenMimeDBReader(cursor);
			}
			return result;
		}

		public Stream OpenMimeMapReader(DataTableCursor cursor)
		{
			return this.blobCollection.OpenReader(2, cursor, false);
		}

		private bool GetPendingReasonValue(MailItemStorage.PendingReasons reason)
		{
			ColumnCache<byte> columnCache = (ColumnCache<byte>)base.Columns[22];
			return columnCache.HasValue && ((MailItemStorage.PendingReasons)columnCache.Value & reason) == reason;
		}

		private void SetPendingReasonValue(bool set, MailItemStorage.PendingReasons reason)
		{
			ColumnCache<byte> columnCache = (ColumnCache<byte>)base.Columns[22];
			MailItemStorage.PendingReasons pendingReasons = (MailItemStorage.PendingReasons)(columnCache.HasValue ? (columnCache.Value & 15) : 0);
			if (set)
			{
				pendingReasons |= reason;
			}
			else
			{
				pendingReasons &= ~reason;
			}
			if (!columnCache.HasValue && pendingReasons != MailItemStorage.PendingReasons.None)
			{
				this.Table.IncrementPendingMessageCount();
			}
			if (columnCache.HasValue && pendingReasons == MailItemStorage.PendingReasons.None)
			{
				this.Table.DecrementPendingMessageCount();
			}
			columnCache.Value = (byte)(pendingReasons | ((MailItemStorage.PendingReasons)columnCache.Value & (MailItemStorage.PendingReasons)(-16)));
			columnCache.HasValue = (pendingReasons != MailItemStorage.PendingReasons.None);
		}

		private const int DsnFormatMask = 15;

		private const int UnderConstructionMask = 64;

		private const int MimeNotSequentialMask = 128;

		private const int SclOffset = 8;

		private const int SclMask = 3840;

		private const int DirectionalityOffset = 12;

		private const int DirectionalityMask = 28672;

		private const int CloneMimeBlockSize = 65536;

		internal static TimeSpan DefaultAsyncCommitTimeout = TimeSpan.FromMilliseconds(250.0);

		private IDataExternalComponent componentRecipients;

		private MimeCache componentMimeCache;

		private XExch50Blob componentXexch50;

		private LazyBytes componentFastIndexBlob;

		private bool isReadOnly;

		private bool retryDeliveryIfRejected;

		private MultiValueHeader transportPropertiesHeader;

		private DeliveryPriority? priority;

		private ExtendedPropertyDictionary componentExtendedProperties;

		private ExtendedPropertyDictionary componentInternalProperties;

		private BlobCollection blobCollection;

		private enum BlobCollectionKeys : byte
		{
			MimeStream = 1,
			MimeMap,
			ExtendedProperties,
			InternalProperties,
			XExch50,
			FastIndex
		}

		[Flags]
		internal enum PendingReasons
		{
			None = 0,
			DiscardPending = 1,
			Active = 2,
			All = 15
		}
	}
}

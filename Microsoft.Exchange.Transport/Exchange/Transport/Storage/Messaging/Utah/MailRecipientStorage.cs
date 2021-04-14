using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Storage.Messaging.Utah
{
	internal class MailRecipientStorage : DataRow, IMailRecipientStorage
	{
		public MailRecipientStorage(DataTable dataTable, long messageId) : this(dataTable)
		{
			this.InitializeDefaults();
			this.MsgId = messageId;
			this.RecipientRowId = ((RecipientTable)dataTable).GetNextRecipientRowId();
			this.AddToActive();
		}

		internal MailRecipientStorage(DataTableCursor cursor) : this(cursor.Table)
		{
			base.LoadFromCurrentRow(cursor);
			if (this.IsActive)
			{
				this.Table.IncrementActiveRecipientCount();
			}
		}

		private MailRecipientStorage(DataTable dataTable) : base(dataTable)
		{
			BlobCollection blobCollection = new BlobCollection(this.Table.Schemas[16], this);
			this.componentExtendedProperties = new ExtendedPropertyDictionary(this, blobCollection, 1);
			this.componentInternalProperties = new ExtendedPropertyDictionary(this, blobCollection, 2);
			base.AddComponent(this.componentExtendedProperties);
			base.AddComponent(this.componentInternalProperties);
		}

		public IExtendedPropertyCollection ExtendedProperties
		{
			get
			{
				return this.componentExtendedProperties;
			}
		}

		public string ORcpt
		{
			get
			{
				return this.ORcptColumn.Value;
			}
			set
			{
				this.ThrowIfDeleted();
				this.ORcptColumn.Value = value;
			}
		}

		public long MsgId
		{
			get
			{
				return this.Table.Generation.CombineIds(this.MessageRowId);
			}
			set
			{
				if (MessagingGeneration.GetGenerationId(value) != this.Table.Generation.GenerationId)
				{
					throw new ArgumentOutOfRangeException("value", "Message generation does not match recipient generation.");
				}
				this.MessageRowId = MessagingGeneration.GetRowId(value);
			}
		}

		public int MessageRowId
		{
			get
			{
				return this.MessageRowIdColumn.Value;
			}
			set
			{
				this.MessageRowIdColumn.Value = value;
				if (this.UndeliveredMessageRowIdColumn.HasValue)
				{
					this.UndeliveredMessageRowIdColumn.Value = this.MessageRowId;
				}
			}
		}

		public long RecipId
		{
			get
			{
				return this.Table.Generation.CombineIds(this.RecipientRowId);
			}
			set
			{
				throw new NotImplementedException("Setting RecipId is deprecated.");
			}
		}

		public int RecipientRowId
		{
			get
			{
				return this.RecipientRowIdColumn.Value;
			}
			protected set
			{
				this.RecipientRowIdColumn.Value = value;
			}
		}

		public AdminActionStatus AdminActionStatus
		{
			get
			{
				return (AdminActionStatus)this.AdminActionStatusColumn.Value;
			}
			set
			{
				this.ThrowIfDeleted();
				this.AdminActionStatusColumn.Value = (byte)value;
			}
		}

		public DsnRequestedFlags DsnRequested
		{
			get
			{
				return (DsnRequestedFlags)this.GetDsn(8);
			}
			set
			{
				this.ThrowIfDeleted();
				this.SetDsn(8, (byte)value);
			}
		}

		public DsnFlags DsnNeeded
		{
			get
			{
				return (DsnFlags)this.GetDsn(0);
			}
			set
			{
				this.ThrowIfDeleted();
				this.SetDsn(0, (byte)value);
			}
		}

		public DsnFlags DsnCompleted
		{
			get
			{
				return (DsnFlags)this.GetDsn(16);
			}
			set
			{
				this.ThrowIfDeleted();
				this.SetDsn(16, (byte)value);
			}
		}

		public Status Status
		{
			get
			{
				return (Status)this.StatusColumn.Value;
			}
			set
			{
				this.ThrowIfDeleted();
				this.StatusColumn.Value = (byte)value;
			}
		}

		public Destination DeliveredDestination
		{
			get
			{
				if (this.deliveredDestination == null && this.DeliveredDestinationColumn.HasValue && this.DeliveredDestinationTypeColumn.HasValue)
				{
					this.deliveredDestination = new Destination((Destination.DestinationType)this.DeliveredDestinationTypeColumn.Value, this.DeliveredDestinationColumn.Value);
				}
				return this.deliveredDestination;
			}
			set
			{
				if (value == null)
				{
					this.ReleaseFromSafetyNet();
					this.DeliveredDestinationColumn.HasValue = false;
					this.DeliveredDestinationTypeColumn.HasValue = false;
					this.deliveredDestination = null;
					return;
				}
				this.deliveredDestination = value;
				this.DeliveredDestinationColumn.Value = value.Blob;
				this.DeliveredDestinationTypeColumn.Value = (byte)value.Type;
			}
		}

		public string PrimaryServerFqdnGuid
		{
			get
			{
				return this.PrimaryServerFqdnGuidColumn.Value;
			}
			set
			{
				this.ThrowIfDeleted();
				this.PrimaryServerFqdnGuidColumn.Value = value;
			}
		}

		public DateTime? DeliveryTime
		{
			get
			{
				if (this.DeliveryTimeColumn.HasValue)
				{
					return new DateTime?(this.DeliveryTimeColumn.Value);
				}
				return null;
			}
			set
			{
				this.ThrowIfDeleted();
				if (value != null)
				{
					this.DeliveryTimeColumn.Value = value.Value;
					return;
				}
				this.ReleaseFromSafetyNet();
				this.DeliveryTimeColumn.HasValue = false;
			}
		}

		public int RetryCount
		{
			get
			{
				return this.RetryCountColumn.Value;
			}
			set
			{
				this.ThrowIfDeleted();
				this.RetryCountColumn.Value = value;
			}
		}

		public string Email
		{
			get
			{
				return this.ToSmtpAddressColumn.Value;
			}
			set
			{
				this.ThrowIfDeleted();
				this.ToSmtpAddressColumn.Value = value;
			}
		}

		public RequiredTlsAuthLevel? TlsAuthLevel
		{
			get
			{
				byte value;
				if (this.componentInternalProperties.TryGetValue<byte>("Microsoft.Exchange.Transport.MailRecipient.RequiredTlsAuthLevel", out value))
				{
					return new RequiredTlsAuthLevel?((RequiredTlsAuthLevel)value);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.componentInternalProperties.SetValue<byte>("Microsoft.Exchange.Transport.MailRecipient.RequiredTlsAuthLevel", (byte)value.Value);
					return;
				}
				this.componentInternalProperties.Remove("Microsoft.Exchange.Transport.MailRecipient.RequiredTlsAuthLevel");
			}
		}

		public int OutboundIPPool
		{
			get
			{
				int result;
				this.componentInternalProperties.TryGetValue<int>("Microsoft.Exchange.Transport.MailRecipient.OutboundIPPool", out result);
				return result;
			}
			set
			{
				if (value > 0)
				{
					int num;
					if (!this.componentInternalProperties.TryGetValue<int>("Microsoft.Exchange.Transport.MailRecipient.OutboundIPPool", out num) || num != value)
					{
						this.componentInternalProperties.SetValue<int>("Microsoft.Exchange.Transport.MailRecipient.OutboundIPPool", value);
						return;
					}
				}
				else
				{
					this.componentInternalProperties.Remove("Microsoft.Exchange.Transport.MailRecipient.OutboundIPPool");
				}
			}
		}

		public bool IsInSafetyNet
		{
			get
			{
				return this.DeliveryTimeOffsetColumn.HasValue && this.DestinationHashColumn.HasValue;
			}
		}

		public bool IsActive
		{
			get
			{
				return !this.IsDeleted && this.UndeliveredMessageRowIdColumn.HasValue;
			}
		}

		public new RecipientTable Table
		{
			get
			{
				return (RecipientTable)base.Table;
			}
		}

		private ColumnCache<int> RecipientRowIdColumn
		{
			get
			{
				return (ColumnCache<int>)base.Columns[0];
			}
		}

		private ColumnCache<int> MessageRowIdColumn
		{
			get
			{
				return (ColumnCache<int>)base.Columns[1];
			}
		}

		private ColumnCache<byte> AdminActionStatusColumn
		{
			get
			{
				return (ColumnCache<byte>)base.Columns[2];
			}
		}

		private ColumnCache<byte> StatusColumn
		{
			get
			{
				return (ColumnCache<byte>)base.Columns[3];
			}
		}

		private ColumnCache<int> DsnColumn
		{
			get
			{
				return (ColumnCache<int>)base.Columns[4];
			}
		}

		private ColumnCache<int> RetryCountColumn
		{
			get
			{
				return (ColumnCache<int>)base.Columns[5];
			}
		}

		private ColumnCache<int> DestinationHashColumn
		{
			get
			{
				return (ColumnCache<int>)base.Columns[6];
			}
		}

		private ColumnCache<int> DeliveryTimeOffsetColumn
		{
			get
			{
				return (ColumnCache<int>)base.Columns[7];
			}
		}

		private ColumnCache<DateTime> DeliveryTimeColumn
		{
			get
			{
				return (ColumnCache<DateTime>)base.Columns[8];
			}
		}

		private ColumnCache<int> UndeliveredMessageRowIdColumn
		{
			get
			{
				return (ColumnCache<int>)base.Columns[9];
			}
		}

		private ColumnCache<byte> DeliveredDestinationTypeColumn
		{
			get
			{
				return (ColumnCache<byte>)base.Columns[10];
			}
		}

		private ColumnCache<byte[]> DeliveredDestinationColumn
		{
			get
			{
				return (ColumnCache<byte[]>)base.Columns[11];
			}
		}

		private ColumnCache<string> ToSmtpAddressColumn
		{
			get
			{
				return (ColumnCache<string>)base.Columns[12];
			}
		}

		private ColumnCache<string> ORcptColumn
		{
			get
			{
				return (ColumnCache<string>)base.Columns[14];
			}
		}

		private ColumnCache<string> PrimaryServerFqdnGuidColumn
		{
			get
			{
				return (ColumnCache<string>)base.Columns[15];
			}
		}

		public static DateTime GetTimeFromOffset(int timeOffset)
		{
			return MailRecipientStorage.TimeOffsetReference + TimeSpan.FromSeconds((double)timeOffset);
		}

		public static int GetTimeOffset(DateTime timeStamp)
		{
			timeStamp = ((timeStamp < MailRecipientStorage.MinDate) ? MailRecipientStorage.MinDate : ((timeStamp > MailRecipientStorage.MaxDate) ? MailRecipientStorage.MaxDate : timeStamp));
			return (int)timeStamp.Subtract(MailRecipientStorage.TimeOffsetReference).TotalSeconds;
		}

		public IMailRecipientStorage MoveTo(long targetMailItemId)
		{
			if (MessagingGeneration.GetGenerationId(this.MsgId) == MessagingGeneration.GetGenerationId(targetMailItemId))
			{
				this.MsgId = targetMailItemId;
				return this;
			}
			if (Components.TransportAppConfig.QueueDatabase.CloneInOriginalGeneration)
			{
				throw new InvalidOperationException("Cannot move recipients between the generations.");
			}
			MailRecipientStorage mailRecipientStorage = (MailRecipientStorage)this.CopyTo(targetMailItemId);
			if (!base.IsNew)
			{
				mailRecipientStorage.SetCloneOrMoveSource(this, false);
				this.ReleaseFromActive();
				this.ReleaseFromSafetyNet();
			}
			return mailRecipientStorage;
		}

		public IMailRecipientStorage CopyTo(long target)
		{
			MailRecipientStorage mailRecipientStorage = (MailRecipientStorage)this.Table.Generation.MessagingDatabase.NewRecipientStorage(target);
			int recipientRowId = mailRecipientStorage.RecipientRowId;
			mailRecipientStorage.Columns.CloneFrom(base.Columns);
			mailRecipientStorage.componentExtendedProperties.CloneFrom(this.componentExtendedProperties);
			mailRecipientStorage.componentInternalProperties.CloneFrom(this.componentInternalProperties);
			mailRecipientStorage.Columns.MarkDirtyForReload();
			mailRecipientStorage.componentExtendedProperties.Dirty = true;
			mailRecipientStorage.componentInternalProperties.Dirty = true;
			mailRecipientStorage.RecipientRowId = recipientRowId;
			mailRecipientStorage.MsgId = target;
			mailRecipientStorage.AddToActive();
			if (mailRecipientStorage.IsInSafetyNet)
			{
				mailRecipientStorage.Table.IncrementSafetyNetRecipientCount(mailRecipientStorage.DeliveredDestination.Type);
			}
			return mailRecipientStorage;
		}

		public new void Commit(TransactionCommitMode commitMode)
		{
			base.Commit(commitMode);
		}

		public new void Materialize(Transaction transaction)
		{
			base.Materialize(transaction);
		}

		public void ReleaseFromActive()
		{
			if (this.IsActive)
			{
				this.UndeliveredMessageRowIdColumn.HasValue = false;
				this.Table.DecrementActiveRecipientCount();
			}
		}

		public override void MarkToDelete()
		{
			this.Table.DecrementRecipientCount();
			if (this.IsActive)
			{
				this.Table.DecrementActiveRecipientCount();
			}
			if (this.IsInSafetyNet)
			{
				this.Table.DecrementSafetyNetRecipientCount(this.DeliveredDestination.Type);
			}
			base.MarkToDelete();
		}

		public void AddToSafetyNet()
		{
			if (this.IsInSafetyNet)
			{
				return;
			}
			if (this.DeliveredDestination == null)
			{
				throw new InvalidOperationException("DeliveredDestination cannot be null.");
			}
			if (this.DeliveryTime == null)
			{
				throw new InvalidOperationException("DeliveryTime cannot be null.");
			}
			this.DeliveryTimeOffsetColumn.Value = MailRecipientStorage.GetTimeOffset(this.DeliveryTime.Value);
			this.DestinationHashColumn.Value = this.DeliveredDestination.GetHashCode();
			this.Table.IncrementSafetyNetRecipientCount(this.DeliveredDestination.Type);
		}

		private void ReleaseFromSafetyNet()
		{
			if (this.IsInSafetyNet)
			{
				this.DeliveryTimeOffsetColumn.HasValue = false;
				this.DestinationHashColumn.HasValue = false;
				this.Table.DecrementSafetyNetRecipientCount(this.DeliveredDestination.Type);
			}
		}

		private void AddToActive()
		{
			if (!this.IsActive)
			{
				this.UndeliveredMessageRowIdColumn.Value = this.MessageRowId;
				this.Table.IncrementActiveRecipientCount();
			}
		}

		private void ThrowIfDeleted()
		{
			if (this.IsDeleted)
			{
				throw new InvalidOperationException("operations not allowed on a deleted recipient");
			}
		}

		private byte GetDsn(byte offset)
		{
			return (byte)((this.DsnColumn.Value & 255 << (int)offset) >> (int)offset);
		}

		private void SetDsn(byte offset, byte value)
		{
			this.DsnColumn.Value = ((this.DsnColumn.Value & ~(255 << (int)offset)) | (int)value << (int)offset);
		}

		private void InitializeDefaults()
		{
			this.Email = string.Empty;
			this.DsnRequested = DsnRequestedFlags.Default;
			this.DsnNeeded = DsnFlags.None;
			this.DsnCompleted = DsnFlags.None;
			this.Status = Status.Ready;
			this.RetryCount = 0;
			this.AdminActionStatus = AdminActionStatus.None;
		}

		private const int DsnNeededOffset = 0;

		private const int DsnRequestedOffset = 8;

		private const int DsnCompletedOffset = 16;

		private static readonly DateTime TimeOffsetReference = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private static readonly DateTime MinDate = MailRecipientStorage.GetTimeFromOffset(int.MinValue);

		private static readonly DateTime MaxDate = MailRecipientStorage.GetTimeFromOffset(int.MaxValue);

		private readonly ExtendedPropertyDictionary componentExtendedProperties;

		private readonly ExtendedPropertyDictionary componentInternalProperties;

		private Destination deliveredDestination;

		private enum BlobCollectionKeys : byte
		{
			ExtendedProperties = 1,
			InternalProperties
		}
	}
}

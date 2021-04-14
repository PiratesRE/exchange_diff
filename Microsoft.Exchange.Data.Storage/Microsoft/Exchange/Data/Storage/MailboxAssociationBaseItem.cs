using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MailboxAssociationBaseItem : Item, IMailboxAssociationBaseItem, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal MailboxAssociationBaseItem(ICoreItem coreItem) : base(coreItem, false)
		{
			if (base.IsNew)
			{
				this.Initialize();
			}
		}

		public string LegacyDN
		{
			get
			{
				this.CheckDisposed("LegacyDN::get");
				return base.GetValueOrDefault<string>(MailboxAssociationBaseSchema.LegacyDN, null);
			}
			set
			{
				this.CheckDisposed("LegacyDN::set");
				this[MailboxAssociationBaseSchema.LegacyDN] = value;
			}
		}

		public string ExternalId
		{
			get
			{
				this.CheckDisposed("ExternalId::get");
				return base.GetValueOrDefault<string>(MailboxAssociationBaseSchema.ExternalId, null);
			}
			set
			{
				this.CheckDisposed("ExternalId::set");
				this[MailboxAssociationBaseSchema.ExternalId] = value;
			}
		}

		public SmtpAddress SmtpAddress
		{
			get
			{
				this.CheckDisposed("SmtpAddress::get");
				string valueOrDefault = base.GetValueOrDefault<string>(MailboxAssociationBaseSchema.SmtpAddress, null);
				if (!string.IsNullOrEmpty(valueOrDefault))
				{
					return new SmtpAddress(valueOrDefault);
				}
				return SmtpAddress.Empty;
			}
			set
			{
				this.CheckDisposed("SmtpAddress::set");
				this[MailboxAssociationBaseSchema.SmtpAddress] = (string)value;
			}
		}

		public bool IsMember
		{
			get
			{
				this.CheckDisposed("IsMember::get");
				return base.GetValueOrDefault<bool>(MailboxAssociationBaseSchema.IsMember, false);
			}
			set
			{
				this.CheckDisposed("IsMember::set");
				this[MailboxAssociationBaseSchema.IsMember] = value;
			}
		}

		public bool ShouldEscalate
		{
			get
			{
				this.CheckDisposed("ShouldEscalate::get");
				return base.GetValueOrDefault<bool>(MailboxAssociationBaseSchema.ShouldEscalate, false);
			}
			set
			{
				this.CheckDisposed("ShouldEscalate::set");
				this[MailboxAssociationBaseSchema.ShouldEscalate] = value;
			}
		}

		public bool IsAutoSubscribed
		{
			get
			{
				this.CheckDisposed("IsAutoSubscribed::get");
				return base.GetValueOrDefault<bool>(MailboxAssociationBaseSchema.IsAutoSubscribed, false);
			}
			set
			{
				this.CheckDisposed("IsAutoSubscribed::set");
				this[MailboxAssociationBaseSchema.IsAutoSubscribed] = value;
			}
		}

		public bool IsPin
		{
			get
			{
				this.CheckDisposed("IsPin::get");
				return base.GetValueOrDefault<bool>(MailboxAssociationBaseSchema.IsPin, false);
			}
			set
			{
				this.CheckDisposed("IsPin::set");
				this[MailboxAssociationBaseSchema.IsPin] = value;
			}
		}

		public ExDateTime JoinDate
		{
			get
			{
				this.CheckDisposed("JoinDate::get");
				return base.GetValueOrDefault<ExDateTime>(MailboxAssociationBaseSchema.JoinDate, default(ExDateTime));
			}
			set
			{
				this.CheckDisposed("JoinDate::set");
				this[MailboxAssociationBaseSchema.JoinDate] = value;
			}
		}

		public string SyncedIdentityHash
		{
			get
			{
				this.CheckDisposed("SyncedIdentityHash::get");
				return base.GetValueOrDefault<string>(MailboxAssociationBaseSchema.SyncedIdentityHash, null);
			}
			set
			{
				this.CheckDisposed("SyncedIdentityHash::set");
				this[MailboxAssociationBaseSchema.SyncedIdentityHash] = value;
			}
		}

		public int CurrentVersion
		{
			get
			{
				this.CheckDisposed("CurrentVersion::get");
				return base.GetValueOrDefault<int>(MailboxAssociationBaseSchema.CurrentVersion, 0);
			}
			set
			{
				this.CheckDisposed("CurrentVersion::set");
				this[MailboxAssociationBaseSchema.CurrentVersion] = value;
			}
		}

		public int SyncedVersion
		{
			get
			{
				this.CheckDisposed("SyncedVersion::get");
				return base.GetValueOrDefault<int>(MailboxAssociationBaseSchema.SyncedVersion, -1);
			}
			set
			{
				this.CheckDisposed("SyncedVersion::set");
				this[MailboxAssociationBaseSchema.SyncedVersion] = value;
			}
		}

		public string LastSyncError
		{
			get
			{
				this.CheckDisposed("LastSyncError::get");
				return base.GetValueOrDefault<string>(MailboxAssociationBaseSchema.LastSyncError, null);
			}
			set
			{
				this.CheckDisposed("LastSyncError::set");
				this[MailboxAssociationBaseSchema.LastSyncError] = value;
			}
		}

		public int SyncAttempts
		{
			get
			{
				this.CheckDisposed("SyncAttempts::get");
				return base.GetValueOrDefault<int>(MailboxAssociationBaseSchema.SyncAttempts, 0);
			}
			set
			{
				this.CheckDisposed("SyncAttempts::set");
				this[MailboxAssociationBaseSchema.SyncAttempts] = value;
			}
		}

		public string SyncedSchemaVersion
		{
			get
			{
				this.CheckDisposed("SyncedSchemaVersion::get");
				return base.GetValueOrDefault<string>(MailboxAssociationBaseSchema.SyncedSchemaVersion, null);
			}
			set
			{
				this.CheckDisposed("SyncedSchemaVersion::set");
				this[MailboxAssociationBaseSchema.SyncedSchemaVersion] = value;
			}
		}

		public abstract string AssociationItemClass { get; }

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return MailboxAssociationBaseSchema.Instance;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			this.AppendDescriptionTo(stringBuilder);
			return stringBuilder.ToString();
		}

		protected void AppendDescriptionTo(StringBuilder buffer)
		{
			ArgumentValidator.ThrowIfNull("buffer", buffer);
			buffer.Append("ItemClass=");
			buffer.Append(this.AssociationItemClass);
			buffer.Append(", Id=");
			buffer.Append(base.Id);
			buffer.Append(", LegacyDN=");
			buffer.Append(this.LegacyDN);
			buffer.Append(", ExternalId=");
			buffer.Append(this.ExternalId);
			buffer.Append(", SmtpAddress=");
			buffer.Append(this.SmtpAddress);
			buffer.Append(", IsMember=");
			buffer.Append(this.IsMember);
			buffer.Append(", ShouldEscalate=");
			buffer.Append(this.ShouldEscalate);
			buffer.Append(", IsAutoSubscribed=");
			buffer.Append(this.IsAutoSubscribed);
			buffer.Append(", IsPin=");
			buffer.Append(this.IsPin);
			buffer.Append(", JoinDate=");
			buffer.Append(this.JoinDate);
			buffer.Append(", CurrentVersion=");
			buffer.Append(this.CurrentVersion);
			buffer.Append(", SyncedVersion=");
			buffer.Append(this.SyncedVersion);
			buffer.Append(", LastSyncError=");
			buffer.Append(this.LastSyncError);
			buffer.Append(", SyncAttempts=");
			buffer.Append(this.SyncAttempts);
			buffer.Append(", SyncedSchemaVersion=");
			buffer.Append(this.SyncedSchemaVersion);
			buffer.Append(", LastModified=");
			buffer.Append(base.GetValueOrDefault<ExDateTime>(InternalSchema.LastModifiedTime, ExDateTime.MinValue));
		}

		private void Initialize()
		{
			this[InternalSchema.ItemClass] = this.AssociationItemClass;
		}
	}
}

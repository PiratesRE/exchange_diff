using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AuditLogSearchItemBase : DisposeTrackableBase
	{
		protected AuditLogSearchItemBase(MailboxSession session, Folder folder)
		{
			this.message = MessageItem.Create(session, folder.Id);
			this.message[StoreObjectSchema.ItemClass] = this.ItemClass;
		}

		protected AuditLogSearchItemBase(MailboxSession session, VersionedId messageId)
		{
			this.message = MessageItem.Bind(session, messageId, this.PropertiesToLoad);
		}

		protected MessageItem Message
		{
			get
			{
				return this.message;
			}
		}

		protected abstract string ItemClass { get; }

		protected abstract PropertyDefinition[] PropertiesToLoad { get; }

		public Guid Identity
		{
			get
			{
				return (Guid)this.Message[AuditLogSearchItemSchema.Identity];
			}
			set
			{
				this.Message[AuditLogSearchItemSchema.Identity] = value;
			}
		}

		public string Name
		{
			get
			{
				return this.Message.GetValueOrDefault<string>(AuditLogSearchItemSchema.Name, string.Empty);
			}
			set
			{
				this.Message[AuditLogSearchItemSchema.Name] = value;
			}
		}

		public VersionedId MessageId
		{
			get
			{
				return this.Message.Id;
			}
		}

		public ExDateTime CreationTime
		{
			get
			{
				return this.Message.CreationTime;
			}
		}

		public ExDateTime StartDate
		{
			get
			{
				return (ExDateTime)this.Message[AuditLogSearchItemSchema.StartDate];
			}
			set
			{
				this.Message[AuditLogSearchItemSchema.StartDate] = value;
			}
		}

		public ExDateTime EndDate
		{
			get
			{
				return (ExDateTime)this.Message[AuditLogSearchItemSchema.EndDate];
			}
			set
			{
				this.Message[AuditLogSearchItemSchema.EndDate] = value;
			}
		}

		public MultiValuedProperty<SmtpAddress> StatusMailRecipients
		{
			get
			{
				return new MultiValuedProperty<SmtpAddress>((from x in (string[])this.Message[AuditLogSearchItemSchema.StatusMailRecipients]
				select new SmtpAddress(x)).ToList<SmtpAddress>());
			}
			set
			{
				this.Message[AuditLogSearchItemSchema.StatusMailRecipients] = (from x in value
				select x.ToString()).ToArray<string>();
			}
		}

		public ADObjectId CreatedByEx
		{
			get
			{
				return new ADObjectId((byte[])this.Message[AuditLogSearchItemSchema.CreatedByEx]);
			}
			set
			{
				this.Message[AuditLogSearchItemSchema.CreatedByEx] = value.GetBytes();
			}
		}

		public string CreatedBy
		{
			get
			{
				return (string)this.Message[AuditLogSearchItemSchema.CreatedBy];
			}
			set
			{
				this.Message[AuditLogSearchItemSchema.CreatedBy] = value;
			}
		}

		public bool? ExternalAccess
		{
			get
			{
				string value = this.Message.TryGetProperty(AuditLogSearchItemSchema.ExternalAccess) as string;
				if (string.IsNullOrEmpty(value))
				{
					return null;
				}
				return new bool?(bool.Parse(value));
			}
			set
			{
				if (value == null)
				{
					this.Message[AuditLogSearchItemSchema.ExternalAccess] = string.Empty;
					return;
				}
				if (value.Value)
				{
					this.Message[AuditLogSearchItemSchema.ExternalAccess] = bool.TrueString;
					return;
				}
				this.Message[AuditLogSearchItemSchema.ExternalAccess] = bool.FalseString;
			}
		}

		public void Save(SaveMode saveMode)
		{
			this.Message.Save(saveMode);
		}

		protected MultiValuedProperty<T> GetPropertiesPossiblyNotFound<T>(StorePropertyDefinition propertyDefinition)
		{
			T[] valueOrDefault = this.Message.GetValueOrDefault<T[]>(propertyDefinition, new T[0]);
			return new MultiValuedProperty<T>(valueOrDefault);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.Message != null)
			{
				this.Message.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AuditLogSearchItemBase>(this);
		}

		private readonly MessageItem message;
	}
}

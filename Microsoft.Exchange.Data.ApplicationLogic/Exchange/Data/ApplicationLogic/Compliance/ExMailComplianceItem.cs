using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Office.CompliancePolicy.ComplianceData;

namespace Microsoft.Exchange.Data.ApplicationLogic.Compliance
{
	internal class ExMailComplianceItem : ComplianceItem
	{
		internal ExMailComplianceItem(MailboxSession session, object[] values)
		{
			if (values[0] != null)
			{
				this.id = (values[0] as StoreId);
			}
			if (values[1] != null && !(values[1] is PropertyError))
			{
				this.whenCreated = ((ExDateTime)values[1]).UniversalTime;
			}
			this.creator = (values[2] as string);
			this.displayName = (values[3] as string);
			if (values[4] != null && !(values[4] is PropertyError))
			{
				this.whenLastModified = ((ExDateTime)values[4]).UniversalTime;
			}
			this.lastModifiedBy = (values[5] as string);
			if (values[6] != null && !(values[6] is PropertyError))
			{
				this.expiryTime = ((ExDateTime)values[6]).UniversalTime;
			}
			else
			{
				this.expiryTime = DateTime.MaxValue;
			}
			this.session = session;
			this.message = null;
		}

		public override string Creator
		{
			get
			{
				return this.creator;
			}
			protected set
			{
				this.creator = value;
				throw new NotImplementedException();
			}
		}

		public override string DisplayName
		{
			get
			{
				return this.displayName;
			}
			protected set
			{
				this.displayName = value;
				throw new NotImplementedException();
			}
		}

		public override string Id
		{
			get
			{
				return this.id.ToString();
			}
			protected set
			{
				throw new NotImplementedException();
			}
		}

		public override DateTime WhenCreated
		{
			get
			{
				return this.whenCreated;
			}
			protected set
			{
				this.whenCreated = value;
				throw new NotImplementedException();
			}
		}

		public override DateTime WhenLastModified
		{
			get
			{
				return this.whenLastModified;
			}
			protected set
			{
				this.whenLastModified = value;
				throw new NotImplementedException();
			}
		}

		public override string LastModifier
		{
			get
			{
				return this.lastModifiedBy;
			}
			protected set
			{
				this.lastModifiedBy = value;
				throw new NotImplementedException();
			}
		}

		public override DateTime ExpiryTime
		{
			get
			{
				return this.expiryTime;
			}
			protected set
			{
				this.expiryTime = value;
				throw new NotImplementedException();
			}
		}

		public override string Extension
		{
			get
			{
				return "ExMessage";
			}
			protected set
			{
				throw new NotImplementedException();
			}
		}

		private void EnsureMessage()
		{
			if (this.message == null)
			{
				this.message = MessageItem.Bind(this.session, this.id, ExMailComplianceItem.MailDataColumns);
			}
		}

		private const int MessageId = 0;

		private const int MessageReceivedTime = 1;

		private const int MessageSenderDisplayName = 2;

		private const int MessageSubject = 3;

		private const int MessageLastModifiedTime = 4;

		private const int MessageLastModifiedBy = 5;

		private const int MessageRetentionDate = 6;

		internal static readonly PropertyDefinition[] MailDataColumns = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.ReceivedTime,
			MessageItemSchema.SenderDisplayName,
			ItemSchema.Subject,
			StoreObjectSchema.LastModifiedTime,
			ItemSchema.LastModifiedBy,
			ItemSchema.RetentionDate
		};

		private StoreId id;

		private string creator;

		private string displayName;

		private DateTime whenCreated;

		private DateTime whenLastModified;

		private string lastModifiedBy;

		private DateTime expiryTime;

		private MailboxSession session;

		private MessageItem message;
	}
}

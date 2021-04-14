using System;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class Message : Item
	{
		public string ParentFolderId
		{
			get
			{
				return (string)base[MessageSchema.ParentFolderId];
			}
			set
			{
				base[MessageSchema.ParentFolderId] = value;
			}
		}

		public string ConversationId
		{
			get
			{
				return (string)base[MessageSchema.ConversationId];
			}
			set
			{
				base[MessageSchema.ConversationId] = value;
			}
		}

		public ItemBody UniqueBody
		{
			get
			{
				return (ItemBody)base[MessageSchema.UniqueBody];
			}
			set
			{
				base[MessageSchema.UniqueBody] = value;
			}
		}

		public Recipient[] ToRecipients
		{
			get
			{
				return (Recipient[])base[MessageSchema.ToRecipients];
			}
			set
			{
				base[MessageSchema.ToRecipients] = value;
			}
		}

		public Recipient[] CcRecipients
		{
			get
			{
				return (Recipient[])base[MessageSchema.CcRecipients];
			}
			set
			{
				base[MessageSchema.CcRecipients] = value;
			}
		}

		public Recipient[] BccRecipients
		{
			get
			{
				return (Recipient[])base[MessageSchema.BccRecipients];
			}
			set
			{
				base[MessageSchema.BccRecipients] = value;
			}
		}

		public Recipient[] ReplyTo
		{
			get
			{
				return (Recipient[])base[MessageSchema.ReplyTo];
			}
			set
			{
				base[MessageSchema.ReplyTo] = value;
			}
		}

		public Recipient Sender
		{
			get
			{
				return (Recipient)base[MessageSchema.Sender];
			}
			set
			{
				base[MessageSchema.Sender] = value;
			}
		}

		public Recipient From
		{
			get
			{
				return (Recipient)base[MessageSchema.From];
			}
			set
			{
				base[MessageSchema.From] = value;
			}
		}

		public bool IsDeliveryReceiptRequested
		{
			get
			{
				return (bool)base[MessageSchema.IsDeliveryReceiptRequested];
			}
			set
			{
				base[MessageSchema.IsDeliveryReceiptRequested] = value;
			}
		}

		public bool IsReadReceiptRequested
		{
			get
			{
				return (bool)base[MessageSchema.IsReadReceiptRequested];
			}
			set
			{
				base[MessageSchema.IsReadReceiptRequested] = value;
			}
		}

		public bool IsRead
		{
			get
			{
				return (bool)base[MessageSchema.IsRead];
			}
			set
			{
				base[MessageSchema.IsRead] = value;
			}
		}

		public bool IsDraft
		{
			get
			{
				return (bool)base[MessageSchema.IsDraft];
			}
			set
			{
				base[MessageSchema.IsDraft] = value;
			}
		}

		public DateTimeOffset DateTimeReceived
		{
			get
			{
				return (DateTimeOffset)base[MessageSchema.DateTimeReceived];
			}
			set
			{
				base[MessageSchema.DateTimeReceived] = value;
			}
		}

		public DateTimeOffset DateTimeSent
		{
			get
			{
				return (DateTimeOffset)base[MessageSchema.DateTimeSent];
			}
			set
			{
				base[MessageSchema.DateTimeSent] = value;
			}
		}

		public string EventId
		{
			get
			{
				return (string)base[MessageSchema.EventId];
			}
			set
			{
				base[MessageSchema.EventId] = value;
			}
		}

		public MeetingMessageType MeetingMessageType
		{
			get
			{
				return (MeetingMessageType)base[MessageSchema.MeetingMessageType];
			}
			set
			{
				base[MessageSchema.MeetingMessageType] = value;
			}
		}

		internal override EntitySchema Schema
		{
			get
			{
				return MessageSchema.SchemaInstance;
			}
		}

		protected override string UserRootNavigationName
		{
			get
			{
				return UserSchema.Messages.Name;
			}
		}

		internal new static readonly EdmEntityType EdmEntityType = new EdmEntityType(typeof(Message).Namespace, typeof(Message).Name, Microsoft.Exchange.Services.OData.Model.Item.EdmEntityType);
	}
}

using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	[Serializable]
	internal class QuarantinedMessage : ConfigurablePropertyBag
	{
		public Guid ExMessageId
		{
			get
			{
				return (Guid)this[QuarantinedMessageSchema.ExMessageIdProperty];
			}
			set
			{
				this[QuarantinedMessageSchema.ExMessageIdProperty] = value;
			}
		}

		public Guid EventId
		{
			get
			{
				return (Guid)this[MessageEventSchema.EventIdProperty];
			}
			set
			{
				this[MessageEventSchema.EventIdProperty] = value;
			}
		}

		public string ClientMessageId
		{
			get
			{
				return (string)this[QuarantinedMessageSchema.ClientMessageIdProperty];
			}
			set
			{
				this[QuarantinedMessageSchema.ClientMessageIdProperty] = value;
			}
		}

		public DateTime Received
		{
			get
			{
				return (DateTime)this[QuarantinedMessageSchema.ReceivedProperty];
			}
			set
			{
				this[QuarantinedMessageSchema.ReceivedProperty] = value;
			}
		}

		public string SenderAddress
		{
			get
			{
				return (string)this[QuarantinedMessageSchema.SenderAddressProperty];
			}
			set
			{
				this[QuarantinedMessageSchema.SenderAddressProperty] = value;
			}
		}

		public string MessageSubject
		{
			get
			{
				return (string)this[QuarantinedMessageSchema.MessageSubjectProperty];
			}
			set
			{
				this[QuarantinedMessageSchema.MessageSubjectProperty] = value;
			}
		}

		public int MessageSize
		{
			get
			{
				return (int)this[QuarantinedMessageSchema.MessageSizeProperty];
			}
			set
			{
				this[QuarantinedMessageSchema.MessageSizeProperty] = value;
			}
		}

		public string MailDirection
		{
			get
			{
				return (string)this[QuarantinedMessageSchema.MailDirectionProperty];
			}
			set
			{
				this[QuarantinedMessageSchema.MailDirectionProperty] = value;
			}
		}

		public string QuarantineType
		{
			get
			{
				return (string)this[QuarantinedMessageSchema.QuarantineTypeProperty];
			}
			set
			{
				this[QuarantinedMessageSchema.QuarantineTypeProperty] = value;
			}
		}

		public DateTime Expires
		{
			get
			{
				return (DateTime)this[QuarantinedMessageSchema.ExpiresProperty];
			}
			set
			{
				this[QuarantinedMessageSchema.ExpiresProperty] = value;
			}
		}

		public bool Notified
		{
			get
			{
				return (bool)(this[QuarantinedMessageSchema.NotifiedProperty] ?? false);
			}
			set
			{
				this[QuarantinedMessageSchema.NotifiedProperty] = value;
			}
		}

		public bool Quarantined
		{
			get
			{
				return (bool)(this[QuarantinedMessageSchema.QuarantinedProperty] ?? false);
			}
			set
			{
				this[QuarantinedMessageSchema.QuarantinedProperty] = value;
			}
		}

		public bool Released
		{
			get
			{
				return (bool)(this[QuarantinedMessageSchema.ReleasedProperty] ?? false);
			}
			set
			{
				this[QuarantinedMessageSchema.ReleasedProperty] = value;
			}
		}

		public bool Reported
		{
			get
			{
				return (bool)(this[QuarantinedMessageSchema.ReportedProperty] ?? false);
			}
			set
			{
				this[QuarantinedMessageSchema.ReportedProperty] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(QuarantinedMessageSchema);
		}
	}
}

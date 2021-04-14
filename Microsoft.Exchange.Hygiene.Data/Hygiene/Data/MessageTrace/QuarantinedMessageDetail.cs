using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class QuarantinedMessageDetail : ConfigurablePropertyBag
	{
		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[QuarantinedMessageDetailSchema.OrganizationalUnitRootProperty];
			}
			set
			{
				this[QuarantinedMessageDetailSchema.OrganizationalUnitRootProperty] = value;
			}
		}

		public Guid ExMessageId
		{
			get
			{
				return (Guid)this[QuarantinedMessageDetailSchema.ExMessageIdProperty];
			}
			set
			{
				this[QuarantinedMessageDetailSchema.ExMessageIdProperty] = value;
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
				return this[QuarantinedMessageDetailSchema.ClientMessageIdProperty] as string;
			}
			set
			{
				this[QuarantinedMessageDetailSchema.ClientMessageIdProperty] = value;
			}
		}

		public DateTime Received
		{
			get
			{
				return (DateTime)this[QuarantinedMessageDetailSchema.ReceivedProperty];
			}
			set
			{
				this[QuarantinedMessageDetailSchema.ReceivedProperty] = value;
			}
		}

		public string MailDirection
		{
			get
			{
				return this[QuarantinedMessageDetailSchema.MailDirectionProperty] as string;
			}
			set
			{
				this[QuarantinedMessageDetailSchema.MailDirectionProperty] = value;
			}
		}

		public int MessageSize
		{
			get
			{
				return (int)this[QuarantinedMessageDetailSchema.MessageSizeProperty];
			}
			set
			{
				this[QuarantinedMessageDetailSchema.MessageSizeProperty] = value;
			}
		}

		public string MessageSubject
		{
			get
			{
				return this[QuarantinedMessageDetailSchema.MessageSubjectProperty] as string;
			}
			set
			{
				this[QuarantinedMessageDetailSchema.MessageSubjectProperty] = value;
			}
		}

		public string QuarantineType
		{
			get
			{
				return this[QuarantinedMessageDetailSchema.QuarantineTypeProperty] as string;
			}
			set
			{
				this[QuarantinedMessageDetailSchema.QuarantineTypeProperty] = value;
			}
		}

		public DateTime Expires
		{
			get
			{
				return (DateTime)this[QuarantinedMessageDetailSchema.ExpiresProperty];
			}
			set
			{
				this[QuarantinedMessageDetailSchema.ExpiresProperty] = value;
			}
		}

		public string PartName
		{
			get
			{
				return this[QuarantinedMessageDetailSchema.PartNameProperty] as string;
			}
			set
			{
				this[QuarantinedMessageDetailSchema.PartNameProperty] = value;
			}
		}

		public string MimeName
		{
			get
			{
				return this[QuarantinedMessageDetailSchema.MimeNameProperty] as string;
			}
			set
			{
				this[QuarantinedMessageDetailSchema.MimeNameProperty] = value;
			}
		}

		public string SenderAddress
		{
			get
			{
				return this[QuarantinedMessageDetailSchema.SenderAddressProperty] as string;
			}
			set
			{
				this[QuarantinedMessageDetailSchema.SenderAddressProperty] = value;
			}
		}

		public string RecipientAddress
		{
			get
			{
				return this[QuarantinedMessageDetailSchema.RecipientAddressProperty] as string;
			}
			set
			{
				this[QuarantinedMessageDetailSchema.RecipientAddressProperty] = value;
			}
		}

		public bool Quarantined
		{
			get
			{
				return (bool)this[QuarantinedMessageDetailSchema.QuarantinedProperty];
			}
			set
			{
				this[QuarantinedMessageDetailSchema.QuarantinedProperty] = value;
			}
		}

		public bool Notified
		{
			get
			{
				return (bool)this[QuarantinedMessageDetailSchema.NotifiedProperty];
			}
			set
			{
				this[QuarantinedMessageDetailSchema.NotifiedProperty] = value;
			}
		}

		public bool Reported
		{
			get
			{
				return (bool)this[QuarantinedMessageDetailSchema.ReportedProperty];
			}
			set
			{
				this[QuarantinedMessageDetailSchema.ReportedProperty] = value;
			}
		}

		public bool Released
		{
			get
			{
				return (bool)this[QuarantinedMessageDetailSchema.ReleasedProperty];
			}
			set
			{
				this[QuarantinedMessageDetailSchema.ReleasedProperty] = value;
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
			return typeof(QuarantinedMessageDetailSchema);
		}
	}
}

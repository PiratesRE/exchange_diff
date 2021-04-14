using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UnsentSpamDigestMessage : ConfigurablePropertyBag
	{
		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[UnsentSpamDigestMessageSchema.OrganizationalUnitRootProperty];
			}
			set
			{
				this[UnsentSpamDigestMessageSchema.OrganizationalUnitRootProperty] = value;
			}
		}

		public Guid ExMessageId
		{
			get
			{
				return (Guid)this[UnsentSpamDigestMessageSchema.ExMessageIdProperty];
			}
			set
			{
				this[UnsentSpamDigestMessageSchema.ExMessageIdProperty] = value;
			}
		}

		public string FromEmailDomain
		{
			get
			{
				return (string)this[UnsentSpamDigestMessageSchema.FromEmailDomainProperty];
			}
			set
			{
				this[UnsentSpamDigestMessageSchema.FromEmailDomainProperty] = value;
			}
		}

		public string FromEmailPrefix
		{
			get
			{
				return (string)this[UnsentSpamDigestMessageSchema.FromEmailPrefixProperty];
			}
			set
			{
				this[UnsentSpamDigestMessageSchema.FromEmailPrefixProperty] = value;
			}
		}

		public string ToEmailDomain
		{
			get
			{
				return (string)this[UnsentSpamDigestMessageSchema.ToEmailDomainProperty];
			}
			set
			{
				this[UnsentSpamDigestMessageSchema.ToEmailDomainProperty] = value;
			}
		}

		public string ToEmailPrefix
		{
			get
			{
				return (string)this[UnsentSpamDigestMessageSchema.ToEmailPrefixProperty];
			}
			set
			{
				this[UnsentSpamDigestMessageSchema.ToEmailPrefixProperty] = value;
			}
		}

		public string SenderName
		{
			get
			{
				return (string)this[UnsentSpamDigestMessageSchema.SenderNameProperty];
			}
			set
			{
				this[UnsentSpamDigestMessageSchema.SenderNameProperty] = value;
			}
		}

		public string RecipientName
		{
			get
			{
				return (string)this[UnsentSpamDigestMessageSchema.RecipientNameProperty];
			}
			set
			{
				this[UnsentSpamDigestMessageSchema.RecipientNameProperty] = value;
			}
		}

		public string Subject
		{
			get
			{
				return (string)this[UnsentSpamDigestMessageSchema.SubjectProperty];
			}
			set
			{
				this[UnsentSpamDigestMessageSchema.SubjectProperty] = value;
			}
		}

		public int MessageSize
		{
			get
			{
				return (int)this[UnsentSpamDigestMessageSchema.MessageSizeProperty];
			}
			set
			{
				this[UnsentSpamDigestMessageSchema.MessageSizeProperty] = value;
			}
		}

		public DateTime DateReceived
		{
			get
			{
				return (DateTime)this[UnsentSpamDigestMessageSchema.DateReceivedProperty];
			}
			set
			{
				this[UnsentSpamDigestMessageSchema.DateReceivedProperty] = value;
			}
		}

		public DateTime? LastNotified
		{
			get
			{
				return (DateTime?)this[UnsentSpamDigestMessageSchema.LastNotifiedProperty];
			}
			set
			{
				this[UnsentSpamDigestMessageSchema.LastNotifiedProperty] = value;
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
			return typeof(UnsentSpamDigestMessageSchema);
		}
	}
}

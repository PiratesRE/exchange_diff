using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	[Serializable]
	internal class QuarantinedMessageRecipient : ConfigurablePropertyBag
	{
		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[QuarantinedMessageRecipientSchema.OrganizationalUnitRootProperty];
			}
			set
			{
				this[QuarantinedMessageRecipientSchema.OrganizationalUnitRootProperty] = value;
			}
		}

		public Guid ExMessageId
		{
			get
			{
				return (Guid)this[QuarantinedMessageRecipientSchema.ExMessageIdProperty];
			}
			set
			{
				this[QuarantinedMessageRecipientSchema.ExMessageIdProperty] = value;
			}
		}

		public string EmailPrefix
		{
			get
			{
				return (string)this[QuarantinedMessageRecipientSchema.EmailPrefixProperty];
			}
			set
			{
				this[QuarantinedMessageRecipientSchema.EmailPrefixProperty] = value;
			}
		}

		public string EmailDomain
		{
			get
			{
				return (string)this[QuarantinedMessageRecipientSchema.EmailDomainProperty];
			}
			set
			{
				this[QuarantinedMessageRecipientSchema.EmailDomainProperty] = value;
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

		public int FssCopyId
		{
			get
			{
				return (int)this[QuarantinedMessageRecipientSchema.FssCopyIdProp];
			}
			set
			{
				this[QuarantinedMessageRecipientSchema.FssCopyIdProp] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new MessageTraceObjectId(this.OrganizationalUnitRoot, this.ExMessageId);
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(QuarantinedMessageRecipientSchema);
		}
	}
}

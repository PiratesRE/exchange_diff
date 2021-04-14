using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class QuarantinedMessageRecipientBatch : ConfigurablePropertyBag
	{
		public QuarantinedMessageRecipientBatch(Guid organizationalUnitRoot)
		{
			this[QuarantinedMessageRecipientBatchSchema.OrganizationalUnitRootProperty] = organizationalUnitRoot;
			this[QuarantinedMessageRecipientBatchSchema.BatchAddressesProperty] = this.batchRecipients;
		}

		public override ObjectId Identity
		{
			get
			{
				return new MessageTraceObjectId((Guid)this[QuarantinedMessageRecipientBatchSchema.OrganizationalUnitRootProperty], Guid.Empty);
			}
		}

		public int FssCopyId
		{
			get
			{
				return (int)this[QuarantinedMessageRecipientBatchSchema.FssCopyIdProp];
			}
			set
			{
				this[QuarantinedMessageRecipientBatchSchema.FssCopyIdProp] = value;
			}
		}

		public void Add(QuarantinedMessageRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (recipient.EmailPrefix == null)
			{
				throw new ArgumentException("recipient.EmailPrefix");
			}
			if (recipient.EmailDomain == null)
			{
				throw new ArgumentException("recipient.EmailDomain");
			}
			Guid identity = CombGuidGenerator.NewGuid();
			this.batchRecipients.AddPropertyValue(identity, QuarantinedMessageRecipientSchema.ExMessageIdProperty, recipient.ExMessageId);
			this.batchRecipients.AddPropertyValue(identity, QuarantinedMessageRecipientSchema.EmailPrefixProperty, recipient.EmailPrefix);
			this.batchRecipients.AddPropertyValue(identity, QuarantinedMessageRecipientSchema.EmailDomainProperty, recipient.EmailDomain);
			this.batchRecipients.AddPropertyValue(identity, QuarantinedMessageRecipientSchema.QuarantinedProperty, recipient.Quarantined);
			this.batchRecipients.AddPropertyValue(identity, QuarantinedMessageRecipientSchema.NotifiedProperty, recipient.Notified);
			this.batchRecipients.AddPropertyValue(identity, QuarantinedMessageRecipientSchema.ReportedProperty, recipient.Reported);
			this.batchRecipients.AddPropertyValue(identity, QuarantinedMessageRecipientSchema.ReleasedProperty, recipient.Released);
		}

		public override Type GetSchemaType()
		{
			return typeof(QuarantinedMessageRecipientBatchSchema);
		}

		private BatchPropertyTable batchRecipients = new BatchPropertyTable();
	}
}

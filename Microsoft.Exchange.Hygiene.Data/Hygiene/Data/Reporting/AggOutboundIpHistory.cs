using System;
using System.Net;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal class AggOutboundIpHistory : ConfigurablePropertyBag
	{
		public Guid TenantId
		{
			get
			{
				return (Guid)this[AggOutboundIPSchema.TenantIdProperty];
			}
			set
			{
				this[AggOutboundIPSchema.TenantIdProperty] = value;
			}
		}

		public IPAddress IPAddress
		{
			get
			{
				return (IPAddress)this[AggOutboundIPSchema.IPAddressProperty];
			}
			set
			{
				this[AggOutboundIPSchema.IPAddressProperty] = value;
			}
		}

		public long SpamMessageCount
		{
			get
			{
				return (long)this[AggOutboundIPSchema.SpamMessageCountProperty];
			}
			set
			{
				this[AggOutboundIPSchema.SpamMessageCountProperty] = value;
			}
		}

		public long TotalMessageCount
		{
			get
			{
				return (long)this[AggOutboundIPSchema.TotalMessageCountProperty];
			}
			set
			{
				this[AggOutboundIPSchema.TotalMessageCountProperty] = value;
			}
		}

		public long SpamRecipientCount
		{
			get
			{
				return (long)this[AggOutboundIPSchema.SpamRecipientCountProperty];
			}
			set
			{
				this[AggOutboundIPSchema.SpamRecipientCountProperty] = value;
			}
		}

		public long TotalRecipientCount
		{
			get
			{
				return (long)this[AggOutboundIPSchema.TotalRecipientCountProperty];
			}
			set
			{
				this[AggOutboundIPSchema.TotalRecipientCountProperty] = value;
			}
		}

		public long NDRSpamMessageCount
		{
			get
			{
				return (long)this[AggOutboundIPSchema.NDRSpamMessageCountProperty];
			}
			set
			{
				this[AggOutboundIPSchema.NDRSpamMessageCountProperty] = value;
			}
		}

		public long NDRTotalMessageCount
		{
			get
			{
				return (long)this[AggOutboundIPSchema.NDRTotalMessageCountProperty];
			}
			set
			{
				this[AggOutboundIPSchema.NDRTotalMessageCountProperty] = value;
			}
		}

		public long NDRSpamRecipientCount
		{
			get
			{
				return (long)this[AggOutboundIPSchema.NDRSpamRecipientCountProperty];
			}
			set
			{
				this[AggOutboundIPSchema.NDRSpamRecipientCountProperty] = value;
			}
		}

		public long NDRTotalRecipientCount
		{
			get
			{
				return (long)this[AggOutboundIPSchema.NDRTotalRecipientCountProperty];
			}
			set
			{
				this[AggOutboundIPSchema.NDRTotalRecipientCountProperty] = value;
			}
		}

		public long UniqueDomainsCount
		{
			get
			{
				return (long)this[AggOutboundIPSchema.UniqueDomainsCountProperty];
			}
			set
			{
				this[AggOutboundIPSchema.UniqueDomainsCountProperty] = value;
			}
		}

		public long NonProvisionedDomainCount
		{
			get
			{
				return (long)this[AggOutboundIPSchema.NonProvisionedDomainCountProperty];
			}
			set
			{
				this[AggOutboundIPSchema.NonProvisionedDomainCountProperty] = value;
			}
		}

		public long UniqueSendersCount
		{
			get
			{
				return (long)this[AggOutboundIPSchema.UniqueSendersCountProperty];
			}
			set
			{
				this[AggOutboundIPSchema.UniqueSendersCountProperty] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.identity.ToString());
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(AggOutboundIPSchema);
		}

		private readonly Guid identity = ReportingSession.GenerateNewId();
	}
}

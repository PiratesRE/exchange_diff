using System;
using System.Net;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal class AggOutboundEmailAddressIpHistory : ConfigurablePropertyBag
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

		public string FromEmailAddress
		{
			get
			{
				return (string)this[AggOutboundIPSchema.FromEmailAddressProperty];
			}
			set
			{
				this[AggOutboundIPSchema.FromEmailAddressProperty] = value;
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

		public long ToSameDomainCount
		{
			get
			{
				return (long)this[AggOutboundIPSchema.ToSameDomainCountProperty];
			}
			set
			{
				this[AggOutboundIPSchema.ToSameDomainCountProperty] = value;
			}
		}

		public long MaxRecipientCount
		{
			get
			{
				return (long)this[AggOutboundIPSchema.MaxRecipientCountProperty];
			}
			set
			{
				this[AggOutboundIPSchema.MaxRecipientCountProperty] = value;
			}
		}

		public long ProvisionedDomainCount
		{
			get
			{
				return (long)this[AggOutboundIPSchema.ProvisionedDomainCountProperty];
			}
			set
			{
				this[AggOutboundIPSchema.ProvisionedDomainCountProperty] = value;
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

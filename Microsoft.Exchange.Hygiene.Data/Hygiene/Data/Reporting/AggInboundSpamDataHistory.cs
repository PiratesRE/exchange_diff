using System;
using System.Net;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal class AggInboundSpamDataHistory : ConfigurablePropertyBag
	{
		public IPAddress IPAddress
		{
			get
			{
				return (IPAddress)this[AggInboundIPSchema.IPAddressProperty];
			}
			set
			{
				this[AggInboundIPSchema.IPAddressProperty] = value;
			}
		}

		public DateTime AggregationDate
		{
			get
			{
				return (DateTime)this[AggInboundIPSchema.AggregationDateProperty];
			}
			set
			{
				this[AggInboundIPSchema.AggregationDateProperty] = value;
			}
		}

		public double SpamPercentage
		{
			get
			{
				return (double)this[AggInboundIPSchema.SpamPercentageProperty];
			}
			set
			{
				this[AggInboundIPSchema.SpamPercentageProperty] = value;
			}
		}

		public long TotalSpam
		{
			get
			{
				return (long)this[AggInboundIPSchema.SpamMessageCountProperty];
			}
			set
			{
				this[AggInboundIPSchema.SpamMessageCountProperty] = value;
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
			return typeof(AggInboundIPSchema);
		}

		private readonly Guid identity = ReportingSession.GenerateNewId();
	}
}

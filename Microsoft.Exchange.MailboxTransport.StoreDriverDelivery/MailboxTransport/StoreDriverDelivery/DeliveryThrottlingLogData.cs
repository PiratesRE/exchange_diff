using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class DeliveryThrottlingLogData
	{
		public ThrottlingScope ThrottlingScope { get; private set; }

		public ThrottlingResource ThrottlingResource { get; private set; }

		public double Threshold { get; private set; }

		public ThrottlingImpactUnits ImpactUnits { get; private set; }

		public uint Impact { get; set; }

		public long Total { get; set; }

		public Guid ExternalOrganizationId { get; private set; }

		public string Recipient { get; private set; }

		public string MDBName { get; private set; }

		public IList<KeyValuePair<string, double>> MDBHealth { get; set; }

		public IList<KeyValuePair<string, string>> CustomData { get; set; }

		public DeliveryThrottlingLogData(ThrottlingScope scope, ThrottlingResource resource, double resourceThreshold, ThrottlingImpactUnits impactUnits, uint impact, long total, Guid externalOrganizationId, string recipient, string mdbName, IList<KeyValuePair<string, double>> mdbHealth, IList<KeyValuePair<string, string>> customData)
		{
			this.ThrottlingScope = scope;
			this.ThrottlingResource = resource;
			this.Threshold = resourceThreshold;
			this.ImpactUnits = impactUnits;
			this.Impact = impact;
			this.Total = total;
			this.ExternalOrganizationId = externalOrganizationId;
			this.Recipient = recipient;
			this.MDBName = mdbName;
			this.MDBHealth = mdbHealth;
			this.CustomData = customData;
		}
	}
}

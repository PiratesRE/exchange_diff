using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.MailboxTransport.Delivery
{
	internal interface IDeliveryThrottlingLog
	{
		event Action<string> TrackSummary;

		bool Enabled { get; }

		void LogSummary(string sequenceNumber, ThrottlingScope scope, ThrottlingResource resource, double resourceThreshold, ThrottlingImpactUnits impactUnits, uint impact, double impactRate, Guid externalOrganizationId, string recipient, string mdbName, IList<KeyValuePair<string, double>> mdbHealth, IList<KeyValuePair<string, string>> customData);
	}
}

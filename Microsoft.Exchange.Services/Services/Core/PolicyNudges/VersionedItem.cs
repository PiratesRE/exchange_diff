using System;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Services.Core.PolicyNudges
{
	internal class VersionedItem : IVersionedItem
	{
		internal VersionedItem(string id, DateTime version)
		{
			this.ID = id;
			this.Version = version;
		}

		public string ID { get; private set; }

		public DateTime Version { get; private set; }
	}
}

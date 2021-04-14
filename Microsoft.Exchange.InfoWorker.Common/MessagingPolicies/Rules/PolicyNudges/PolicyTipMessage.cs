using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules.PolicyNudges
{
	internal sealed class PolicyTipMessage : IVersionedItem
	{
		public PolicyTipMessage(string val, string id, DateTime version)
		{
			this.Value = val;
			this.ID = id;
			this.Version = version;
		}

		public string Value { get; private set; }

		public string ID { get; private set; }

		public DateTime Version { get; private set; }

		internal static readonly PolicyTipMessage Empty = new PolicyTipMessage("empty", string.Empty, DateTime.MinValue);
	}
}

using System;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	public class PolicyConfigChangeEventArgs : EventArgs
	{
		public PolicyConfigChangeEventArgs(PolicyConfigProvider sender, PolicyConfigBase policyConfig, ChangeType changeType)
		{
			ArgumentValidator.ThrowIfNull("sender", sender);
			ArgumentValidator.ThrowIfNull("policyConfig", policyConfig);
			this.Sender = sender;
			this.PolicyConfig = policyConfig;
			this.ChangeType = changeType;
		}

		public ChangeType ChangeType { get; private set; }

		public PolicyConfigBase PolicyConfig { get; private set; }

		public PolicyConfigProvider Sender { get; set; }

		public bool Handled { get; set; }
	}
}

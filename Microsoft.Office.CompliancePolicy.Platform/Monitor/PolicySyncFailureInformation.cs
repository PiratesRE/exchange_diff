using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Monitor
{
	internal class PolicySyncFailureInformation
	{
		public PolicySyncFailureInformation(Guid policyId)
		{
			this.PolicyId = policyId;
			this.ObjectTypes = new HashSet<ConfigurationObjectType>();
		}

		public Guid PolicyId { get; set; }

		public HashSet<ConfigurationObjectType> ObjectTypes { get; set; }

		public Exception LastException { get; set; }

		public override string ToString()
		{
			string text = string.Format("PolicyId={0}", this.PolicyId);
			if (this.ObjectTypes.Any<ConfigurationObjectType>())
			{
				text += string.Format("\r\nFailures:{0}", string.Join<ConfigurationObjectType>(",", this.ObjectTypes));
			}
			return text;
		}
	}
}

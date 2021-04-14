using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Get", "AuditConfigurationPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetAuditConfigurationPolicy : GetCompliancePolicyBase
	{
		public GetAuditConfigurationPolicy() : base(PolicyScenario.AuditSettings)
		{
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			AuditConfigurationPolicy auditConfigurationPolicy = new AuditConfigurationPolicy(dataObject as PolicyStorage);
			base.PopulateDistributionStatus(auditConfigurationPolicy, dataObject as PolicyStorage);
			base.WriteResult(auditConfigurationPolicy);
		}

		protected override void InternalValidate()
		{
			if (this.Identity != null && !AuditPolicyUtility.IsAuditConfigurationPolicy(this.Identity.ToString()))
			{
				base.WriteError(new ArgumentException(Strings.CanOnlyManipulateAuditConfigurationPolicy), ErrorCategory.InvalidArgument, null);
			}
			base.InternalValidate();
		}
	}
}

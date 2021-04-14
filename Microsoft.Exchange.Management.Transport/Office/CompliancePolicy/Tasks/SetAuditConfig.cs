using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Set", "AuditConfig", DefaultParameterSetName = "Identity")]
	public sealed class SetAuditConfig : GetMultitenancySystemConfigurationObjectTask<PolicyIdParameter, PolicyStorage>
	{
		[Parameter(Mandatory = true)]
		public MultiValuedProperty<Workload> Workload { get; set; }

		protected override void InternalProcessRecord()
		{
			Func<ErrorRecord, bool> predicate = (ErrorRecord e) => !(e.Exception is ManagementObjectNotFoundException);
			foreach (Workload workload in AuditConfigUtility.AuditableWorkloads)
			{
				AuditSwitchStatus auditSwitch = this.Workload.Contains(workload) ? AuditSwitchStatus.On : AuditSwitchStatus.Off;
				MultiValuedProperty<AuditableOperations> auditOperations = AuditConfigUtility.GetAuditOperations(workload, auditSwitch);
				IEnumerable<ErrorRecord> errRecords;
				AuditConfigurationRule auditConfigurationRule = AuditConfigUtility.GetAuditConfigurationRule(workload, this.Organization, out errRecords);
				if (AuditConfigUtility.ValidateErrorRecords(this, errRecords, predicate))
				{
					if (auditConfigurationRule != null)
					{
						AuditConfigUtility.SetAuditConfigurationRule(workload, this.Organization, auditOperations, out errRecords);
						AuditConfigUtility.ValidateErrorRecords(this, errRecords);
					}
					else
					{
						AuditConfigurationPolicy auditConfigurationPolicy = AuditConfigUtility.GetAuditConfigurationPolicy(workload, this.Organization, out errRecords);
						if (AuditConfigUtility.ValidateErrorRecords(this, errRecords, predicate))
						{
							if (auditConfigurationPolicy == null)
							{
								auditConfigurationPolicy = AuditConfigUtility.NewAuditConfigurationPolicy(workload, this.Organization, out errRecords);
								if (!AuditConfigUtility.ValidateErrorRecords(this, errRecords))
								{
									continue;
								}
							}
							auditConfigurationRule = AuditConfigUtility.NewAuditConfigurationRule(workload, this.Organization, auditOperations, out errRecords);
							AuditConfigUtility.ValidateErrorRecords(this, errRecords);
						}
					}
				}
			}
		}
	}
}

using System;
using Microsoft.Exchange.Data;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public class AuditConfig : IConfigurable
	{
		public AuditConfig(Workload wl = Workload.None)
		{
			this.Identity = new ConfigObjectId(Guid.NewGuid().ToString());
			this.Workload = wl;
			this.Setting = AuditSwitchStatus.None;
			this.PolicyDistributionStatus = PolicyApplyStatus.Error;
			this.DistributionResults = new MultiValuedProperty<PolicyDistributionErrorDetails>();
		}

		public Workload Workload { get; set; }

		public AuditSwitchStatus Setting { get; set; }

		public PolicyApplyStatus PolicyDistributionStatus { get; set; }

		public MultiValuedProperty<PolicyDistributionErrorDetails> DistributionResults { get; set; }

		public ObjectId Identity { get; private set; }

		public ObjectState ObjectState
		{
			get
			{
				return ObjectState.New;
			}
		}

		public bool IsValid
		{
			get
			{
				return true;
			}
		}

		public ValidationError[] Validate()
		{
			return new ValidationError[0];
		}

		public void CopyChangesFrom(IConfigurable changedObject)
		{
			throw new NotImplementedException();
		}

		public void ResetChangeTracking()
		{
			throw new NotImplementedException();
		}
	}
}

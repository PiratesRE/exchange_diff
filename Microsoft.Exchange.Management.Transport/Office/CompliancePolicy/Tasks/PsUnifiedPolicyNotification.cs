using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public sealed class PsUnifiedPolicyNotification : IConfigurable
	{
		public PsUnifiedPolicyNotification(Workload workload, string identity, IEnumerable<SyncChangeInfo> syncChangeInfos, bool fullSync)
		{
			this.Workload = workload;
			this.Identity = new ConfigObjectId(identity);
			this.SyncChangeInfos = new MultiValuedProperty<string>(from x in syncChangeInfos
			select x.ToString());
			this.FullSync = fullSync;
		}

		public Workload Workload { get; private set; }

		public ObjectId Identity { get; private set; }

		public bool FullSync { get; private set; }

		public MultiValuedProperty<string> SyncChangeInfos { get; private set; }

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

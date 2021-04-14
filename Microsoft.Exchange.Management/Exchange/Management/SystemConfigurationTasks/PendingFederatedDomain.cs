using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class PendingFederatedDomain : IConfigurable
	{
		public ObjectId Identity
		{
			get
			{
				return null;
			}
		}

		public bool IsValid
		{
			get
			{
				return true;
			}
		}

		public ObjectState ObjectState
		{
			get
			{
				return ObjectState.New;
			}
		}

		public SmtpDomain PendingAccountNamespace { get; internal set; }

		public SmtpDomain[] PendingDomains { get; internal set; }

		public PendingFederatedDomain()
		{
		}

		public PendingFederatedDomain(SmtpDomain pendingAccountNamespace, List<SmtpDomain> pendingDomains)
		{
			this.PendingAccountNamespace = pendingAccountNamespace;
			this.PendingDomains = pendingDomains.ToArray();
		}

		public void CopyChangesFrom(IConfigurable source)
		{
		}

		public void ResetChangeTracking()
		{
		}

		public ValidationError[] Validate()
		{
			return new ValidationError[0];
		}
	}
}

using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "PendingFederatedDomain")]
	public sealed class GetPendingFederatedDomain : GetTaskBase<PendingFederatedDomain>
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ADPagedReader<AcceptedDomain> adpagedReader = this.ConfigurationSession.FindPaged<AcceptedDomain>(null, QueryScope.SubTree, null, null, 0);
			SmtpDomain pendingAccountNamespace = null;
			List<SmtpDomain> list = new List<SmtpDomain>();
			foreach (AcceptedDomain acceptedDomain in adpagedReader)
			{
				if (acceptedDomain.PendingFederatedAccountNamespace)
				{
					pendingAccountNamespace = acceptedDomain.DomainName.SmtpDomain;
				}
				else if (acceptedDomain.PendingFederatedDomain)
				{
					list.Add(acceptedDomain.DomainName.SmtpDomain);
				}
			}
			this.WriteResult(new PendingFederatedDomain(pendingAccountNamespace, list));
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			return null;
		}
	}
}

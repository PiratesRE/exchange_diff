using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "PendingFederatedDomain", SupportsShouldProcess = true)]
	public sealed class SetPendingFederatedDomain : SetFederatedOrganizationIdBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetPendingFederationDomain;
			}
		}

		[Parameter]
		public SmtpDomain PendingAccountNamespace
		{
			get
			{
				return base.Fields["PendingAccountNamespace"] as SmtpDomain;
			}
			set
			{
				base.Fields["PendingAccountNamespace"] = value;
			}
		}

		[Parameter]
		public SmtpDomain[] PendingDomains
		{
			get
			{
				return base.Fields["PendingFederatedDomain"] as SmtpDomain[];
			}
			set
			{
				base.Fields["PendingFederatedDomain"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			IEnumerable<AcceptedDomain> enumerable = base.DataSession.FindPaged<AcceptedDomain>(null, base.CurrentOrgContainerId, true, null, 0);
			using (IEnumerator<AcceptedDomain> enumerator = enumerable.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AcceptedDomain acceptedDomain = enumerator.Current;
					bool pendingFederatedAccountNamespace = acceptedDomain.PendingFederatedAccountNamespace;
					bool pendingFederatedDomain = acceptedDomain.PendingFederatedDomain;
					acceptedDomain.PendingFederatedAccountNamespace = false;
					acceptedDomain.PendingFederatedDomain = false;
					if (acceptedDomain.DomainName.SmtpDomain.Equals(this.PendingAccountNamespace))
					{
						acceptedDomain.PendingFederatedAccountNamespace = true;
					}
					if (this.PendingDomains != null)
					{
						if (Array.Exists<SmtpDomain>(this.PendingDomains, (SmtpDomain pendingDomain) => pendingDomain.Equals(acceptedDomain.DomainName.SmtpDomain)))
						{
							acceptedDomain.PendingFederatedDomain = true;
						}
					}
					if (acceptedDomain.PendingFederatedAccountNamespace != pendingFederatedAccountNamespace || acceptedDomain.PendingFederatedDomain != pendingFederatedDomain)
					{
						base.DataSession.Save(acceptedDomain);
					}
				}
			}
			TaskLogger.LogExit();
		}
	}
}

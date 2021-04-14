using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "DefaultAcceptedDomain")]
	public sealed class InstallDefaultAcceptedDomain : NewMultitenancySystemConfigurationObjectTask<AcceptedDomain>
	{
		[Parameter(Mandatory = true)]
		public SmtpDomainWithSubdomains DomainName
		{
			get
			{
				return this.DataObject.DomainName;
			}
			set
			{
				this.DataObject.DomainName = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			AcceptedDomain acceptedDomain = (AcceptedDomain)base.PrepareDataObject();
			acceptedDomain.SetId(this.ConfigurationSession, base.Name);
			return acceptedDomain;
		}

		protected override void InternalProcessRecord()
		{
			if (this.IsAcceptedDomainListEmpty())
			{
				this.DataObject.Default = true;
				this.DataObject.AddressBookEnabled = true;
				base.InternalProcessRecord();
			}
		}

		private bool IsAcceptedDomainListEmpty()
		{
			ADPagedReader<AcceptedDomain> adpagedReader = this.ConfigurationSession.FindPaged<AcceptedDomain>(base.CurrentOrgContainerId, QueryScope.SubTree, null, null, 1);
			return !adpagedReader.GetEnumerator().MoveNext();
		}
	}
}

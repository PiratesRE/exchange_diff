using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ManagementEndpoint
{
	[Cmdlet("Add", "ManagementEndPointHook", SupportsShouldProcess = true)]
	public sealed class AddManagementEndpointHook : ManagementEndpointBase
	{
		[Parameter(Mandatory = true, Position = 0)]
		[ValidateNotNullOrEmpty]
		public Guid ExternalDirectoryOrganizationId { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "Domain")]
		[ValidateNotNullOrEmpty]
		public SmtpDomain DomainName { get; set; }

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "Organization")]
		public AccountPartitionIdParameter AccountPartition { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "Organization")]
		public SmtpDomain PopulateCacheWithDomainName { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "Organization")]
		public string TenantContainerCN { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "Domain")]
		public bool InitialDomain { get; set; }

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.AccountPartition != null)
			{
				this.accountPartitionId = RecipientTaskHelper.ResolvePartitionId(this.AccountPartition, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
		}

		internal override void ProcessRedirectionEntry(IGlobalDirectorySession session)
		{
			if (this.accountPartitionId != null)
			{
				ADForest localForest = ADForest.GetLocalForest();
				session.AddTenant(this.ExternalDirectoryOrganizationId, localForest.Fqdn, this.accountPartitionId.ForestFQDN, ManagementEndpointBase.GetSmtpNextHopDomain(), GlsTenantFlags.None, this.TenantContainerCN);
				if (this.PopulateCacheWithDomainName != null)
				{
					ADAccountPartitionLocator.AddTenantDataToCache(this.ExternalDirectoryOrganizationId, localForest.Fqdn, this.accountPartitionId.ForestFQDN, this.PopulateCacheWithDomainName.Domain, this.TenantContainerCN);
					return;
				}
			}
			else
			{
				session.AddAcceptedDomain(this.ExternalDirectoryOrganizationId, this.DomainName.Domain, this.InitialDomain);
			}
		}

		private const string OrganizationParameterSet = "Organization";

		private const string DomainParameterSet = "Domain";

		private PartitionId accountPartitionId;
	}
}

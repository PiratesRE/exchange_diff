using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ManagementEndpoint
{
	[Cmdlet("Set", "ManagementEndPointHook", SupportsShouldProcess = true)]
	public sealed class SetManagementEndpointHook : ManagementEndpointBase
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, Position = 0)]
		public Guid ExternalDirectoryOrganizationId { get; set; }

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "Domain")]
		public SmtpDomain DomainName { get; set; }

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "Organization")]
		public AccountPartitionIdParameter AccountPartition { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "TenantFlag")]
		public GlsTenantFlags? TenantFlag { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "Organization")]
		public string TenantContainerCN { get; set; }

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.AccountPartition != null)
			{
				PartitionId partitionId;
				Guid guid;
				if (ADAccountPartitionLocator.IsSingleForestTopology(out partitionId) && Guid.TryParse(this.AccountPartition.RawIdentity, out guid) && guid.Equals(ADObjectId.ResourcePartitionGuid))
				{
					this.accountPartitionId = partitionId;
				}
				if (null == this.accountPartitionId)
				{
					this.accountPartitionId = RecipientTaskHelper.ResolvePartitionId(this.AccountPartition, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
			}
		}

		internal override void ProcessRedirectionEntry(IGlobalDirectorySession session)
		{
			if (this.accountPartitionId != null)
			{
				ADForest localForest = ADForest.GetLocalForest();
				session.UpdateTenant(this.ExternalDirectoryOrganizationId, localForest.Fqdn, this.accountPartitionId.ForestFQDN, ManagementEndpointBase.GetSmtpNextHopDomain(), GlsTenantFlags.None, this.TenantContainerCN);
				return;
			}
			if (this.TenantFlag != null)
			{
				session.SetTenantFlag(this.ExternalDirectoryOrganizationId, this.TenantFlag.Value, true);
				return;
			}
			session.UpdateAcceptedDomain(this.ExternalDirectoryOrganizationId, this.DomainName.Domain);
		}

		private const string OrganizationParameterSet = "Organization";

		private const string DomainParameterSet = "Domain";

		private const string TenantFlagParameterSet = "TenantFlag";

		private PartitionId accountPartitionId;
	}
}

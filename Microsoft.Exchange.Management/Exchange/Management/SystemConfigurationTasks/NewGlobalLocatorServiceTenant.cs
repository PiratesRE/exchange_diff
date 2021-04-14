using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "GlobalLocatorServiceTenant", DefaultParameterSetName = "ExternalDirectoryOrganizationIdParameterSet", SupportsShouldProcess = true)]
	public sealed class NewGlobalLocatorServiceTenant : ManageGlobalLocatorServiceBase
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public string ResourceForest
		{
			get
			{
				return (string)base.Fields["ResourceForest"];
			}
			set
			{
				base.Fields["ResourceForest"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public string AccountForest
		{
			get
			{
				return (string)base.Fields["AccountForest"];
			}
			set
			{
				base.Fields["AccountForest"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public string PrimarySite
		{
			get
			{
				return (string)base.Fields["PrimarySite"];
			}
			set
			{
				base.Fields["PrimarySite"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNull]
		public SmtpDomain SmtpNextHopDomain
		{
			get
			{
				return (SmtpDomain)base.Fields["SmtpNextHopDomain"];
			}
			set
			{
				base.Fields["SmtpNextHopDomain"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public GlsTenantFlags TenantFlags
		{
			get
			{
				return (GlsTenantFlags)base.Fields["TenantFlags"];
			}
			set
			{
				base.Fields["TenantFlags"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string TenantContainerCN
		{
			get
			{
				return (string)base.Fields["TenantContainerCN"];
			}
			set
			{
				base.Fields["TenantContainerCN"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				string id;
				if (base.Fields.IsModified("ExternalDirectoryOrganizationId"))
				{
					id = ((Guid)base.Fields["ExternalDirectoryOrganizationId"]).ToString();
				}
				else
				{
					id = ((SmtpDomain)base.Fields["DomainName"]).Domain;
				}
				return Strings.ConfirmationMessageNewGls("Tenant", id);
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			GlobalLocatorServiceTenant globalLocatorServiceTenant = new GlobalLocatorServiceTenant();
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
			globalLocatorServiceTenant.ExternalDirectoryOrganizationId = (Guid)base.Fields["ExternalDirectoryOrganizationId"];
			globalLocatorServiceTenant.ResourceForest = (string)base.Fields["ResourceForest"];
			PartitionId partitionId;
			Exception ex;
			if (!PartitionId.TryParse(globalLocatorServiceTenant.ResourceForest, out partitionId, out ex))
			{
				base.WriteInvalidFqdnError(globalLocatorServiceTenant.ResourceForest);
			}
			globalLocatorServiceTenant.AccountForest = (string)base.Fields["AccountForest"];
			if (!PartitionId.TryParse(globalLocatorServiceTenant.AccountForest, out partitionId, out ex))
			{
				base.WriteInvalidFqdnError(globalLocatorServiceTenant.AccountForest);
			}
			globalLocatorServiceTenant.PrimarySite = (string)base.Fields["PrimarySite"];
			globalLocatorServiceTenant.SmtpNextHopDomain = (SmtpDomain)base.Fields["SmtpNextHopDomain"];
			globalLocatorServiceTenant.TenantContainerCN = (string)base.Fields["TenantContainerCN"];
			if (base.Fields.IsModified("TenantFlags"))
			{
				globalLocatorServiceTenant.TenantFlags = (GlsTenantFlags)base.Fields["TenantFlags"];
			}
			glsDirectorySession.AddTenant(globalLocatorServiceTenant.ExternalDirectoryOrganizationId, globalLocatorServiceTenant.ResourceForest, globalLocatorServiceTenant.AccountForest, globalLocatorServiceTenant.SmtpNextHopDomain.Domain, globalLocatorServiceTenant.TenantFlags, globalLocatorServiceTenant.TenantContainerCN, globalLocatorServiceTenant.PrimarySite);
			base.WriteObject(globalLocatorServiceTenant);
		}
	}
}

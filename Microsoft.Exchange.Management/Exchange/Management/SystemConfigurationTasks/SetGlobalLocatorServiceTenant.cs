using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "GlobalLocatorServiceTenant", DefaultParameterSetName = "ExternalDirectoryOrganizationIdParameterSet", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class SetGlobalLocatorServiceTenant : ManageGlobalLocatorServiceBase
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, ParameterSetName = "DomainNameParameterSet")]
		[ValidateNotNull]
		public SmtpDomain DomainName
		{
			get
			{
				return (SmtpDomain)base.Fields["DomainName"];
			}
			set
			{
				base.Fields["DomainName"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "ExternalDirectoryOrganizationIdParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "DomainNameParameterSet")]
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

		[Parameter(Mandatory = false, ParameterSetName = "ExternalDirectoryOrganizationIdParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "DomainNameParameterSet")]
		[ValidateNotNullOrEmpty]
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

		[Parameter(Mandatory = false, ParameterSetName = "ExternalDirectoryOrganizationIdParameterSet")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "DomainNameParameterSet")]
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

		[ValidateNotNull]
		[Parameter(Mandatory = false, ParameterSetName = "DomainNameParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "ExternalDirectoryOrganizationIdParameterSet")]
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

		[Parameter(Mandatory = false, ParameterSetName = "ExternalDirectoryOrganizationIdParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "DomainNameParameterSet")]
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

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "DomainNameParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "ExternalDirectoryOrganizationIdParameterSet")]
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

		protected override void InternalValidate()
		{
			base.InternalValidate();
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
				return Strings.ConfirmationMessageSetGls("Tenant", id);
			}
		}

		protected override void InternalProcessRecord()
		{
			GlobalLocatorServiceTenant globalLocatorServiceTenant = new GlobalLocatorServiceTenant();
			GlobalLocatorServiceTenant oldGlsTenant = new GlobalLocatorServiceTenant();
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
			if (base.Fields.IsModified("ExternalDirectoryOrganizationId"))
			{
				Guid guid = (Guid)base.Fields["ExternalDirectoryOrganizationId"];
				if (!glsDirectorySession.TryGetTenantInfoByOrgGuid(guid, out oldGlsTenant))
				{
					base.WriteGlsTenantNotFoundError(guid);
				}
			}
			else
			{
				SmtpDomain smtpDomain = (SmtpDomain)base.Fields["DomainName"];
				if (!glsDirectorySession.TryGetTenantInfoByDomain(smtpDomain.Domain, out oldGlsTenant))
				{
					base.WriteGlsDomainNotFoundError(smtpDomain.Domain);
				}
			}
			globalLocatorServiceTenant = this.GetUpdatedGLSTenant(oldGlsTenant);
			glsDirectorySession.UpdateTenant(globalLocatorServiceTenant.ExternalDirectoryOrganizationId, globalLocatorServiceTenant.ResourceForest, globalLocatorServiceTenant.AccountForest, globalLocatorServiceTenant.SmtpNextHopDomain.Domain, globalLocatorServiceTenant.TenantFlags, globalLocatorServiceTenant.TenantContainerCN, globalLocatorServiceTenant.PrimarySite);
		}

		private GlobalLocatorServiceTenant GetUpdatedGLSTenant(GlobalLocatorServiceTenant oldGlsTenant)
		{
			GlobalLocatorServiceTenant globalLocatorServiceTenant = new GlobalLocatorServiceTenant();
			globalLocatorServiceTenant.ExternalDirectoryOrganizationId = oldGlsTenant.ExternalDirectoryOrganizationId;
			globalLocatorServiceTenant.DomainNames = oldGlsTenant.DomainNames;
			if (base.Fields.IsModified("ResourceForest"))
			{
				globalLocatorServiceTenant.ResourceForest = (string)base.Fields["ResourceForest"];
				PartitionId partitionId;
				Exception ex;
				if (!PartitionId.TryParse(globalLocatorServiceTenant.ResourceForest, out partitionId, out ex))
				{
					base.WriteInvalidFqdnError(globalLocatorServiceTenant.ResourceForest);
				}
			}
			else
			{
				globalLocatorServiceTenant.ResourceForest = oldGlsTenant.ResourceForest;
			}
			if (base.Fields.IsModified("AccountForest"))
			{
				globalLocatorServiceTenant.AccountForest = (string)base.Fields["AccountForest"];
				PartitionId partitionId2;
				Exception ex2;
				if (!PartitionId.TryParse(globalLocatorServiceTenant.AccountForest, out partitionId2, out ex2))
				{
					base.WriteInvalidFqdnError(globalLocatorServiceTenant.AccountForest);
				}
			}
			else
			{
				globalLocatorServiceTenant.AccountForest = oldGlsTenant.AccountForest;
			}
			if (base.Fields.IsModified("PrimarySite"))
			{
				globalLocatorServiceTenant.PrimarySite = (string)base.Fields["PrimarySite"];
			}
			else
			{
				globalLocatorServiceTenant.PrimarySite = oldGlsTenant.PrimarySite;
			}
			if (base.Fields.IsModified("SmtpNextHopDomain"))
			{
				globalLocatorServiceTenant.SmtpNextHopDomain = (SmtpDomain)base.Fields["SmtpNextHopDomain"];
			}
			else
			{
				globalLocatorServiceTenant.SmtpNextHopDomain = oldGlsTenant.SmtpNextHopDomain;
			}
			if (base.Fields.IsModified("TenantFlags"))
			{
				globalLocatorServiceTenant.TenantFlags = (GlsTenantFlags)base.Fields["TenantFlags"];
			}
			else
			{
				globalLocatorServiceTenant.TenantFlags = oldGlsTenant.TenantFlags;
			}
			if (base.Fields.IsModified("TenantContainerCN"))
			{
				globalLocatorServiceTenant.TenantContainerCN = (string)base.Fields["TenantContainerCN"];
			}
			else
			{
				globalLocatorServiceTenant.TenantContainerCN = oldGlsTenant.TenantContainerCN;
			}
			return globalLocatorServiceTenant;
		}
	}
}

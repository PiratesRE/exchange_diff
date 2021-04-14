using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "GlobalLocatorServiceTenant", DefaultParameterSetName = "ExternalDirectoryOrganizationIdParameterSet", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveGlobalLocatorServiceTenant : ManageGlobalLocatorServiceBase
	{
		[ValidateNotNull]
		[Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, ParameterSetName = "DomainNameParameterSet")]
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
				return Strings.ConfirmationMessageRemoveGls("Tenant", id);
			}
		}

		protected override void InternalProcessRecord()
		{
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
			Guid guid = Guid.Empty;
			if (base.Fields.IsModified("ExternalDirectoryOrganizationId"))
			{
				guid = (Guid)base.Fields["ExternalDirectoryOrganizationId"];
				GlobalLocatorServiceTenant globalLocatorServiceTenant;
				if (!glsDirectorySession.TryGetTenantInfoByOrgGuid(guid, out globalLocatorServiceTenant))
				{
					base.WriteGlsTenantNotFoundError(guid);
				}
			}
			else
			{
				SmtpDomain smtpDomain = (SmtpDomain)base.Fields["DomainName"];
				GlobalLocatorServiceTenant globalLocatorServiceTenant;
				if (!glsDirectorySession.TryGetTenantInfoByDomain(smtpDomain.Domain, out globalLocatorServiceTenant))
				{
					base.WriteGlsDomainNotFoundError(smtpDomain.Domain);
				}
				guid = globalLocatorServiceTenant.ExternalDirectoryOrganizationId;
			}
			glsDirectorySession.RemoveTenant(guid);
		}
	}
}

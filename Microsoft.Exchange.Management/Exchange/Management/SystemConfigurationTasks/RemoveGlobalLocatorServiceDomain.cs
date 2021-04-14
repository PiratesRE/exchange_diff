using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "GlobalLocatorServiceDomain", DefaultParameterSetName = "DomainNameParameterSet", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveGlobalLocatorServiceDomain : ManageGlobalLocatorServiceBase
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

		private new Guid ExternalDirectoryOrganizationId
		{
			get
			{
				return (Guid)base.Fields["ExternalDirectoryOrganizationId"];
			}
			set
			{
				base.Fields["ExternalDirectoryOrganizationId"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				string domain = ((SmtpDomain)base.Fields["DomainName"]).Domain;
				return Strings.ConfirmationMessageRemoveGls("Domain", domain);
			}
		}

		protected override void InternalProcessRecord()
		{
			GlobalLocatorServiceDomain globalLocatorServiceDomain = new GlobalLocatorServiceDomain();
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
			SmtpDomain smtpDomain = (SmtpDomain)base.Fields["DomainName"];
			if (!glsDirectorySession.TryGetTenantDomainFromDomainFqdn(smtpDomain.Domain, out globalLocatorServiceDomain, true))
			{
				base.WriteGlsDomainNotFoundError(smtpDomain.Domain);
			}
			glsDirectorySession.RemoveAcceptedDomain(globalLocatorServiceDomain.ExternalDirectoryOrganizationId, smtpDomain.Domain, true);
		}
	}
}

using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "GlobalLocatorServiceDomain", DefaultParameterSetName = "DomainNameParameterSet", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class GetGlobalLocatorServiceDomain : ManageGlobalLocatorServiceBase
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

		[Parameter(Mandatory = false, ParameterSetName = "DomainNameParameterSet")]
		public SwitchParameter UseOfflineGLS
		{
			get
			{
				return (SwitchParameter)(base.Fields["UseOfflineGLS"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UseOfflineGLS"] = value;
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

		protected override void InternalProcessRecord()
		{
			GlobalLocatorServiceDomain sendToPipeline = new GlobalLocatorServiceDomain();
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
			SmtpDomain smtpDomain = (SmtpDomain)base.Fields["DomainName"];
			if (this.UseOfflineGLS)
			{
				if (!glsDirectorySession.TryGetTenantDomainFromDomainFqdn(smtpDomain.Domain, out sendToPipeline, true, GlsCacheServiceMode.CacheOnly))
				{
					base.WriteGlsDomainNotFoundError(smtpDomain.Domain);
				}
			}
			else if (!glsDirectorySession.TryGetTenantDomainFromDomainFqdn(smtpDomain.Domain, out sendToPipeline, true))
			{
				base.WriteGlsDomainNotFoundError(smtpDomain.Domain);
			}
			base.WriteObject(sendToPipeline);
		}
	}
}

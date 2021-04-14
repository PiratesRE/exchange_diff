using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "GlobalLocatorServiceDomain", DefaultParameterSetName = "DomainNameParameterSet", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class SetGlobalLocatorServiceDomain : ManageGlobalLocatorServiceBase
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
		[ValidateNotNull]
		public GlsDomainFlags DomainFlags
		{
			get
			{
				return (GlsDomainFlags)base.Fields["DomainFlags"];
			}
			set
			{
				base.Fields["DomainFlags"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DomainNameParameterSet")]
		[ValidateNotNull]
		public bool DomainInUse
		{
			get
			{
				return (bool)base.Fields["DomainInUse"];
			}
			set
			{
				base.Fields["DomainInUse"] = value;
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
				string id;
				if (base.Fields.IsModified("ExternalDirectoryOrganizationId"))
				{
					id = ((Guid)base.Fields["ExternalDirectoryOrganizationId"]).ToString();
				}
				else
				{
					id = ((SmtpDomain)base.Fields["DomainName"]).Domain;
				}
				return Strings.ConfirmationMessageSetGls("Domain", id);
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
			List<KeyValuePair<DomainProperty, PropertyValue>> list = new List<KeyValuePair<DomainProperty, PropertyValue>>();
			if (base.Fields.IsModified("DomainInUse"))
			{
				list.Add(new KeyValuePair<DomainProperty, PropertyValue>(DomainProperty.ExoDomainInUse, new PropertyValue((bool)base.Fields["DomainInUse"])));
			}
			if (base.Fields.IsModified("DomainFlags"))
			{
				list.Add(new KeyValuePair<DomainProperty, PropertyValue>(DomainProperty.ExoFlags, new PropertyValue((int)base.Fields["DomainFlags"])));
			}
			glsDirectorySession.UpdateAcceptedDomain(globalLocatorServiceDomain.ExternalDirectoryOrganizationId, globalLocatorServiceDomain.DomainName.Domain, list.ToArray());
		}
	}
}

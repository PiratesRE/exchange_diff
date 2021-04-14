using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "GlobalLocatorServiceDomain", DefaultParameterSetName = "ExternalDirectoryOrganizationIdParameterSet", SupportsShouldProcess = true)]
	public sealed class NewGlobalLocatorServiceDomain : ManageGlobalLocatorServiceBase
	{
		[ValidateNotNull]
		[Parameter(Mandatory = true)]
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

		[Parameter(Mandatory = false)]
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

		[ValidateNotNull]
		[Parameter(Mandatory = true)]
		public DomainKeyType DomainType
		{
			get
			{
				return (DomainKeyType)base.Fields["DomainType"];
			}
			set
			{
				base.Fields["DomainType"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false)]
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
				return Strings.ConfirmationMessageNewGls("Domain", id);
			}
		}

		protected override void InternalProcessRecord()
		{
			GlobalLocatorServiceDomain globalLocatorServiceDomain = new GlobalLocatorServiceDomain();
			List<KeyValuePair<DomainProperty, PropertyValue>> list = new List<KeyValuePair<DomainProperty, PropertyValue>>();
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
			BitVector32 bitVector = default(BitVector32);
			bitVector[1] = false;
			globalLocatorServiceDomain.ExternalDirectoryOrganizationId = (Guid)base.Fields["ExternalDirectoryOrganizationId"];
			globalLocatorServiceDomain.DomainName = (SmtpDomain)base.Fields["DomainName"];
			globalLocatorServiceDomain.DomainInUse = (base.Fields.IsModified("DomainInUse") && (bool)base.Fields["DomainInUse"]);
			list.Add(new KeyValuePair<DomainProperty, PropertyValue>(DomainProperty.ExoDomainInUse, new PropertyValue(globalLocatorServiceDomain.DomainInUse)));
			if (base.Fields.IsModified("DomainFlags"))
			{
				globalLocatorServiceDomain.DomainFlags = new GlsDomainFlags?((GlsDomainFlags)base.Fields["DomainFlags"]);
				if ((globalLocatorServiceDomain.DomainFlags & GlsDomainFlags.Nego2Enabled) == GlsDomainFlags.Nego2Enabled)
				{
					bitVector[1] = true;
				}
				if ((globalLocatorServiceDomain.DomainFlags & GlsDomainFlags.OAuth2ClientProfileEnabled) == GlsDomainFlags.OAuth2ClientProfileEnabled)
				{
					bitVector[2] = true;
				}
			}
			list.Add(new KeyValuePair<DomainProperty, PropertyValue>(DomainProperty.ExoFlags, new PropertyValue(bitVector.Data)));
			glsDirectorySession.AddAcceptedDomain(globalLocatorServiceDomain.ExternalDirectoryOrganizationId, globalLocatorServiceDomain.DomainName.Domain, (DomainKeyType)base.Fields["DomainType"], list.ToArray());
			base.WriteObject(globalLocatorServiceDomain);
		}
	}
}

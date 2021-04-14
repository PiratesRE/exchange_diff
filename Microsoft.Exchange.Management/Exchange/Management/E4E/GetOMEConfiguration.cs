using System;
using System.Management.Automation;
using System.Web;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.E4E
{
	[Cmdlet("Get", "OMEConfiguration", DefaultParameterSetName = "Identity")]
	public sealed class GetOMEConfiguration : GetTenantADObjectWithIdentityTaskBase<OMEConfigurationIdParameter, EncryptionConfiguration>
	{
		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (this.Organization != null)
			{
				this.SetCurrentOrganizationId();
			}
			if (base.CurrentOrganizationId == null || base.CurrentOrganizationId.OrganizationalUnit == null || string.IsNullOrWhiteSpace(base.CurrentOrganizationId.OrganizationalUnit.Name))
			{
				base.WriteError(new LocalizedException(Strings.ErrorParameterRequired("Organization")), ErrorCategory.InvalidArgument, null);
			}
			string organizationRawIdentity;
			if (this.Organization == null)
			{
				organizationRawIdentity = base.CurrentOrganizationId.OrganizationalUnit.Name;
			}
			else
			{
				organizationRawIdentity = this.Organization.RawIdentity;
			}
			return new EncryptionConfigurationDataProvider(organizationRawIdentity);
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			EncryptionConfiguration encryptionConfiguration = (EncryptionConfiguration)dataObject;
			encryptionConfiguration.EmailText = HttpUtility.HtmlDecode(encryptionConfiguration.EmailText);
			encryptionConfiguration.PortalText = HttpUtility.HtmlDecode(encryptionConfiguration.PortalText);
			encryptionConfiguration.DisclaimerText = HttpUtility.HtmlDecode(encryptionConfiguration.DisclaimerText);
			base.WriteResult(encryptionConfiguration);
		}

		private void SetCurrentOrganizationId()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, 97, "SetCurrentOrganizationId", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\E4E\\GetEncryptionConfiguration.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())), ExchangeErrorCategory.Client);
			base.CurrentOrganizationId = adorganizationalUnit.OrganizationId;
		}
	}
}

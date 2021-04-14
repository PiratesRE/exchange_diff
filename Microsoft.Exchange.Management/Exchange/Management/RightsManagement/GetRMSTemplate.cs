using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.RightsManagement;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[Cmdlet("Get", "RMSTemplate", DefaultParameterSetName = "OrganizationSet")]
	public sealed class GetRMSTemplate : GetTenantADObjectWithIdentityTaskBase<RmsTemplateIdParameter, RmsTemplatePresentation>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ValueFromPipeline = true, ParameterSetName = "OrganizationSet")]
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

		[Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override RmsTemplateIdParameter Identity
		{
			get
			{
				return (RmsTemplateIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RmsTrustedPublishingDomainIdParameter TrustedPublishingDomain
		{
			get
			{
				return (RmsTrustedPublishingDomainIdParameter)base.Fields["TrustedPublishingDomain"];
			}
			set
			{
				base.Fields["TrustedPublishingDomain"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RmsTemplateType Type
		{
			get
			{
				return (RmsTemplateType)base.Fields["Type"];
			}
			set
			{
				base.Fields["Type"] = value;
			}
		}

		protected override Unlimited<uint> DefaultResultSize
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 98, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\rms\\GetRMSTemplate.cs");
			RmsTemplateType typeToFetch = (this.TrustedPublishingDomain != null) ? RmsTemplateType.All : RmsTemplateType.Distributed;
			bool flag = false;
			if (base.Fields.IsModified("Type"))
			{
				typeToFetch = this.Type;
				flag = true;
			}
			RMSTrustedPublishingDomain rmstrustedPublishingDomain = null;
			if (this.TrustedPublishingDomain != null)
			{
				rmstrustedPublishingDomain = (RMSTrustedPublishingDomain)base.GetDataObject<RMSTrustedPublishingDomain>(this.TrustedPublishingDomain, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorTrustedPublishingDomainNotFound(this.TrustedPublishingDomain.ToString())), new LocalizedString?(Strings.ErrorTrustedPublishingDomainNotUnique(this.TrustedPublishingDomain.ToString())));
			}
			return new RmsTemplateDataProvider(tenantOrTopologyConfigurationSession, typeToFetch, flag || rmstrustedPublishingDomain != null, rmstrustedPublishingDomain);
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, ConfigScopes.TenantSubTree, 153, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\rms\\GetRMSTemplate.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				return adorganizationalUnit.OrganizationId;
			}
			return base.ResolveCurrentOrganization();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || RmsUtil.IsKnownException(exception);
		}
	}
}

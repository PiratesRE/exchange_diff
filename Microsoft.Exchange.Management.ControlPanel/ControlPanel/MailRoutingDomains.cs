using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class MailRoutingDomains : DataSourceService, IMailRoutingDomains, IGetListService<MailRoutingDomainFilter, MailRoutingDomain>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectService<MailRoutingDomain, SetMailRoutingDomain>, IGetObjectService<MailRoutingDomain>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-AcceptedDomain@C:OrganizationConfig")]
		public PowerShellResults<MailRoutingDomain> GetList(MailRoutingDomainFilter filter, SortOptions sort)
		{
			return base.GetList<MailRoutingDomain, MailRoutingDomainFilter>("Get-AcceptedDomain", filter, sort);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-AcceptedDomain?Identity@C:OrganizationConfig")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters wsParameters)
		{
			return base.RemoveObjects("Remove-AcceptedDomain", identities, wsParameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-AcceptedDomain?Identity@C:OrganizationConfig+Set-AcceptedDomain?Identity@C:OrganizationConfig")]
		public PowerShellResults<MailRoutingDomain> SetObject(Identity identity, SetMailRoutingDomain properties)
		{
			return base.SetObject<MailRoutingDomain, SetMailRoutingDomain>("Set-AcceptedDomain", identity, properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-AcceptedDomain?Identity@C:OrganizationConfig")]
		public PowerShellResults<MailRoutingDomain> GetObject(Identity identity)
		{
			return base.GetObject<MailRoutingDomain>("Get-AcceptedDomain", identity);
		}

		private const string Noun = "AcceptedDomain";

		internal const string GetCmdlet = "Get-AcceptedDomain";

		internal const string SetCmdlet = "Set-AcceptedDomain";

		private const string RemoveCmdlet = "Remove-AcceptedDomain";

		internal const string ReadScope = "@C:OrganizationConfig";

		private const string WriteScope = "@C:OrganizationConfig";

		private const string GetListRole = "Get-AcceptedDomain@C:OrganizationConfig";

		private const string RemoveObjectsRole = "Remove-AcceptedDomain?Identity@C:OrganizationConfig";

		private const string SetObjectRole = "Get-AcceptedDomain?Identity@C:OrganizationConfig+Set-AcceptedDomain?Identity@C:OrganizationConfig";

		private const string GetObjectRole = "Get-AcceptedDomain?Identity@C:OrganizationConfig";
	}
}

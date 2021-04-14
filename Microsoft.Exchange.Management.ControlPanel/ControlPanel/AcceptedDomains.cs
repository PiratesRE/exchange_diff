using System;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class AcceptedDomains : DataSourceService, IAcceptedDomains, IGetListService<AcceptedDomainFilter, AcceptedDomain>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-AcceptedDomain@C:OrganizationConfig")]
		public PowerShellResults<AcceptedDomain> GetList(AcceptedDomainFilter filter, SortOptions sort)
		{
			PowerShellResults<AcceptedDomain> list = base.GetList<AcceptedDomain, AcceptedDomainFilter>("Get-AcceptedDomain", filter, sort);
			if (list.HasValue)
			{
				list.Output = (from x in list.Output
				where !((AcceptedDomain)x.ConfigurationObject).DomainName.IsStar
				select x).ToArray<AcceptedDomain>();
			}
			return list;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-AcceptedDomain@C:OrganizationConfig")]
		public PowerShellResults<AcceptedDomain> GetManagedDomains(AcceptedDomainFilter filter, SortOptions sort)
		{
			PowerShellResults<AcceptedDomain> list = this.GetList(filter, sort);
			if (list.Failed)
			{
				return list;
			}
			AcceptedDomain[] output = (from x in list.Output
			where x.AuthenticationType != AuthenticationType.Federated
			select x).ToArray<AcceptedDomain>();
			list.Output = output;
			return list;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-AcceptedDomain@C:OrganizationConfig")]
		public PowerShellResults<AcceptedDomain> GetAcceptedDomainsWithOutExternalRelay(AcceptedDomainFilter filter, SortOptions sort)
		{
			PowerShellResults<AcceptedDomain> list = this.GetList(filter, sort);
			if (list.Failed)
			{
				return list;
			}
			list.Output = (from x in list.Output
			where x.DomainType != AcceptedDomainType.ExternalRelay
			select x).ToArray<AcceptedDomain>();
			return list;
		}

		private const string Noun = "AcceptedDomain";

		internal const string GetCmdlet = "Get-AcceptedDomain";

		internal const string ReadScope = "@C:OrganizationConfig";

		private const string GetListRole = "Get-AcceptedDomain@C:OrganizationConfig";
	}
}

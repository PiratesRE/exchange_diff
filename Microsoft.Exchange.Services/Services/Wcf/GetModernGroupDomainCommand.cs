using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class GetModernGroupDomainCommand : ServiceCommand<GetModernGroupDomainResponse>
	{
		public GetModernGroupDomainCommand(CallContext callContext) : base(callContext)
		{
		}

		protected override GetModernGroupDomainResponse InternalExecute()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(base.CallContext.AccessingPrincipal.MailboxInfo.OrganizationId);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, sessionSettings, 36, "InternalExecute", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\jsonservice\\servicecommands\\GetModernGroupDomainCommand.cs");
			return new GetModernGroupDomainResponse(tenantOrTopologyConfigurationSession.GetDefaultAcceptedDomain().DomainName.Domain);
		}
	}
}

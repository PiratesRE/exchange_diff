using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Get", "MailboxJunkEmailConfiguration")]
	public sealed class GetMailboxJunkEmailConfiguration : GetXsoObjectWithIdentityTaskBase<MailboxJunkEmailConfiguration, ADUser>
	{
		internal override IConfigDataProvider CreateXsoMailboxDataProvider(ExchangePrincipal principal, ISecurityAccessToken userToken)
		{
			return new MailboxJunkEmailConfigurationDataProvider(principal, base.TenantGlobalCatalogSession, "Get-MailboxJunkEmailConfiguration");
		}
	}
}

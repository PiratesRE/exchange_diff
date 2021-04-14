using System;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class RbacPrincipalExtensions
	{
		public static bool IsFederatedUser(this RbacPrincipal rbacPrincipal)
		{
			bool result = false;
			ExchangeRunspaceConfiguration rbacConfiguration = rbacPrincipal.RbacConfiguration;
			SmtpAddress executingUserPrimarySmtpAddress = rbacConfiguration.ExecutingUserPrimarySmtpAddress;
			if (!executingUserPrimarySmtpAddress.IsValidAddress)
			{
				string executingUserPrincipalName = rbacConfiguration.ExecutingUserPrincipalName;
				if (!string.IsNullOrEmpty(executingUserPrincipalName) && SmtpAddress.IsValidSmtpAddress(executingUserPrincipalName))
				{
					executingUserPrimarySmtpAddress = new SmtpAddress(executingUserPrincipalName);
				}
			}
			if (executingUserPrimarySmtpAddress.IsValidAddress)
			{
				SmtpDomainWithSubdomains smtpDomainWithSubdomains = new SmtpDomainWithSubdomains(executingUserPrimarySmtpAddress.Domain);
				DomainCacheValue domainCacheValue = DomainCache.Singleton.Get(smtpDomainWithSubdomains, rbacConfiguration.OrganizationId);
				result = (domainCacheValue != null && domainCacheValue.AuthenticationType != null && domainCacheValue.AuthenticationType.Value == AuthenticationType.Federated);
			}
			return result;
		}
	}
}

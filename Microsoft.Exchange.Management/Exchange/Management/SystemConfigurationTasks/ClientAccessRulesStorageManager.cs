using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class ClientAccessRulesStorageManager
	{
		public static IEnumerable<ADClientAccessRule> GetClientAccessRules(IConfigurationSession session)
		{
			if (session.SessionSettings.ConfigScopes == ConfigScopes.AllTenants)
			{
				throw new ArgumentException(Strings.AllTenantsScopedSessionNotSupported);
			}
			return session.FindPaged<ADClientAccessRule>(null, ClientAccessRulesStorageManager.GetRulesADContainer(session), false, null, 0);
		}

		public static void SaveRules(IConfigurationSession session, IEnumerable<ADClientAccessRule> rules)
		{
			if (session.SessionSettings.ConfigScopes == ConfigScopes.AllTenants)
			{
				throw new ArgumentException(Strings.AllTenantsScopedSessionNotSupported);
			}
			foreach (ADClientAccessRule instanceToSave in rules)
			{
				session.Save(instanceToSave);
			}
		}

		public static bool IsADRuleValid(ADClientAccessRule rule)
		{
			return ClientAccessRulesStorageManager.IsAuthenticationTypeParameterValid(rule) && rule.ValidateUserRecipientFilterParsesWithSchema();
		}

		private static ADObjectId GetRulesADContainer(IConfigurationSession session)
		{
			return session.GetOrgContainerId().GetChildId(ADClientAccessRuleCollection.ContainerName);
		}

		private static bool IsAuthenticationTypeParameterValid(ADClientAccessRule rule)
		{
			if (rule.HasAnyOfSpecificProtocolsPredicate(new List<ClientAccessProtocol>
			{
				ClientAccessProtocol.RemotePowerShell
			}))
			{
				return !rule.HasAuthenticationMethodPredicate(ClientAccessAuthenticationMethod.AdfsAuthentication);
			}
			if (rule.HasAnyOfSpecificProtocolsPredicate(new List<ClientAccessProtocol>
			{
				ClientAccessProtocol.OutlookWebApp,
				ClientAccessProtocol.ExchangeAdminCenter
			}))
			{
				return !rule.HasAuthenticationMethodPredicate(ClientAccessAuthenticationMethod.NonBasicAuthentication);
			}
			return !rule.HasAnyAuthenticationMethodPredicate();
		}
	}
}

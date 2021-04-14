using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.Prompts.Provisioning;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;
using Microsoft.Exchange.UM.UMCommon.FaultInjection;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class InterServerMailboxAccessor
	{
		public static bool TestXSOHook { get; set; }

		public static IUMPromptStorage GetUMPromptStoreAccessor(ADUser user, Guid configurationObject)
		{
			ValidateArgument.NotNull(user, "User passed is null");
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(user, RemotingOptions.AllowCrossSite);
			if (exchangePrincipal.MailboxInfo.Location.ServerVersion < Server.E15MinVersion || InterServerMailboxAccessor.TestXSOHook)
			{
				return new XSOUMPromptStoreAccessor(exchangePrincipal, configurationObject);
			}
			return new EWSUMPromptStoreAccessor(exchangePrincipal, configurationObject);
		}

		public static IUMCallDataRecordStorage GetUMCallDataRecordsAcessor(ADUser user)
		{
			ValidateArgument.NotNull(user, "User passed is null");
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(user, RemotingOptions.AllowCrossSite);
			if (exchangePrincipal.MailboxInfo.Location.ServerVersion < Server.E15MinVersion || InterServerMailboxAccessor.TestXSOHook)
			{
				return new XSOUMCallDataRecordAccessor(exchangePrincipal);
			}
			return new EWSUMCallDataRecordAccessor(exchangePrincipal);
		}

		public static IUMUserMailboxStorage GetUMUserMailboxAccessor(ADUser user, bool useLocalServerOptimization = false)
		{
			ValidateArgument.NotNull(user, "User passed is null");
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(user, RemotingOptions.AllowCrossSite);
			bool flag = InterServerMailboxAccessor.TestXSOHook || exchangePrincipal.MailboxInfo.Location.ServerVersion < Server.E15MinVersion || (useLocalServerOptimization && InterServerMailboxAccessor.IsUsersMailboxOnLocalServer(user));
			if (flag)
			{
				return new XSOUMUserMailboxAccessor(exchangePrincipal, user);
			}
			return new EWSUMUserMailboxAccessor(exchangePrincipal, user);
		}

		private static bool IsUsersMailboxOnLocalServer(ADUser user)
		{
			bool flag = false;
			FaultInjectionUtils.FaultInjectChangeValue<bool>(3576048957U, ref flag);
			if (flag)
			{
				return false;
			}
			BackEndServer backEndServer = BackEndLocator.GetBackEndServer(user);
			Server server = LocalServer.GetServer();
			return backEndServer != null && server != null && backEndServer.Fqdn.Equals(server.Fqdn, StringComparison.OrdinalIgnoreCase);
		}
	}
}

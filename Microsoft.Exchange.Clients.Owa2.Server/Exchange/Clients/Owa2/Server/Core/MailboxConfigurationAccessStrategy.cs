using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class MailboxConfigurationAccessStrategy : IUserConfigurationAccessStrategy
	{
		public IUserConfiguration CreateConfiguration(MailboxSession mailboxSession, string configurationName, UserConfigurationTypes dataType)
		{
			return mailboxSession.UserConfigurationManager.CreateMailboxConfiguration(configurationName, dataType);
		}

		public IReadableUserConfiguration GetReadOnlyConfiguration(MailboxSession mailboxSession, string configName, UserConfigurationTypes freefetchDataTypes)
		{
			return mailboxSession.UserConfigurationManager.GetReadOnlyMailboxConfiguration(configName, freefetchDataTypes);
		}

		public IUserConfiguration GetConfiguration(MailboxSession mailboxSession, string configName, UserConfigurationTypes freefetchDataTypes)
		{
			return mailboxSession.UserConfigurationManager.GetMailboxConfiguration(configName, freefetchDataTypes);
		}

		public OperationResult DeleteConfigurations(MailboxSession mailboxSession, params string[] configurationNames)
		{
			return mailboxSession.UserConfigurationManager.DeleteMailboxConfigurations(configurationNames);
		}
	}
}

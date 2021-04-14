using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal interface IUserConfigurationAccessStrategy
	{
		IUserConfiguration CreateConfiguration(MailboxSession mailboxSession, string configurationName, UserConfigurationTypes dataType);

		IReadableUserConfiguration GetReadOnlyConfiguration(MailboxSession mailboxSession, string configName, UserConfigurationTypes dataType);

		IUserConfiguration GetConfiguration(MailboxSession mailboxSession, string configName, UserConfigurationTypes dataType);

		OperationResult DeleteConfigurations(MailboxSession mailboxSession, params string[] configurationNames);
	}
}

using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class DefaultFolderConfigurationAccessStrategy : IUserConfigurationAccessStrategy
	{
		public DefaultFolderConfigurationAccessStrategy(DefaultFolderType folderType)
		{
			this.folderType = folderType;
		}

		public IUserConfiguration CreateConfiguration(MailboxSession mailboxSession, string configurationName, UserConfigurationTypes dataType)
		{
			return mailboxSession.UserConfigurationManager.CreateFolderConfiguration(configurationName, dataType, this.GetDefaultFolderId(mailboxSession));
		}

		public IReadableUserConfiguration GetReadOnlyConfiguration(MailboxSession mailboxSession, string configName, UserConfigurationTypes freefetchDataTypes)
		{
			return mailboxSession.UserConfigurationManager.GetReadOnlyFolderConfiguration(configName, freefetchDataTypes, this.GetDefaultFolderId(mailboxSession));
		}

		public IUserConfiguration GetConfiguration(MailboxSession mailboxSession, string configName, UserConfigurationTypes freefetchDataTypes)
		{
			return mailboxSession.UserConfigurationManager.GetFolderConfiguration(configName, freefetchDataTypes, this.GetDefaultFolderId(mailboxSession));
		}

		public OperationResult DeleteConfigurations(MailboxSession mailboxSession, params string[] configurationNames)
		{
			return mailboxSession.UserConfigurationManager.DeleteFolderConfigurations(this.GetDefaultFolderId(mailboxSession), configurationNames);
		}

		private StoreId GetDefaultFolderId(MailboxSession mailboxSession)
		{
			if (this.folderId == null)
			{
				this.folderId = mailboxSession.GetDefaultFolderId(this.folderType);
			}
			return this.folderId;
		}

		private DefaultFolderType folderType;

		private StoreId folderId;
	}
}

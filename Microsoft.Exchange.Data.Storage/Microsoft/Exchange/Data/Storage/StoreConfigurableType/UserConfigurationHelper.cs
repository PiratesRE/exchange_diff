using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.StoreConfigurableType
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class UserConfigurationHelper
	{
		public static IReadableUserConfiguration GetReadOnlyMailboxConfiguration(MailboxSession session, string configuration, UserConfigurationTypes type, bool createIfNonexisting)
		{
			return UserConfigurationHelper.InternalGetMailboxConfiguration<IReadableUserConfiguration>(session, configuration, type, createIfNonexisting, new Func<string, UserConfigurationTypes, IReadableUserConfiguration>(session.UserConfigurationManager.GetReadOnlyMailboxConfiguration), new Func<MailboxSession, string, UserConfigurationTypes, IReadableUserConfiguration>(UserConfigurationHelper.CreateMailboxConfiguration));
		}

		public static UserConfiguration GetMailboxConfiguration(MailboxSession session, string configuration, UserConfigurationTypes type, bool createIfNonexisting)
		{
			return UserConfigurationHelper.InternalGetMailboxConfiguration<UserConfiguration>(session, configuration, type, createIfNonexisting, new Func<string, UserConfigurationTypes, UserConfiguration>(session.UserConfigurationManager.GetMailboxConfiguration), new Func<MailboxSession, string, UserConfigurationTypes, UserConfiguration>(UserConfigurationHelper.CreateMailboxConfiguration));
		}

		public static UserConfiguration GetCalendarConfiguration(MailboxSession session, string configuration, UserConfigurationTypes type, bool createIfNonexisting)
		{
			StoreId defaultFolderId = session.GetDefaultFolderId(DefaultFolderType.Calendar);
			if (defaultFolderId != null)
			{
				return UserConfigurationHelper.GetFolderConfiguration(session, defaultFolderId, configuration, type, createIfNonexisting, false);
			}
			return null;
		}

		public static IReadableUserConfiguration GetReadOnlyCalendarConfiguration(MailboxSession session, string configuration, UserConfigurationTypes type, bool createIfNonexisting)
		{
			StoreId defaultFolderId = session.GetDefaultFolderId(DefaultFolderType.Calendar);
			if (defaultFolderId != null)
			{
				return UserConfigurationHelper.InternalGetFolderConfiguration<IReadableUserConfiguration>(session, defaultFolderId, configuration, type, (UserConfigurationManager configManager, string configName, UserConfigurationTypes configType, StoreId id) => configManager.GetReadOnlyFolderConfiguration(configName, configType, id), new Func<UserConfigurationManager, string, UserConfigurationTypes, StoreId, bool, IReadableUserConfiguration>(UserConfigurationHelper.RecreateFolderConfiguration), createIfNonexisting, false);
			}
			return null;
		}

		public static UserConfiguration GetPublishingConfiguration(MailboxSession session, StoreId folderId, bool createIfNonexisting)
		{
			return UserConfigurationHelper.GetFolderConfiguration(session, folderId, "Calendar.PublishOptions", UserConfigurationTypes.Dictionary, createIfNonexisting, false);
		}

		public static UserConfiguration GetFolderConfiguration(MailboxSession mailboxSession, StoreId folderId, string configName, UserConfigurationTypes configType, bool createIfNonexisting, bool saveIfNonexisting = false)
		{
			return UserConfigurationHelper.InternalGetFolderConfiguration<UserConfiguration>(mailboxSession, folderId, configName, configType, (UserConfigurationManager manager, string name, UserConfigurationTypes type, StoreId id) => manager.GetFolderConfiguration(name, type, id), new Func<UserConfigurationManager, string, UserConfigurationTypes, StoreId, bool, UserConfiguration>(UserConfigurationHelper.RecreateFolderConfiguration), createIfNonexisting, saveIfNonexisting);
		}

		private static T InternalGetFolderConfiguration<T>(MailboxSession mailboxSession, StoreId folderId, string configName, UserConfigurationTypes configType, Func<UserConfigurationManager, string, UserConfigurationTypes, StoreId, T> getter, Func<UserConfigurationManager, string, UserConfigurationTypes, StoreId, bool, T> recreator, bool createIfNonexisting, bool saveIfNonexisting = false) where T : class, IReadableUserConfiguration
		{
			if (folderId == null)
			{
				throw new InvalidOperationException();
			}
			UserConfigurationManager userConfigurationManager = mailboxSession.UserConfigurationManager;
			T result = default(T);
			try
			{
				result = getter(userConfigurationManager, configName, configType, folderId);
			}
			catch (ObjectNotFoundException)
			{
				if (createIfNonexisting)
				{
					try
					{
						result = recreator(userConfigurationManager, configName, configType, folderId, saveIfNonexisting);
					}
					catch (ObjectExistedException)
					{
						result = getter(userConfigurationManager, configName, configType, folderId);
					}
				}
			}
			return result;
		}

		private static UserConfiguration RecreateFolderConfiguration(UserConfigurationManager configManager, string configName, UserConfigurationTypes configType, StoreId folderId, bool saveIfNonexisting)
		{
			UserConfiguration userConfiguration = configManager.CreateFolderConfiguration(configName, configType, folderId);
			if (saveIfNonexisting)
			{
				bool flag = false;
				try
				{
					userConfiguration.Save();
					flag = true;
				}
				finally
				{
					if (!flag && userConfiguration != null)
					{
						userConfiguration.Dispose();
						userConfiguration = null;
					}
				}
			}
			return userConfiguration;
		}

		public static void DeleteMailboxConfiguration(MailboxSession session, string configuration)
		{
			session.UserConfigurationManager.DeleteMailboxConfigurations(new string[]
			{
				configuration
			});
		}

		private static T InternalGetMailboxConfiguration<T>(MailboxSession session, string configuration, UserConfigurationTypes type, bool createIfNonexisting, Func<string, UserConfigurationTypes, T> getter, Func<MailboxSession, string, UserConfigurationTypes, T> creator) where T : class, IReadableUserConfiguration
		{
			T result = default(T);
			try
			{
				result = getter(configuration, type);
			}
			catch (ObjectNotFoundException)
			{
				if (createIfNonexisting)
				{
					result = creator(session, configuration, type);
				}
			}
			catch (CorruptDataException)
			{
				session.UserConfigurationManager.DeleteMailboxConfigurations(new string[]
				{
					configuration
				});
			}
			return result;
		}

		private static UserConfiguration CreateMailboxConfiguration(MailboxSession session, string configuration, UserConfigurationTypes type)
		{
			UserConfiguration userConfiguration = session.UserConfigurationManager.CreateMailboxConfiguration(configuration, type);
			userConfiguration.Save();
			return userConfiguration;
		}

		internal const string AggregatedAccountConfigurationName = "AggregatedAccount";

		internal const string AggregatedAccountListConfigurationName = "AggregatedAccountList";

		internal const string OwaUserOptionConfigurationName = "OWA.UserOptions";

		internal const string CalendarConfigurationName = "Calendar";

		internal const string CalendarFolderConfigurationName = "Calendar.PublishOptions";
	}
}

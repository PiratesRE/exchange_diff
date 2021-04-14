using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class UserConfigurationUtilities
	{
		private UserConfigurationUtilities()
		{
		}

		public static UserConfiguration GetUserConfiguration(string configurationName, UserContext userContext)
		{
			if (string.IsNullOrEmpty(configurationName))
			{
				throw new ArgumentException("configurationName must not be null or empty");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			UserConfiguration userConfiguration = null;
			try
			{
				userConfiguration = userContext.MailboxSession.UserConfigurationManager.GetMailboxConfiguration(configurationName, UserConfigurationTypes.XML);
			}
			catch (ObjectNotFoundException)
			{
				userConfiguration = userContext.MailboxSession.UserConfigurationManager.CreateMailboxConfiguration(configurationName, UserConfigurationTypes.XML);
				try
				{
					UserConfigurationUtilities.TrySaveConfiguration(userConfiguration, false);
				}
				catch (ObjectExistedException)
				{
					try
					{
						userConfiguration = userContext.MailboxSession.UserConfigurationManager.GetMailboxConfiguration(configurationName, UserConfigurationTypes.XML);
					}
					catch (ObjectNotFoundException thisObject)
					{
						throw new OwaSaveConflictException("A save conflict happened during the creation and save of the userconfiguration.", thisObject);
					}
				}
				catch (StoragePermanentException)
				{
				}
			}
			return userConfiguration;
		}

		public static void TrySaveConfiguration(UserConfiguration configuration)
		{
			UserConfigurationUtilities.TrySaveConfiguration(configuration, true);
		}

		internal static void TrySaveConfiguration(UserConfiguration configuration, bool ignoreStorePermanentExceptions)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			bool flag = false;
			bool flag2 = false;
			Exception ex = null;
			try
			{
				configuration.Save();
			}
			catch (StoragePermanentException ex2)
			{
				flag = true;
				ex = ex2;
				flag2 = true;
			}
			catch (StorageTransientException ex3)
			{
				flag = true;
				ex = ex3;
			}
			if (flag)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "UserConfigurationUtilities.TrySaveConfiguration: Failed. Exception: {0}", ex.Message);
				if (!ignoreStorePermanentExceptions && flag2)
				{
					throw ex;
				}
			}
		}

		public static UserConfiguration GetFolderConfiguration(string configurationName, UserContext userContext, StoreId folderId)
		{
			if (string.IsNullOrEmpty(configurationName))
			{
				throw new ArgumentException("configurationName must not be null or empty");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			bool flag = false;
			Exception ex = null;
			UserConfiguration userConfiguration = null;
			try
			{
				userConfiguration = userContext.MailboxSession.UserConfigurationManager.GetFolderConfiguration(configurationName, UserConfigurationTypes.Dictionary, folderId);
				userConfiguration.GetDictionary();
			}
			catch (ObjectNotFoundException)
			{
				flag = true;
			}
			catch (InvalidOperationException ex2)
			{
				ex = ex2;
				userContext.MailboxSession.UserConfigurationManager.DeleteFolderConfigurations(folderId, new string[]
				{
					configurationName
				});
				flag = true;
			}
			catch (CorruptDataException ex3)
			{
				ex = ex3;
				userContext.MailboxSession.UserConfigurationManager.DeleteFolderConfigurations(folderId, new string[]
				{
					configurationName
				});
				flag = true;
			}
			catch (Exception ex4)
			{
				ex = ex4;
				if (userConfiguration != null)
				{
					userConfiguration.Dispose();
				}
				userConfiguration = null;
			}
			if (ex != null)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string, string, string>(0L, "UserConfiguration.GetFolderConfiguration failed for configuration object {0}. Error: {1}. Stack: {2}", configurationName, ex.Message, ex.StackTrace);
			}
			if (flag)
			{
				if (userConfiguration != null)
				{
					userConfiguration.Dispose();
				}
				userConfiguration = UserConfigurationUtilities.CreateAndSaveFolderConfiguration(configurationName, userContext, folderId);
			}
			if (userConfiguration == null)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "UserConfiguration.GetFolderConfiguration failed for configuration object {0}. Returning null", configurationName);
			}
			return userConfiguration;
		}

		private static UserConfiguration CreateAndSaveFolderConfiguration(string configurationName, UserContext userContext, StoreId folderId)
		{
			UserConfiguration result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				UserConfiguration userConfiguration = userContext.MailboxSession.UserConfigurationManager.CreateFolderConfiguration(configurationName, UserConfigurationTypes.Dictionary, folderId);
				disposeGuard.Add<UserConfiguration>(userConfiguration);
				try
				{
					userConfiguration.Save();
				}
				catch (QuotaExceededException ex)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "UserConfiguration.Save: Failed. Exception: {0}", ex.Message);
				}
				disposeGuard.Success();
				result = userConfiguration;
			}
			return result;
		}
	}
}

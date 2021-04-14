using System;
using System.Globalization;
using System.Management.Automation;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.StoreTasks
{
	public static class StoreTasksHelper
	{
		internal static MailboxSession OpenMailboxSession(ExchangePrincipal principal, string taskName)
		{
			return StoreTasksHelper.OpenMailboxSession(principal, taskName, false);
		}

		internal static MailboxSession OpenMailboxSession(ExchangePrincipal principal, string taskName, bool allowAdminLocalization)
		{
			TaskLogger.LogEnter();
			MailboxSession result = MailboxSession.OpenAsAdmin(principal, CultureInfo.InvariantCulture, string.Format("Client=Management;Action={0};Privilege:ActAsAdmin", taskName), false, false, null, allowAdminLocalization);
			TaskLogger.LogExit();
			return result;
		}

		internal static MailboxSession OpenMailboxSessionAsOwner(ExchangePrincipal principal, ISecurityAccessToken userToken, string taskName)
		{
			TaskLogger.LogEnter();
			MailboxSession result = null;
			if (principal == null)
			{
				throw new ArgumentNullException("principal");
			}
			if (string.IsNullOrEmpty(taskName))
			{
				throw new ArgumentNullException("taskName");
			}
			if (userToken == null)
			{
				result = MailboxSession.Open(principal, new WindowsPrincipal(WindowsIdentity.GetCurrent()), CultureInfo.InvariantCulture, string.Format("Client=Management;Action={0}", taskName));
			}
			else
			{
				try
				{
					using (ClientSecurityContext clientSecurityContext = new ClientSecurityContext(userToken, AuthzFlags.AuthzSkipTokenGroups))
					{
						clientSecurityContext.SetSecurityAccessToken(userToken);
						result = MailboxSession.Open(principal, clientSecurityContext, CultureInfo.InvariantCulture, string.Format("Client=Management;Action={0}", taskName));
					}
				}
				catch (AuthzException ex)
				{
					throw new AccessDeniedException(new LocalizedString(ex.Message));
				}
			}
			TaskLogger.LogExit();
			return result;
		}

		internal static void CleanupMailboxStoreTypeProvider(IConfigDataProvider provider)
		{
			if (provider != null)
			{
				MailboxStoreTypeProvider mailboxStoreTypeProvider = (MailboxStoreTypeProvider)provider;
				if (mailboxStoreTypeProvider.MailboxSession != null)
				{
					mailboxStoreTypeProvider.MailboxSession.Dispose();
					mailboxStoreTypeProvider.MailboxSession = null;
				}
			}
		}

		internal static void CheckUserVersion(ADUser user, Task.TaskErrorLoggingDelegate writeError)
		{
			if (user.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
			{
				writeError(new InvalidOperationException(Strings.ErrorCannotOpenLegacyMailbox(user.Identity.ToString())), ErrorCategory.InvalidOperation, user.Identity);
			}
		}
	}
}

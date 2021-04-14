using System;
using System.Management.Automation;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.Extension
{
	internal static class OWAExtensionHelper
	{
		public static void ProcessRecord(Action action, Task.TaskErrorLoggingDelegate handleError, object identity)
		{
			try
			{
				action();
			}
			catch (OwaExtensionOperationException exception)
			{
				handleError(exception, ErrorCategory.InvalidOperation, identity);
			}
			catch (StorageTransientException exception2)
			{
				handleError(exception2, ErrorCategory.WriteError, null);
			}
			catch (StoragePermanentException exception3)
			{
				handleError(exception3, ErrorCategory.WriteError, null);
			}
			catch (XmlException exception4)
			{
				handleError(exception4, ErrorCategory.InvalidData, null);
			}
			catch (XPathException exception5)
			{
				handleError(exception5, ErrorCategory.InvalidData, null);
			}
		}

		internal static AppId CreateOWAExtensionId(Task task, ADObjectId mailboxOwnerId, string displayName, string extensionId)
		{
			AppId result;
			try
			{
				result = new AppId(mailboxOwnerId, displayName, extensionId);
			}
			catch (ArgumentException exception)
			{
				task.WriteError(exception, ErrorCategory.InvalidArgument, null);
				result = null;
			}
			return result;
		}

		internal static void CleanupOWAExtensionDataProvider(IConfigDataProvider provider)
		{
			XsoMailboxDataProviderBase xsoMailboxDataProviderBase = provider as XsoMailboxDataProviderBase;
			if (xsoMailboxDataProviderBase != null)
			{
				xsoMailboxDataProviderBase.Dispose();
				return;
			}
			OWAAppDataProviderForNonMailboxUser owaappDataProviderForNonMailboxUser = provider as OWAAppDataProviderForNonMailboxUser;
			if (owaappDataProviderForNonMailboxUser != null)
			{
				owaappDataProviderForNonMailboxUser.Dispose();
			}
		}
	}
}

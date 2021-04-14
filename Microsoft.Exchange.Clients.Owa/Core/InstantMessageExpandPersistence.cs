using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class InstantMessageExpandPersistence
	{
		public static void SetExpandedGroups(UserContext context, string[] groupIds)
		{
			try
			{
				if (groupIds.Length == 0)
				{
					groupIds = new string[]
					{
						string.Empty
					};
				}
				using (Folder folder = Utilities.SafeFolderBind(context.MailboxSession, DefaultFolderType.Root, new PropertyDefinition[]
				{
					ViewStateProperties.ExpandedGroups
				}))
				{
					if (folder != null)
					{
						folder[ViewStateProperties.ExpandedGroups] = groupIds;
						folder.Save();
					}
				}
			}
			catch (ThreadAbortException)
			{
			}
			catch (Exception arg)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError<Exception>((long)context.GetHashCode(), "InstantMessageExpandPersistence.SetExpandedGroups. Exception {0}.", arg);
			}
		}

		public static HashSet<string> GetExpandedGroups(UserContext context)
		{
			if (context.State != UserContextState.Active)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)context.GetHashCode(), "InstantMessageExpandPersistence.GetExpandedGroups: User context is no longer active. Bailing out.");
				return null;
			}
			try
			{
				context.Lock();
				using (Folder folder = Utilities.SafeFolderBind(context.MailboxSession, DefaultFolderType.Root, new PropertyDefinition[]
				{
					ViewStateProperties.ExpandedGroups
				}))
				{
					if (folder != null)
					{
						string[] defaultValue = null;
						string[] array = Utilities.GetFolderProperty<string[]>(folder, ViewStateProperties.ExpandedGroups, defaultValue);
						if (array == null)
						{
							return null;
						}
						if (array.Length == 1 && array[0] == string.Empty)
						{
							array = new string[0];
						}
						return new HashSet<string>(array);
					}
				}
			}
			catch (StoragePermanentException arg)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError<StoragePermanentException>((long)context.GetHashCode(), "InstantMessageExpandPersistence.GetExpandedGroups. Exception {0}.", arg);
			}
			catch (TransientException arg2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError<TransientException>((long)context.GetHashCode(), "InstantMessageExpandPersistence.GetExpandedGroups. Exception {0}.", arg2);
			}
			catch (ThreadAbortException)
			{
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageExpandPersistence.GetExpandedGroups", context, exception);
			}
			finally
			{
				if (context.LockedByCurrentThread())
				{
					context.Unlock();
				}
			}
			return null;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FreeEntryIdStrategy : EntryIdStrategy
	{
		internal static byte[] GetRootIdDelegate(MailboxSession session)
		{
			return session.Mailbox.MapiStore.GetIpmSubtreeFolderEntryId();
		}

		internal static byte[] GetConfigurationIdDelegate(MailboxSession session)
		{
			return session.Mailbox.MapiStore.GetNonIpmSubtreeFolderEntryId();
		}

		internal static byte[] GetInboxIdDelegate(MailboxSession session)
		{
			return session.Mailbox.MapiStore.GetInboxFolderEntryId();
		}

		internal static byte[] GetSpoolerQueueIdDelegate(MailboxSession session)
		{
			return session.Mailbox.MapiStore.GetSpoolerQueueFolderEntryId();
		}

		internal FreeEntryIdStrategy(FreeEntryIdStrategy.GetFreeIdDelegate getFreeId)
		{
			this.getFreeId = getFreeId;
		}

		internal override void GetDependentProperties(object location, IList<StorePropertyDefinition> result)
		{
		}

		internal override byte[] GetEntryId(DefaultFolderContext context)
		{
			StoreSession session = context.Session;
			bool flag = false;
			byte[] result;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				result = this.getFreeId(context.Session);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenFolder, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FreeEntryIdStrategy::GetEntryId. Hit exception when adding ``free'' default folders.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenFolder, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FreeEntryIdStrategy::GetEntryId. Hit exception when adding ``free'' default folders.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return result;
		}

		internal override void SetEntryId(DefaultFolderContext context, byte[] entryId)
		{
			throw new NotSupportedException(string.Format("The default folder cannot be changed. Delegate = {0}.", this.getFreeId.ToString()));
		}

		internal override FolderSaveResult UnsetEntryId(DefaultFolderContext context)
		{
			throw new NotSupportedException(string.Format("The default folder cannot be unset. Delegate = {0}.", this.getFreeId.ToString()));
		}

		private FreeEntryIdStrategy.GetFreeIdDelegate getFreeId;

		internal delegate byte[] GetFreeIdDelegate(MailboxSession session);
	}
}

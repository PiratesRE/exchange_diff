using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal static class OtherMailboxConfiguration
	{
		private static SimpleConfiguration<OtherMailboxConfigEntry> GetOtherMailboxConfig(UserContext userContext)
		{
			SimpleConfiguration<OtherMailboxConfigEntry> simpleConfiguration = new SimpleConfiguration<OtherMailboxConfigEntry>(userContext);
			simpleConfiguration.Load();
			return simpleConfiguration;
		}

		private static OtherMailboxConfigEntry FindOtherMailboxConfigEntry(SimpleConfiguration<OtherMailboxConfigEntry> config, string legacyDN)
		{
			foreach (OtherMailboxConfigEntry otherMailboxConfigEntry in config.Entries)
			{
				OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromString(otherMailboxConfigEntry.RootFolderId);
				if (string.Equals(owaStoreObjectId.MailboxOwnerLegacyDN, legacyDN, StringComparison.OrdinalIgnoreCase))
				{
					return otherMailboxConfigEntry;
				}
			}
			return null;
		}

		internal static IList<OtherMailboxConfigEntry> GetOtherMailboxes(UserContext userContext)
		{
			return OtherMailboxConfiguration.GetOtherMailboxConfig(userContext).Entries;
		}

		internal static OtherMailboxConfigEntry AddOtherMailboxSession(UserContext userContext, MailboxSession mailboxSession)
		{
			SimpleConfiguration<OtherMailboxConfigEntry> otherMailboxConfig = OtherMailboxConfiguration.GetOtherMailboxConfig(userContext);
			if (OtherMailboxConfiguration.FindOtherMailboxConfigEntry(otherMailboxConfig, mailboxSession.MailboxOwnerLegacyDN) != null)
			{
				return null;
			}
			StoreObjectId defaultFolderId = Utilities.GetDefaultFolderId(mailboxSession, DefaultFolderType.Root);
			OtherMailboxConfigEntry otherMailboxConfigEntry = new OtherMailboxConfigEntry(Utilities.GetMailboxOwnerDisplayName(mailboxSession), OwaStoreObjectId.CreateFromOtherUserMailboxFolderId(defaultFolderId, mailboxSession.MailboxOwner.LegacyDn));
			otherMailboxConfig.Entries.Add(otherMailboxConfigEntry);
			otherMailboxConfig.Save();
			return otherMailboxConfigEntry;
		}

		internal static bool RemoveOtherMailbox(UserContext userContext, string legacyDN)
		{
			SimpleConfiguration<OtherMailboxConfigEntry> otherMailboxConfig = OtherMailboxConfiguration.GetOtherMailboxConfig(userContext);
			OtherMailboxConfigEntry otherMailboxConfigEntry = OtherMailboxConfiguration.FindOtherMailboxConfigEntry(otherMailboxConfig, legacyDN);
			if (otherMailboxConfigEntry != null)
			{
				otherMailboxConfig.Entries.Remove(otherMailboxConfigEntry);
				otherMailboxConfig.Save();
				return true;
			}
			return false;
		}
	}
}

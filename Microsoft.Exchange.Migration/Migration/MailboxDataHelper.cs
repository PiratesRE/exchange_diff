using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxDataHelper
	{
		internal static PropertyDefinition[] GetPropertyDefinitions(MigrationType migrationType)
		{
			if (migrationType == MigrationType.XO1)
			{
				return ConsumerMailboxData.ConsumerMailboxDataPropertyDefinition;
			}
			return MailboxData.MailboxDataPropertyDefinition;
		}

		internal static IMailboxData CreateFromMessage(IMigrationStoreObject message, MigrationType migrationType)
		{
			IMailboxData mailboxData;
			if (migrationType == MigrationType.XO1)
			{
				mailboxData = new ConsumerMailboxData();
			}
			else
			{
				mailboxData = new MailboxData();
			}
			if (!mailboxData.ReadFromMessageItem(message))
			{
				return null;
			}
			return mailboxData;
		}
	}
}

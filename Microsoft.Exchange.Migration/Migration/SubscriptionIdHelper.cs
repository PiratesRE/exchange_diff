using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal static class SubscriptionIdHelper
	{
		internal static ISubscriptionId CreateFromMessage(IMigrationStoreObject message, MigrationType migrationType, IMailboxData mailboxData, bool isPAW)
		{
			ISubscriptionId subscriptionId = SubscriptionIdHelper.Create(migrationType, mailboxData, isPAW);
			if (subscriptionId != null && subscriptionId.ReadFromMessageItem(message))
			{
				return subscriptionId;
			}
			return null;
		}

		internal static PropertyDefinition[] GetPropertyDefinitions(MigrationType migrationType, bool isPAW)
		{
			if (migrationType <= MigrationType.ExchangeRemoteMove)
			{
				if (migrationType != MigrationType.IMAP)
				{
					if (migrationType != MigrationType.ExchangeOutlookAnywhere && migrationType != MigrationType.ExchangeRemoteMove)
					{
						goto IL_3D;
					}
				}
				else
				{
					if (!isPAW)
					{
						return SyncSubscriptionId.SyncSubscriptionIdPropertyDefinitions;
					}
					return MRSSubscriptionId.MRSSubscriptionIdPropertyDefinitions;
				}
			}
			else if (migrationType != MigrationType.ExchangeLocalMove && migrationType != MigrationType.PSTImport && migrationType != MigrationType.PublicFolder)
			{
				goto IL_3D;
			}
			return MRSSubscriptionId.MRSSubscriptionIdPropertyDefinitions;
			IL_3D:
			return null;
		}

		internal static ISubscriptionId Create(MigrationType migrationType, IMailboxData mailboxData, bool isPAW)
		{
			if (migrationType <= MigrationType.ExchangeRemoteMove)
			{
				if (migrationType != MigrationType.IMAP)
				{
					if (migrationType != MigrationType.ExchangeOutlookAnywhere && migrationType != MigrationType.ExchangeRemoteMove)
					{
						goto IL_44;
					}
				}
				else
				{
					if (!isPAW)
					{
						return new SyncSubscriptionId(mailboxData);
					}
					return new MRSSubscriptionId(migrationType, mailboxData);
				}
			}
			else if (migrationType != MigrationType.ExchangeLocalMove && migrationType != MigrationType.PSTImport && migrationType != MigrationType.PublicFolder)
			{
				goto IL_44;
			}
			return new MRSSubscriptionId(migrationType, mailboxData);
			IL_44:
			return null;
		}
	}
}

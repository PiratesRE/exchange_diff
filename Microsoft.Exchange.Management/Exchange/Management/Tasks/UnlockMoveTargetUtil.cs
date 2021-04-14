using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.Tasks
{
	public static class UnlockMoveTargetUtil
	{
		public static bool IsValidLockedStatus(RequestStatus status)
		{
			if (status != RequestStatus.None)
			{
				switch (status)
				{
				case RequestStatus.Completed:
				case RequestStatus.CompletedWithWarning:
					break;
				default:
					if (status != RequestStatus.Failed)
					{
						return true;
					}
					break;
				}
			}
			return false;
		}

		public static bool IsMailboxLocked(string serverFQDN, Guid dbGuid, Guid mbxGuid)
		{
			bool result;
			using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=Management", serverFQDN, null, null, null))
			{
				PropValue[][] array;
				try
				{
					array = exRpcAdmin.GetMailboxTableInfo(dbGuid, mbxGuid, new PropTag[]
					{
						PropTag.UserGuid,
						PropTag.MailboxMiscFlags
					});
				}
				catch (MapiExceptionNotFound)
				{
					array = null;
				}
				bool flag = false;
				if (array != null)
				{
					foreach (PropValue[] array3 in array)
					{
						if (array3 != null && array3.Length == 2 && array3[0].PropTag == PropTag.UserGuid)
						{
							byte[] bytes = array3[0].GetBytes();
							Guid a = (bytes != null && bytes.Length == 16) ? new Guid(bytes) : Guid.Empty;
							if (a == mbxGuid)
							{
								MailboxMiscFlags mailboxMiscFlags = (MailboxMiscFlags)((array3[1].PropTag == PropTag.MailboxMiscFlags) ? array3[1].GetInt() : 0);
								flag = ((mailboxMiscFlags & MailboxMiscFlags.CreatedByMove) != MailboxMiscFlags.None);
								break;
							}
						}
					}
				}
				result = flag;
			}
			return result;
		}

		public static void UnlockMoveTarget(string serverFQDN, Guid dbGuid, Guid mbxGuid, OrganizationId ordID)
		{
			using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=Management", serverFQDN, null, null, null))
			{
				exRpcAdmin.PurgeCachedMailboxObject(mbxGuid);
			}
			ExchangePrincipal mailboxOwner = ExchangePrincipal.FromMailboxData(mbxGuid, dbGuid, ordID ?? OrganizationId.ForestWideOrgId, UnlockMoveTargetUtil.EmptyCultures, RemotingOptions.AllowCrossSite);
			using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(mailboxOwner, CultureInfo.InvariantCulture, "Client=MSExchangeMigration;Action=MailboxRepairRequestUnlockMailbox"))
			{
				mailboxSession.Mailbox.SetProperties(new PropertyDefinition[]
				{
					MailboxSchema.InTransitStatus
				}, new object[]
				{
					InTransitStatus.SyncDestination
				});
				mailboxSession.Mailbox.Save();
				mailboxSession.Mailbox.Load();
				mailboxSession.Mailbox.SetProperties(new PropertyDefinition[]
				{
					MailboxSchema.InTransitStatus
				}, new object[]
				{
					InTransitStatus.NotInTransit
				});
				mailboxSession.Mailbox.Save();
			}
		}

		private static readonly CultureInfo[] EmptyCultures = new CultureInfo[0];
	}
}

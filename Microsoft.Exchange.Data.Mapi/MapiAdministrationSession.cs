using System;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	internal class MapiAdministrationSession : MapiSession
	{
		public MapiAdministrationSession(Fqdn serverFqdn) : base(null, serverFqdn)
		{
		}

		public MapiAdministrationSession(string serverExchangeLegacyDn, Fqdn serverFqdn) : base(serverExchangeLegacyDn, serverFqdn)
		{
		}

		public MapiAdministrationSession(string serverExchangeLegacyDn, Fqdn serverFqdn, ConsistencyMode consistencyMode) : base(serverExchangeLegacyDn, serverFqdn, consistencyMode)
		{
		}

		private void CheckRequirementsOnMailboxIdToContinue(MailboxId mailboxId)
		{
			if (null == mailboxId)
			{
				throw new ArgumentNullException("mailboxId");
			}
			if (null == mailboxId.MailboxDatabaseId || Guid.Empty == mailboxId.MailboxDatabaseId.Guid || Guid.Empty == mailboxId.MailboxGuid)
			{
				throw new ArgumentException(Strings.ExceptionIdentityInvalid);
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MapiAdministrationSession>(this);
		}

		public RawSecurityDescriptor GetMailboxSecurityDescriptor(MailboxId mailboxId)
		{
			this.CheckRequirementsOnMailboxIdToContinue(mailboxId);
			RawSecurityDescriptor returnValue = null;
			base.InvokeWithWrappedException(delegate()
			{
				returnValue = this.Administration.GetMailboxSecurityDescriptor(mailboxId.MailboxDatabaseId.Guid, mailboxId.MailboxGuid);
			}, Strings.ExceptionGetMailboxSecurityDescriptor(mailboxId.MailboxDatabaseId.Guid.ToString(), mailboxId.MailboxGuid.ToString()), mailboxId);
			return returnValue;
		}

		public void SetMailboxSecurityDescriptor(MailboxId mailboxId, RawSecurityDescriptor rawSecurityDescriptor)
		{
			this.CheckRequirementsOnMailboxIdToContinue(mailboxId);
			if (rawSecurityDescriptor == null)
			{
				throw new ArgumentNullException("rawSecurityDescriptor");
			}
			base.InvokeWithWrappedException(delegate()
			{
				this.Administration.SetMailboxSecurityDescriptor(mailboxId.MailboxDatabaseId.Guid, mailboxId.MailboxGuid, rawSecurityDescriptor);
			}, Strings.ExceptionSetMailboxSecurityDescriptor(mailboxId.MailboxDatabaseId.Guid.ToString(), mailboxId.MailboxGuid.ToString()), mailboxId);
		}

		public void SyncMailboxWithDS(MailboxId mailboxId)
		{
			this.CheckRequirementsOnMailboxIdToContinue(mailboxId);
			base.InvokeWithWrappedException(delegate()
			{
				this.Administration.SyncMailboxWithDS(mailboxId.MailboxDatabaseId.Guid, mailboxId.MailboxGuid);
			}, Strings.ExceptionFailedToLetStorePickupMailboxChange(mailboxId.MailboxGuid.ToString(), mailboxId.MailboxDatabaseId.Guid.ToString()), mailboxId);
		}

		public void ForceStoreToRefreshMailbox(MailboxId mailboxId)
		{
			this.CheckRequirementsOnMailboxIdToContinue(mailboxId);
			MapiSession.ErrorTranslatorDelegate translateError = delegate(ref LocalizedString message, Exception e)
			{
				return new FailedToRefreshMailboxException(e.Message, mailboxId.ToString(), e);
			};
			base.InvokeWithWrappedException(delegate()
			{
				int num = 0;
				while (3 > num)
				{
					try
					{
						this.Administration.ClearAbsentInDsFlagOnMailbox(mailboxId.MailboxDatabaseId.Guid, mailboxId.MailboxGuid);
						this.Administration.PurgeCachedMailboxObject(mailboxId.MailboxGuid);
						break;
					}
					catch (MapiExceptionNoAccess)
					{
						if (2 <= num)
						{
							throw;
						}
						this.Administration.PurgeCachedMailboxObject(mailboxId.MailboxGuid);
					}
					catch (MapiExceptionNotFound)
					{
						if (2 <= num)
						{
							throw;
						}
						this.Administration.PurgeCachedMailboxObject(mailboxId.MailboxGuid);
					}
					catch (MapiExceptionUnknownMailbox)
					{
						this.Administration.PurgeCachedMailboxObject(mailboxId.MailboxGuid);
						break;
					}
					num++;
				}
			}, LocalizedString.Empty, null, translateError);
		}

		public void DeleteMailbox(MailboxId mailboxId)
		{
			this.CheckRequirementsOnMailboxIdToContinue(mailboxId);
			base.InvokeWithWrappedException(delegate()
			{
				this.Administration.DeletePrivateMailbox(mailboxId.MailboxDatabaseId.Guid, mailboxId.MailboxGuid, 2);
			}, Strings.ExceptionDeleteMailbox(mailboxId.MailboxDatabaseId.Guid.ToString(), mailboxId.MailboxGuid.ToString()), mailboxId);
		}
	}
}

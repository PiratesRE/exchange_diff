using System;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ContactChangeLogger : StorageLoggerBase
	{
		public ContactChangeLogger(StoreSession storeSession) : this(ContactChangeLogger.Logger.Member, storeSession)
		{
		}

		public ContactChangeLogger(IExtensibleLogger logger, StoreSession storeSession) : base(logger)
		{
			ArgumentValidator.ThrowIfNull("storeSession", storeSession);
			MailboxSession mailboxSession = storeSession as MailboxSession;
			if (mailboxSession != null && mailboxSession.MailboxOwner.MailboxInfo.OrganizationId != null && mailboxSession.MailboxOwner.MailboxInfo.OrganizationId.OrganizationalUnit != null)
			{
				this.tenantName = mailboxSession.MailboxOwner.MailboxInfo.OrganizationId.OrganizationalUnit.ToString();
			}
			this.mailboxGuid = storeSession.MailboxGuid;
		}

		protected override string TenantName
		{
			get
			{
				return this.tenantName;
			}
		}

		protected override Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		private static readonly LazyMember<ExtensibleLogger> Logger = new LazyMember<ExtensibleLogger>(() => new ExtensibleLogger(ContactChangeLogConfiguration.Default));

		private readonly string tenantName;

		private readonly Guid mailboxGuid;
	}
}

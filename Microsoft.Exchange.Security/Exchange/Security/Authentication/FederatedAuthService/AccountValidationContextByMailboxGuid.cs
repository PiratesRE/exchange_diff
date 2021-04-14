using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class AccountValidationContextByMailboxGuid : AccountValidationContextBase
	{
		public Guid MailboxGuid { get; protected set; }

		public AccountValidationContextByMailboxGuid(Guid mailboxGuid, ExDateTime accountAuthTime) : this(mailboxGuid, null, accountAuthTime, null)
		{
		}

		public AccountValidationContextByMailboxGuid(Guid mailboxGuid, ExDateTime accountAuthTime, string appName) : this(mailboxGuid, null, accountAuthTime, appName)
		{
		}

		public AccountValidationContextByMailboxGuid(Guid mailboxGuid, OrganizationId orgId, ExDateTime accountAuthTime, string appName) : base(orgId, accountAuthTime, appName)
		{
			this.MailboxGuid = mailboxGuid;
		}

		public override AccountState CheckAccount()
		{
			return base.CheckAccount();
		}
	}
}

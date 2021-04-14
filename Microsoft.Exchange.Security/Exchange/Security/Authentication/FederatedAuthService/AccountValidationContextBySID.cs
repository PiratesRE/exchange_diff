using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class AccountValidationContextBySID : AccountValidationContextBase
	{
		public SecurityIdentifier SID { get; protected set; }

		public AccountValidationContextBySID(SecurityIdentifier sid, ExDateTime accountAuthTime) : this(sid, null, accountAuthTime, null)
		{
		}

		public AccountValidationContextBySID(SecurityIdentifier sid, ExDateTime accountAuthTime, string appName) : this(sid, null, accountAuthTime, appName)
		{
		}

		public AccountValidationContextBySID(SecurityIdentifier sid, OrganizationId orgId, ExDateTime accountAuthTime, string appName) : base(orgId, accountAuthTime, appName)
		{
			if (sid == null)
			{
				throw new ArgumentNullException("SID");
			}
			this.SID = sid;
		}

		public override AccountState CheckAccount()
		{
			return base.CheckAccount();
		}
	}
}

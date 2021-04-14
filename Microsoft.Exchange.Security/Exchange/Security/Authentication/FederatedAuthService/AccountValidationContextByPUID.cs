using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class AccountValidationContextByPUID : AccountValidationContextBase
	{
		public string PUID { get; protected set; }

		public AccountValidationContextByPUID(string puid, ExDateTime accountAuthTime) : this(puid, null, accountAuthTime, null)
		{
		}

		public AccountValidationContextByPUID(string puid, ExDateTime accountAuthTime, string appName) : this(puid, null, accountAuthTime, appName)
		{
		}

		public AccountValidationContextByPUID(string puid, OrganizationId orgId, ExDateTime accountAuthTime, string appName) : base(orgId, accountAuthTime, appName)
		{
			if (string.IsNullOrEmpty(puid))
			{
				throw new ArgumentNullException("PUID");
			}
			this.PUID = puid;
		}

		public override AccountState CheckAccount()
		{
			return base.CheckAccount();
		}
	}
}

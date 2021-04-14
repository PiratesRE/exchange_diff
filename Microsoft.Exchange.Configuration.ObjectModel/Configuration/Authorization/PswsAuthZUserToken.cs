using System;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal class PswsAuthZUserToken : AuthZPluginUserToken
	{
		internal PswsAuthZUserToken(DelegatedPrincipal delegatedPrincipal, ADRawEntry userEntry, Microsoft.Exchange.Configuration.Core.AuthenticationType authenticationType, string defaultUserName, string executingUserName) : base(delegatedPrincipal, userEntry, authenticationType, defaultUserName)
		{
			ExAssert.RetailAssert(!string.IsNullOrWhiteSpace(executingUserName), "The executingUserName should not be null or white space.");
			this.ExecutingUserName = executingUserName;
		}

		internal string ExecutingUserName { get; private set; }
	}
}

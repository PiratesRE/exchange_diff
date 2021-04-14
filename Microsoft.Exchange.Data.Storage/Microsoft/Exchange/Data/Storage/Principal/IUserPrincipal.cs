using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IUserPrincipal : IExchangePrincipal
	{
		string UserPrincipalName { get; }

		SmtpAddress WindowsLiveId { get; }

		NetID NetId { get; }
	}
}

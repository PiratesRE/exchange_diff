using System;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFrontEndLocator
	{
		Uri GetWebServicesUrl(IExchangePrincipal exchangePrincipal);

		Uri GetOwaUrl(IExchangePrincipal exchangePrincipal);
	}
}

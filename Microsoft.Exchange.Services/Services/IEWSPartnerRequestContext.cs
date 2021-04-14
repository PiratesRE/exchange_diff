using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services
{
	internal interface IEWSPartnerRequestContext
	{
		AuthZClientInfo CallerClientInfo { get; }

		ExchangePrincipal ExchangePrincipal { get; }

		string UserAgent { get; }
	}
}

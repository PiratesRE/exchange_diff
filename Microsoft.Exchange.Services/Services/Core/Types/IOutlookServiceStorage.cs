using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal interface IOutlookServiceStorage : IDisposable
	{
		string TenantId { get; }

		IOutlookServiceSubscriptionStorage GetOutlookServiceSubscriptionStorage();
	}
}

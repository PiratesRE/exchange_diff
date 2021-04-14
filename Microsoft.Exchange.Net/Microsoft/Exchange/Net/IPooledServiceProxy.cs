using System;

namespace Microsoft.Exchange.Net
{
	internal interface IPooledServiceProxy<TClient>
	{
		TClient Client { get; }

		string Tag { get; set; }
	}
}

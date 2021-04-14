using System;
using Microsoft.Exchange.Autodiscover.Providers;

namespace Microsoft.Exchange.Autodiscover
{
	internal delegate Provider CreateProviderDelegate(RequestData requestData);
}

using System;

namespace Microsoft.Exchange.Common.Cache
{
	internal interface IExpirationWindowProvider<T>
	{
		TimeSpan GetExpirationWindow(T value);
	}
}

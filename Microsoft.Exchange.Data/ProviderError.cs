using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class ProviderError
	{
		public static readonly ProviderError NotFound = new ProviderError();
	}
}

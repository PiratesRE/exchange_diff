using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal interface IReusable
	{
		void Initialize(object newSourceOrDestination);
	}
}

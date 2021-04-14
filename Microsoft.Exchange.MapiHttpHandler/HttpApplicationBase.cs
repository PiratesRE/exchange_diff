using System;

namespace Microsoft.Exchange.MapiHttp
{
	public abstract class HttpApplicationBase
	{
		public virtual void CompleteRequest()
		{
			throw new NotImplementedException();
		}
	}
}

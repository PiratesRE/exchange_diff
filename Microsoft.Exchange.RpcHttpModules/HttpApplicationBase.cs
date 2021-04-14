using System;

namespace Microsoft.Exchange.RpcHttpModules
{
	public abstract class HttpApplicationBase
	{
		public virtual void CompleteRequest()
		{
			throw new NotImplementedException();
		}
	}
}

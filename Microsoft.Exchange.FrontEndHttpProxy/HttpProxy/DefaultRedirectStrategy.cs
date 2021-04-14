using System;

namespace Microsoft.Exchange.HttpProxy
{
	internal class DefaultRedirectStrategy : DatacenterRedirectStrategy
	{
		public DefaultRedirectStrategy(IRequestContext requestContext) : base(requestContext)
		{
		}
	}
}

using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface ICallHandler
	{
		void HandleCall(CafeRoutingContext context);
	}
}

using System;
using System.Web;

namespace Microsoft.Exchange.HttpProxy
{
	internal interface IRequestContext
	{
		HttpContext HttpContext { get; }

		RequestDetailsLogger Logger { get; }

		LatencyTracker LatencyTracker { get; }

		int TraceContext { get; }

		Guid ActivityId { get; }

		IAuthBehavior AuthBehavior { get; }
	}
}

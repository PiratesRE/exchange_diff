using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Feature
	{
		public static RopExecutionException NotImplemented(int bugNumber, string message)
		{
			ExTraceGlobals.NotImplementedTracer.TraceError(bugNumber, Feature.TraceContextGetter(), message);
			return new RopExecutionException(string.Format("#{0}: {1} is not yet implemented", bugNumber, message), (ErrorCode)2147749887U);
		}

		public static void Stubbed(int bugNumber, string message)
		{
			ExTraceGlobals.NotImplementedTracer.TraceWarning(bugNumber, Feature.TraceContextGetter(), message);
		}

		public static Func<long> TraceContextGetter = () => 0L;
	}
}

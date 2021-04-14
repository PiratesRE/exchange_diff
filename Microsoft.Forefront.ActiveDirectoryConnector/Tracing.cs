using System;

namespace Microsoft
{
	internal static class Tracing
	{
		internal static readonly TraceProvider tracer = new TraceProvider("Microsoft.Forefront.ActiveDirectoryConnector", new Guid("{b873680d-be62-4181-b678-bb651fa11c25}"), true);
	}
}

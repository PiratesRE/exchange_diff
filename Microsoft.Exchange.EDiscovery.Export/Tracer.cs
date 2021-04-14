using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal static class Tracer
	{
		public static ITracer TracerInstance { get; set; } = new Tracer.DefaultTracer();

		public static void TraceError(string format, params object[] args)
		{
			Tracer.TracerInstance.TraceError(format, args);
		}

		public static void TraceWarning(string format, params object[] args)
		{
			Tracer.TracerInstance.TraceWarning(format, args);
		}

		public static void TraceInformation(string format, params object[] args)
		{
			Tracer.TracerInstance.TraceInformation(format, args);
		}

		private class DefaultTracer : ITracer
		{
			public void TraceError(string format, params object[] args)
			{
			}

			public void TraceWarning(string format, params object[] args)
			{
			}

			public void TraceInformation(string format, params object[] args)
			{
			}
		}
	}
}

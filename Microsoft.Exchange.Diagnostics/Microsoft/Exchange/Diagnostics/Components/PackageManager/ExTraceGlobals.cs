using System;

namespace Microsoft.Exchange.Diagnostics.Components.PackageManager
{
	public static class ExTraceGlobals
	{
		public static Trace PackageManagerTracer
		{
			get
			{
				if (ExTraceGlobals.packageManagerTracer == null)
				{
					ExTraceGlobals.packageManagerTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.packageManagerTracer;
			}
		}

		private static Guid componentGuid = new Guid("1C570B41-B6CF-4319-AD48-B2DD9D192CC7");

		private static Trace packageManagerTracer = null;
	}
}

using System;

namespace Microsoft.Exchange.Diagnostics.Components.MALListGenerators
{
	public static class ExTraceGlobals
	{
		public static Trace MALListGeneratorTracer
		{
			get
			{
				if (ExTraceGlobals.mALListGeneratorTracer == null)
				{
					ExTraceGlobals.mALListGeneratorTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.mALListGeneratorTracer;
			}
		}

		private static Guid componentGuid = new Guid("cf0a0f23-0ada-4c01-a8b9-bf1a0cfbcdb7");

		private static Trace mALListGeneratorTracer = null;
	}
}

using System;

namespace Microsoft.Exchange.Diagnostics.Components.RulesDataBlobGenerator
{
	public static class ExTraceGlobals
	{
		public static Trace RulesDataBlobGeneratorTracer
		{
			get
			{
				if (ExTraceGlobals.rulesDataBlobGeneratorTracer == null)
				{
					ExTraceGlobals.rulesDataBlobGeneratorTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.rulesDataBlobGeneratorTracer;
			}
		}

		private static Guid componentGuid = new Guid("EBFB2854-BBAC-4746-9ABB-C8C0B7190D60");

		private static Trace rulesDataBlobGeneratorTracer = null;
	}
}

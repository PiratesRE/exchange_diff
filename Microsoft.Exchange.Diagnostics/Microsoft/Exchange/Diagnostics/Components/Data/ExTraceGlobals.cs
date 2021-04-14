using System;

namespace Microsoft.Exchange.Diagnostics.Components.Data
{
	public static class ExTraceGlobals
	{
		public static Trace PropertyBagTracer
		{
			get
			{
				if (ExTraceGlobals.propertyBagTracer == null)
				{
					ExTraceGlobals.propertyBagTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.propertyBagTracer;
			}
		}

		public static Trace ValidationTracer
		{
			get
			{
				if (ExTraceGlobals.validationTracer == null)
				{
					ExTraceGlobals.validationTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.validationTracer;
			}
		}

		public static Trace SerializationTracer
		{
			get
			{
				if (ExTraceGlobals.serializationTracer == null)
				{
					ExTraceGlobals.serializationTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.serializationTracer;
			}
		}

		public static Trace ValueConvertorTracer
		{
			get
			{
				if (ExTraceGlobals.valueConvertorTracer == null)
				{
					ExTraceGlobals.valueConvertorTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.valueConvertorTracer;
			}
		}

		private static Guid componentGuid = new Guid("E7FE6E6D-7B3D-4942-B672-BBFD89AC4DC5");

		private static Trace propertyBagTracer = null;

		private static Trace validationTracer = null;

		private static Trace serializationTracer = null;

		private static Trace valueConvertorTracer = null;
	}
}

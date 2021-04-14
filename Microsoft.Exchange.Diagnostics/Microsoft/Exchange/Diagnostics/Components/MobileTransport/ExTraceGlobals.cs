using System;

namespace Microsoft.Exchange.Diagnostics.Components.MobileTransport
{
	public static class ExTraceGlobals
	{
		public static Trace XsoTracer
		{
			get
			{
				if (ExTraceGlobals.xsoTracer == null)
				{
					ExTraceGlobals.xsoTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.xsoTracer;
			}
		}

		public static Trace CoreTracer
		{
			get
			{
				if (ExTraceGlobals.coreTracer == null)
				{
					ExTraceGlobals.coreTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.coreTracer;
			}
		}

		public static Trace TransportTracer
		{
			get
			{
				if (ExTraceGlobals.transportTracer == null)
				{
					ExTraceGlobals.transportTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.transportTracer;
			}
		}

		public static Trace SessionTracer
		{
			get
			{
				if (ExTraceGlobals.sessionTracer == null)
				{
					ExTraceGlobals.sessionTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.sessionTracer;
			}
		}

		public static Trace ServiceTracer
		{
			get
			{
				if (ExTraceGlobals.serviceTracer == null)
				{
					ExTraceGlobals.serviceTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.serviceTracer;
			}
		}

		public static Trace ApplicationlogicTracer
		{
			get
			{
				if (ExTraceGlobals.applicationlogicTracer == null)
				{
					ExTraceGlobals.applicationlogicTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.applicationlogicTracer;
			}
		}

		private static Guid componentGuid = new Guid("344A3E26-44B9-45b3-B5EC-623311EAA0AA");

		private static Trace xsoTracer = null;

		private static Trace coreTracer = null;

		private static Trace transportTracer = null;

		private static Trace sessionTracer = null;

		private static Trace serviceTracer = null;

		private static Trace applicationlogicTracer = null;
	}
}

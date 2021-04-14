using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.HygieneData
{
	public static class ExTraceGlobals
	{
		public static Trace WebstoreProviderTracer
		{
			get
			{
				if (ExTraceGlobals.webstoreProviderTracer == null)
				{
					ExTraceGlobals.webstoreProviderTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.webstoreProviderTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace DomainSessionTracer
		{
			get
			{
				if (ExTraceGlobals.domainSessionTracer == null)
				{
					ExTraceGlobals.domainSessionTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.domainSessionTracer;
			}
		}

		public static Trace GLSQueryTracer
		{
			get
			{
				if (ExTraceGlobals.gLSQueryTracer == null)
				{
					ExTraceGlobals.gLSQueryTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.gLSQueryTracer;
			}
		}

		public static Trace WebServiceProviderTracer
		{
			get
			{
				if (ExTraceGlobals.webServiceProviderTracer == null)
				{
					ExTraceGlobals.webServiceProviderTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.webServiceProviderTracer;
			}
		}

		private static Guid componentGuid = new Guid("4B65DA35-2EAC-4452-B7B7-375D986BCA91");

		private static Trace webstoreProviderTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace domainSessionTracer = null;

		private static Trace gLSQueryTracer = null;

		private static Trace webServiceProviderTracer = null;
	}
}

using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Authorization
{
	public static class ExTraceGlobals
	{
		public static Trace ADConfigTracer
		{
			get
			{
				if (ExTraceGlobals.aDConfigTracer == null)
				{
					ExTraceGlobals.aDConfigTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.aDConfigTracer;
			}
		}

		public static Trace AccessDeniedTracer
		{
			get
			{
				if (ExTraceGlobals.accessDeniedTracer == null)
				{
					ExTraceGlobals.accessDeniedTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.accessDeniedTracer;
			}
		}

		public static Trace RunspaceConfigTracer
		{
			get
			{
				if (ExTraceGlobals.runspaceConfigTracer == null)
				{
					ExTraceGlobals.runspaceConfigTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.runspaceConfigTracer;
			}
		}

		public static Trace AccessCheckTracer
		{
			get
			{
				if (ExTraceGlobals.accessCheckTracer == null)
				{
					ExTraceGlobals.accessCheckTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.accessCheckTracer;
			}
		}

		public static Trace PublicCreationAPITracer
		{
			get
			{
				if (ExTraceGlobals.publicCreationAPITracer == null)
				{
					ExTraceGlobals.publicCreationAPITracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.publicCreationAPITracer;
			}
		}

		public static Trace PublicInstanceAPITracer
		{
			get
			{
				if (ExTraceGlobals.publicInstanceAPITracer == null)
				{
					ExTraceGlobals.publicInstanceAPITracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.publicInstanceAPITracer;
			}
		}

		public static Trace IssBuilderTracer
		{
			get
			{
				if (ExTraceGlobals.issBuilderTracer == null)
				{
					ExTraceGlobals.issBuilderTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.issBuilderTracer;
			}
		}

		public static Trace PublicPluginAPITracer
		{
			get
			{
				if (ExTraceGlobals.publicPluginAPITracer == null)
				{
					ExTraceGlobals.publicPluginAPITracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.publicPluginAPITracer;
			}
		}

		public static Trace IssBuilderDetailTracer
		{
			get
			{
				if (ExTraceGlobals.issBuilderDetailTracer == null)
				{
					ExTraceGlobals.issBuilderDetailTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.issBuilderDetailTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("96825f4e-464a-44ef-af25-a76d1d0cec77");

		private static Trace aDConfigTracer = null;

		private static Trace accessDeniedTracer = null;

		private static Trace runspaceConfigTracer = null;

		private static Trace accessCheckTracer = null;

		private static Trace publicCreationAPITracer = null;

		private static Trace publicInstanceAPITracer = null;

		private static Trace issBuilderTracer = null;

		private static Trace publicPluginAPITracer = null;

		private static Trace issBuilderDetailTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}

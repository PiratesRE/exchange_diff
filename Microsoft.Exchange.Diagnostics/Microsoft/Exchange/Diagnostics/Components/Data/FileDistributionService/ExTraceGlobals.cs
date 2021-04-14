using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Data.FileDistributionService
{
	public static class ExTraceGlobals
	{
		public static Trace CustomCommandTracer
		{
			get
			{
				if (ExTraceGlobals.customCommandTracer == null)
				{
					ExTraceGlobals.customCommandTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.customCommandTracer;
			}
		}

		public static Trace FileReplicationTracer
		{
			get
			{
				if (ExTraceGlobals.fileReplicationTracer == null)
				{
					ExTraceGlobals.fileReplicationTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.fileReplicationTracer;
			}
		}

		public static Trace ADRequestsTracer
		{
			get
			{
				if (ExTraceGlobals.aDRequestsTracer == null)
				{
					ExTraceGlobals.aDRequestsTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.aDRequestsTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("0f0a52f9-4d72-460d-9928-1da8215066d4");

		private static Trace customCommandTracer = null;

		private static Trace fileReplicationTracer = null;

		private static Trace aDRequestsTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}

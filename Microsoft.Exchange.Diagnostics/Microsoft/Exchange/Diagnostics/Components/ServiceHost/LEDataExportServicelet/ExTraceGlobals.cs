﻿using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ServiceHost.LEDataExportServicelet
{
	public static class ExTraceGlobals
	{
		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace ServiceletTracer
		{
			get
			{
				if (ExTraceGlobals.serviceletTracer == null)
				{
					ExTraceGlobals.serviceletTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.serviceletTracer;
			}
		}

		public static Trace WorkerTracer
		{
			get
			{
				if (ExTraceGlobals.workerTracer == null)
				{
					ExTraceGlobals.workerTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.workerTracer;
			}
		}

		private static Guid componentGuid = new Guid("f9dbde22-ed1e-4059-b757-51053ed786b8");

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace serviceletTracer = null;

		private static Trace workerTracer = null;
	}
}

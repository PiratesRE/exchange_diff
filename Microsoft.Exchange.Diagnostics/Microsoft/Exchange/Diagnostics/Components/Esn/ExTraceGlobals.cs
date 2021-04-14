using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Esn
{
	public static class ExTraceGlobals
	{
		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		public static Trace DataTracer
		{
			get
			{
				if (ExTraceGlobals.dataTracer == null)
				{
					ExTraceGlobals.dataTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.dataTracer;
			}
		}

		public static Trace PreProcessorTracer
		{
			get
			{
				if (ExTraceGlobals.preProcessorTracer == null)
				{
					ExTraceGlobals.preProcessorTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.preProcessorTracer;
			}
		}

		public static Trace ComposerTracer
		{
			get
			{
				if (ExTraceGlobals.composerTracer == null)
				{
					ExTraceGlobals.composerTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.composerTracer;
			}
		}

		public static Trace PostProcessorTracer
		{
			get
			{
				if (ExTraceGlobals.postProcessorTracer == null)
				{
					ExTraceGlobals.postProcessorTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.postProcessorTracer;
			}
		}

		public static Trace MailSenderTracer
		{
			get
			{
				if (ExTraceGlobals.mailSenderTracer == null)
				{
					ExTraceGlobals.mailSenderTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.mailSenderTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("A0D123B0-CF78-4BCA-AAC9-F892D98199F4");

		private static Trace generalTracer = null;

		private static Trace dataTracer = null;

		private static Trace preProcessorTracer = null;

		private static Trace composerTracer = null;

		private static Trace postProcessorTracer = null;

		private static Trace mailSenderTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}

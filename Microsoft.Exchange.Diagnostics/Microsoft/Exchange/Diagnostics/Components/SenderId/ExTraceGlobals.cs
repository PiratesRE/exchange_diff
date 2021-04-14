using System;

namespace Microsoft.Exchange.Diagnostics.Components.SenderId
{
	public static class ExTraceGlobals
	{
		public static Trace ValidationTracer
		{
			get
			{
				if (ExTraceGlobals.validationTracer == null)
				{
					ExTraceGlobals.validationTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.validationTracer;
			}
		}

		public static Trace ParsingTracer
		{
			get
			{
				if (ExTraceGlobals.parsingTracer == null)
				{
					ExTraceGlobals.parsingTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.parsingTracer;
			}
		}

		public static Trace MacroExpansionTracer
		{
			get
			{
				if (ExTraceGlobals.macroExpansionTracer == null)
				{
					ExTraceGlobals.macroExpansionTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.macroExpansionTracer;
			}
		}

		public static Trace AgentTracer
		{
			get
			{
				if (ExTraceGlobals.agentTracer == null)
				{
					ExTraceGlobals.agentTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.agentTracer;
			}
		}

		public static Trace OtherTracer
		{
			get
			{
				if (ExTraceGlobals.otherTracer == null)
				{
					ExTraceGlobals.otherTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.otherTracer;
			}
		}

		private static Guid componentGuid = new Guid("AA6A0F4B-6EC1-472d-84BA-FDCB84F20449");

		private static Trace validationTracer = null;

		private static Trace parsingTracer = null;

		private static Trace macroExpansionTracer = null;

		private static Trace agentTracer = null;

		private static Trace otherTracer = null;
	}
}

using System;

namespace Microsoft.Exchange.Diagnostics.Components.DataMining
{
	public static class ExTraceGlobals
	{
		public static Trace EventsTracer
		{
			get
			{
				if (ExTraceGlobals.eventsTracer == null)
				{
					ExTraceGlobals.eventsTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.eventsTracer;
			}
		}

		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		public static Trace ConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.configurationTracer == null)
				{
					ExTraceGlobals.configurationTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.configurationTracer;
			}
		}

		public static Trace ConfigurationServiceTracer
		{
			get
			{
				if (ExTraceGlobals.configurationServiceTracer == null)
				{
					ExTraceGlobals.configurationServiceTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.configurationServiceTracer;
			}
		}

		public static Trace SchedulerTracer
		{
			get
			{
				if (ExTraceGlobals.schedulerTracer == null)
				{
					ExTraceGlobals.schedulerTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.schedulerTracer;
			}
		}

		public static Trace PumperTracer
		{
			get
			{
				if (ExTraceGlobals.pumperTracer == null)
				{
					ExTraceGlobals.pumperTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.pumperTracer;
			}
		}

		public static Trace UploaderTracer
		{
			get
			{
				if (ExTraceGlobals.uploaderTracer == null)
				{
					ExTraceGlobals.uploaderTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.uploaderTracer;
			}
		}

		private static Guid componentGuid = new Guid("{54300D03-CEA2-43CB-9522-2F6B1CD5DAA4}");

		private static Trace eventsTracer = null;

		private static Trace generalTracer = null;

		private static Trace configurationTracer = null;

		private static Trace configurationServiceTracer = null;

		private static Trace schedulerTracer = null;

		private static Trace pumperTracer = null;

		private static Trace uploaderTracer = null;
	}
}

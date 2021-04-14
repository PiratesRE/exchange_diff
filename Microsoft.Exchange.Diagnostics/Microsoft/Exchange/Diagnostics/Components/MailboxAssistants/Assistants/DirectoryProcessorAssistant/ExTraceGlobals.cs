using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.DirectoryProcessorAssistant
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

		public static Trace ADCrawlerTracer
		{
			get
			{
				if (ExTraceGlobals.aDCrawlerTracer == null)
				{
					ExTraceGlobals.aDCrawlerTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.aDCrawlerTracer;
			}
		}

		public static Trace DtmfMapGeneratorTracer
		{
			get
			{
				if (ExTraceGlobals.dtmfMapGeneratorTracer == null)
				{
					ExTraceGlobals.dtmfMapGeneratorTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.dtmfMapGeneratorTracer;
			}
		}

		public static Trace GrammarGeneratorTracer
		{
			get
			{
				if (ExTraceGlobals.grammarGeneratorTracer == null)
				{
					ExTraceGlobals.grammarGeneratorTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.grammarGeneratorTracer;
			}
		}

		private static Guid componentGuid = new Guid("C585941C-EDA7-11E0-A0A8-1BCA4724019B");

		private static Trace generalTracer = null;

		private static Trace aDCrawlerTracer = null;

		private static Trace dtmfMapGeneratorTracer = null;

		private static Trace grammarGeneratorTracer = null;
	}
}

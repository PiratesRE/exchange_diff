using System;

namespace Microsoft.Exchange.Diagnostics.Components.RusPublishing
{
	public static class ExTraceGlobals
	{
		public static Trace CommonTracer
		{
			get
			{
				if (ExTraceGlobals.commonTracer == null)
				{
					ExTraceGlobals.commonTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.commonTracer;
			}
		}

		public static Trace PublisherWebTracer
		{
			get
			{
				if (ExTraceGlobals.publisherWebTracer == null)
				{
					ExTraceGlobals.publisherWebTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.publisherWebTracer;
			}
		}

		public static Trace EngineUpdateDownloaderTracer
		{
			get
			{
				if (ExTraceGlobals.engineUpdateDownloaderTracer == null)
				{
					ExTraceGlobals.engineUpdateDownloaderTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.engineUpdateDownloaderTracer;
			}
		}

		public static Trace EngineUpdatePublisherTracer
		{
			get
			{
				if (ExTraceGlobals.engineUpdatePublisherTracer == null)
				{
					ExTraceGlobals.engineUpdatePublisherTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.engineUpdatePublisherTracer;
			}
		}

		public static Trace SignaturePollingJobTracer
		{
			get
			{
				if (ExTraceGlobals.signaturePollingJobTracer == null)
				{
					ExTraceGlobals.signaturePollingJobTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.signaturePollingJobTracer;
			}
		}

		public static Trace EnginePackagingStepTracer
		{
			get
			{
				if (ExTraceGlobals.enginePackagingStepTracer == null)
				{
					ExTraceGlobals.enginePackagingStepTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.enginePackagingStepTracer;
			}
		}

		public static Trace EngineTestingStepTracer
		{
			get
			{
				if (ExTraceGlobals.engineTestingStepTracer == null)
				{
					ExTraceGlobals.engineTestingStepTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.engineTestingStepTracer;
			}
		}

		public static Trace EngineCodeSignStepTracer
		{
			get
			{
				if (ExTraceGlobals.engineCodeSignStepTracer == null)
				{
					ExTraceGlobals.engineCodeSignStepTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.engineCodeSignStepTracer;
			}
		}

		public static Trace EngineCleanUpStepTracer
		{
			get
			{
				if (ExTraceGlobals.engineCleanUpStepTracer == null)
				{
					ExTraceGlobals.engineCleanUpStepTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.engineCleanUpStepTracer;
			}
		}

		private static Guid componentGuid = new Guid("534d6f5a-8ca8-4c44-abd7-481335889364");

		private static Trace commonTracer = null;

		private static Trace publisherWebTracer = null;

		private static Trace engineUpdateDownloaderTracer = null;

		private static Trace engineUpdatePublisherTracer = null;

		private static Trace signaturePollingJobTracer = null;

		private static Trace enginePackagingStepTracer = null;

		private static Trace engineTestingStepTracer = null;

		private static Trace engineCodeSignStepTracer = null;

		private static Trace engineCleanUpStepTracer = null;
	}
}

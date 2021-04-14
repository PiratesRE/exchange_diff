using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class SubmissionConfiguration : ISubmissionConfiguration
	{
		private SubmissionConfiguration()
		{
			SubmissionConfiguration.components = new Components(string.Empty, false);
			SubmissionConfiguration.app = AppConfig.Load();
			this.isInitialized = false;
		}

		public static ISubmissionConfiguration Instance
		{
			get
			{
				if (SubmissionConfiguration.configuration == null)
				{
					SubmissionConfiguration.configuration = new SubmissionConfiguration();
				}
				return SubmissionConfiguration.configuration;
			}
			set
			{
				SubmissionConfiguration.configuration = value;
			}
		}

		public IAppConfiguration App
		{
			get
			{
				return SubmissionConfiguration.app;
			}
		}

		public void Load()
		{
			if (!this.isInitialized)
			{
				SubmissionConfiguration.components.Start(new Components.StopServiceHandler(SubmissionConfiguration.OnStopServiceBecauseOfFailure), false, false, true, true);
				SubmissionConfiguration.components.Continue();
				Components.StoreDriverSubmission.Continue();
				LatencyTracker.Start(Components.TransportAppConfig.LatencyTracker, ProcessTransportRole.MailboxSubmission);
				SubmissionConfiguration.StartSystemProbe();
				this.isInitialized = true;
			}
		}

		public void Unload()
		{
			if (this.isInitialized)
			{
				SystemProbe.Stop();
				SubmissionConfiguration.components.Stop();
				this.isInitialized = false;
			}
		}

		public void ConfigUpdate()
		{
			SubmissionConfiguration.components.ConfigUpdate();
		}

		private static void StartSystemProbe()
		{
			try
			{
				SystemProbe.Start("SYSPRB", ProcessTransportRole.MailboxSubmission.ToString());
				ExTraceGlobals.GeneralTracer.TraceDebug(0L, "MBTSubmission: System probe started successfully.");
				SystemProbe.ActivityId = CombGuidGenerator.NewGuid();
				SystemProbe.TracePass("MBTSubmission", "System probe started successfully.", new object[0]);
				SystemProbe.ActivityId = Guid.Empty;
			}
			catch (LogException ex)
			{
				ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "MBTSubmission: Failed to initialize system probe. {0}", ex.Message);
			}
		}

		private static void OnStopServiceBecauseOfFailure(string reason, bool canRetry, bool retryAlways, bool failServiceWithException)
		{
			Environment.Exit(1);
		}

		private static Components components;

		private static IAppConfiguration app;

		private static ISubmissionConfiguration configuration;

		private bool isInitialized;
	}
}

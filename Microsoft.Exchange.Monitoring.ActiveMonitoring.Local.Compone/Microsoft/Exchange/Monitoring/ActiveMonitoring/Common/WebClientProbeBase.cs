using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Net.MonitoringWebClient;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public abstract class WebClientProbeBase : ProbeWorkItem
	{
		internal virtual SslValidationOptions DefaultSslValidationOptions
		{
			get
			{
				return SslValidationOptions.NoSslValidation;
			}
		}

		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (propertyBag.ContainsKey("SslValidationOptions"))
			{
				pDef.Attributes["SslValidationOptions"] = propertyBag["SslValidationOptions"].ToString().Trim();
				return;
			}
			throw new ArgumentException("Please specify value forSslValidationOptions");
		}

		internal virtual IHttpSession CreateHttpSession()
		{
			IRequestAdapter requestAdapter = this.CreateRequestAdapter();
			IExceptionAnalyzer exceptionAnalyzer = this.CreateExceptionAnalyzer();
			IResponseTracker responseTracker = this.CreateResponseTracker();
			IHttpSession httpSession = new HttpSession(requestAdapter, exceptionAnalyzer, responseTracker);
			httpSession.UserAgent = "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1; MSEXCHMON; ACTIVEMONITORING)";
			httpSession.PersistentHeaders[WellKnownHeader.FrontEndToBackEndTimeout] = WebClientProbeBase.FrontEndToBackEndTimeout.TotalSeconds.ToString();
			SslValidationOptions sslValidationOptions;
			if (base.Definition.Attributes.ContainsKey("SslValidationOptions") && Enum.TryParse<SslValidationOptions>(base.Definition.Attributes["SslValidationOptions"], true, out sslValidationOptions))
			{
				httpSession.SslValidationOptions = sslValidationOptions;
			}
			else
			{
				httpSession.SslValidationOptions = this.DefaultSslValidationOptions;
			}
			return httpSession;
		}

		internal virtual IRequestAdapter CreateRequestAdapter()
		{
			return new RequestAdapter
			{
				RequestTimeout = WebClientProbeBase.RequestTimeout
			};
		}

		internal abstract IExceptionAnalyzer CreateExceptionAnalyzer();

		internal virtual IResponseTracker CreateResponseTracker()
		{
			return new ResponseTracker();
		}

		internal abstract Task ExecuteScenario(IHttpSession session);

		internal abstract void ScenarioSucceeded(Task scenarioTask);

		internal abstract void ScenarioFailed(Task scenarioTask);

		internal abstract void ScenarioCancelled(Task scenarioTask);

		internal virtual void TestStepStarted(object sender, TestEventArgs eventArgs)
		{
			this.TraceInformation("Test step started: {0}", new object[]
			{
				eventArgs.TestId
			});
		}

		internal virtual void TestStepFinished(object sender, TestEventArgs eventArgs)
		{
			this.TraceInformation("Test step finished: {0}", new object[]
			{
				eventArgs.TestId
			});
		}

		internal virtual void RequestSent(object sender, HttpWebEventArgs eventArgs)
		{
			this.TraceInformation("Request being sent to url: {0}{2}{2}{1}", new object[]
			{
				eventArgs.Request.RequestUri.AbsoluteUri,
				eventArgs.Request.ToStringNoBody(),
				Environment.NewLine
			});
		}

		internal virtual void ResponseReceived(object sender, HttpWebEventArgs eventArgs)
		{
			this.TraceInformation("Response received from url {0}, Status Code = {1}, Responding Server = {2}:{4}{4}{3}", new object[]
			{
				eventArgs.Request.RequestUri.AbsoluteUri,
				eventArgs.Response.StatusCode,
				eventArgs.Response.RespondingFrontEndServer,
				eventArgs.Response.ToStringNoBody(),
				Environment.NewLine
			});
		}

		internal abstract void TraceInformation(string message, params object[] parameters);

		protected sealed override void DoWork(CancellationToken cancellationToken)
		{
			IHttpSession httpSession = this.CreateHttpSession();
			httpSession.TestStarted += this.TestStepStarted;
			httpSession.TestFinished += this.TestStepFinished;
			httpSession.SendingRequest += this.RequestSent;
			httpSession.ResponseReceived += this.ResponseReceived;
			Task task = this.ExecuteScenario(httpSession);
			task.ContinueWith(new Action<Task>(this.ScenarioSucceeded), TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			task.ContinueWith(new Action<Task>(this.ScenarioFailed), TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnRanToCompletion | TaskContinuationOptions.NotOnCanceled);
			task.ContinueWith(new Action<Task>(this.ScenarioCancelled), TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnRanToCompletion | TaskContinuationOptions.NotOnFaulted);
		}

		public const string SslValidationOptionsParameterName = "SslValidationOptions";

		protected static readonly TimeSpan RequestTimeout = TimeSpan.FromMinutes(2.0);

		protected static readonly TimeSpan FrontEndToBackEndTimeout = TimeSpan.FromSeconds(100.0);
	}
}

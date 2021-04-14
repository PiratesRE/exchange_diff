using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Calendar.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Ews.Probes;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.AvailabilityService.Probes
{
	public class AvailabilityServiceProbe : CalendarProbeBase
	{
		public string ProbeErrorMessage { get; set; }

		public bool IsProbeFailed { get; set; }

		protected override string ComponentId
		{
			get
			{
				return "AvailabilityService_AM_Probe";
			}
		}

		protected override void Configure()
		{
			base.Configure();
			this.AppendKnownErrorCodes();
			base.ConfigureResultName();
			this.traceListener = new EWSCommon.TraceListener(base.TraceContext, ExTraceGlobals.AvailabilityServiceTracer);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Initialize(ExTraceGlobals.AvailabilityServiceTracer);
			this.DoWorkInternal(cancellationToken);
		}

		protected virtual void DoWorkInternal(CancellationToken cancellationToken)
		{
			try
			{
				base.LogTrace(string.Format("Executing FreeBusy AM probe with {0}", base.EffectiveAuthN));
				base.LatencyMeasurement.Start();
				this.ExecuteFreeBusyProbe(base.Definition.Endpoint);
			}
			catch (Exception innerException)
			{
				while (innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
				}
				ProbeResult result = base.Result;
				result.ExecutionContext += innerException.Message;
				if (string.IsNullOrEmpty(this.probeErrorCode))
				{
					this.probeErrorCode = innerException.GetType().Name;
				}
				if (string.IsNullOrEmpty(this.ProbeErrorMessage))
				{
					this.ProbeErrorMessage = innerException.Message;
				}
			}
			finally
			{
				this.VerifyFreeBusyProbeError();
				this.UpdateProbeResultAttributes();
				this.ThrowProbeError();
			}
			base.LogTrace("FreeBusyProbe succeeded");
		}

		protected virtual void VerifyFreeBusyProbeError()
		{
			if (this.probeErrorCode == 0.ToString())
			{
				this.IsProbeFailed = false;
				return;
			}
			this.BucketIssues();
			if (base.Result.FailureCategory == 7 || base.Result.FailureCategory == 2 || base.Result.FailureCategory == 5)
			{
				this.IsProbeFailed = false;
				return;
			}
			this.IsProbeFailed = true;
		}

		private void BucketIssues()
		{
			if (string.IsNullOrEmpty(this.ProbeErrorMessage))
			{
				base.Result.FailureCategory = 1;
				return;
			}
			if (this.ProbeErrorMessage.ToLower().Contains("the request was aborted") || this.ProbeErrorMessage.ToLower().Contains("the server cannot service this request right now") || this.ProbeErrorMessage.ToLower().Contains("the xml document ended unexpectedly"))
			{
				this.probeErrorCode = "Server Too Busy";
			}
			if (this.ProbeErrorMessage.ToLower().Contains("(401) unauthorized") && this.ProbeErrorMessage.ToLower().Contains("autodiscover"))
			{
				this.probeErrorCode = "AutoDiscover Failed";
			}
			base.LatencyMeasurement.Stop();
			long elapsedMilliseconds = base.LatencyMeasurement.ElapsedMilliseconds;
			if (elapsedMilliseconds > (long)(base.ProbeTimeLimit - 1000) && (string.IsNullOrEmpty(this.probeErrorCode) || string.Compare(this.probeErrorCode, "Unknown", true) == 0))
			{
				this.probeErrorCode = "Probe Time Out";
			}
			if (AvailabilityServiceProbeUtil.KnownErrors.ContainsKey(this.probeErrorCode))
			{
				base.Result.FailureCategory = (int)AvailabilityServiceProbeUtil.KnownErrors[this.probeErrorCode];
				return;
			}
			base.Result.FailureCategory = 1;
		}

		protected virtual void UpdateProbeResultAttributes()
		{
			if (!this.probeErrorCode.Equals(string.Empty))
			{
				base.Result.StateAttribute1 = ((AvailabilityServiceProbeUtil.FailingComponent)base.Result.FailureCategory).ToString();
				base.Result.StateAttribute2 = this.probeErrorCode;
			}
			base.Result.StateAttribute3 = base.Result.StateAttribute12;
			base.Result.StateAttribute12 = base.Definition.Account;
			base.Result.StateAttribute4 = base.Result.StateAttribute13;
			base.Result.StateAttribute13 = base.Definition.SecondaryAccount;
			base.Result.StateAttribute5 = base.Definition.TargetResource;
			ProbeResult result = base.Result;
			result.ExecutionContext += base.TransformResultPairsToString(base.VitalResultPairs);
			if (base.Verbose)
			{
				ProbeResult result2 = base.Result;
				result2.ExecutionContext += base.TransformResultPairsToString(base.VerboseResultPairs);
				ProbeResult result3 = base.Result;
				result3.FailureContext += base.TraceBuilder;
			}
			if (this.exchangeService == null)
			{
				WTFDiagnostics.TraceError(base.Tracer, base.TraceContext, "[AvailabilityServiceProbe.UpdateProbeResultAttributes] this.exchangeService should not have been null!", null, "UpdateProbeResultAttributes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\AvailabilityService\\availabilityserviceprobe.cs", 280);
			}
			else
			{
				base.Result.StateAttribute24 = this.exchangeService.ClientRequestId;
			}
			base.LatencyMeasurement.Stop();
			base.Result.SampleValue = (double)base.LatencyMeasurement.ElapsedMilliseconds;
		}

		protected virtual void ThrowProbeError()
		{
			if (this.IsProbeFailed)
			{
				throw new AvailabilityServiceValidationException(this.ProbeErrorMessage);
			}
		}

		private void ExecuteFreeBusyProbe(string endPoint)
		{
			base.LogTrace(string.Format("Starting FreeBusyProbe with Username: {0} ", base.Definition.Account));
			this.ConfigureExchangeVersion(ExTraceGlobals.AvailabilityServiceTracer);
			this.exchangeService = base.GetExchangeService(base.Definition.Account, base.Definition.AccountPassword, this.traceListener, base.GetEwsUrl(this.traceListener, "SourceEwsUrl"), base.ExchangeServerVersion);
			this.exchangeService.SetComponentId(this.ComponentId);
			this.exchangeService.HttpHeaders.Add("X-ProbeType", "X-MS-Backend-AvailabilityService-Probe");
			TimeWindow window = new TimeWindow(DateTime.UtcNow, DateTime.UtcNow.AddDays(1.0));
			IEnumerable<AttendeeInfo> attendees = new List<AttendeeInfo>
			{
				new AttendeeInfo(base.Definition.SecondaryAccount)
			};
			this.PerformFreeBusyCall(window, attendees);
		}

		private void PerformFreeBusyCall(TimeWindow window, IEnumerable<AttendeeInfo> attendees)
		{
			this.exchangeService.ClientRequestId = Guid.NewGuid().ToString();
			this.exchangeService.ReturnClientRequestId = true;
			base.RetrySoapActionAndThrow(delegate()
			{
				GetUserAvailabilityResults userAvailability = this.exchangeService.GetUserAvailability(attendees, window, 2);
				if (userAvailability != null && userAvailability.AttendeesAvailability != null && userAvailability.AttendeesAvailability.Count > 0)
				{
					this.probeErrorCode = userAvailability.AttendeesAvailability[0].ErrorCode.ToString();
					if (userAvailability.AttendeesAvailability[0].Result != null)
					{
						this.ProbeErrorMessage = userAvailability.AttendeesAvailability[0].ErrorMessage;
						int val = 1024;
						string text = this.exchangeService.HttpResponseHeaders["X-DEBUG_BE_ASGenericInfo"];
						if (!string.IsNullOrEmpty(text))
						{
							text = HttpUtility.HtmlDecode(text);
							this.Result.StateAttribute15 = text.Substring(0, Math.Min(val, text.Length));
						}
						text = this.exchangeService.HttpResponseHeaders["X-DEBUG_BE_GenericErrors"];
						if (!string.IsNullOrEmpty(text))
						{
							text = HttpUtility.HtmlDecode(text);
							this.Result.StateAttribute22 = text.Substring(0, Math.Min(val, text.Length));
						}
					}
				}
			}, "GetUserAvailability", this.exchangeService);
		}

		private void AppendKnownErrorCodes()
		{
			string text = this.ReadAttribute("KnownErrorCodes", string.Empty);
			List<string> errorCodesToBeIgnored = new List<string>();
			if (!string.IsNullOrEmpty(text))
			{
				char[] separator = new char[]
				{
					','
				};
				text.Split(separator, 100).ToList<string>().ForEach(delegate(string r)
				{
					errorCodesToBeIgnored.Add(r.Trim());
				});
				errorCodesToBeIgnored.RemoveAll((string r) => string.IsNullOrWhiteSpace(r));
			}
			foreach (string key in errorCodesToBeIgnored)
			{
				AvailabilityServiceProbeUtil.KnownErrors.Add(key, AvailabilityServiceProbeUtil.FailingComponent.Ignore);
			}
		}

		private void ConfigureExchangeVersion(Trace tracer)
		{
			base.ExchangeServerVersion = 0;
			if (base.Definition.Attributes != null && base.Definition.Attributes.ContainsKey("ExchangeVersion"))
			{
				string text = base.Definition.Attributes["ExchangeVersion"] ?? "Invalid";
				if (!text.Equals(this.exchVerExchange2013))
				{
					throw new ArgumentException("An invalid ExchangeVersion was passed in from the extended attributes. '" + text + "'", "ExchangeVersion");
				}
				base.ExchangeServerVersion = 4;
			}
			WTFDiagnostics.TraceInformation(tracer, base.TraceContext, "Probe ExchangeVersion: " + base.ExchangeServerVersion.ToString(), null, "ConfigureExchangeVersion", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\AvailabilityService\\availabilityserviceprobe.cs", 419);
		}

		private const string OperationName = "GetUserAvailability";

		private const string ClientRequestIDName = "client-request-id";

		public const string ProbeHeaderName = "X-ProbeType";

		public const string ProbeHeaderValue = "X-MS-Backend-AvailabilityService-Probe";

		private readonly string exchVerExchange2013 = 4.ToString();

		private ExchangeService exchangeService;

		private EWSCommon.TraceListener traceListener;

		protected string probeErrorCode = string.Empty;
	}
}

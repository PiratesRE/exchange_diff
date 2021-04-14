using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.CalendarSharing.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Ews.Probes;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OnlineMeeting.Probes
{
	internal abstract class OnlineMeetingCreateProbe : AutodiscoverCommon
	{
		protected HttpWebRequestUtility WebRequestUtil { get; set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				base.Initialize(ExTraceGlobals.OnlineMeetingTracer);
				this.traceListener = new EWSCommon.TraceListener(base.TraceContext, ExTraceGlobals.OnlineMeetingTracer);
				string ewsUrl;
				try
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OnlineMeetingTracer, base.TraceContext, "Getting Source EWS Url", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OnlineMeeting\\OnlineMeetingCreateProbe.cs", 66);
					ewsUrl = base.GetEwsUrl(this.traceListener, base.Definition.Endpoint);
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OnlineMeetingTracer, base.TraceContext, "Returned Source EWS Url: " + ewsUrl, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OnlineMeeting\\OnlineMeetingCreateProbe.cs", 68);
				}
				catch
				{
					base.Result.StateAttribute5 = "Autodiscover failed";
					throw;
				}
				ExchangeService exchangeService = base.GetExchangeService(base.Definition.Account, base.Definition.AccountPassword, this.traceListener, ewsUrl, base.ExchangeServerVersion);
				if (!string.IsNullOrEmpty(this.ComponentId) && !base.IsOutsideInMonitoring)
				{
					exchangeService.SetComponentId(this.ComponentId);
				}
				Appointment appointment = CalendarSharingUtils.CreateTestAppointment(exchangeService);
				string userSipUri = base.Definition.Attributes["UMMbxAccountSipUri"];
				NetworkCredential credential = this.GetCredential(base.Definition.Account, base.Definition.AccountPassword);
				this.CreateOnlineMeeting(credential, appointment.Id.ToString(), userSipUri);
			}
			catch (AggregateException ex)
			{
				WebException ex2 = ex.Flatten().InnerException as WebException;
				HttpWebResponse httpWebResponse = (HttpWebResponse)ex2.Response;
				base.Result.StateAttribute5 = httpWebResponse.StatusCode.ToString();
				throw ex2;
			}
			catch (Exception ex3)
			{
				base.Result.StateAttribute15 = "Exception = " + ex3.ToString();
				throw ex3;
			}
			finally
			{
				base.Result.StateAttribute11 = string.Format("BE Server: {0} FE Server: {1}", "X-DiagInfo", "X-FEServer");
			}
		}

		private void CreateOnlineMeeting(NetworkCredential credential, string calendarItemId, string userSipUri)
		{
			HttpWebRequest ewsRequest = this.GetEwsRequest(credential, calendarItemId, userSipUri);
			OnlineMeetingCreateProbe.RequestState requestState = new OnlineMeetingCreateProbe.RequestState
			{
				WebRequest = ewsRequest,
				SentTimeUtc = DateTime.UtcNow
			};
			ewsRequest.BeginGetResponse(delegate(IAsyncResult x)
			{
				OnlineMeetingCreateProbe.RequestState requestState2 = (OnlineMeetingCreateProbe.RequestState)x.AsyncState;
				try
				{
					using (HttpWebResponse httpWebResponse = (HttpWebResponse)requestState2.WebRequest.EndGetResponse(x))
					{
						requestState2.ResponseReceivedTimeUtc = DateTime.UtcNow;
						requestState2.StatusCode = httpWebResponse.StatusCode.ToString();
						string text = "X-Exchange-GetUserPhoto-Traces";
						if (httpWebResponse.Headers.AllKeys != null && httpWebResponse.Headers.AllKeys.Count<string>() != 0 && httpWebResponse.Headers.AllKeys.Contains(text))
						{
							base.Result.ExecutionContext = httpWebResponse.Headers[text];
						}
						base.Result.StateAttribute1 = base.Definition.Endpoint;
						base.Result.StateAttribute5 = httpWebResponse.StatusCode.ToString();
						using (Stream responseStream = httpWebResponse.GetResponseStream())
						{
							StreamReader streamReader = new StreamReader(responseStream);
							base.Result.StateAttribute2 = streamReader.ReadToEnd();
						}
					}
				}
				catch (WebException ex)
				{
					requestState2.StatusCode = ex.Status.ToString();
					requestState2.ResponseReceivedTimeUtc = DateTime.UtcNow;
					requestState2.Exception = ex;
				}
				catch (Exception exception)
				{
					requestState2.StatusCode = WebExceptionStatus.UnknownError.ToString();
					requestState2.ResponseReceivedTimeUtc = DateTime.UtcNow;
					requestState2.Exception = exception;
				}
				finally
				{
					base.Result.StateAttribute12 = "Sent Time = " + requestState2.SentTimeUtc.ToString();
					base.Result.StateAttribute13 = "Received Time = " + requestState2.ResponseReceivedTimeUtc.ToString();
					base.Result.StateAttribute14 = "Status Code = " + requestState2.StatusCode.ToString();
					this.allDone.Set();
				}
			}, requestState);
			this.allDone.WaitOne();
			if (requestState.Exception != null)
			{
				throw requestState.Exception;
			}
		}

		private NetworkCredential GetCredential(string userName, string password)
		{
			NetworkCredential networkCredential = new NetworkCredential
			{
				UserName = userName,
				Password = password
			};
			if (userName.Contains("@"))
			{
				string[] array = userName.Split(new char[]
				{
					'@'
				});
				if (array.Length == 2)
				{
					networkCredential.UserName = array[0];
					networkCredential.Domain = array[1];
				}
			}
			return networkCredential;
		}

		protected abstract HttpWebRequest GetEwsRequest(NetworkCredential credential, string calendarItemId, string userSipUri);

		protected const string CompleteEndpointStr = "{0}?sipUri={1},itemId={2}";

		public const string MailboxDatabaseSipUriParameterName = "UMMbxAccountSipUri";

		protected ManualResetEvent allDone = new ManualResetEvent(false);

		protected EWSCommon.TraceListener traceListener;

		private class RequestState
		{
			public DateTime SentTimeUtc { get; set; }

			public DateTime ResponseReceivedTimeUtc { get; set; }

			public HttpWebRequest WebRequest { get; set; }

			public Exception Exception { get; set; }

			public string StatusCode { get; set; }
		}
	}
}

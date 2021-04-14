using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Psws.Probes
{
	public class PswsProbeBase : ProbeWorkItem
	{
		protected virtual string ComponentId
		{
			get
			{
				return "PswsProbeBase";
			}
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Entering Dowork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsProbeBase.cs", 93);
			try
			{
				this.DoInitialize();
				this.DoTask(cancellationToken);
			}
			catch (ApplicationException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new ApplicationException(this.GetProbeInfo() + "Failed in Psws DoWork.", innerException);
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Leaving Dowork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsProbeBase.cs", 109);
		}

		protected virtual void DoInitialize()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Entering DoInitialize", null, "DoInitialize", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsProbeBase.cs", 117);
			this.url = base.Definition.Endpoint;
			this.urlSuffix = base.Definition.Attributes["PswsMailboxUrlSuffix"];
			this.requestUrl = string.Format("{0}{1}", this.url, this.urlSuffix);
			this.probeInfo = this.GetProbeInfo();
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Leaving DoInitialize", null, "DoInitialize", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsProbeBase.cs", 125);
		}

		private void DoTask(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Entering DoTask", null, "DoTask", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsProbeBase.cs", 134);
			HttpWebRequest request = this.GetRequest();
			Task<WebResponse> task = Task.Factory.FromAsync<WebResponse>(delegate(AsyncCallback asyncCallback, object state)
			{
				this.startTime = DateTime.UtcNow;
				return request.BeginGetResponse(asyncCallback, state);
			}, new Func<IAsyncResult, WebResponse>(request.EndGetResponse), request);
			task.Continue(new Func<Task<WebResponse>, string>(this.GetResponse), cancellationToken, TaskContinuationOptions.NotOnCanceled);
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Leaving DoTask", null, "DoTask", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsProbeBase.cs", 149);
		}

		private string GetResponse(Task<WebResponse> task)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Entering GetResponse", null, "GetResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsProbeBase.cs", 159);
			string text = null;
			if (!task.IsFaulted)
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)task.Result;
				if (httpWebResponse.StatusCode == HttpStatusCode.OK)
				{
					try
					{
						using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
						{
							text = streamReader.ReadToEnd();
						}
						base.Result.SampleValue = (DateTime.UtcNow - this.startTime).TotalMilliseconds;
						string text2 = string.Format("Psws request success. status : {0} , X-FEServer : {1} , X-CalculatedBETarget : {2} , Response : {3}", new object[]
						{
							httpWebResponse.StatusCode,
							httpWebResponse.Headers["X-FEServer"],
							httpWebResponse.Headers["X-CalculatedBETarget"],
							text
						});
						WTFDiagnostics.TraceInformation(ExTraceGlobals.PswsTracer, base.TraceContext, text2, null, "GetResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsProbeBase.cs", 192);
						base.Result.StateAttribute1 = text2;
						goto IL_1A4;
					}
					catch (Exception ex)
					{
						base.Result.SampleValue = (DateTime.UtcNow - this.startTime).TotalMilliseconds;
						throw new ApplicationException(string.Format("probe information : {0} Read success response body failed , message : {1} . ", this.probeInfo, ex.ToString()));
					}
				}
				base.Result.SampleValue = (DateTime.UtcNow - this.startTime).TotalMilliseconds;
				string failedResponseMessage = this.GetFailedResponseMessage(httpWebResponse, string.Empty);
				throw new ApplicationException(string.Format("Psws response code is not ok , message : ", failedResponseMessage));
			}
			this.HandleFailedRequest(task);
			IL_1A4:
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Leaving GetResponse", null, "GetResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsProbeBase.cs", 209);
			return text;
		}

		private void HandleFailedRequest(Task task)
		{
			WTFDiagnostics.TraceError<AggregateException>(ExTraceGlobals.PswsTracer, base.TraceContext, "HandleFailedRequest : Task exception= {0}", task.Exception, null, "HandleFailedRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsProbeBase.cs", 219);
			if (task.Exception == null)
			{
				WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Leaving HandleFailedRequest", null, "HandleFailedRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsProbeBase.cs", 239);
				return;
			}
			WebException ex = task.Exception.Flatten().InnerException as WebException;
			if (ex != null && ex.Response != null && ex.Response is HttpWebResponse)
			{
				HttpWebResponse response = ex.Response as HttpWebResponse;
				string failedResponseMessage = this.GetFailedResponseMessage(response, ex.ToString());
				throw new ApplicationException(failedResponseMessage, ex);
			}
			throw new ApplicationException(string.Format("probe information : {0} UnWebException catched! Message : {1}", this.probeInfo, task.Exception.Flatten().ToString()));
		}

		private string GetFailedResponseMessage(HttpWebResponse response, string exceptionMessage)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Entering GetFailedResponseMessage", null, "GetFailedResponseMessage", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsProbeBase.cs", 248);
			string text = string.Empty;
			try
			{
				using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
				{
					text = streamReader.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				text = string.Format("Read exception response body failed , message : {0} . ", ex.ToString());
			}
			string str = string.Format("Psws request error. Error status : {0} , X-FEServer : {1} , X-CalculatedBETarget : {2} , X-DiagInfo : {3} , Error message : {4} , Exception string: {5}", new object[]
			{
				response.StatusCode,
				response.Headers["X-FEServer"],
				response.Headers["X-CalculatedBETarget"],
				response.Headers["X-DiagInfo"],
				text,
				exceptionMessage
			});
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Leaving GetFailedResponseMessage", null, "GetFailedResponseMessage", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsProbeBase.cs", 274);
			return this.probeInfo + str;
		}

		private string GetProbeInfo()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Entering GetProbeInfo", null, "GetProbeInfo", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsProbeBase.cs", 284);
			string result = string.Format("Probe Information : Endpoint = {0} , Account = {1} , RequestURL = {2}. ", base.Definition.Endpoint, base.Definition.Account, this.requestUrl);
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Leaving GetProbeInfo", null, "GetProbeInfo", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsProbeBase.cs", 293);
			return result;
		}

		protected virtual HttpWebRequest GetRequest()
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.requestUrl);
			httpWebRequest.UserAgent = PswsProbeBase.DefaultUserAgent;
			httpWebRequest.AllowAutoRedirect = false;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Headers.Add(PswsProbeBase.ClientApplication, ExchangeRunspaceConfigurationSettings.ExchangeApplication.ActiveMonitor.ToString());
			return httpWebRequest;
		}

		public static readonly string PswsMailboxUrlSuffixAttrName = "PswsMailboxUrlSuffix";

		public static readonly string AccessTokenTypeAttrName = "AccessTokenType";

		internal static readonly string DefaultUserAgent = "PswsMonitor/1.0";

		internal static readonly string ClientApplication = "clientApplication";

		protected string probeInfo;

		protected string url;

		protected string urlSuffix;

		protected string requestUrl;

		protected DateTime startTime;
	}
}

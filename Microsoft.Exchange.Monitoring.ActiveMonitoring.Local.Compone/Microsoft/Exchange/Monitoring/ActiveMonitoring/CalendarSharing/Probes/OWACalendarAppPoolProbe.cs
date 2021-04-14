using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.CalendarSharing.Probes
{
	public class OWACalendarAppPoolProbe : ProbeWorkItem
	{
		public HttpWebRequestUtility WebRequestUtil { get; set; }

		public string ExpectedStatusCode { get; private set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.Configure();
			HttpWebResponse httpWebResponse = null;
			try
			{
				HttpWebRequest request = this.GetRequest();
				Task<WebResponse> task = this.WebRequestUtil.SendRequest(request);
				httpWebResponse = (HttpWebResponse)task.Result;
				if (!(httpWebResponse.StatusCode.ToString() == this.ExpectedStatusCode))
				{
					throw new AggregateException(new Exception[]
					{
						new WebException("Non-242 response received", new Exception(string.Format("Unexpected response received. Expected 242 received {0}", httpWebResponse.StatusCode)), WebExceptionStatus.UnknownError, httpWebResponse)
					});
				}
				base.Result.StateAttribute5 = 0.ToString();
			}
			catch (AggregateException ex)
			{
				WebException ex2 = ex.Flatten().InnerException as WebException;
				if (ex2 != null)
				{
					base.Result.StateAttribute5 = ex2.GetType().ToString();
					httpWebResponse = (HttpWebResponse)ex2.Response;
					string text = httpWebResponse.StatusCode.ToString();
					base.Result.StateAttribute4 = text;
					if (this.ErrorCodesToBeIgnored.Contains(text))
					{
						base.Result.StateAttribute5 = "KnownErrorWasIgnored";
						return;
					}
					ProbeResult result = base.Result;
					result.ExecutionContext += ex.Message;
				}
				throw;
			}
			finally
			{
				if (httpWebResponse != null)
				{
					httpWebResponse.Close();
				}
			}
		}

		private void Configure()
		{
			this.ExpectedStatusCode = this.ReadAttribute("ExpectedStatusCode", "242");
			string text = this.ReadAttribute("KnownErrorCodes", string.Empty);
			if (!string.IsNullOrEmpty(text))
			{
				char[] separator = new char[]
				{
					','
				};
				text.Split(separator, 100).ToList<string>().ForEach(delegate(string r)
				{
					this.ErrorCodesToBeIgnored.Add(r.Trim());
				});
				this.ErrorCodesToBeIgnored.RemoveAll((string r) => string.IsNullOrWhiteSpace(r));
			}
		}

		protected HttpWebRequest GetRequest()
		{
			this.WebRequestUtil = new HttpWebRequestUtility(base.TraceContext);
			HttpWebRequest httpWebRequest = this.WebRequestUtil.CreateBasicHttpWebRequest(base.Definition.Endpoint, false);
			httpWebRequest.ContentType = "text/xml";
			httpWebRequest.Method = "GET";
			return httpWebRequest;
		}

		public const string KnownErrorCodesKey = "KnownErrorCodes";

		public const string ExpectedStatusCodeKey = "ExpectedStatusCode";

		public const string KnownErrorWasIgnored = "KnownErrorWasIgnored";

		private const string DefaultExpectedStatusCode = "242";

		protected List<string> ErrorCodesToBeIgnored = new List<string>
		{
			HttpStatusCode.OK.ToString()
		};
	}
}

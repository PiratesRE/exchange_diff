using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Oab.Probes
{
	public class OabProtocolProbe : OabBaseLocalProbe
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			HttpWebRequest request = base.GetRequest();
			request.Headers.Add("X-WLID-MemberName", base.Definition.Account);
			Task<WebResponse> task = base.WebRequestUtil.SendRequest(request);
			try
			{
				WebResponse result = task.Result;
			}
			catch (AggregateException ex)
			{
				WebException ex2 = ex.Flatten().InnerException as WebException;
				HttpWebResponse httpWebResponse = (HttpWebResponse)ex2.Response;
				if (httpWebResponse.StatusCode != HttpStatusCode.Forbidden)
				{
					base.Result.StateAttribute5 = httpWebResponse.StatusCode.ToString();
					throw ex2;
				}
			}
		}
	}
}

using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp
{
	internal class EcpPing : BaseTestStep
	{
		protected override TestId Id
		{
			get
			{
				return TestId.EcpPing;
			}
		}

		public EcpPing(Uri uri)
		{
			this.Uri = new Uri(uri, "exhealth.check");
		}

		public Uri Uri { get; private set; }

		protected override void StartTest()
		{
			this.session.PersistentHeaders.Add("X-IsFromCafe", "1");
			this.session.BeginGet(this.Id, this.Uri.ToString(), delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.PingResponseReceived), tempResult);
			}, null);
		}

		private void PingResponseReceived(IAsyncResult result)
		{
			this.session.EndGet<object>(result, delegate(HttpWebResponseWrapper response)
			{
				string text = response.Headers["X-MSExchApplicationHealthHandlerStatus"];
				if (!text.Equals("Passed", StringComparison.OrdinalIgnoreCase))
				{
					throw new MissingKeywordException(MonitoringWebClientStrings.HealthCheckRequestFailed, response.Request, response, "Passed");
				}
				return null;
			});
			base.ExecutionCompletedSuccessfully();
		}
	}
}

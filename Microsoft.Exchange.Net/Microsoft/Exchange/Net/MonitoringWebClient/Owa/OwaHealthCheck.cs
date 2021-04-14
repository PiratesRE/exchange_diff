using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa
{
	internal class OwaHealthCheck : BaseTestStep
	{
		public Uri Uri { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.OwaHealthCheck;
			}
		}

		public OwaHealthCheck(Uri uri)
		{
			this.Uri = uri;
		}

		protected override void StartTest()
		{
			this.session.BeginGet(this.Id, new Uri(this.Uri, "exhealth.check").ToString(), delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.HealthCheckResponseReceived), tempResult);
			}, null);
		}

		private void HealthCheckResponseReceived(IAsyncResult result)
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

		private const TestId ID = TestId.OwaHealthCheck;
	}
}

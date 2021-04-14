using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa
{
	internal class OwaPing : BaseTestStep
	{
		public Uri Uri { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.OwaPing;
			}
		}

		public OwaPing(Uri uri)
		{
			this.Uri = uri;
		}

		protected override void StartTest()
		{
			this.session.BeginGet(this.Id, new Uri(this.Uri, "ev.owa?oeh=1&ns=Monitoring&ev=Ping").ToString(), delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.PingResponseReceived), tempResult);
			}, null);
		}

		private void PingResponseReceived(IAsyncResult result)
		{
			this.session.EndGet<object>(result, null);
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.OwaPing;
	}
}

using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Rws
{
	internal class RwsCall : BaseTestStep
	{
		public Uri Uri { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.RwsCall;
			}
		}

		public RwsCall(Uri uri)
		{
			this.Uri = uri;
		}

		protected override void StartTest()
		{
			this.session.BeginGetFollowingRedirections(this.Id, this.Uri.ToString(), delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.EndpointCallResponseReceived), tempResult);
			}, null);
		}

		private void EndpointCallResponseReceived(IAsyncResult result)
		{
			this.session.EndGet<object>(result, null);
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.RwsCall;
	}
}

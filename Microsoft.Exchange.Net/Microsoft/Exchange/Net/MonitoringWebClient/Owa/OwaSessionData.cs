using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa
{
	internal class OwaSessionData : BaseTestStep
	{
		public Uri Uri { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.OwaSessionData;
			}
		}

		public OwaSessionData(Uri uri)
		{
			this.Uri = uri;
		}

		protected override void StartTest()
		{
			this.session.BeginPost(this.Id, new Uri(this.Uri, "sessiondata.ashx").ToString(), OwaSessionData.EmptyRequestBody, "application/x-www-form-urlencoded", delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.SessionDataResponseReceived), tempResult);
			}, null);
		}

		private void SessionDataResponseReceived(IAsyncResult result)
		{
			this.session.EndGet<object>(result, null);
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.OwaSessionData;

		private static readonly RequestBody EmptyRequestBody = RequestBody.Format(string.Empty, new object[0]);
	}
}

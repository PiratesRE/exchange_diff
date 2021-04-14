using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp
{
	internal class EcpWebServiceCall : BaseTestStep
	{
		public Uri Uri { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.EcpWebServiceCall;
			}
		}

		public EcpWebServiceCall(Uri uri)
		{
			this.Uri = uri;
		}

		protected override void StartTest()
		{
			string value = this.session.CookieContainer.GetCookies(this.Uri)["msExchEcpCanary"].Value;
			this.session.BeginPost(this.Id, new Uri(this.Uri, "RulesEditor/InboxRules.svc/GetList?msExchEcpCanary=" + value).ToString(), RequestBody.Format("{\"filter\":{\"SearchText\":\"\"},\"sort\":{\"Direction\":0,\"PropertyName\":\"Name\"}}", new object[0]), "application/json", delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.WebServiceResponseReceived), tempResult);
			}, null);
		}

		private void WebServiceResponseReceived(IAsyncResult result)
		{
			this.session.EndPost<object>(result, null);
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.EcpWebServiceCall;
	}
}

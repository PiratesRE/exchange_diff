using System;
using Microsoft.Exchange.Net.MonitoringWebClient.Owa.Parsers;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa
{
	internal class OwaStaticPage : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public ITestFactory TestFactory { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.OwaStaticPage;
			}
		}

		public OwaStaticPage(Uri uri, ITestFactory factory)
		{
			this.Uri = uri;
			this.TestFactory = factory;
		}

		protected override void Finally()
		{
			this.session.CloseConnections();
		}

		protected override void StartTest()
		{
			this.session.BeginGet(this.Id, new Uri(this.Uri, "auth/preload.aspx").ToString(), delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.PreloadPageResponseReceived), tempResult);
			}, null);
		}

		private void PreloadPageResponseReceived(IAsyncResult result)
		{
			OwaPreloadPage owaPreloadPage = this.session.EndGet<OwaPreloadPage>(result, (HttpWebResponseWrapper response) => OwaPreloadPage.Parse(response));
			string str = "/themes/resources/clear1x1.gif";
			if (owaPreloadPage.CdnUri.PathAndQuery.Contains("/15."))
			{
				str = "/owa2/scripts/microsoft.exchange.clients.owa2.client.core.framework.js";
			}
			UriBuilder uriBuilder = new UriBuilder(owaPreloadPage.CdnUri);
			UriBuilder uriBuilder2 = uriBuilder;
			uriBuilder2.Path += str;
			ITestStep testStep = this.TestFactory.CreateOwaDownloadStaticFileStep(uriBuilder.Uri);
			testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.OwaDownloadStaticFileStepCompleted), tempResult);
			}, testStep);
		}

		private void OwaDownloadStaticFileStepCompleted(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.OwaStaticPage;
	}
}

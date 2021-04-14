using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp
{
	internal abstract class EcpWebServiceCallBase : BaseTestStep
	{
		public Uri Uri { get; private set; }

		protected abstract Uri WebServiceRelativeUri { get; }

		protected abstract RequestBody RequestBody { get; }

		public EcpWebServiceCallBase(Uri uri, Func<Uri, ITestStep> getFollowingTestStep = null)
		{
			this.Uri = uri;
			this.getFollowingTestStep = getFollowingTestStep;
		}

		protected override void StartTest()
		{
			string value = this.session.CookieContainer.GetCookies(this.Uri)["msExchEcpCanary"].Value;
			Uri baseUri = new Uri(this.Uri, this.WebServiceRelativeUri);
			Uri relativeUri = new Uri(string.Format("?msExchEcpCanary={0}", value), UriKind.Relative);
			Uri uri = new Uri(baseUri, relativeUri);
			this.session.BeginPost(this.Id, uri.ToString(), this.RequestBody, "application/json", delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.WebServiceResponseReceived), tempResult);
			}, null);
		}

		private void WebServiceResponseReceived(IAsyncResult result)
		{
			this.session.EndPost<object>(result, null);
			ITestStep testStep = (this.getFollowingTestStep != null) ? this.getFollowingTestStep(this.Uri) : null;
			if (testStep != null)
			{
				testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
				{
					base.AsyncCallbackWrapper(new AsyncCallback(this.FollowingTestStepCompleted), tempResult);
				}, testStep);
				return;
			}
			base.ExecutionCompletedSuccessfully();
		}

		private void FollowingTestStepCompleted(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			base.ExecutionCompletedSuccessfully();
		}

		private Func<Uri, ITestStep> getFollowingTestStep;
	}
}

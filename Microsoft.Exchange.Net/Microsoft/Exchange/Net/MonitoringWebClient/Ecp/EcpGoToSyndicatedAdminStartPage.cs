using System;
using Microsoft.Exchange.Net.MonitoringWebClient.Ecp.Parsers;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp
{
	internal class EcpGoToSyndicatedAdminStartPage : BaseTestStep
	{
		protected override TestId Id
		{
			get
			{
				return TestId.EcpGoToSyndicatedAdminStartPage;
			}
		}

		public Uri Uri { get; private set; }

		public string PartnerDomain { get; private set; }

		public string TargetDomain { get; private set; }

		public EcpGoToSyndicatedAdminStartPage(Uri uri, string partnerDomain, string targetDomain, Func<Uri, ITestStep> getFollowingTestStep)
		{
			this.Uri = uri;
			this.PartnerDomain = partnerDomain;
			this.TargetDomain = targetDomain;
			this.getFollowingTestStep = getFollowingTestStep;
		}

		protected override void StartTest()
		{
			this.session.BeginGetFollowingRedirections(this.Id, new Uri(this.Uri, string.Format("/ecp/@{0}/?realm={1}&mkt=en-US&exsvurl=1", this.TargetDomain, this.PartnerDomain)).ToString(), delegate(IAsyncResult result)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.SyndicatedAdminStartPageReceived), result);
			}, null);
		}

		private void SyndicatedAdminStartPageReceived(IAsyncResult result)
		{
			EcpHelpDeskStartPage ecpHelpDeskStartPage = this.session.EndGetFollowingRedirections<EcpHelpDeskStartPage>(result, (HttpWebResponseWrapper response) => EcpHelpDeskStartPage.Parse(response));
			this.Uri = ecpHelpDeskStartPage.FinalUri;
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

		private const TestId ID = TestId.EcpGoToSyndicatedAdminStartPage;

		private Func<Uri, ITestStep> getFollowingTestStep;
	}
}

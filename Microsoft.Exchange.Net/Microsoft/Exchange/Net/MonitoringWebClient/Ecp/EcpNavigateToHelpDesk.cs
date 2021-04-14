using System;
using System.Net;
using Microsoft.Exchange.Net.MonitoringWebClient.Ecp.Parsers;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp
{
	internal class EcpNavigateToHelpDesk : BaseTestStep
	{
		protected override TestId Id
		{
			get
			{
				return TestId.EcpNavigateToHelpDesk;
			}
		}

		public Uri Uri { get; private set; }

		public string TargetMailbox { get; private set; }

		public EcpNavigateToHelpDesk(Uri uri, string targetMailbox, Func<Uri, ITestStep> getFollowingTestStep)
		{
			this.Uri = uri;
			this.TargetMailbox = targetMailbox;
			this.getFollowingTestStep = getFollowingTestStep;
		}

		protected override void StartTest()
		{
			this.session.BeginGetFollowingRedirections(this.Id, new Uri(this.Uri, string.Format("/ecp/{0}/?exsvurl=1", this.TargetMailbox)).ToString(), delegate(IAsyncResult result)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.TargetOptionPageReceived), result);
			}, null);
		}

		private void TargetOptionPageReceived(IAsyncResult result)
		{
			object obj = this.session.EndGetFollowingRedirections<object>(result, delegate(HttpWebResponseWrapper response)
			{
				LiveIdCompactTokenPage result2;
				if (LiveIdCompactTokenPage.TryParse(response, out result2))
				{
					return result2;
				}
				return EcpHelpDeskStartPage.Parse(response);
			});
			if (!(obj is EcpHelpDeskStartPage))
			{
				if (obj is LiveIdCompactTokenPage)
				{
					LiveIdCompactTokenPage liveIdCompactTokenPage = obj as LiveIdCompactTokenPage;
					this.session.BeginPost(this.Id, liveIdCompactTokenPage.PostUrl, RequestBody.Format(liveIdCompactTokenPage.HiddenFieldsString, new object[0]), "application/x-www-form-urlencoded", delegate(IAsyncResult resultTemp)
					{
						base.AsyncCallbackWrapper(new AsyncCallback(this.CompactTicketPostResponseRecived), resultTemp);
					}, null);
				}
				return;
			}
			EcpHelpDeskStartPage ecpHelpDeskStartPage = obj as EcpHelpDeskStartPage;
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

		private void CompactTicketPostResponseRecived(IAsyncResult result)
		{
			Uri uri = this.session.EndPost<Uri>(result, delegate(HttpWebResponseWrapper response)
			{
				if (response.StatusCode != HttpStatusCode.Found)
				{
					return response.Request.RequestUri;
				}
				return new Uri(response.Headers["Location"]);
			});
			if (uri != null && uri != this.Uri)
			{
				this.Uri = uri;
			}
			this.session.BeginGetFollowingRedirections(this.Id, this.Uri.ToString(), delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.TargetOptionPageReceived), tempResult);
			}, null);
		}

		private const TestId ID = TestId.EcpNavigateToHelpDesk;

		private Func<Uri, ITestStep> getFollowingTestStep;
	}
}

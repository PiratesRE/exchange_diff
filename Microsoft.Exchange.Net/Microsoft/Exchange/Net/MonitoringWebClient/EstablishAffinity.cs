using System;
using System.Net;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class EstablishAffinity : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public string ServerToHit { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.EstablishAffinity;
			}
		}

		public EstablishAffinity(Uri uri, string serverToHit)
		{
			this.Uri = uri;
			this.ServerToHit = serverToHit;
		}

		protected override void StartTest()
		{
			this.session.BeginGet(this.Id, this.Uri.ToString(), delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.ResponseReceived), tempResult);
			}, null);
		}

		private void ResponseReceived(IAsyncResult result)
		{
			try
			{
				var <>f__AnonymousType = this.session.EndGet(result, Enum.GetValues(typeof(HttpStatusCode)) as HttpStatusCode[], (HttpWebResponseWrapper response) => new
				{
					StatusCode = response.StatusCode,
					RedirectUrl = response.Headers["Location"],
					RespondingServer = response.RespondingFrontEndServer
				});
				if (<>f__AnonymousType.RespondingServer != null && (<>f__AnonymousType.StatusCode == HttpStatusCode.OK || <>f__AnonymousType.StatusCode == HttpStatusCode.Found) && this.ServerToHit.StartsWith(<>f__AnonymousType.RespondingServer, StringComparison.OrdinalIgnoreCase))
				{
					base.ExecutionCompletedSuccessfully();
					return;
				}
			}
			catch (ScenarioException)
			{
			}
			this.responseCount++;
			if (this.responseCount >= 50)
			{
				throw new ScenarioException(MonitoringWebClientStrings.NoResponseFromServerThroughPublicName(this.ServerToHit, this.responseCount, this.Uri), null, RequestTarget.Owa, FailureReason.ServerUnreachable, FailingComponent.Networking, "AffinityFailure");
			}
			this.session.CookieContainer = new ExCookieContainer();
			this.session.CloseConnections();
			this.session.BeginGet(this.Id, this.Uri.ToString(), delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.ResponseReceived), tempResult);
			}, null);
		}

		private const TestId ID = TestId.EstablishAffinity;

		internal const int MaxRequests = 50;

		private int responseCount;
	}
}

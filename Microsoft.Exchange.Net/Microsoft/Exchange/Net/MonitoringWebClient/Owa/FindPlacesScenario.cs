using System;
using System.Security;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa
{
	internal class FindPlacesScenario : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public string UserName { get; private set; }

		public string UserDomain { get; private set; }

		public SecureString Password { get; private set; }

		public ITestFactory TestFactory { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.OwaFindPlacesScenario;
			}
		}

		public FindPlacesScenario(Uri uri, string userName, string userDomain, SecureString password, ITestFactory factory)
		{
			this.Uri = uri;
			this.UserName = userName;
			this.UserDomain = userDomain;
			this.Password = password;
			this.TestFactory = factory;
		}

		protected override void Finally()
		{
			this.session.CloseConnections();
		}

		protected override void StartTest()
		{
			this.session.PersistentHeaders.Add("X-OWA-ActionName", "Monitoring");
			ITestStep testStep = this.TestFactory.CreateAuthenticateStep(this.Uri, this.UserName, this.UserDomain, this.Password, null, this.TestFactory);
			testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.AuthenticationCompleted), tempResult);
			}, testStep);
		}

		private void AuthenticationCompleted(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			Uri uri = testStep.Result as Uri;
			if (uri != null)
			{
				this.Uri = uri;
			}
			ITestStep testStep2 = this.TestFactory.CreateOwaStartPageStep(this.Uri);
			testStep2.BeginExecute(this.session, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.StartPageTestCompleted), tempResult);
			}, testStep2);
		}

		private void StartPageTestCompleted(IAsyncResult result)
		{
			OwaStartPage owaStartPage = result.AsyncState as OwaStartPage;
			owaStartPage.EndExecute(result);
			string bodyFormat = string.Format("{{\"Query\":\"{0}\",\"Sources\":{1},\"MaxResults\":12,\"Culture\":\"en-US\"}}", "1 Microsoft Way, Redmond, WA, US", "2");
			RequestBody requestBody = RequestBody.Format(bodyFormat, new object[0]);
			ITestStep testStep = this.TestFactory.CreateOwaWebServiceStep(this.Uri, "FindPlaces", requestBody);
			testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.FindPlacesLocationRequestCompleted), tempResult);
			}, testStep);
		}

		private void FindPlacesLocationRequestCompleted(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			string bodyFormat = string.Format("{{\"Query\":\"{0}\",\"Sources\":{1},\"MaxResults\":12,\"Culture\":\"en-US\"}}", "Starbucks, Redmond, WA", "4");
			RequestBody requestBody = RequestBody.Format(bodyFormat, new object[0]);
			ITestStep testStep2 = this.TestFactory.CreateOwaWebServiceStep(this.Uri, "FindPlaces", requestBody);
			testStep2.BeginExecute(this.session, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.FindPlacesPhonebookRequestCompleted), tempResult);
			}, testStep2);
		}

		private void FindPlacesPhonebookRequestCompleted(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			this.KickoffLogoffStep();
		}

		private void KickoffLogoffStep()
		{
			ITestStep testStep = this.TestFactory.CreateLogoffStep(this.Uri, "logoff.owa");
			testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.LogOffStepCompleted), tempResult);
			}, testStep);
		}

		private void LogOffStepCompleted(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.OwaFindPlacesScenario;

		private const string LocationServices = "2";

		private const string PhonebookServices = "4";

		private const string FindPlacesRequestBody = "{{\"Query\":\"{0}\",\"Sources\":{1},\"MaxResults\":12,\"Culture\":\"en-US\"}}";
	}
}

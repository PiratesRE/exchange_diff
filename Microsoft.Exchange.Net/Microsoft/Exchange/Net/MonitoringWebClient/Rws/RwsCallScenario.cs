using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Rws
{
	internal class RwsCallScenario : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public RwsAuthenticationInfo AuthenticationInfo { get; private set; }

		public ITestFactory TestFactory { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.RwsCallScenario;
			}
		}

		public RwsCallScenario(Uri uri, RwsAuthenticationInfo authenticationInfo, ITestFactory factory)
		{
			this.Uri = uri;
			this.AuthenticationInfo = authenticationInfo;
			this.TestFactory = factory;
		}

		protected override void Finally()
		{
			this.session.CloseConnections();
		}

		protected override void StartTest()
		{
			ITestStep testStep = this.TestFactory.CreateRwsAuthenticateStep(this.Uri, this.AuthenticationInfo, this.TestFactory);
			testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.AuthenticationCompleted), tempResult);
			}, testStep);
		}

		private void AuthenticationCompleted(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			ITestStep testStep2 = this.TestFactory.CreateRwsCallStep(this.Uri);
			testStep2.BeginExecute(this.session, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.EndpointCallCompleted), tempResult);
			}, testStep2);
		}

		private void EndpointCallCompleted(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.RwsCallScenario;
	}
}

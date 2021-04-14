using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa
{
	internal class OwaHealthCheckScenario : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public ITestFactory TestFactory { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.OwaHealthCheckScenario;
			}
		}

		public OwaHealthCheckScenario(Uri uri, ITestFactory factory)
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
			ITestStep testStep = this.TestFactory.CreateOwaHealthCheckStep(this.Uri);
			testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.HealthCheckStepCompleted), tempResult);
			}, testStep);
		}

		private void HealthCheckStepCompleted(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.OwaHealthCheckScenario;
	}
}

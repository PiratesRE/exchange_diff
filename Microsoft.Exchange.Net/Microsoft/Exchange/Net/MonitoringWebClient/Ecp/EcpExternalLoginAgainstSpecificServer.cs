using System;
using System.Security;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp
{
	internal class EcpExternalLoginAgainstSpecificServer : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public string UserName { get; private set; }

		public string UserDomain { get; private set; }

		public SecureString Password { get; private set; }

		public string ServerToHit { get; private set; }

		public ITestFactory TestFactory { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.EcpExternalLoginAgainstSpecificServer;
			}
		}

		public EcpExternalLoginAgainstSpecificServer(Uri uri, string userName, string userDomain, SecureString password, ITestFactory factory, string serverShortName)
		{
			this.Uri = uri;
			this.UserName = userName;
			this.UserDomain = userDomain;
			this.Password = password;
			this.TestFactory = factory;
			this.ServerToHit = serverShortName;
		}

		protected override void Finally()
		{
			this.session.CloseConnections();
		}

		protected override void StartTest()
		{
			ITestStep testStep = this.TestFactory.CreateEstablishAffinityStep(this.Uri, this.ServerToHit);
			testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.AffinityEstablished), tempResult);
			}, testStep);
		}

		private void AffinityEstablished(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			ITestStep testStep2 = this.TestFactory.CreateEcpLoginScenario(this.Uri, this.UserName, this.UserDomain, this.Password, this.TestFactory);
			testStep2.BeginExecute(this.session, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.LoginScenarioCompleted), tempResult);
			}, testStep2);
		}

		private void LoginScenarioCompleted(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.EcpExternalLoginAgainstSpecificServer;
	}
}

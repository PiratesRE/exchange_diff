using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Rws
{
	internal class RwsAuthentication : BaseTestStep
	{
		public ITestFactory TestFactory { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.RwsAuthentication;
			}
		}

		public Uri Uri { get; private set; }

		public RwsAuthenticationInfo AuthenticationInfo { get; private set; }

		public RwsAuthentication(Uri uri, RwsAuthenticationInfo authenticationInfo, ITestFactory factory)
		{
			this.Uri = uri;
			this.AuthenticationInfo = authenticationInfo;
			this.TestFactory = factory;
		}

		protected override void StartTest()
		{
			ITestStep testStep;
			if (this.AuthenticationInfo.AuthenticationType == RwsAuthenticationType.Brick)
			{
				testStep = this.TestFactory.CreateRwsBrickAuthenticateStep(this.AuthenticationInfo.Token, this.Uri);
			}
			else
			{
				testStep = this.TestFactory.CreateIisAuthenticateStep(this.Uri, this.AuthenticationInfo.UserName, this.AuthenticationInfo.UserDomain, this.AuthenticationInfo.Password);
			}
			testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.AuthenticationStepFinished), tempResult);
			}, testStep);
		}

		private void AuthenticationStepFinished(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.RwsAuthentication;
	}
}

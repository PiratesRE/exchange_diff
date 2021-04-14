using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa
{
	internal class OwaCertificateRevocationCheckScenario : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public ITestFactory TestFactory { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.OwaCertificateRevocationCheckScenario;
			}
		}

		public OwaCertificateRevocationCheckScenario(Uri uri, ITestFactory factory)
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
			this.session.SslValidationOptions = SslValidationOptions.Revocation;
			this.session.BeginGet(this.Id, this.Uri.ToString(), delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.ResponseReceived), tempResult);
			}, null);
		}

		private void ResponseReceived(IAsyncResult result)
		{
			this.session.EndGet<object>(result, HttpSession.AllHttpStatusCodes, (HttpWebResponseWrapper response) => null);
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.OwaCertificateRevocationCheckScenario;
	}
}

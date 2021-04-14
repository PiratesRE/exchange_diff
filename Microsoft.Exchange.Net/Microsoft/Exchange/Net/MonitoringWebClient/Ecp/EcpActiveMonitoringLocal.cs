using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp
{
	internal class EcpActiveMonitoringLocal : EcpFeatureTestStepBase
	{
		protected override TestId Id
		{
			get
			{
				return TestId.EcpActiveMonitoringLocal;
			}
		}

		public EcpActiveMonitoringLocal(Uri uri, string userName, string userDomain, AuthenticationParameters authenticationParameters, ITestFactory factory, Func<EcpStartPage, ITestStep> getFollowingTestStep) : base(uri, userName, userDomain, null, authenticationParameters, factory)
		{
			this.getFollowingTestStep = getFollowingTestStep;
		}

		protected override ITestStep GetFeatureTestStep(EcpStartPage startPage)
		{
			if (this.getFollowingTestStep == null)
			{
				return null;
			}
			return this.getFollowingTestStep(startPage);
		}

		private const TestId ID = TestId.EcpActiveMonitoringLocal;

		private Func<EcpStartPage, ITestStep> getFollowingTestStep;
	}
}

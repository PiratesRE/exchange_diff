using System;
using System.Security;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp
{
	internal class EcpActiveMonitoringOutsideIn : EcpFeatureTestStepBase
	{
		protected override TestId Id
		{
			get
			{
				return TestId.EcpActiveMonitoringOutsideIn;
			}
		}

		public EcpActiveMonitoringOutsideIn(Uri uri, string userName, SecureString password, ITestFactory factory, Func<EcpStartPage, ITestStep> getFollowingTestStep) : base(uri, userName, null, password, null, factory)
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

		private const TestId ID = TestId.EcpActiveMonitoringOutsideIn;

		private Func<EcpStartPage, ITestStep> getFollowingTestStep;
	}
}

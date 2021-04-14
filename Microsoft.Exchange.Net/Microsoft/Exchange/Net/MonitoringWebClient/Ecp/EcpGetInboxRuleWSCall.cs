using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp
{
	internal class EcpGetInboxRuleWSCall : EcpWebServiceCallBase
	{
		protected override TestId Id
		{
			get
			{
				return TestId.EcpGetInboxRuleWSCall;
			}
		}

		protected override Uri WebServiceRelativeUri
		{
			get
			{
				return new Uri("RulesEditor/InboxRules.svc/GetList", UriKind.Relative);
			}
		}

		protected override RequestBody RequestBody
		{
			get
			{
				return RequestBody.Format("{\"filter\":{\"SearchText\":\"\"},\"sort\":{\"Direction\":0,\"PropertyName\":\"Name\"}}", new object[0]);
			}
		}

		public EcpGetInboxRuleWSCall(Uri uri, Func<Uri, ITestStep> getFollowingTestStep = null) : base(uri, getFollowingTestStep)
		{
		}

		private const TestId ID = TestId.EcpGetInboxRuleWSCall;
	}
}

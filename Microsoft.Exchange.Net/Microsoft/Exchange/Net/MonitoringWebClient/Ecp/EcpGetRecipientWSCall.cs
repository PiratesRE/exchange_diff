using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp
{
	internal class EcpGetRecipientWSCall : EcpWebServiceCallBase
	{
		protected override TestId Id
		{
			get
			{
				return TestId.EcpGetRecipientWSCall;
			}
		}

		protected override Uri WebServiceRelativeUri
		{
			get
			{
				return new Uri("PersonalSettings/Accounts.svc/GetList", UriKind.Relative);
			}
		}

		protected override RequestBody RequestBody
		{
			get
			{
				return RequestBody.Format("{\"filter\":{\"SearchText\":\"\"},\"sort\":{\"Direction\":0,\"PropertyName\":\"DisplayName\"}}", new object[0]);
			}
		}

		public EcpGetRecipientWSCall(Uri uri, Func<Uri, ITestStep> getFollowingTestStep = null) : base(uri, getFollowingTestStep)
		{
		}

		private const TestId ID = TestId.EcpGetRecipientWSCall;
	}
}

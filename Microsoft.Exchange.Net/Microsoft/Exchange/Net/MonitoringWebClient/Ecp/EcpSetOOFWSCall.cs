using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp
{
	internal class EcpSetOOFWSCall : EcpWebServiceCallBase
	{
		protected override TestId Id
		{
			get
			{
				return TestId.ECPSetOOFWSCall;
			}
		}

		protected override Uri WebServiceRelativeUri
		{
			get
			{
				return new Uri("Organize/AutomaticReplies.svc/SetObject", UriKind.Relative);
			}
		}

		protected override RequestBody RequestBody
		{
			get
			{
				return EcpSetOOFWSCall.BuildRequestBody();
			}
		}

		public EcpSetOOFWSCall(Uri uri, Func<Uri, ITestStep> getFollowingTestStep = null) : base(uri, getFollowingTestStep)
		{
		}

		private static RequestBody BuildRequestBody()
		{
			DateTime localTime = ExDateTime.Now.AddDays(2.0).LocalTime;
			DateTime dateTime = localTime.AddDays(1.0);
			string text = string.Format("{0:yyyy/MM/dd HH:mm:ss}", localTime);
			string text2 = string.Format("{0:yyyy/MM/dd HH:mm:ss}", dateTime);
			return RequestBody.Format("{{\"identity\":null,\"properties\":{{\"AutoReplyStateDisabled\":\"false\",\"AutoReplyStateScheduled\":true,\"StartTime\":\"{0}\",\"EndTime\":\"{1}\",\"InternalMessage\":\"<div style=\\\"font-family: Tahoma; font-size: 10pt;\\\">message for Set OOF Web Service Call</div>\",\"ExternalAudience\":true,\"ExternalAudienceKnownOnly\":\"false\"}}}}", new object[]
			{
				text,
				text2
			});
		}

		private const string bodyTemplate = "{{\"identity\":null,\"properties\":{{\"AutoReplyStateDisabled\":\"false\",\"AutoReplyStateScheduled\":true,\"StartTime\":\"{0}\",\"EndTime\":\"{1}\",\"InternalMessage\":\"<div style=\\\"font-family: Tahoma; font-size: 10pt;\\\">message for Set OOF Web Service Call</div>\",\"ExternalAudience\":true,\"ExternalAudienceKnownOnly\":\"false\"}}}}";

		private const TestId ID = TestId.ECPSetOOFWSCall;
	}
}

using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct SpamAnalysisTags
	{
		public const int SmtpReceiveAgent = 0;

		public const int RoutingAgent = 1;

		public static Guid guid = new Guid("31331149-AA27-4F2D-9B69-5B46ED4ED829");
	}
}

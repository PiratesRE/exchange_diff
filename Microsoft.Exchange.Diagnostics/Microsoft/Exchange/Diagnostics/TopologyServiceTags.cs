using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct TopologyServiceTags
	{
		public const int Service = 0;

		public const int Client = 1;

		public const int WCFServiceEndpoint = 2;

		public const int WCFClientEndpoint = 3;

		public const int Topology = 4;

		public const int SuitabilityVerifier = 5;

		public const int Discovery = 6;

		public const int DnsTroubleshooter = 7;

		public const int FaultInjection = 8;

		public static Guid guid = new Guid("23c20436-ba78-481d-99c3-5c523ff23024");
	}
}

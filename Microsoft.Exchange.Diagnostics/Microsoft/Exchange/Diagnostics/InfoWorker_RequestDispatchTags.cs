using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct InfoWorker_RequestDispatchTags
	{
		public const int RequestRouting = 0;

		public const int DistributionListHandling = 1;

		public const int ProxyWebRequest = 2;

		public const int FaultInjection = 3;

		public const int GetFolderRequest = 4;

		public static Guid guid = new Guid("92915F00-6982-4d61-818A-6931EBA87182");
	}
}

using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct SystemLoggingTags
	{
		public const int SystemNet = 0;

		public const int SystemNetSocket = 1;

		public const int SystemNetHttpListener = 2;

		public const int SystemIdentityModelTrace = 3;

		public const int SystemServiceModelTrace = 4;

		public const int SystemServiceModelMessageLogging = 5;

		public const int SystemServiceModelMessageLogging_LogMalformedMessages = 6;

		public const int SystemServiceModelMessageLogging_LogMessagesAtServiceLevel = 7;

		public const int SystemServiceModelMessageLogging_LogMessagesAtTransportLevel = 8;

		public const int SystemServiceModelMessageLogging_LogMessageBody = 9;

		public static Guid guid = new Guid("F21F1E57-9689-46E5-BE7D-A84C9BCE0D94");
	}
}

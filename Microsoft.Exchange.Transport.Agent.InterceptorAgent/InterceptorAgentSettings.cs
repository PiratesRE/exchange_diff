using System;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal static class InterceptorAgentSettings
	{
		public static string ArchivePath
		{
			get
			{
				return "D:\\TransportRoles\\Interceptor";
			}
		}

		public static string ArchivedMessagesDirectory
		{
			get
			{
				return "ArchivedMessages";
			}
		}

		public static string ArchivedMessageHeadersDirectory
		{
			get
			{
				return "ArchivedMessageHeaders";
			}
		}
	}
}

using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class OAuthUtilities
	{
		internal static string ServerVersionString
		{
			get
			{
				if (OAuthUtilities.serverVersion == null)
				{
					OAuthUtilities.serverVersion = LocalServer.GetServer().AdminDisplayVersion;
				}
				return string.Format("{0}.{1}.{2}.{3}", new object[]
				{
					OAuthUtilities.serverVersion.Major,
					OAuthUtilities.serverVersion.Minor,
					OAuthUtilities.serverVersion.Build,
					OAuthUtilities.serverVersion.Revision
				});
			}
		}

		public const string ClientRequestIdHeaderString = "client-request-id";

		public const string ReturnClientRequestIdHeaderString = "return-client-request-id";

		private static ServerVersion serverVersion;
	}
}

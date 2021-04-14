using System;
using System.DirectoryServices.Protocols;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Common;
using Microsoft.Exchange.EdgeSync.Logging;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	internal class TestEdgeConnectionFactory
	{
		public static TargetConnection Create(Server localHub, TargetServerConfig targetServerConfig, NetworkCredential credential, SyncTreeType type, EdgeSyncLogSession logSession)
		{
			TargetConnection result;
			try
			{
				result = new LdapTargetConnection(localHub.VersionNumber, targetServerConfig, credential, type, logSession);
			}
			catch (ExDirectoryException ex)
			{
				if (ex.InnerException is LdapException)
				{
					LdapException ex2 = ex.InnerException as LdapException;
					if (ex2.ErrorCode == 49)
					{
						string userName = credential.UserName;
						string host = targetServerConfig.Host;
					}
				}
				throw;
			}
			return result;
		}
	}
}

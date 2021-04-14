using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	internal class ServerInfo
	{
		public ADObjectId Identity;

		public ServerRole Role;
	}
}

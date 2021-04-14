using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class CASServiceError
	{
		internal CASServiceError(string mbxServer)
		{
			this.mbxServer = mbxServer;
		}

		internal CASServiceError(string mbxServer, string vdir)
		{
			this.mbxServer = mbxServer;
			this.vdir = vdir;
		}

		internal string GetKey()
		{
			if (string.IsNullOrEmpty(this.vdir))
			{
				return this.mbxServer.ToLowerInvariant();
			}
			return this.mbxServer.ToLowerInvariant() + "_" + this.vdir.ToLowerInvariant();
		}

		private readonly string mbxServer;

		private readonly string vdir;
	}
}

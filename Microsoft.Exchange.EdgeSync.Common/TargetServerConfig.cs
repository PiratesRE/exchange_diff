using System;

namespace Microsoft.Exchange.EdgeSync
{
	internal class TargetServerConfig
	{
		public TargetServerConfig(string name, string host, int port)
		{
			this.name = name;
			this.host = host;
			this.port = port;
		}

		public int Port
		{
			get
			{
				return this.port;
			}
		}

		public string Host
		{
			get
			{
				return this.host;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public virtual string ShortHostName
		{
			get
			{
				return string.Empty;
			}
		}

		private readonly string host;

		private readonly string name;

		private readonly int port;
	}
}

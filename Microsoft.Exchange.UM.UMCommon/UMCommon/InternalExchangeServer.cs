using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class InternalExchangeServer
	{
		public InternalExchangeServer(Server s)
		{
			this.server = s;
		}

		public string Fqdn
		{
			get
			{
				return this.server.Fqdn;
			}
		}

		public ADObjectId Id
		{
			get
			{
				return this.server.Id;
			}
		}

		public ADObjectId ServerSite
		{
			get
			{
				return this.server.ServerSite;
			}
		}

		public ServerStatus Status
		{
			get
			{
				return this.server.Status;
			}
		}

		public Server Server
		{
			get
			{
				return this.server;
			}
		}

		internal string LegacyDN
		{
			get
			{
				return this.server.ExchangeLegacyDN;
			}
		}

		internal string MachineName
		{
			get
			{
				return this.server.Name;
			}
		}

		public override bool Equals(object obj)
		{
			InternalExchangeServer internalExchangeServer = obj as InternalExchangeServer;
			return obj != null && this.Id.Equals(internalExchangeServer.Id);
		}

		public override int GetHashCode()
		{
			return this.server.Id.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
			{
				this.server.Fqdn,
				this.Status.ToString()
			});
		}

		private Server server;
	}
}

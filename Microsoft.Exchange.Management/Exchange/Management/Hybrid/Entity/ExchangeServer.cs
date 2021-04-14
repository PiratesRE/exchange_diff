using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Hybrid.Entity
{
	internal class ExchangeServer : IExchangeServer
	{
		public ADObjectId Identity { get; set; }

		public string Name { get; set; }

		public ServerRole ServerRole { get; set; }

		public ServerVersion AdminDisplayVersion { get; set; }

		public override string ToString()
		{
			return this.Name;
		}
	}
}

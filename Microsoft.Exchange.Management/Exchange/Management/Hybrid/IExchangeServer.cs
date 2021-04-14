using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IExchangeServer
	{
		ADObjectId Identity { get; }

		string Name { get; }

		ServerRole ServerRole { get; }

		ServerVersion AdminDisplayVersion { get; }
	}
}

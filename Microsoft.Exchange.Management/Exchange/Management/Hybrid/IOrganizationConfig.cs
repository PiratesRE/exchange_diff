using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IOrganizationConfig
	{
		string Name { get; }

		Guid Guid { get; }

		ExchangeObjectVersion AdminDisplayVersion { get; }

		bool IsUpgradingOrganization { get; }

		string OrganizationConfigHash { get; }

		bool IsDehydrated { get; }
	}
}

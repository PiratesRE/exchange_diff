using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Hybrid.Entity
{
	internal class OrganizationConfig : IOrganizationConfig
	{
		public string Name { get; set; }

		public Guid Guid { get; set; }

		public ExchangeObjectVersion AdminDisplayVersion { get; set; }

		public bool IsUpgradingOrganization { get; set; }

		public string OrganizationConfigHash { get; set; }

		public bool IsDehydrated { get; set; }

		public override string ToString()
		{
			return this.Name;
		}
	}
}

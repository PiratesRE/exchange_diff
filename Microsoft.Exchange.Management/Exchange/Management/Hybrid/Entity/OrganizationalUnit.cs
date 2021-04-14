using System;

namespace Microsoft.Exchange.Management.Hybrid.Entity
{
	internal class OrganizationalUnit : IOrganizationalUnit
	{
		public string Name { get; set; }

		public override string ToString()
		{
			return this.Name;
		}
	}
}

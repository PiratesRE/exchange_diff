using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class TenantProvisioningRequestSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRoot = new HygienePropertyDefinition("OrganizationalUnitRoot", typeof(Guid));

		public static readonly HygienePropertyDefinition RequestType = new HygienePropertyDefinition("RequestType", typeof(string));

		public static readonly HygienePropertyDefinition RequestFlags = new HygienePropertyDefinition("RequestFlags", typeof(TenantProvisioningRequestFlags), TenantProvisioningRequestFlags.Default, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition MigrateToRegion = new HygienePropertyDefinition("MigrateToRegion", typeof(string));

		public static readonly HygienePropertyDefinition MigrateToInstance = new HygienePropertyDefinition("MigrateToInstance", typeof(string));
	}
}

using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class DomainProvisioningRequestSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRoot = new HygienePropertyDefinition("OrganizationalUnitRoot", typeof(Guid));

		internal static readonly HygienePropertyDefinition DomainName = new HygienePropertyDefinition("DomainName", typeof(string));

		public static readonly HygienePropertyDefinition RequestType = new HygienePropertyDefinition("RequestType", typeof(string));

		public static readonly HygienePropertyDefinition RequestFlags = new HygienePropertyDefinition("RequestFlags", typeof(DomainProvisioningRequestFlags), DomainProvisioningRequestFlags.Default, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}

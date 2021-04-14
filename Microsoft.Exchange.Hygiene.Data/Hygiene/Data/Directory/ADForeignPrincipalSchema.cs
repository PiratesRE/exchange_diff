using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data.Sync;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class ADForeignPrincipalSchema : CommonSyncProperties
	{
		public static readonly HygienePropertyDefinition DisplayNameProperty = new HygienePropertyDefinition("DisplayName", typeof(string));

		public static readonly HygienePropertyDefinition DescriptionProperty = new HygienePropertyDefinition("Description", typeof(string));

		public static readonly HygienePropertyDefinition ForeignContextIdProperty = new HygienePropertyDefinition("LinkedPartnerOrganizationId", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition ForeignPrincipalIdProperty = new HygienePropertyDefinition("LinkedPartnerGroupId", typeof(ADObjectId));
	}
}

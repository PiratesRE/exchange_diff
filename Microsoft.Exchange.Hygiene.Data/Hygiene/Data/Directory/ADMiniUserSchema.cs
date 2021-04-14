using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class ADMiniUserSchema
	{
		public static readonly HygienePropertyDefinition UserIdProp = new HygienePropertyDefinition("UserId", typeof(ADObjectId));

		public static readonly PropertyDefinition TenantIdProp = new HygienePropertyDefinition("TenantId", typeof(ADObjectId));

		public static readonly PropertyDefinition ConfigurationIdProp = new HygienePropertyDefinition("ConfigurationId", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition NetIdProp = new HygienePropertyDefinition("NetId", typeof(NetID));
	}
}

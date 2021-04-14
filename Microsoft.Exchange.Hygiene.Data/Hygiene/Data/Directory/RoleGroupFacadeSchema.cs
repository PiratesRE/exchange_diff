using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class RoleGroupFacadeSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition Members = new HygienePropertyDefinition("Members", typeof(string), null, ADPropertyDefinitionFlags.MultiValued);

		public static readonly HygienePropertyDefinition BypassSecurityGroupManagerCheck = new HygienePropertyDefinition("BypassSecurityGroupManagerCheck", typeof(bool), true, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}

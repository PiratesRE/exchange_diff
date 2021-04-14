using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class DepartmentSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition DisplayName = SharedPropertyDefinitions.MandatoryDisplayName;

		public static readonly ADPropertyDefinition HABSeniorityIndex = new ADPropertyDefinition("HABSeniorityIndex", ExchangeObjectVersion.Exchange2007, typeof(int), "msDS-HABSeniorityIndex", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PhoneticDisplayName = new ADPropertyDefinition("PhoneticDisplayName", ExchangeObjectVersion.Exchange2007, typeof(string), "msDS-PhoneticDisplayName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HABChildDepartments = new ADPropertyDefinition("HABChildDepartments", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchHABChildDepartmentsLink", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HABChildDepartmentsBL = new ADPropertyDefinition("HABChildDepartmentsBL", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchHABChildDepartmentsBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HABRootDepartmentBL = new ADPropertyDefinition("HABRootDepartmentBL", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchHABRootDepartmentBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HABShowInDepartmentsBL = new ADPropertyDefinition("HABShowInDepartmentsBL", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchHABShowInDepartmentsBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}

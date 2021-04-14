using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class AdministrativeGroupContainer : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return AdministrativeGroupContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AdministrativeGroupContainer.mostDerivedClass;
			}
		}

		public const string DefaultName = "Administrative Groups";

		private static AdministrativeGroupContainerSchema schema = ObjectSchema.GetInstance<AdministrativeGroupContainerSchema>();

		private static string mostDerivedClass = "msExchAdminGroupContainer";
	}
}

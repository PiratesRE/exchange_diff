using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class DatabasesContainer : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return DatabasesContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return DatabasesContainer.mostDerivedClass;
			}
		}

		public static readonly string DefaultName = "Databases";

		private static DatabasesContainerSchema schema = ObjectSchema.GetInstance<DatabasesContainerSchema>();

		private static string mostDerivedClass = "msExchMDBContainer";
	}
}

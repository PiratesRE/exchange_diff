using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class DefaultMessageFilterGlobalSettings : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return DefaultMessageFilterGlobalSettings.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return DefaultMessageFilterGlobalSettings.mostDerivedClass;
			}
		}

		private static DefaultMessageFilterGlobalSettingsSchema schema = ObjectSchema.GetInstance<DefaultMessageFilterGlobalSettingsSchema>();

		private static string mostDerivedClass = "msExchSMTPTurfList";

		public static readonly string DefaultName = "Default Message Filter";
	}
}

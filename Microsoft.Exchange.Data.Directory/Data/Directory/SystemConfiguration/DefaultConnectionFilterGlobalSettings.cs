using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class DefaultConnectionFilterGlobalSettings : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return DefaultConnectionFilterGlobalSettings.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return DefaultConnectionFilterGlobalSettings.mostDerivedClass;
			}
		}

		private static DefaultConnectionFilterGlobalSettingsSchema schema = ObjectSchema.GetInstance<DefaultConnectionFilterGlobalSettingsSchema>();

		private static string mostDerivedClass = "msExchSmtpConnectionTurfList";

		public static readonly string DefaultName = "Default Connection Filter";
	}
}

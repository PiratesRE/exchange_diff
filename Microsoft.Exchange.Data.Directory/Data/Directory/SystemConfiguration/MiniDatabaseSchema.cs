using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class MiniDatabaseSchema : ADObjectSchema
	{
		public static readonly ADPropertyDefinition Server = DatabaseSchema.Server;

		public static readonly ADPropertyDefinition ServerName = DatabaseSchema.ServerName;

		public static readonly ADPropertyDefinition MasterServerOrAvailabilityGroup = DatabaseSchema.MasterServerOrAvailabilityGroup;

		public static readonly ADPropertyDefinition UsnChanged = SharedPropertyDefinitions.UsnChanged;
	}
}

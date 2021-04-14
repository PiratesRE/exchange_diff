using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class MiniServerSchema : ADObjectSchema
	{
		public static readonly ADPropertyDefinition Fqdn = ServerSchema.Fqdn;

		public static readonly ADPropertyDefinition VersionNumber = ServerSchema.VersionNumber;

		public static readonly ADPropertyDefinition IsE14OrLater = ServerSchema.IsE14OrLater;

		public static readonly ADPropertyDefinition MajorVersion = ServerSchema.MajorVersion;

		public static readonly ADPropertyDefinition AdminDisplayVersion = ServerSchema.AdminDisplayVersion;

		public static readonly ADPropertyDefinition ServerSite = ServerSchema.ServerSite;
	}
}

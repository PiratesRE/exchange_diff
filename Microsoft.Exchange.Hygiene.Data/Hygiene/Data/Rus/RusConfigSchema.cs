using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Rus
{
	internal class RusConfigSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition UniversalManifestVersion = new HygienePropertyDefinition("UniversalManifestVersion", typeof(string));

		public static readonly HygienePropertyDefinition UniversalManifestVersionV2 = new HygienePropertyDefinition("UniversalManifestVersionV2", typeof(string));
	}
}

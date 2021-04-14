using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ContentConfigContainerSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition BlobMimeTypes = OrganizationSchema.BlobMimeTypes;

		public static readonly ADPropertyDefinition MimeTypes = OrganizationSchema.MimeTypes;
	}
}

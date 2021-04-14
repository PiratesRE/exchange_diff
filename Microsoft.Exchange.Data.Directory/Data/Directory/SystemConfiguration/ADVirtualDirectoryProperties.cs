using System;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ADVirtualDirectoryProperties : ADPropertyUnionSchema
	{
		public override ReadOnlyCollection<ADObjectSchema> ObjectSchemas
		{
			get
			{
				return ADVirtualDirectoryProperties.allVirtualDirectorySchemas;
			}
		}

		private static ReadOnlyCollection<ADObjectSchema> allVirtualDirectorySchemas = new ReadOnlyCollection<ADObjectSchema>(new ADObjectSchema[]
		{
			ObjectSchema.GetInstance<ADAutodiscoverVirtualDirectorySchema>(),
			ObjectSchema.GetInstance<ADMobileVirtualDirectorySchema>(),
			ObjectSchema.GetInstance<ADWebServicesVirtualDirectorySchema>(),
			ObjectSchema.GetInstance<ADOwaVirtualDirectorySchema>(),
			ObjectSchema.GetInstance<ADRpcHttpVirtualDirectorySchema>(),
			ObjectSchema.GetInstance<ADO365SuiteServiceVirtualDirectorySchema>(),
			ObjectSchema.GetInstance<ADSnackyServiceVirtualDirectorySchema>(),
			ObjectSchema.GetInstance<ADOabVirtualDirectorySchema>(),
			ObjectSchema.GetInstance<ADAvailabilityForeignConnectorVirtualDirectorySchema>(),
			ObjectSchema.GetInstance<ADEcpVirtualDirectorySchema>(),
			ObjectSchema.GetInstance<ADMapiVirtualDirectorySchema>()
		});
	}
}

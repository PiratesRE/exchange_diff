using System;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ADEmailTransportProperties : ADPropertyUnionSchema
	{
		public override ReadOnlyCollection<ADObjectSchema> ObjectSchemas
		{
			get
			{
				return ADEmailTransportProperties.allEmailTransportSchemas;
			}
		}

		private static ReadOnlyCollection<ADObjectSchema> allEmailTransportSchemas = new ReadOnlyCollection<ADObjectSchema>(new ADObjectSchema[]
		{
			ObjectSchema.GetInstance<Pop3AdConfigurationSchema>(),
			ObjectSchema.GetInstance<Imap4AdConfigurationSchema>()
		});
	}
}

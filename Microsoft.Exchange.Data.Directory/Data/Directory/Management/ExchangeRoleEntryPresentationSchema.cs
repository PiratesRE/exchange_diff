using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class ExchangeRoleEntryPresentationSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ExchangeRoleSchema>();
		}

		public static readonly ADPropertyDefinition Role = ADObjectSchema.Id;
	}
}

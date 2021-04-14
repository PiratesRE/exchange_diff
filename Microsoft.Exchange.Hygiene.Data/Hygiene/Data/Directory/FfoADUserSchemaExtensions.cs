using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class FfoADUserSchemaExtensions : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition LocalUserIdProp = new HygienePropertyDefinition("LocalUserId", typeof(ADObjectId));
	}
}

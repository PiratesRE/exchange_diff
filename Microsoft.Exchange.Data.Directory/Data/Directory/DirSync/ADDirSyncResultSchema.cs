using System;

namespace Microsoft.Exchange.Data.Directory.DirSync
{
	internal class ADDirSyncResultSchema : ObjectSchema
	{
		public static readonly ADPropertyDefinition Id = ADObjectSchema.Id;

		public static readonly ADPropertyDefinition Name = ADObjectSchema.RawName;

		public static readonly ADPropertyDefinition WhenCreated = ADObjectSchema.WhenCreatedRaw;

		public static readonly ADPropertyDefinition IsDeleted = new ADPropertyDefinition("IsDeleted", ExchangeObjectVersion.Exchange2003, typeof(bool), "isDeleted", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}

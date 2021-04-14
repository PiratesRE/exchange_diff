using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class SyncDeletedObjectSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<DeletedObjectSchema>();
		}

		public static readonly ADPropertyDefinition EndOfList = DeletedObjectSchema.EndOfList;

		public static readonly ADPropertyDefinition Cookie = DeletedObjectSchema.Cookie;
	}
}

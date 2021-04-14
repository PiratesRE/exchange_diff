using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MasterCategoryListSchema : Schema
	{
		public new static MasterCategoryListSchema Instance
		{
			get
			{
				if (MasterCategoryListSchema.instance == null)
				{
					MasterCategoryListSchema.instance = new MasterCategoryListSchema();
				}
				return MasterCategoryListSchema.instance;
			}
		}

		public static StorePropertyDefinition DefaultCategory = InternalSchema.CategoryListDefaultCategory;

		public static StorePropertyDefinition LastSavedTime = InternalSchema.CategoryListLastSavedTime;

		private static MasterCategoryListSchema instance = null;
	}
}

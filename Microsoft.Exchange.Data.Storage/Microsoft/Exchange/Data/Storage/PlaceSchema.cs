using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PlaceSchema : ContactSchema
	{
		public new static PlaceSchema Instance
		{
			get
			{
				if (PlaceSchema.instance == null)
				{
					PlaceSchema.instance = new PlaceSchema();
				}
				return PlaceSchema.instance;
			}
		}

		private static PlaceSchema instance = null;

		[Autoload]
		public static readonly StorePropertyDefinition LocationRelevanceRank = InternalSchema.LocationRelevanceRank;
	}
}

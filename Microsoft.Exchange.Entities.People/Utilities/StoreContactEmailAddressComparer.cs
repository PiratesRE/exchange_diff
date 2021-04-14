using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Entities.People.Utilities
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class StoreContactEmailAddressComparer : IComparer<IStorePropertyBag>
	{
		private StoreContactEmailAddressComparer()
		{
		}

		public int Compare(IStorePropertyBag x, IStorePropertyBag y)
		{
			if (object.ReferenceEquals(x, y))
			{
				return 0;
			}
			if (x == null)
			{
				return -1;
			}
			if (y == null)
			{
				return 1;
			}
			string valueOrDefault = x.GetValueOrDefault<string>(ContactSchema.Email1EmailAddress, string.Empty);
			string valueOrDefault2 = y.GetValueOrDefault<string>(ContactSchema.Email1EmailAddress, string.Empty);
			return StringComparer.OrdinalIgnoreCase.Compare(valueOrDefault, valueOrDefault2);
		}

		internal static readonly StoreContactEmailAddressComparer Instance = new StoreContactEmailAddressComparer();
	}
}

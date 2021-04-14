using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Entities.People.Utilities
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class StoreContactPersonIdComparer : IComparer<IStorePropertyBag>
	{
		private StoreContactPersonIdComparer()
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
			PersonId valueOrDefault = x.GetValueOrDefault<PersonId>(ContactSchema.PersonId, PersonId.Create(Guid.Empty.ToByteArray()));
			PersonId valueOrDefault2 = y.GetValueOrDefault<PersonId>(ContactSchema.PersonId, PersonId.Create(Guid.Empty.ToByteArray()));
			return valueOrDefault.ToBase64String().CompareTo(valueOrDefault2.ToBase64String());
		}

		internal static readonly StoreContactPersonIdComparer Instance = new StoreContactPersonIdComparer();
	}
}

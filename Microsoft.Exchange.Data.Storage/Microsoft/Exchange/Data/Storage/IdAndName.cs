using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class IdAndName
	{
		public IdAndName(StoreObjectId id, LocalizedString name)
		{
			this.Id = id;
			this.Name = name;
		}

		public StoreObjectId Id { get; private set; }

		public LocalizedString Name { get; private set; }

		public override string ToString()
		{
			return string.Format("Id={0},Name={1}", this.Id, this.Name);
		}
	}
}

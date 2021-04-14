using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FileSetItem
	{
		public FileSetItem(string name, StoreId id, ExDateTime time)
		{
			this.Name = name;
			this.Id = id;
			this.Time = time;
		}

		public string Name { get; private set; }

		public StoreId Id { get; private set; }

		public ExDateTime Time { get; private set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Name=",
				this.Name,
				", Id=",
				this.Id,
				", Time=",
				this.Time
			});
		}
	}
}

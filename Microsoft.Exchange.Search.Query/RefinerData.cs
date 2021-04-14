using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Search.Query
{
	internal class RefinerData
	{
		internal RefinerData(PropertyDefinition property, IReadOnlyCollection<RefinerDataEntry> entries)
		{
			InstantSearch.ThrowOnNullArgument(property, "property");
			InstantSearch.ThrowOnNullArgument(entries, "entries");
			this.Property = property;
			this.Entries = entries;
		}

		public PropertyDefinition Property { get; private set; }

		public IReadOnlyCollection<RefinerDataEntry> Entries { get; private set; }

		public override string ToString()
		{
			return this.Property.Name + ":" + this.Entries.Count;
		}
	}
}

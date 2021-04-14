using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport
{
	internal struct TypeEntry
	{
		public TypeEntry(Type type, StreamPropertyType identifier)
		{
			this.Type = type;
			this.Identifier = identifier;
		}

		public readonly Type Type;

		public readonly StreamPropertyType Identifier;
	}
}

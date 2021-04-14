using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UniquelyNamedObject : INamedObject
	{
		public UniquelyNamedObject()
		{
			string text = Guid.NewGuid().ToString();
			this.Name = text;
			this.DetailedName = text;
		}

		public string Name { get; private set; }

		public string DetailedName { get; private set; }
	}
}

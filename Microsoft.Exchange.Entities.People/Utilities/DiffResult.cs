using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Entities.People.Utilities
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DiffResult<T, K>
	{
		public DiffResult()
		{
			this.RemoveList = new List<T>();
			this.AddList = new List<T>();
			this.UpdateList = new Dictionary<T, ICollection<Tuple<K, object>>>();
		}

		public ICollection<T> RemoveList { get; private set; }

		public ICollection<T> AddList { get; private set; }

		public Dictionary<T, ICollection<Tuple<K, object>>> UpdateList { get; private set; }
	}
}

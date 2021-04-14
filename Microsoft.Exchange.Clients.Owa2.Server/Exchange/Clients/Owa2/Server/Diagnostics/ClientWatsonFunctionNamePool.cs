using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal sealed class ClientWatsonFunctionNamePool
	{
		public int GetOrAddFunctionNameIndex(string name)
		{
			int count;
			if (!this.pool.TryGetValue(name, out count))
			{
				count = this.pool.Count;
				this.pool.Add(name, count);
			}
			return count;
		}

		public string[] ToArray()
		{
			string[] array = new string[this.pool.Count];
			this.pool.Keys.CopyTo(array, 0);
			return array;
		}

		private readonly Dictionary<string, int> pool = new Dictionary<string, int>();
	}
}

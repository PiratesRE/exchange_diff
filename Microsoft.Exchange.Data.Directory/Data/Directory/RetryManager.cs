using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class RetryManager
	{
		public void Tried(string serverFqdn)
		{
			if (string.IsNullOrEmpty(serverFqdn))
			{
				throw new ArgumentNullException("serverFqdn");
			}
			string text = serverFqdn.ToLowerInvariant();
			if (this.triedServers == null)
			{
				this.triedServers = new Dictionary<string, int>();
			}
			this.totalRetries++;
			if (this.triedServers.ContainsKey(text))
			{
				Dictionary<string, int> dictionary;
				string key;
				(dictionary = this.triedServers)[key = text] = dictionary[key] + 1;
				return;
			}
			this.triedServers.Add(text, 1);
		}

		public int this[string serverFqdn]
		{
			get
			{
				if (string.IsNullOrEmpty(serverFqdn))
				{
					throw new ArgumentNullException("serverFqdn");
				}
				string key = serverFqdn.ToLowerInvariant();
				if (this.triedServers == null || !this.triedServers.ContainsKey(key))
				{
					return 0;
				}
				return this.triedServers[key];
			}
		}

		public int TotalRetries
		{
			get
			{
				return this.totalRetries;
			}
		}

		private int totalRetries;

		private Dictionary<string, int> triedServers;
	}
}

using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Monitoring
{
	internal class TransientErrorCache
	{
		private TransientErrorCache()
		{
			this.errors = new Dictionary<string, CASServiceError>();
		}

		public void Add(CASServiceError error)
		{
			if (error == null)
			{
				throw new ArgumentNullException("error");
			}
			lock (this.lockObject)
			{
				if (!this.errors.ContainsKey(error.GetKey()))
				{
					this.errors.Add(error.GetKey(), error);
				}
			}
		}

		public bool ContainsError(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("key");
			}
			key = key.ToLowerInvariant();
			bool result;
			lock (this.lockObject)
			{
				result = this.errors.ContainsKey(key);
			}
			return result;
		}

		public bool ContainsError(string key1, string key2)
		{
			if (string.IsNullOrEmpty(key1) || string.IsNullOrEmpty(key2))
			{
				throw new ArgumentException("key");
			}
			key1 = key1.ToLowerInvariant();
			key2 = key2.ToLowerInvariant();
			bool result;
			lock (this.lockObject)
			{
				result = this.errors.ContainsKey(key1 + "_" + key2);
			}
			return result;
		}

		internal void Remove(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("key");
			}
			key = key.ToLowerInvariant();
			lock (this.lockObject)
			{
				if (this.errors.ContainsKey(key))
				{
					this.errors.Remove(key);
				}
			}
		}

		internal void Remove(string key1, string key2)
		{
			if (string.IsNullOrEmpty(key1) || string.IsNullOrEmpty(key2))
			{
				throw new ArgumentException("key");
			}
			key1 = key1.ToLowerInvariant();
			key2 = key2.ToLowerInvariant();
			string key3 = key1 + "_" + key2;
			lock (this.lockObject)
			{
				if (this.errors.ContainsKey(key3))
				{
					this.errors.Remove(key3);
				}
			}
		}

		internal static TransientErrorCache EWSTransientCache = new TransientErrorCache();

		internal static TransientErrorCache EASTransientCache = new TransientErrorCache();

		internal static TransientErrorCache OWAInternalTransientCache = new TransientErrorCache();

		internal static TransientErrorCache OWAExternalTransientCache = new TransientErrorCache();

		internal static TransientErrorCache EcpInternalTransientCache = new TransientErrorCache();

		internal static TransientErrorCache EcpExternalTransientCache = new TransientErrorCache();

		internal static TransientErrorCache POPTransientErrorCache = new TransientErrorCache();

		internal static TransientErrorCache IMAPTransientErrorCache = new TransientErrorCache();

		internal static TransientErrorCache PowerShellTransientErrorCache = new TransientErrorCache();

		internal static TransientErrorCache CalendarTransientErrorCache = new TransientErrorCache();

		private object lockObject = new object();

		private Dictionary<string, CASServiceError> errors;
	}
}

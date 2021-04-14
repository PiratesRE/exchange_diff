using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class SecureNameValueCollection : DisposeTrackableBase, IEnumerable<string>, IEnumerable
	{
		public SecureNameValueCollection()
		{
			this.names = new List<string>();
			this.unsecuredValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			this.secureValues = new Dictionary<string, SecureString>(StringComparer.OrdinalIgnoreCase);
		}

		public void AddUnsecureNameValue(string name, string value)
		{
			if (name == null || value == null)
			{
				throw new ArgumentException("name or value is null");
			}
			if (this.secureValues.ContainsKey(name))
			{
				throw new InvalidOperationException("Name was already added as secure pair. Name value:" + name);
			}
			if (this.unsecuredValues.ContainsKey(name))
			{
				throw new ArgumentException("Name is already in the collection. Name:" + name);
			}
			this.names.Add(name);
			this.unsecuredValues.Add(name, value);
		}

		public void AddSecureNameValue(string name, SecureString value)
		{
			if (name == null || value == null)
			{
				throw new ArgumentException("name or value is null");
			}
			if (this.unsecuredValues.ContainsKey(name))
			{
				throw new InvalidOperationException("Name was already added unsecure pair. Name value:" + name);
			}
			if (this.secureValues.ContainsKey(name))
			{
				throw new ArgumentException("Name is already in the collection. Name:" + name);
			}
			this.names.Add(name);
			this.secureValues.Add(name, value);
		}

		public IEnumerator<string> GetEnumerator()
		{
			return this.names.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool TryGetSecureValue(string name, out SecureString value)
		{
			value = null;
			return this.secureValues.TryGetValue(name, out value);
		}

		public bool TryGetUnsecureValue(string name, out string value)
		{
			value = null;
			return this.unsecuredValues.TryGetValue(name, out value);
		}

		public bool ContainsUnsecureValue(string name)
		{
			return this.unsecuredValues.ContainsKey(name);
		}

		public bool ContainsSecureValue(string name)
		{
			return this.secureValues.ContainsKey(name);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && !this.isDisposed)
			{
				foreach (SecureString secureString in this.secureValues.Values)
				{
					secureString.Dispose();
				}
				this.unsecuredValues = null;
				this.secureValues = null;
				this.isDisposed = true;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SecureNameValueCollection>(this);
		}

		private List<string> names;

		private Dictionary<string, string> unsecuredValues;

		private Dictionary<string, SecureString> secureValues;

		private bool isDisposed;
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "ClientVersionCollection")]
	[Serializable]
	public sealed class ClientVersionCollection : XMLSerializableBase, ICollection<ClientVersion>, IEnumerable<ClientVersion>, IEnumerable
	{
		public void Add(ClientVersion item)
		{
			this.list.Add(item);
		}

		public void Clear()
		{
			this.list.Clear();
		}

		public bool Contains(ClientVersion item)
		{
			return this.list.Contains(item);
		}

		public void CopyTo(ClientVersion[] array, int arrayIndex)
		{
			this.list.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(ClientVersion item)
		{
			return this.list.Remove(item);
		}

		public IEnumerator<ClientVersion> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			IEnumerable enumerable = this.list;
			return enumerable.GetEnumerator();
		}

		public ClientVersion GetRequiredClientVersion(Version requestClientVersion)
		{
			ClientVersion clientVersion = null;
			foreach (ClientVersion clientVersion2 in this.list)
			{
				if (requestClientVersion.Major == clientVersion2.Version.Major && requestClientVersion < clientVersion2.Version)
				{
					if (clientVersion == null)
					{
						clientVersion = new ClientVersion
						{
							Version = clientVersion2.Version,
							ExpirationDate = clientVersion2.ExpirationDate
						};
					}
					else if (clientVersion2.Version > clientVersion.Version)
					{
						clientVersion.Version = clientVersion2.Version;
					}
					else if (clientVersion2.ExpirationDate < clientVersion.ExpirationDate)
					{
						clientVersion.ExpirationDate = clientVersion2.ExpirationDate;
					}
				}
			}
			return clientVersion;
		}

		public bool IsClientVersionSufficient(Version requestedVersion)
		{
			foreach (ClientVersion clientVersion in this.list)
			{
				if (requestedVersion.Major == clientVersion.Version.Major && requestedVersion < clientVersion.Version && clientVersion.ExpirationDate < DateTime.UtcNow)
				{
					return false;
				}
			}
			return true;
		}

		private List<ClientVersion> list = new List<ClientVersion>();
	}
}

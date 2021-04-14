using System;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal sealed class SharingAnonymousIdentityCollection : IEquatable<SharingAnonymousIdentityCollection>
	{
		internal SharingAnonymousIdentityCollection(MultiValuedProperty<string> sharingAnonymousIdentities)
		{
			if (sharingAnonymousIdentities == null)
			{
				throw new ArgumentNullException("mvp");
			}
			this.sharingAnonymousIdentities = sharingAnonymousIdentities;
		}

		public bool Changed
		{
			get
			{
				return this.sharingAnonymousIdentities.Changed;
			}
		}

		public int Count
		{
			get
			{
				return this.sharingAnonymousIdentities.Count;
			}
		}

		public bool Equals(SharingAnonymousIdentityCollection other)
		{
			return other != null && this.sharingAnonymousIdentities.Equals(other.sharingAnonymousIdentities);
		}

		public void Clear()
		{
			this.sharingAnonymousIdentities.Clear();
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SharingAnonymousIdentityCollection);
		}

		public override int GetHashCode()
		{
			return this.sharingAnonymousIdentities.GetHashCode();
		}

		public bool Contains(string urlId)
		{
			return null != this.Find(urlId);
		}

		public void AddOrUpdate(string urlIdPrefix, string urlId, string folderId)
		{
			string text = this.FindByFolder(urlIdPrefix, folderId);
			if (!string.IsNullOrEmpty(text))
			{
				if (StringComparer.OrdinalIgnoreCase.Equals(urlId, this.ParseIdentity(text).Id))
				{
					return;
				}
				this.sharingAnonymousIdentities.Remove(text);
			}
			this.sharingAnonymousIdentities.Add(this.FormatIdentity(urlId, folderId));
		}

		public string GetFolder(string urlIdentity)
		{
			string text = this.Find(urlIdentity);
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return this.ParseIdentity(text).Folder;
		}

		public string FindExistingUrlId(string urlIdPrefix, string folderId)
		{
			string text = this.FindByFolder(urlIdPrefix, folderId);
			if (!string.IsNullOrEmpty(text))
			{
				return this.ParseIdentity(text).Id;
			}
			return null;
		}

		public bool Remove(string urlIdentity)
		{
			string text = this.Find(urlIdentity);
			return !string.IsNullOrEmpty(text) && this.sharingAnonymousIdentities.Remove(text);
		}

		internal ReadOnlyCollection<string> GetRawSharingAnonymousIdentities()
		{
			return new ReadOnlyCollection<string>(this.sharingAnonymousIdentities);
		}

		private string Find(string urlIdentity)
		{
			string stringToStart = urlIdentity + ":";
			return this.sharingAnonymousIdentities.Find((string identity) => identity.StartsWith(stringToStart, StringComparison.OrdinalIgnoreCase));
		}

		private string FindByFolder(string urlIdprefix, string folderId)
		{
			string stringToEnd = ":" + folderId;
			return this.sharingAnonymousIdentities.Find((string identity) => identity.StartsWith(urlIdprefix, StringComparison.OrdinalIgnoreCase) && identity.EndsWith(stringToEnd, StringComparison.Ordinal));
		}

		private string FormatIdentity(string urlId, string folderId)
		{
			return urlId + ":" + folderId;
		}

		private SharingAnonymousIdentityCollection.IdAndFolder ParseIdentity(string identity)
		{
			int num = identity.IndexOf(":");
			if (num == -1)
			{
				return default(SharingAnonymousIdentityCollection.IdAndFolder);
			}
			return new SharingAnonymousIdentityCollection.IdAndFolder
			{
				Id = identity.Substring(0, num),
				Folder = identity.Substring(num + 1)
			};
		}

		private const string Delimiter = ":";

		private readonly MultiValuedProperty<string> sharingAnonymousIdentities;

		private struct IdAndFolder
		{
			public string Id;

			public string Folder;
		}
	}
}

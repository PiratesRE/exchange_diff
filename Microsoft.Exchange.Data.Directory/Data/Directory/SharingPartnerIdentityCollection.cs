using System;
using System.Globalization;
using Microsoft.Exchange.Transport.RecipientAPI;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal sealed class SharingPartnerIdentityCollection : IEquatable<SharingPartnerIdentityCollection>
	{
		internal MultiValuedProperty<string> InnerCollection
		{
			get
			{
				return this.innerCollection;
			}
		}

		private StringHasher StringHasher
		{
			get
			{
				if (this.stringHasher == null)
				{
					this.stringHasher = new StringHasher();
				}
				return this.stringHasher;
			}
		}

		internal SharingPartnerIdentityCollection(MultiValuedProperty<string> mvp)
		{
			if (mvp == null)
			{
				throw new ArgumentNullException("mvp");
			}
			this.innerCollection = mvp;
		}

		public bool Changed
		{
			get
			{
				return this.InnerCollection.Changed;
			}
		}

		public int Count
		{
			get
			{
				return this.InnerCollection.Count;
			}
		}

		public bool Contains(string externalId)
		{
			return this.InnerCollection.Contains(this.GetSharingPartnerIdentity(externalId));
		}

		public void Add(string externalId)
		{
			this.InnerCollection.Add(this.GetSharingPartnerIdentity(externalId));
		}

		public bool Remove(string externalId)
		{
			return this.InnerCollection.Remove(this.GetSharingPartnerIdentity(externalId));
		}

		private string GetSharingPartnerIdentity(string externalId)
		{
			string input = externalId.Trim();
			return this.StringHasher.GetHash(input).ToString(NumberFormatInfo.InvariantInfo);
		}

		public bool Equals(SharingPartnerIdentityCollection other)
		{
			return other != null && this.InnerCollection.Equals(other.InnerCollection);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SharingPartnerIdentityCollection);
		}

		public override int GetHashCode()
		{
			return this.InnerCollection.GetHashCode();
		}

		private readonly MultiValuedProperty<string> innerCollection;

		private StringHasher stringHasher;
	}
}

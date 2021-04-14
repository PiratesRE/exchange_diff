using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal struct SubscriptionCacheEntry : IComparable<SubscriptionCacheEntry>
	{
		public SubscriptionCacheEntry(Guid id, string address, string displayName, byte[] instanceKey, CultureInfo culture)
		{
			this.id = id;
			this.address = address;
			this.displayName = displayName;
			this.instanceKey = instanceKey;
			this.culture = culture;
		}

		public Guid Id
		{
			get
			{
				return this.id;
			}
			internal set
			{
				this.id = value;
			}
		}

		public string Address
		{
			get
			{
				return this.address;
			}
			internal set
			{
				this.address = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			internal set
			{
				this.displayName = value;
			}
		}

		public byte[] InstanceKey
		{
			get
			{
				return this.instanceKey;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
			internal set
			{
				this.culture = value;
			}
		}

		public int CompareTo(SubscriptionCacheEntry other)
		{
			return this.CompareCultureSensitiveStrings(this.address, other.address);
		}

		public bool DisplayNameMatch(SubscriptionCacheEntry other)
		{
			return 0 == this.CompareCultureSensitiveStrings(this.displayName, other.displayName);
		}

		public bool MatchOnInstanceKey(byte[] otherInstanceKey)
		{
			if (this.instanceKey == null || otherInstanceKey == null)
			{
				return false;
			}
			for (int i = 0; i < this.instanceKey.Length; i++)
			{
				if (this.instanceKey[i] != otherInstanceKey[i])
				{
					return false;
				}
			}
			return true;
		}

		public void RenderToJavascript(TextWriter writer)
		{
			RecipientInfoCacheEntry entry = new RecipientInfoCacheEntry(this.displayName, this.address, this.address, null, "SMTP", AddressOrigin.OneOff, 0, null, EmailAddressIndex.None, null, null);
			AutoCompleteCacheEntry.RenderEntryJavascript(writer, entry);
		}

		private int CompareCultureSensitiveStrings(string source, string target)
		{
			CultureInfo cultureInfo = (this.culture == null) ? CultureInfo.InvariantCulture : this.culture;
			return cultureInfo.CompareInfo.Compare(source, target, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
		}

		private Guid id;

		private string address;

		private string displayName;

		private byte[] instanceKey;

		private CultureInfo culture;
	}
}

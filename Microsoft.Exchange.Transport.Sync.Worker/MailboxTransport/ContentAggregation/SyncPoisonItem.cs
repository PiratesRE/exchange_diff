using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SyncPoisonItem
	{
		public SyncPoisonItem(string id, SyncPoisonEntitySource source, SyncPoisonEntityType type)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("id", id);
			this.id = id;
			this.source = source;
			this.type = type;
		}

		public string Key
		{
			get
			{
				if (this.key == null)
				{
					this.key = string.Concat(new object[]
					{
						(int)this.Source,
						":",
						(int)this.Type,
						":",
						SyncPoisonItem.Encode(this.id)
					});
				}
				return this.key;
			}
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public SyncPoisonEntitySource Source
		{
			get
			{
				return this.source;
			}
		}

		public SyncPoisonEntityType Type
		{
			get
			{
				return this.type;
			}
		}

		public override string ToString()
		{
			if (this.cachedToString == null)
			{
				this.cachedToString = string.Format(CultureInfo.InvariantCulture, "ItemId: {0}, Source: {1}, Type: {2}", new object[]
				{
					this.id,
					this.source,
					this.type
				});
			}
			return this.cachedToString;
		}

		private static string Encode(string inputString)
		{
			return Convert.ToBase64String(Encoding.Unicode.GetBytes(inputString));
		}

		private const string FormatString = "ItemId: {0}, Source: {1}, Type: {2}";

		private const string Seperator = ":";

		private readonly string id;

		private readonly SyncPoisonEntitySource source;

		private readonly SyncPoisonEntityType type;

		private string key;

		private string cachedToString;
	}
}

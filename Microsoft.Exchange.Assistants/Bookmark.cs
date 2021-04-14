using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class Bookmark : ISortKey<Guid>
	{
		private Bookmark(Guid identity, int numberOfWatermarks)
		{
			this.Identity = identity;
			this.watermarks = new Dictionary<Guid, long>(numberOfWatermarks);
		}

		private Bookmark(Guid identity, Bookmark bookmark) : this(identity, bookmark.watermarks.Count)
		{
			foreach (KeyValuePair<Guid, long> keyValuePair in bookmark.watermarks)
			{
				this.watermarks.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public Guid Identity { get; private set; }

		public Guid SortKey
		{
			get
			{
				return this.Identity;
			}
		}

		public long this[Guid assistantId]
		{
			get
			{
				return this.watermarks[assistantId];
			}
			set
			{
				this.watermarks[assistantId] = value;
			}
		}

		public static Bookmark CreateFromDatabaseBookmark(Guid mailboxGuid, Bookmark databaseBookmark)
		{
			return new Bookmark(mailboxGuid, databaseBookmark);
		}

		public static Bookmark Create(Guid identity, int numberOfWatermarks)
		{
			return new Bookmark(identity, numberOfWatermarks);
		}

		public long GetLowestWatermark()
		{
			long num = long.MaxValue;
			foreach (long val in this.watermarks.Values)
			{
				num = Math.Min(num, val);
			}
			return num;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder((this.watermarks.Count + 1) * 60);
			stringBuilder.Append("Bookmark for " + this.Identity + ". Watermarks:");
			foreach (KeyValuePair<Guid, long> keyValuePair in this.watermarks)
			{
				stringBuilder.AppendFormat(" [{0},{1}]", keyValuePair.Key, keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}

		private Dictionary<Guid, long> watermarks;
	}
}

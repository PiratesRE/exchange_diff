using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Diagnostics.Tracing
{
	internal sealed class NameInfo : ConcurrentSetItem<KeyValuePair<string, EventTags>, NameInfo>
	{
		internal static void ReserveEventIDsBelow(int eventId)
		{
			int num;
			int num2;
			do
			{
				num = NameInfo.lastIdentity;
				num2 = (NameInfo.lastIdentity & -16777216) + eventId;
				num2 = Math.Max(num2, num);
			}
			while (Interlocked.CompareExchange(ref NameInfo.lastIdentity, num2, num) != num);
		}

		public NameInfo(string name, EventTags tags, int typeMetadataSize)
		{
			this.name = name;
			this.tags = (tags & (EventTags)268435455);
			this.identity = Interlocked.Increment(ref NameInfo.lastIdentity);
			int prefixSize = 0;
			Statics.EncodeTags((int)this.tags, ref prefixSize, null);
			this.nameMetadata = Statics.MetadataForString(name, prefixSize, 0, typeMetadataSize);
			prefixSize = 2;
			Statics.EncodeTags((int)this.tags, ref prefixSize, this.nameMetadata);
		}

		public override int Compare(NameInfo other)
		{
			return this.Compare(other.name, other.tags);
		}

		public override int Compare(KeyValuePair<string, EventTags> key)
		{
			return this.Compare(key.Key, key.Value & (EventTags)268435455);
		}

		private int Compare(string otherName, EventTags otherTags)
		{
			int num = StringComparer.Ordinal.Compare(this.name, otherName);
			if (num == 0 && this.tags != otherTags)
			{
				num = ((this.tags < otherTags) ? -1 : 1);
			}
			return num;
		}

		private static int lastIdentity = 184549376;

		internal readonly string name;

		internal readonly EventTags tags;

		internal readonly int identity;

		internal readonly byte[] nameMetadata;
	}
}

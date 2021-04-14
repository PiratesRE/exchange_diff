using System;
using System.Net;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;

namespace Microsoft.Exchange.HttpProxy
{
	internal class NegativeAnchorMailboxCacheEntry
	{
		public NegativeAnchorMailboxCacheEntry.CacheGeneration Generation { get; set; }

		public DateTime StartTime { get; set; }

		public HttpStatusCode ErrorCode { get; set; }

		public HttpProxySubErrorCode SubErrorCode { get; set; }

		public string SourceObject { get; set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.Generation,
				'~',
				this.StartTime.ToString("s"),
				'~',
				this.ErrorCode,
				'~',
				this.SubErrorCode,
				'~',
				this.SourceObject
			});
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (this.Generation != (NegativeAnchorMailboxCacheEntry.CacheGeneration)0)
			{
				num ^= this.Generation.GetHashCode();
			}
			num ^= this.StartTime.ToString("s").GetHashCode();
			if (this.ErrorCode != (HttpStatusCode)0)
			{
				num ^= this.ErrorCode.GetHashCode();
			}
			if (this.SubErrorCode != (HttpProxySubErrorCode)0)
			{
				num ^= this.SubErrorCode.GetHashCode();
			}
			if (!string.IsNullOrEmpty(this.SourceObject))
			{
				num ^= this.SourceObject.GetHashCode();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			NegativeAnchorMailboxCacheEntry negativeAnchorMailboxCacheEntry = obj as NegativeAnchorMailboxCacheEntry;
			return negativeAnchorMailboxCacheEntry != null && this.Generation == negativeAnchorMailboxCacheEntry.Generation && this.ErrorCode == negativeAnchorMailboxCacheEntry.ErrorCode && this.SubErrorCode == negativeAnchorMailboxCacheEntry.SubErrorCode && this.StartTime == negativeAnchorMailboxCacheEntry.StartTime && ((string.IsNullOrEmpty(this.SourceObject) && string.IsNullOrEmpty(negativeAnchorMailboxCacheEntry.SourceObject)) || string.Equals(this.SourceObject, negativeAnchorMailboxCacheEntry.SourceObject));
		}

		private const char Separator = '~';

		public enum CacheGeneration : ushort
		{
			One = 1,
			Two,
			Max = 65535
		}
	}
}

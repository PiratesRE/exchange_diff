using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct CacheEntry<TValue> : ILifetimeTrackable
	{
		public CacheEntry(TValue value)
		{
			this.Value = value;
			this.createTime = DateTime.UtcNow;
			this.lastAccessTime = this.createTime;
		}

		public DateTime CreateTime
		{
			get
			{
				return this.createTime;
			}
		}

		public DateTime LastAccessTime
		{
			get
			{
				return this.lastAccessTime;
			}
			set
			{
				this.lastAccessTime = value;
			}
		}

		public readonly TValue Value;

		private readonly DateTime createTime;

		private DateTime lastAccessTime;
	}
}

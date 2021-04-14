using System;

namespace Microsoft.Exchange.Configuration.FailFast
{
	internal class FailFastUserCacheValue
	{
		internal FailFastUserCacheValue(BlockedType blockedType, TimeSpan blockedTime)
		{
			this.BlockedType = blockedType;
			this.AddedTime = DateTime.UtcNow;
			this.BlockedTime = blockedTime;
			this.HitCount = 1;
		}

		internal BlockedType BlockedType { get; set; }

		internal DateTime AddedTime { get; set; }

		internal TimeSpan BlockedTime { get; set; }

		internal int HitCount { get; set; }

		internal bool IsValid
		{
			get
			{
				return this.AddedTime + this.BlockedTime >= DateTime.UtcNow;
			}
		}

		public override string ToString()
		{
			return string.Format("AddedTime: {0}; BlockedType: {1}; BlockedTime: {2}; HitCount: {3}.", new object[]
			{
				this.AddedTime,
				this.BlockedType,
				this.BlockedTime,
				this.HitCount
			});
		}

		private const string StringFormat = "AddedTime: {0}; BlockedType: {1}; BlockedTime: {2}; HitCount: {3}.";
	}
}

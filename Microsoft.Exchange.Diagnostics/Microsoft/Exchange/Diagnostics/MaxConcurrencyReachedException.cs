using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class MaxConcurrencyReachedException : Exception
	{
		public string BucketName { get; private set; }

		public IConcurrencyGuard Guard { get; set; }

		public MaxConcurrencyReachedException(IConcurrencyGuard guard, string bucketName)
		{
			this.Guard = guard;
			this.BucketName = bucketName;
		}

		public override string Message
		{
			get
			{
				return string.Concat(new object[]
				{
					"The ConcurrencyGuard '",
					ConcurrencyGuard.FormatGuardBucketName(this.Guard, this.BucketName),
					"' has hit the limit of ",
					this.Guard.MaxConcurrency,
					"."
				});
			}
		}
	}
}

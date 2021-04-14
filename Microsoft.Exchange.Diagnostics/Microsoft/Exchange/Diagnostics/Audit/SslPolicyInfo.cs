using System;
using System.Linq;

namespace Microsoft.Exchange.Diagnostics.Audit
{
	public sealed class SslPolicyInfo
	{
		public SslPolicyInfo()
		{
			this.errors = new SslError[25];
		}

		internal static SslPolicyInfo Instance
		{
			get
			{
				return SslPolicyInfo.instance;
			}
		}

		public DateTime LastValidationTime { get; set; }

		public SslError[] Errors
		{
			get
			{
				SslError[] result;
				lock (this.mutex)
				{
					result = (from e in this.errors
					where e != null
					orderby e.Index
					select e).ToArray<SslError>();
				}
				return result;
			}
			set
			{
			}
		}

		public void Add(SslError error)
		{
			lock (this.mutex)
			{
				this.errors[this.errorPosition] = error;
				error.Index = (long)this.errorCount++;
				this.errorPosition = (this.errorPosition + 1) % 25;
			}
		}

		private const int CyclicListLength = 25;

		private static readonly SslPolicyInfo instance = new SslPolicyInfo();

		private volatile int errorPosition;

		private volatile int errorCount;

		private object mutex = new object();

		private readonly SslError[] errors;
	}
}

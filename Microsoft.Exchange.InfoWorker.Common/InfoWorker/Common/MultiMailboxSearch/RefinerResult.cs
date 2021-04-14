using System;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class RefinerResult : IRefinerResult
	{
		public RefinerResult(string value, long count)
		{
			this.value = value;
			this.count = count;
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public long Count
		{
			get
			{
				return this.count;
			}
		}

		public void Merge(IRefinerResult result)
		{
			if (result == null)
			{
				return;
			}
			if (string.Compare(this.value, result.Value, StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new ArgumentException("RefinerResult: Invalid merge");
			}
			checked
			{
				this.count += result.Count;
			}
		}

		private readonly string value;

		private long count;
	}
}

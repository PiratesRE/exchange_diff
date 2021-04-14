using System;

namespace Microsoft.Exchange.HttpProxy
{
	internal class HttpRange
	{
		public HttpRange(long firstBytePosition, long lastBytePosition)
		{
			this.FirstBytePosition = firstBytePosition;
			this.LastBytePosition = lastBytePosition;
			if (this.HasFirstBytePosition && this.HasLastBytePosition)
			{
				if (this.FirstBytePosition > this.LastBytePosition)
				{
					throw new ArgumentOutOfRangeException("firstBytePosition", "FirstBytePosition cannot be larger than LastBytePosition");
				}
			}
			else if (!this.HasFirstBytePosition && !this.HasLastBytePosition && !this.HasSuffixLength)
			{
				throw new ArgumentOutOfRangeException("firstBytePosition", "At least firstBytePosition or lastBytePosition must be larger than or equal to 0.");
			}
		}

		public long FirstBytePosition { get; private set; }

		public long LastBytePosition { get; private set; }

		public long SuffixLength
		{
			get
			{
				return this.LastBytePosition;
			}
		}

		public bool HasFirstBytePosition
		{
			get
			{
				return this.FirstBytePosition >= 0L;
			}
		}

		public bool HasLastBytePosition
		{
			get
			{
				return this.HasFirstBytePosition && this.LastBytePosition >= 0L;
			}
		}

		public bool HasSuffixLength
		{
			get
			{
				return this.FirstBytePosition < 0L && this.LastBytePosition >= 0L;
			}
		}
	}
}

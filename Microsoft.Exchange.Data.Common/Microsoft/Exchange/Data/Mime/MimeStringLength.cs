using System;

namespace Microsoft.Exchange.Data.Mime
{
	internal struct MimeStringLength
	{
		public MimeStringLength(int value)
		{
			this.inChars = value;
			this.inBytes = value;
		}

		public MimeStringLength(int valueInChars, int valueInBytes)
		{
			this.inChars = valueInChars;
			this.inBytes = valueInBytes;
		}

		public int InChars
		{
			get
			{
				return this.inChars;
			}
		}

		public int InBytes
		{
			get
			{
				return this.inBytes;
			}
		}

		public void IncrementBy(int count)
		{
			this.inChars += count;
			this.inBytes += count;
		}

		public void IncrementBy(int countInChars, int countInBytes)
		{
			this.inChars += countInChars;
			this.inBytes += countInBytes;
		}

		public void IncrementBy(MimeStringLength count)
		{
			this.inChars += count.InChars;
			this.inBytes += count.InBytes;
		}

		public void DecrementBy(int count)
		{
			this.inChars -= count;
			this.inBytes -= count;
		}

		public void DecrementBy(int countInChars, int countInBytes)
		{
			this.inChars -= countInChars;
			this.inBytes -= countInBytes;
		}

		public void DecrementBy(MimeStringLength count)
		{
			this.inChars -= count.InChars;
			this.inBytes -= count.InBytes;
		}

		public void SetAs(int value)
		{
			this.inChars = value;
			this.inBytes = value;
		}

		public void SetAs(int valueInChars, int valueInBytes)
		{
			this.inChars = valueInChars;
			this.inBytes = valueInBytes;
		}

		public void SetAs(MimeStringLength value)
		{
			this.inChars = value.InChars;
			this.inBytes = value.InBytes;
		}

		public override string ToString()
		{
			return string.Format("InChars={0}, InBytes={1}", this.inChars, this.inBytes);
		}

		private int inChars;

		private int inBytes;
	}
}

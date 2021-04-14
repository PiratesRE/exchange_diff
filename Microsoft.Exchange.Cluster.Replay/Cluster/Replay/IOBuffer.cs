using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class IOBuffer
	{
		public IOBuffer(int size, bool preAllocated)
		{
			this.AppendOffset = 0;
			this.NextBuffer = null;
			this.Buffer = new byte[size];
			this.PreAllocated = preAllocated;
		}

		public byte[] Buffer { get; private set; }

		public int AppendOffset
		{
			get
			{
				return this.appendOffset;
			}
			set
			{
				this.appendOffset = value;
			}
		}

		public IOBuffer NextBuffer
		{
			get
			{
				return this.nextBuffer;
			}
			set
			{
				this.nextBuffer = value;
			}
		}

		public int RemainingSpace
		{
			get
			{
				return this.Buffer.Length - this.AppendOffset;
			}
		}

		public bool PreAllocated { get; private set; }

		private int appendOffset;

		private IOBuffer nextBuffer;
	}
}

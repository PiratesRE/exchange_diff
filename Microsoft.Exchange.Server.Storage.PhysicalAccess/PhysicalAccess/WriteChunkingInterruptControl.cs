using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class WriteChunkingInterruptControl : IInterruptControl
	{
		public WriteChunkingInterruptControl(int writeMax, Func<bool> canContinue)
		{
			this.canContinue = canContinue;
			this.writeMax = writeMax;
		}

		public bool WantToInterrupt
		{
			get
			{
				return this.writeCount >= this.writeMax || (this.canContinue != null && !this.canContinue());
			}
		}

		public void RegisterRead(bool probe, TableClass tableClass)
		{
		}

		public void RegisterWrite(TableClass tableClass)
		{
			this.writeCount++;
		}

		public void Reset()
		{
			this.writeCount = 0;
		}

		private readonly Func<bool> canContinue;

		private readonly int writeMax;

		private int writeCount;
	}
}

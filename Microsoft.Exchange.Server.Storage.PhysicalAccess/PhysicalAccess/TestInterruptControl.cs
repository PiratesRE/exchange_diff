using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class TestInterruptControl : IInterruptControl
	{
		public TestInterruptControl() : this(1, 1, 1, null)
		{
		}

		public TestInterruptControl(int scanReadMax, int probeReadMax, int writeMax) : this(scanReadMax, probeReadMax, writeMax, null)
		{
		}

		public TestInterruptControl(int scanReadMax, int probeReadMax, int writeMax, Func<bool> wantToInterrupt)
		{
			this.scanReadMax = scanReadMax;
			this.probeReadMax = probeReadMax;
			this.writeMax = writeMax;
			this.wantToInterrupt = wantToInterrupt;
		}

		public bool WantToInterrupt
		{
			get
			{
				return this.scanReadCount >= this.scanReadMax || this.probeReadCount >= this.probeReadMax || this.writeCount >= this.writeMax || (this.wantToInterrupt != null && this.wantToInterrupt());
			}
		}

		public void RegisterRead(bool probe, TableClass tableClass)
		{
			if (probe)
			{
				this.probeReadCount++;
				return;
			}
			this.scanReadCount++;
		}

		public void RegisterWrite(TableClass tableClass)
		{
			this.writeCount++;
		}

		public void Reset()
		{
			this.scanReadCount = 0;
			this.probeReadCount = 0;
			this.writeCount = 0;
		}

		private readonly int scanReadMax;

		private readonly int probeReadMax;

		private readonly int writeMax;

		private readonly Func<bool> wantToInterrupt;

		private int scanReadCount;

		private int probeReadCount;

		private int writeCount;
	}
}

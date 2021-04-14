using System;

namespace Microsoft.Exchange.Security
{
	internal abstract class SafeSspiHandle : DebugSafeHandle
	{
		public override bool IsInvalid
		{
			get
			{
				return base.IsClosed || this.SspiHandle.IsZero;
			}
		}

		public override string ToString()
		{
			return this.SspiHandle.ToString();
		}

		public SspiHandle SspiHandle;
	}
}

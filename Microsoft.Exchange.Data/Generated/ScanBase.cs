using System;
using mppg;

namespace Microsoft.Exchange.Data.Generated
{
	public abstract class ScanBase : AScanner<LexValue, LexLocation>
	{
		protected abstract int CurrentSc { get; set; }

		public virtual int EolState
		{
			get
			{
				return this.CurrentSc;
			}
			set
			{
				this.CurrentSc = value;
			}
		}
	}
}

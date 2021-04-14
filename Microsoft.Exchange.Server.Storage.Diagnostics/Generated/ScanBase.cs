using System;
using mppg;

namespace Microsoft.Exchange.Server.Storage.Diagnostics.Generated
{
	public abstract class ScanBase : AScanner<Token, LexLocation>
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

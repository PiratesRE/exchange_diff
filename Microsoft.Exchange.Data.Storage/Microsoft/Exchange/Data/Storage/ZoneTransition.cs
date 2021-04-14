using System;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class ZoneTransition
	{
		public int Bias
		{
			get
			{
				return this.bias;
			}
			set
			{
				this.bias = value;
			}
		}

		public ChangeDate ChangeDate
		{
			get
			{
				return this.changeDate;
			}
			set
			{
				this.changeDate = value;
			}
		}

		public ZoneTransition()
		{
		}

		internal ZoneTransition(int bias, NativeMethods.SystemTime systemTime)
		{
			this.bias = bias;
			this.changeDate = new ChangeDate(systemTime);
		}

		private int bias;

		private ChangeDate changeDate;
	}
}

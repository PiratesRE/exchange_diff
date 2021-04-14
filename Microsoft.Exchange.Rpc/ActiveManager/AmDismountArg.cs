using System;

namespace Microsoft.Exchange.Rpc.ActiveManager
{
	public class AmDismountArg
	{
		public AmDismountArg(AmDismountArg amDismountArg)
		{
			this.m_flags = amDismountArg.m_flags;
			this.m_reason = amDismountArg.m_reason;
		}

		public AmDismountArg(int flags, int reason)
		{
			this.m_flags = flags;
			this.m_reason = reason;
		}

		public int Flags
		{
			get
			{
				return this.m_flags;
			}
			set
			{
				this.m_flags = value;
			}
		}

		public int Reason
		{
			get
			{
				return this.m_reason;
			}
			set
			{
				this.m_reason = value;
			}
		}

		private int m_flags;

		private int m_reason;
	}
}

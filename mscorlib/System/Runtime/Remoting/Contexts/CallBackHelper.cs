using System;
using System.Security;

namespace System.Runtime.Remoting.Contexts
{
	[Serializable]
	internal class CallBackHelper
	{
		internal bool IsEERequested
		{
			get
			{
				return (this._flags & 1) == 1;
			}
			set
			{
				if (value)
				{
					this._flags |= 1;
				}
			}
		}

		internal bool IsCrossDomain
		{
			set
			{
				if (value)
				{
					this._flags |= 256;
				}
			}
		}

		internal CallBackHelper(IntPtr privateData, bool bFromEE, int targetDomainID)
		{
			this.IsEERequested = bFromEE;
			this.IsCrossDomain = (targetDomainID != 0);
			this._privateData = privateData;
		}

		[SecurityCritical]
		internal void Func()
		{
			if (this.IsEERequested)
			{
				Context.ExecuteCallBackInEE(this._privateData);
			}
		}

		internal const int RequestedFromEE = 1;

		internal const int XDomainTransition = 256;

		private int _flags;

		private IntPtr _privateData;
	}
}

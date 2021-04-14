using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class GetCallInfoResponse : ResponseBase
	{
		public UMCallInfoEx CallInfo
		{
			get
			{
				return this.callInfo;
			}
			set
			{
				this.callInfo = value;
			}
		}

		private UMCallInfoEx callInfo;
	}
}

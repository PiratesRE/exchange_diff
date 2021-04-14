using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public abstract class PlayOnPhoneRequest : RequestBase
	{
		public string DialString
		{
			get
			{
				return this.dialString;
			}
			set
			{
				this.dialString = value;
			}
		}

		internal UMOperationResult EndResult
		{
			get
			{
				return this.endResult;
			}
			set
			{
				this.endResult = value;
			}
		}

		private string dialString;

		private UMOperationResult endResult;
	}
}

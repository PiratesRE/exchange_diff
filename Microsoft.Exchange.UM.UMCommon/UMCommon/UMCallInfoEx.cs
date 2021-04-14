using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	public class UMCallInfoEx : UMCallInfo
	{
		public int ResponseCode
		{
			get
			{
				return this.responseCode;
			}
			set
			{
				this.responseCode = value;
			}
		}

		public string ResponseText
		{
			get
			{
				return this.responseText;
			}
			set
			{
				this.responseText = value;
			}
		}

		public UMOperationResult EndResult
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

		private int responseCode;

		private string responseText = string.Empty;

		private UMOperationResult endResult;
	}
}

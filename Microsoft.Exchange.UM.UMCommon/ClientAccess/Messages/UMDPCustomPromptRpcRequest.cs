using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class UMDPCustomPromptRpcRequest : PromptPreviewRpcRequest
	{
		public UMDPCustomPromptRpcRequest(UMDialPlan dp, string promptFileName)
		{
			this.DialPlan = dp;
			this.PromptFileName = promptFileName;
		}

		public string PromptFileName { get; set; }

		public UMDialPlan DialPlan { get; set; }

		internal override string GetFriendlyName()
		{
			return Strings.AutoAttendantCustomPromptRequest;
		}
	}
}

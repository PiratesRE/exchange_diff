using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class UMMailboxPromptRpcRequest : PromptPreviewRpcRequest
	{
		public static UMMailboxPromptRpcRequest CreateAwayPromptRequest(ADUser userId, string displayName)
		{
			return new UMMailboxPromptRpcRequest(userId, false, true, displayName);
		}

		public static UMMailboxPromptRpcRequest CreateVoicemailPromptRequest(ADUser userId, string displayName)
		{
			return new UMMailboxPromptRpcRequest(userId, false, false, displayName);
		}

		public static UMMailboxPromptRpcRequest CreateCustomAwayPromptRequest(ADUser userId)
		{
			return new UMMailboxPromptRpcRequest(userId, true, true);
		}

		public static UMMailboxPromptRpcRequest CreateCustomVoicemailPromptRequest(ADUser userId)
		{
			return new UMMailboxPromptRpcRequest(userId, true, false);
		}

		private UMMailboxPromptRpcRequest(ADUser userId, bool customFlag, bool awayFlag, string displayName)
		{
			this.User = userId;
			this.CustomFlag = customFlag;
			this.AwayFlag = awayFlag;
			this.DisplayName = displayName;
		}

		private UMMailboxPromptRpcRequest(ADUser userId, bool customFlag, bool awayFlag)
		{
			this.User = userId;
			this.CustomFlag = customFlag;
			this.AwayFlag = awayFlag;
		}

		public bool CustomFlag { get; private set; }

		public bool AwayFlag { get; private set; }

		public string DisplayName { get; private set; }

		public ADUser User { get; private set; }

		internal override string GetFriendlyName()
		{
			return Strings.UMMailboxPromptRequest;
		}
	}
}

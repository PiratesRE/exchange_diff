using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Rpc
{
	[Serializable]
	public class SubmitPINResetMessageRequest : SubmitMessageRequest
	{
		public SubmitPINResetMessageRequest()
		{
		}

		internal SubmitPINResetMessageRequest(ADUser user) : base(user)
		{
		}

		internal override UMRpcResponse Execute()
		{
			Utils.SendPasswordResetMail(base.User, base.AccessNumbers, base.Extension, base.PIN, base.To);
			return null;
		}

		internal override string GetFriendlyName()
		{
			return Strings.SubmitPINResetMessageRequest;
		}
	}
}

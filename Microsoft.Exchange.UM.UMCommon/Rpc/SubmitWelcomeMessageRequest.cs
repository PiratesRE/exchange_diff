using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Rpc
{
	[Serializable]
	public class SubmitWelcomeMessageRequest : SubmitMessageRequest
	{
		public SubmitWelcomeMessageRequest()
		{
		}

		internal SubmitWelcomeMessageRequest(ADUser user) : base(user)
		{
		}

		internal override UMRpcResponse Execute()
		{
			Utils.SendWelcomeMail(base.User, base.AccessNumbers, base.Extension, base.PIN, base.To, null);
			return null;
		}

		internal override string GetFriendlyName()
		{
			return Strings.SubmitWelcomeMessageRequest;
		}
	}
}

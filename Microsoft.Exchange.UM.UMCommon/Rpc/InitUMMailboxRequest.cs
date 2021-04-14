using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Rpc
{
	[Serializable]
	public class InitUMMailboxRequest : UpdateUMMailboxRequest
	{
		public InitUMMailboxRequest()
		{
		}

		internal InitUMMailboxRequest(ADUser user) : base(user)
		{
		}

		internal override UMRpcResponse Execute()
		{
			Utils.InitUMMailbox(base.User);
			return null;
		}

		internal override string GetFriendlyName()
		{
			return Strings.InitializeUMMailboxRequest;
		}
	}
}

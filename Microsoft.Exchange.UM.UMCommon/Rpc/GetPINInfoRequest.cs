using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Rpc
{
	[Serializable]
	public class GetPINInfoRequest : UMRpcRequest
	{
		public GetPINInfoRequest()
		{
		}

		internal GetPINInfoRequest(ADUser user) : base(user)
		{
		}

		internal override UMRpcResponse Execute()
		{
			return Utils.GetPINInfo(base.User);
		}

		internal override string GetFriendlyName()
		{
			return Strings.GetPINInfoRequest;
		}
	}
}

using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Rpc
{
	[Serializable]
	public class DisableUMMailboxRequest : UMRpcRequest
	{
		public DisableUMMailboxRequest()
		{
		}

		internal DisableUMMailboxRequest(ADUser user) : base(user)
		{
		}

		public bool KeepProperties
		{
			get
			{
				return this.keepProperties;
			}
			set
			{
				this.keepProperties = value;
			}
		}

		internal override UMRpcResponse Execute()
		{
			Utils.ResetUMMailbox(base.User, this.keepProperties);
			return null;
		}

		internal override string GetFriendlyName()
		{
			return Strings.DisableUMMailboxRequest;
		}

		private bool keepProperties;
	}
}

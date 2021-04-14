using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Rpc
{
	[Serializable]
	public class ValidateOrGeneratePINRequest : UpdateUMMailboxRequest
	{
		public ValidateOrGeneratePINRequest()
		{
		}

		internal ValidateOrGeneratePINRequest(ADUser user) : base(user)
		{
		}

		public string PIN
		{
			get
			{
				return this.pin;
			}
			set
			{
				this.pin = value;
			}
		}

		internal override UMRpcResponse Execute()
		{
			return Utils.ValidateOrGeneratePIN(base.User, this.pin);
		}

		internal override string GetFriendlyName()
		{
			return Strings.ValidateOrGeneratePINRequest;
		}

		private string pin;
	}
}

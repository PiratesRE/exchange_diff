using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Rpc
{
	[Serializable]
	public class SavePINRequest : UpdateUMMailboxRequest
	{
		public SavePINRequest()
		{
		}

		internal SavePINRequest(ADUser user) : base(user)
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

		public bool Expired
		{
			get
			{
				return this.expired;
			}
			set
			{
				this.expired = value;
			}
		}

		public bool LockedOut
		{
			get
			{
				return this.lockedOut;
			}
			set
			{
				this.lockedOut = value;
			}
		}

		internal override UMRpcResponse Execute()
		{
			Utils.SetUserPassword(base.User, this.pin, this.expired, this.lockedOut);
			return null;
		}

		internal override string GetFriendlyName()
		{
			return Strings.SavePINRequest;
		}

		private string pin;

		private bool expired;

		private bool lockedOut;
	}
}

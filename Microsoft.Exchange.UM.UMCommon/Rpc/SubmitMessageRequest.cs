using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.UM.Rpc
{
	[Serializable]
	public abstract class SubmitMessageRequest : UMRpcRequest
	{
		public SubmitMessageRequest()
		{
		}

		internal SubmitMessageRequest(ADUser user) : base(user)
		{
		}

		public string To
		{
			get
			{
				return this.to;
			}
			set
			{
				this.to = value;
			}
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

		public string Extension
		{
			get
			{
				return this.extension;
			}
			set
			{
				this.extension = value;
			}
		}

		public string[] AccessNumbers
		{
			get
			{
				return this.accessNumbers;
			}
			set
			{
				this.accessNumbers = value;
			}
		}

		private string to;

		private string pin;

		private string extension;

		private string[] accessNumbers;
	}
}

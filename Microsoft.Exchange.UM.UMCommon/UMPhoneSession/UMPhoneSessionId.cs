using System;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.UM.UMPhoneSession
{
	[Serializable]
	public class UMPhoneSessionId : ObjectId
	{
		public string RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
			private set
			{
				this.rawIdentity = value;
			}
		}

		public UMPhoneSessionId(string sessionId)
		{
			this.RawIdentity = sessionId;
		}

		public override byte[] GetBytes()
		{
			return Encoding.UTF8.GetBytes(this.RawIdentity);
		}

		public override string ToString()
		{
			return this.RawIdentity;
		}

		private string rawIdentity;
	}
}
